using AutoMapper;
using MeetingScheduler.Bussines.DTOs.JwtToken;
using MeetingScheduler.Bussines.DTOs.User;
using MeetingScheduler.Bussines.Services.JwtTokenGenerator;
using MeetingScheduler.Infrastructure.Models;
using MeetingScheduler.Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using StudentRecords.Bussines.Exceptions;
using System.Net;

namespace MeetingScheduler.Bussines.Services.Authentication
{
    public class AuthenticationService(ITokenGenerator tokenGenerator,
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        IMapper mapper,
        IUserRepository userRepository) : IAuthenticationService
    {
        private readonly ITokenGenerator _tokenGenerator = tokenGenerator;
        private readonly UserManager<User> _userManager = userManager;
        private readonly SignInManager<User> _signInManager = signInManager;
        private readonly IMapper _mapper = mapper;
        private readonly IUserRepository _userRepository = userRepository;

        public async Task<LogInUserResponse> LogInUser(LogInUserDto logInUserDto)
        {
            var user = await _userRepository.GetUserByEmail(logInUserDto.Email);

            ApiExceptionHandler.ObjectNotFound(user, $"User {user.UserName}");

            if (user == null)
            {
                ApiExceptionHandler.ThrowApiException(HttpStatusCode.BadRequest, "Email or password is incorrect, try again.");
            }

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, logInUserDto.Password);

            if (!isPasswordValid)
            {
                ApiExceptionHandler.ThrowApiException(HttpStatusCode.BadRequest, "Email or password is incorrect, try again.");
            }

            await _signInManager.SignInAsync(user, isPersistent : false);

            var userToken = _mapper.Map<JwtCreationToken>(user);
            userToken.RoleNames = (List<string>)await _userManager.GetRolesAsync(user);

            return await _tokenGenerator.CreateToken(userToken);
        }

        public async Task LogOut()
        {
            await _signInManager.SignOutAsync();
        }
    }
}
