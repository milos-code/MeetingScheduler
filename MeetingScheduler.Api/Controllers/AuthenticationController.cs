using MeetingScheduler.Bussines.DTOs.User;
using MeetingScheduler.Bussines.Services.Authentication;
using MeetingScheduler.Bussines.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MeetingScheduler.Api.Controllers
{
    [Route("api/[controller]")]
    [AllowAnonymous]
    [ApiController]
    public class AuthenticationController(IUserService userService,
        IAuthenticationService authenticationService) : ControllerBase
    {
        private readonly IUserService _userService = userService;
        private readonly IAuthenticationService _authenticationService = authenticationService;

        [HttpPost("Register")]
        public async Task<ActionResult<string>> RegisterUser(RegisterUserDto registerUser)
        {
            return await _userService.SignUpUser(registerUser);
        }

        [HttpPost("LogIn")]
        public async Task<ActionResult<LogInUserResponse>> LogIn(LogInUserDto logInUserDto)
        {
            var token = await _authenticationService.LogInUser(logInUserDto);

            return Ok(token);
        }

        [HttpPost("LogOut")]
        public async Task LogOut()
        {
            await _authenticationService.LogOut();
        }
    }
}
