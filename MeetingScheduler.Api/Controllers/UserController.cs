using MeetingScheduler.Bussines.DTOs.User;
using MeetingScheduler.Bussines.Services.Interfaces;
using MeetingScheduler.Infrastructure.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MeetingScheduler.Api.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class UserController(IUserService userService, UserManager<User> userManager) : ControllerBase
    {
        private readonly IUserService _userService = userService;
        private readonly UserManager<User> _userManager = userManager;

        [Authorize(Roles = "Admin, PeopleManager")]
        [HttpGet("GetAllUsers")]
        public async Task<ActionResult<List<UserDto>>> GetAllUsers()
        {
            return Ok(await _userService.GetAllUsers());
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("GetUserById")]
        public async Task<ActionResult<UserDto>> GetUser(Guid userId)
        {
            return await _userService.GetUserById(userId);
        }

        [HttpGet("GetUserByUserName")]
        public async Task<ActionResult<UserDto>> GetUserByUserName()
        {
            return await _userService.GetUserByUserName();
        }

        [Authorize(Roles = "PeopleManager")]
        [HttpGet("GetUsersForPeopleManager")]
        public async Task<ActionResult<List<UserDto>>> GetEmployeesForPeopleManager()
        {
            return Ok(await _userService.GetEmployeesForPeopleManager());
        }

        [Authorize(Roles = "PeopleManager")]
        [HttpGet("GetAllFreeEmployees")]
        public async Task<ActionResult<List<UserDto>>> GetAllFreeEmployees()
        {
            return Ok(await _userService.GetAllFreeEmployees());
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("CreateUser")]
        public async Task<ActionResult<UserDto>> CreateUser(CreateUserDto createUserDto)
        {
            var createdUser = await _userService.CreateUser(createUserDto);

            return CreatedAtAction(nameof(GetUser), new { id = createdUser.Id }, createdUser);
        }

        [Authorize(Roles = "PeopleManager")]
        [HttpPost("AssignEmployeeToPeopleManager")]
        public async Task<ActionResult> AssignEmployeeToPeopleManager([FromBody]AssignUserIdDto assignUserIdDto)
        {
            return Ok(await _userService.AssignEmployeeToPeopleManager(assignUserIdDto.UserId));
        }

        [Authorize(Roles = "PeopleManager")]
        [HttpPost("UnassignEmployeeToPeopleManager")]
        public async Task<ActionResult> UnassignEmployeeFromPeopleManager(UnassignEmployeeDto unassignEmployeeDto)
        {
            return Ok(await _userService.UnassignEmployeeFromPeopleManage(unassignEmployeeDto.UserId));
        }

        [HttpPut("UpdateUser")]
        public async Task<ActionResult<UserDto>> UpdateUser(UpdateUserDto updateUserDto)
        {
            return await _userService.UpdateUser(updateUserDto);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("DeleteUser")]
        public async Task<ActionResult> DeleteUser(Guid userId)
        {
            await _userService.DeleteUser(userId);

            return Ok("User has been deleted.");
        }
    }
}
