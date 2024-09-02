using AutoMapper;
using MeetingScheduler.Bussines.DTOs.User;
using MeetingScheduler.Bussines.Services.Interfaces;
using MeetingScheduler.Infrastructure.Models;
using MeetingScheduler.Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StudentRecords.Bussines.Exceptions;
using System.Net;
using System.Transactions;

namespace MeetingScheduler.Bussines.Services
{
    public class UserService(IUserRepository userRepository,
        IMapper mapper,
        IEmailService emailService,
        IRoleRepository roleRepository,
        UserManager<User> userManager,
        IUserHelperService userHelperService,
        IHttpContextAccessor httpContextAccessor) : IUserService
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IMapper _mapper = mapper;
        private readonly IEmailService _emailService = emailService;
        private readonly IRoleRepository _roleRepository = roleRepository;
        private readonly UserManager<User> _userManager = userManager;
        private readonly IUserHelperService _userHelperService = userHelperService;

        public async Task<List<UserDto>> GetAllUsers()
        {
            var users = await _userRepository.GetAllUsers();

            return await _userHelperService.MapUsersDtoListWithRoles(users);
        }

        public async Task<UserDto> GetUserById(Guid userId)
        {
            var user = await _userRepository.GetUserById(userId);

            ApiExceptionHandler.ObjectNotFound(user, $"User {user.UserName}");

            return await _userHelperService.MapUserDtoWithRoles(user);
        }

        public async Task<UserDto> GetUserByUserName()
        {
            return await _userHelperService.MapUserDtoWithRoles(await _userRepository.GetUserByUserName(httpContextAccessor.HttpContext.User.Identity.Name));
        }

        public async Task<List<UserDto>> GetEmployeesForPeopleManager()
        {
            var peopleManager = await _userRepository.GetUserByUserName(httpContextAccessor.HttpContext.User.Identity.Name);

            var users = await _userRepository.GetEmployeesForPeopleManager(peopleManager.Id);

            return await _userHelperService.MapUsersDtoListWithRoles(users);
        }

        public async Task<List<UserDto>> GetAllFreeEmployees()
        {
            var users = await _userRepository.GetAllFreeEmployees();

            return await _userHelperService.MapUsersDtoListWithRoles(users);
        }

