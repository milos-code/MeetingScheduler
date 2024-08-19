using AutoMapper;
using MeetingScheduler.Bussines.DTOs.JwtToken;
using MeetingScheduler.Bussines.DTOs.Meeting;
using MeetingScheduler.Bussines.DTOs.MeetingNotes;
using MeetingScheduler.Bussines.DTOs.MeetingRoom;
using MeetingScheduler.Bussines.DTOs.User;
using MeetingScheduler.Infrastructure.Models;

namespace MeetingScheduler.Bussines.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<User, CreateUserDto>().ReverseMap();
            CreateMap<User, UpdateUserDto>().ReverseMap();
            CreateMap<Meeting, MeetingDto>().ReverseMap();
            CreateMap<Meeting, CreateMeetingDto>().ReverseMap();
            CreateMap<Meeting, EmployeeCreateMeetingDto>().ReverseMap();
            CreateMap<Meeting, UpdateMeetingDto>().ReverseMap();
            CreateMap<Meeting, PostponeMeetingDto>().ReverseMap();
            CreateMap<MeetingRoom, MeetingRoomDto>().ReverseMap();
            CreateMap<MeetingRoom, CreateMeetingRoomDto>().ReverseMap();
            CreateMap<MeetingRoom, UpdateMeetingRoomDto>().ReverseMap();
            CreateMap<MeetingNotes, MeetingNotesDto>().ReverseMap();
            CreateMap<MeetingNotes, CreateMeetingNotesDto>().ReverseMap();
            CreateMap<MeetingNotes, UpdateMeetingNoteDto>().ReverseMap();
            CreateMap<User, JwtCreationToken>().ReverseMap();
        }
    }
}
