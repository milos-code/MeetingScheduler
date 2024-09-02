using MeetingScheduler.Infrastructure.Models;

namespace MeetingScheduler.Infrastructure.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<List<User>> GetAllUsers();
        Task<User> GetUserById(Guid userId);
        Task<User> GetUserByUserName(string userName);
        Task<List<User>> GetEmployeesForPeopleManager(Guid peopleManagerId);
        Task<List<User>> GetAllFreeEmployees();
        Task<User> GetUserWithPeopleManagerById(Guid userId);
        Task<List<string>> GetRoleNames(User user);
        Task<User> AddUser(User user);
        Task<User> UpdateUser(User user);
        Task<bool> DeleteUser(Guid userId);
        Task AddRoleToUser(User user, string roleName);
        Task<User> GetUserByEmail(string email);
    }
}
