using MeetingScheduler.Infrastructure.Models;
using MeetingScheduler.Infrastructure.Models.Enums;
using MeetingScheduler.Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using System.Transactions;

namespace MeetingScheduler.Infrastructure.Seeder
{
    public class SeedData(
        UserManager<User> userManager,
        RoleManager<IdentityRole<Guid>> roleManager,
        IUserRepository userRepository)
    {
        private readonly UserManager<User> _userManager = userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager = roleManager;
        private readonly IUserRepository _userRepository = userRepository;

        public async Task Seed()
        {
            foreach (var role in Enum.GetNames(typeof(Roles)))
            {
                var idRole = new IdentityRole<Guid>(role);
                await _roleManager.CreateAsync(idRole);
            }

            if (_userRepository.GetAllUsers().Result.Count == 0)
            {
                await CreateAdmin("Admin@admin.com");
            }
        }

        private async Task CreateAdmin(string email)
        {
            User user = new()
            {
                FirstName = "Admin",
                LastName = "Admin",
                Email = email,
                UserName = email
            };

            await _userManager.CreateAsync(user);
            await _userManager.AddToRoleAsync(user, Roles.Admin.ToString());
            user = await _userRepository.GetUserByEmail(user.Email);

            string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            using var scope = new TransactionScope(
                      TransactionScopeOption.Required,
                      new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
                      TransactionScopeAsyncFlowOption.Enabled);
            try
            {
                await _userManager.ConfirmEmailAsync(user, token);

                var identityResultPassword = await _userManager.AddPasswordAsync(user, "Admin123");

                if (!identityResultPassword.Succeeded)
                {
                    throw new Exception();
                }

                scope.Complete();
            }
            catch { throw; }
        }
    }
}