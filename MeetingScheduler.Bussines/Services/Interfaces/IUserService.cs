using MeetingScheduler.Bussines.DTOs.User;

namespace MeetingScheduler.Bussines.Services.Interfaces
{
    public interface IUserService
    {
        Task<List<UserDto>> GetAllUsers();
        Task<UserDto> GetUserById(Guid userId);
        Task<List<UserDto>> GetEmployeesForPeopleManager();
        Task<UserDto> CreateUser(CreateUserDto createUserDto);
        Task AssignEmployeeToPeopleManager(Guid userId);
        Task UnassignEmployeeFromPeopleManage(Guid userId);
        Task<UserDto> UpdateUser(UpdateUserDto updateUserDto);
        Task<bool> DeleteUser(Guid userId);
        Task<string> SignUpUser(RegisterUserDto signUpUserDto);
    }
}