        public async Task<UserDto> CreateUser(CreateUserDto createUserDto)
        {
            var existingUser = await _userRepository.GetUserByEmail(createUserDto.Email);
            if (existingUser != null)
            {
                ApiExceptionHandler.ThrowApiException(HttpStatusCode.BadRequest, "User already exists.");
            }

            var user = _mapper.Map<User>(createUserDto);

            user.UserName = user.FirstName + user.LastName;

            using (var scope = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
                TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var roleName = await _roleRepository.GetRoleByName(createUserDto.RoleName);

                    var newUser = await _userRepository.AddUser(user);

                    await _userRepository.AddRoleToUser(newUser, roleName.ToString());

                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                    var emailResult = await _emailService.SendEmail([user.Email], "Email confirmation token", token);

                    if (emailResult is false)
                    {
                        ApiExceptionHandler.ThrowApiException(HttpStatusCode.BadRequest, "Error sending confirmation token via email.");
                    }

                    //TODO: Implement email token encoding

                    scope.Complete();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            return await _userHelperService.MapUserDtoWithRoles(user);
        }

        public async Task<bool> AssignEmployeeToPeopleManager(string userId)
        {
            if (!Guid.TryParse(userId, out var guidId))
            {
                throw new ArgumentException("Invalid userId format.");
            }

            var user = await _userRepository.GetUserWithPeopleManagerById(guidId);

            ApiExceptionHandler.ObjectNotFound(user, $"User {user.UserName}");

            var peopleManager = await _userRepository.GetUserByUserName(httpContextAccessor.HttpContext.User.Identity.Name);

            user.PeopleManagerId = peopleManager.Id;

            await _userRepository.UpdateUser(user);

            return true;
        }

        public async Task<bool> UnassignEmployeeFromPeopleManage(string userId)
        {
            if (!Guid.TryParse(userId, out var guidId))
            {
                throw new ArgumentException("Invalid userId format.");
            }

            var user = await _userRepository.GetUserWithPeopleManagerById(guidId);

            ApiExceptionHandler.ObjectNotFound(user, $"User {user.UserName}");

            user.PeopleManagerId = null;

            await _userRepository.UpdateUser(user);

            return true;
        }

        public async Task<UserDto> UpdateUser(UpdateUserDto updateUserDto)
        {
            var loggedInUser = await _userRepository.GetUserByUserName(httpContextAccessor.HttpContext.User.Identity.Name);

            ApiExceptionHandler.ObjectNotFound(loggedInUser, $"User {loggedInUser.UserName}");

            loggedInUser.BirthDate = (DateTime)updateUserDto.BirthDate;

            if (loggedInUser.FirstName != updateUserDto.FirstName || loggedInUser.LastName != updateUserDto.LastName)
            {
                loggedInUser.FirstName = updateUserDto.FirstName;
                loggedInUser.LastName = updateUserDto.LastName;

                loggedInUser.UserName = updateUserDto.FirstName + updateUserDto.LastName;
            }

            await _userRepository.UpdateUser(loggedInUser);
            return await _userHelperService.MapUserDtoWithRoles(loggedInUser);
        }

        public async Task<bool> DeleteUser(Guid userId)
        {
            var user = await _userRepository.GetUserById(userId);

            ApiExceptionHandler.ObjectNotFound(user, $"User {user.UserName}");

            return await _userRepository.DeleteUser(userId);
        }

        public async Task<string> SignUpUser(RegisterUserDto signUpUserDto)
        {
            var user = await _userRepository.GetUserByEmail(signUpUserDto.Email);

            ApiExceptionHandler.ObjectNotFound(user, $"User {signUpUserDto.Email}");

            if (await _userManager.IsEmailConfirmedAsync(user))
            {
                ApiExceptionHandler.ThrowApiException(HttpStatusCode.BadRequest, "User is already signed up.");
            }

            using var scope = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
                TransactionScopeAsyncFlowOption.Enabled);
            try
            {
                (bool confirmEmailSuccess, string confirmEmailError) = await ConfirmEmail(user, signUpUserDto.Token);

                if (!confirmEmailSuccess && !String.IsNullOrEmpty(confirmEmailError))
                {
                    var resendToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    await _emailService.SendEmail(
                        [user.Email],
                        "Resent email token as previous one timed out.",
                        resendToken);

                    ApiExceptionHandler.ThrowApiException(HttpStatusCode.BadRequest, "Token has expired. Email resent.");
                }

                (bool addPasswordSuccess, string addPasswordError) = await AddPassword(user, signUpUserDto.Password);

                if (!addPasswordSuccess && !String.IsNullOrEmpty(addPasswordError))
                {
                    ApiExceptionHandler.ThrowApiException(HttpStatusCode.BadRequest, addPasswordError);
                }

                scope.Complete();
            }
            catch { throw; }

            return "Registration complete";
        }

        private async Task<(bool, string)> ConfirmEmail(User user, string token)
        {
            var identityResult = await _userManager.ConfirmEmailAsync(user, token);

            if (!identityResult.Succeeded)
            {
                return (false, string.Join(", ", identityResult.Errors.Select(x => x.Description).ToList()));
            }

            return (true, "");
        }

        private async Task<(bool, string)> AddPassword(User user, string password)
        {
            var identityResultPassword = await _userManager.AddPasswordAsync(user, password);

            if (!identityResultPassword.Succeeded)
            {
                return (false, string.Join(", ", identityResultPassword.Errors.Select(x => x.Description).ToList()));
            }

            var identityResultUpdate = await _userManager.UpdateAsync(user);

            if (!identityResultUpdate.Succeeded)
            {
                return (false, string.Join(", ", identityResultPassword.Errors.Select(x => x.Description).ToList()));
            }

            return (true, "");
        }
    }
}
