using MeetingScheduler.Bussines.DTOs.User;

namespace MeetingScheduler.Bussines.Services.Interfaces
{
    public interface IUserService
    {
        Task<List<UserDto>> GetAllUsers();
        Task<UserDto> GetUserById(Guid userId);
        Task<UserDto> GetUserByUserName();
        Task<List<UserDto>> GetEmployeesForPeopleManager();
        Task<List<UserDto>> GetAllFreeEmployees();
        Task<UserDto> CreateUser(CreateUserDto createUserDto);
        Task<bool> AssignEmployeeToPeopleManager(string userId);
        Task<bool> UnassignEmployeeFromPeopleManage(string userId);
        Task<UserDto> UpdateUser(UpdateUserDto updateUserDto);
        Task<bool> DeleteUser(Guid userId);
        Task<string> SignUpUser(RegisterUserDto signUpUserDto);
    }
}
