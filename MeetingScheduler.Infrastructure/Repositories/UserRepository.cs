using MeetingScheduler.Infrastructure.Models;
using MeetingScheduler.Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MeetingScheduler.Infrastructure.Repositories
{
    public class UserRepository(UserManager<User> userManager) : IUserRepository
    {
        public UserManager<User> _userManager = userManager;

        public async Task<List<User>> GetAllUsers()
        {
            return await _userManager.Users.ToListAsync();
        }

        public async Task<User> GetUserById(Guid userId)
        {
            return await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<User> GetUserByUserName(string userName)
        {
            return await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == userName);
        }

        public async Task<List<User>> GetEmployeesForPeopleManager(Guid peopleManagerId)
        {
            return await _userManager.Users.
                Where(u => u.PeopleManagerId == peopleManagerId).
                ToListAsync();
        }

        public async Task<List<User>> GetAllFreeEmployees()
        {
            var employees = await _userManager.GetUsersInRoleAsync("Employee");
            return employees
                .Where(u => u.PeopleManagerId == null)
                .ToList();
        }

        public async Task<User> GetUserWithPeopleManagerById(Guid userId)
        {
            return await _userManager.Users.Include(p => p.PeopleManager).FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<List<string>> GetRoleNames(User user)
        {
            return (List<string>)await _userManager.GetRolesAsync(user);
        }

        public async Task<User> AddUser(User user)
        {
            await _userManager.CreateAsync(user);
            return user;
        }

        public async Task<User> UpdateUser(User user)
        {
            await _userManager.UpdateAsync(user);
            return user;
        }

        public async Task<bool> DeleteUser(Guid userId)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) { return false; }

            return (await _userManager.DeleteAsync(user)).Succeeded;
        }

        public async Task AddRoleToUser(User user, string roleName)
        {
            await _userManager.AddToRoleAsync(user, roleName);
        }

        public async Task<User> GetUserByEmail(string email)
        {
            return await _userManager.Users.FirstOrDefaultAsync(u => u.Email == email);
        }
    }
}
