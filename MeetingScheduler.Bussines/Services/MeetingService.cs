using AutoMapper;
using MeetingScheduler.Bussines.DTOs.Meeting;
using MeetingScheduler.Bussines.Services.BackgroundJobs;
using MeetingScheduler.Bussines.Services.Interfaces;
using MeetingScheduler.Infrastructure.Models;
using MeetingScheduler.Infrastructure.Models.Enums;
using MeetingScheduler.Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Identity.Client;
using StudentRecords.Bussines.Exceptions;
using System.Net;
using System.Transactions;

namespace MeetingScheduler.Bussines.Services
{
    public class MeetingService(
        IMeetingRepository meetingRepository,
        IMapper mapper,
        IUserRepository userRepository,
        IMeetingUserRepository meetingUserRepository,
        IEmailService emailService,
        IMeetingRoomRepository meetingRoomRepository,
        IUserHelperService userHelperService,
        IMeetingReminderService meetingReminderService,
        IHttpContextAccessor httpContextAccessor) : IMeetingService
    {
        private readonly IMeetingRepository _meetingRepository = meetingRepository;
        private readonly IMapper _mapper = mapper;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IMeetingUserRepository _meetingUserRepository = meetingUserRepository;
        private readonly IEmailService _emailService = emailService;
        private readonly IMeetingRoomRepository _meetingRoomRepository = meetingRoomRepository;
        private readonly IUserHelperService _userHelperService = userHelperService;
        private readonly IMeetingReminderService _meetingReminderService = meetingReminderService;

        public async Task<List<MeetingDto>> GetAllMeetings()
        {
            return _mapper.Map<List<MeetingDto>>(await _meetingRepository.GetAllMeetings());
        }

        public async Task<MeetingDto> GetMeetingById(Guid meetingId)
        {
            var meeting = await _meetingRepository.GetMeetingById(meetingId);

            ApiExceptionHandler.ObjectNotFound(meeting, $"Meeting {meeting.Id}");

            var meetingDTO = _mapper.Map<MeetingDto>(meeting);

            meetingDTO.Users = await _userHelperService.MapUsersDtoListWithRoles(meeting.Users);

            meetingDTO.RoomName = meeting.MeetingRoom.RoomName;

            return meetingDTO;
        }

        public async Task<MeetingDto> GetMeetingByDate(DateTime date)
        {
            var meeting = await _meetingRepository.GetMeetingByDate(date);

            ApiExceptionHandler.ObjectNotFound(meeting, $"Meeting {meeting.Id}");

            return _mapper.Map<MeetingDto>(meeting);
        }

        public async Task<List<MeetingDto>> GetAllMeetingsByRoomNameOrStartTime(DateOnly? meetingStartTime, string? meetingRoomName)
        {
            var meetings = await _meetingRepository.GetAllMeetingsByRoomNameOrStartTime(meetingStartTime, meetingRoomName);

            List<MeetingDto> meetingsDto = new();

            foreach (var meeting in meetings)
            {
                var meetingDto = _mapper.Map<MeetingDto>(meeting);

                meetingDto.Users = await _userHelperService.MapUsersDtoListWithRoles(meeting.Users);

                meetingDto.RoomName = meeting.MeetingRoom.RoomName;

                meetingsDto.Add(meetingDto);
            }

            return meetingsDto;
        }

        public async Task<MeetingDto> CreateMeeting(CreateMeetingDto createMeetingDto)
        {
            var meeting = _mapper.Map<Meeting>(createMeetingDto);

            using (var scope = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
                TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var peopleManager = await _userRepository.GetUserByUserName(httpContextAccessor.HttpContext.User.Identity.Name);

                    meeting.Users.Add(peopleManager);

                    var meetingRoom = await _meetingRoomRepository.GetMeetingRoomByName(createMeetingDto.RoomName);

                    if( meetingRoom == null)
                    {
                        ApiExceptionHandler.ThrowApiException(HttpStatusCode.BadRequest, "Meeting room doesn't exist.");
                    }

                    meeting.MeetingRoom = meetingRoom;

                    foreach (var userEmail in createMeetingDto.EmployeesEmails)
                    {
                        var user = await _userRepository.GetUserByEmail(userEmail);

                        if(user == null)
                        {
                            ApiExceptionHandler.ThrowApiException(HttpStatusCode.BadRequest, "User doesn't exists.");
                        }

                        if (user.PeopleManagerId != peopleManager.Id)
                        {
                            ApiExceptionHandler.ThrowApiException(HttpStatusCode.BadRequest, "User not belongs to signed in people manager.");
                        }

                        meeting.Users.Add(user);
                    }

                    meeting.CreationDate = DateTime.UtcNow;
                    meeting = await _meetingRepository.AddMeeting(meeting);

                    await _emailService.SendEmail(
                        createMeetingDto.EmployeesEmails,
                        "Meeting notice",
                        $"A meeting has been scheduled for {meeting.MeetingStartTime}. Please respond accordingly.");

                    await UpdateUserMeetingStatus(peopleManager.Id, meeting.Id, MeetingStatus.Confirmed, "");

                    scope.Complete();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            return _mapper.Map<MeetingDto>(meeting);
        }

        public async Task<MeetingDto> EmployeeCreateMeeting(EmployeeCreateMeetingDto employeeCreateMeetingDto)
        {
            var meeting = _mapper.Map<Meeting>(employeeCreateMeetingDto);

            using (var scope = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
                TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var employeeFromToken = await _userRepository.GetUserByUserName(httpContextAccessor.HttpContext.User.Identity.Name);

                    meeting.Users.Add(employeeFromToken);

                    var meetingRoom = await _meetingRoomRepository.GetMeetingRoomByName(employeeCreateMeetingDto.RoomName);

                    if (meetingRoom == null)
                    {
                        ApiExceptionHandler.ThrowApiException(HttpStatusCode.BadRequest, "Meeting room doesn't exist.");
                    }

                    meeting.MeetingRoom = meetingRoom;

                    var peopleManager = await _userRepository.GetUserById((Guid)employeeFromToken.PeopleManagerId);

                    if (peopleManager == null)
                    {
                        ApiExceptionHandler.ThrowApiException(HttpStatusCode.BadRequest, "You don't have an assigned People Manager.");
                    }

                    meeting.Users.Add(peopleManager);

                    meeting.CreationDate = DateTime.UtcNow;
                    meeting = await _meetingRepository.AddMeeting(meeting);

                    await _emailService.SendEmail(
                        [peopleManager.Email],
                        "Meeting notice",
                        $"Your employee {employeeFromToken.UserName} scheduled a meeting for {meeting.MeetingStartTime}. Please respond accordingly.");

                    await UpdateUserMeetingStatus(employeeFromToken.Id, meeting.Id, MeetingStatus.Confirmed, "");

                    scope.Complete();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            return _mapper.Map<MeetingDto>(meeting);
        }

        public async Task<MeetingDto> PostponeMeetinng(PostponeMeetingDto postponeMeetingDto)
        {
            var meeting = await _meetingRepository.GetMeetingById(postponeMeetingDto.Id);

            if (meeting == null)
            {
                ApiExceptionHandler.ObjectNotFound(meeting, $"Meeting {postponeMeetingDto.Id}");
            }

            meeting.Status = MeetingStatus.Postponed;
            meeting.MeetingStartTime = postponeMeetingDto.MeetingStartTime;
            meeting.MeetingEndTime = postponeMeetingDto.MeetingEndTime;
            meeting.DelayOrCancelationNote = postponeMeetingDto.ReasonForPostponment;

            await _meetingRepository.UpdateMeeting(meeting);

            var usersEmails = meeting.Users
                .Select(e => e.Email)
                .ToList();

            await _emailService.SendEmail(usersEmails, "Meeting postponed", "Please respond for the given meeting, thanks.");

            return _mapper.Map<MeetingDto>(meeting);
        }

        public async Task<MeetingDto> UpdateMeeting(UpdateMeetingDto updateMeetingDto)
        {
            var meeting = await _meetingRepository.GetMeetingById(updateMeetingDto.Id);

            ApiExceptionHandler.ObjectNotFound(meeting, $"Meeting {meeting.Id}");

            return _mapper.Map<MeetingDto>(await _meetingRepository.UpdateMeeting(_mapper.Map<Meeting>(updateMeetingDto)));
        }

        public async Task<MeetingDto> CancelMeeting(Guid meetingId, string reasonForCancelation)
        {
            var meeting = await _meetingRepository.GetMeetingById(meetingId);

            ApiExceptionHandler.ObjectNotFound(meeting, $"Meeting {meeting.Id}");

            meeting.Status = MeetingStatus.Canceled;
            meeting.DelayOrCancelationNote = reasonForCancelation;

            var cancledMeeting = await _meetingRepository.UpdateMeeting(meeting);

            var usersEmails = meeting.Users
                .Select(e => e.Email)
                .ToList();

            await _emailService.SendEmail(usersEmails, "Meeting cancled", $"Meeting {meeting.MeetingTopic} has been cancled. Sorry for the inconvinience.");

            return _mapper.Map<MeetingDto>(cancledMeeting);
        }

        public async Task UpdateUserMeetingStatus(Guid userId, Guid meetingId, MeetingStatus status, string? note)
        {
            var userMeeting = await _meetingUserRepository.GetMeetingUserStatus(userId, meetingId);

            ApiExceptionHandler.ObjectNotFound(userMeeting, $"Meeting {meetingId}");

            userMeeting.Status = status;
            userMeeting.DelayOrCancelationNote = note;

            await _meetingUserRepository.UpdateMeetingStatus(userMeeting);
        }

        public async Task<List<EmployeeMeetingsDto>> GetAllMeetingsForAnEmployee(string userEmail)
        {
            var user = await _userRepository.GetUserByEmail(userEmail);

            ApiExceptionHandler.ObjectNotFound(user, $"User {user.UserName}");

            var peopleManager = await _userRepository.GetUserByUserName(httpContextAccessor.HttpContext.User.Identity.Name);

            if(user.PeopleManagerId != peopleManager.Id)
            {
                ApiExceptionHandler.ThrowApiException(HttpStatusCode.BadRequest, "Check if this is your employee.");
            }

            var meetings = await _meetingRepository.GetAllMeetingsWithAnEmployee(userEmail);

            var result = meetings
                .Select(m => new EmployeeMeetingsDto
                {
                    MeetingTopic = m.MeetingTopic,
                    CreationDate = m.CreationDate,
                    MeetingStartTime = m.MeetingStartTime,
                    MeetingEndTime = m.MeetingEndTime,
                    Username = user.UserName
                })
                .OrderBy(m => m.Username)
                .ToList();

            return result;
        }

        public async Task<List<EmployeeMeetingsDto>> GetAllMeetingsForEmployeeWithPeopleManager()
        {
            var user = await _userRepository.GetUserByUserName(httpContextAccessor.HttpContext.User.Identity.Name);

            var peopleManager = await _userRepository.GetUserById((Guid)user.PeopleManagerId);

            ApiExceptionHandler.ObjectNotFound(peopleManager, $"User {peopleManager.UserName}");

            var meetings = await _meetingRepository.GetAllMeetingsForEmployeeWithPeopleManager(user.Id, peopleManager.Id);

            var result = meetings
                .Select(m => new EmployeeMeetingsDto
                {
                    Username = user.UserName,
                    MeetingTopic = m.MeetingTopic,
                    CreationDate = m.CreationDate,
                    MeetingStartTime = m.MeetingStartTime,
                    MeetingEndTime = m.MeetingEndTime
                })
                .OrderBy(m => m.CreationDate)
                .ToList();

            return result;
        }
    }
}
