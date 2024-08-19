using MeetingScheduler.Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MeetingScheduler.Infrastructure.Repositories
{
    public class RoleRepository(RoleManager<IdentityRole<Guid>> roleManager) : IRoleRepository
    {
        private readonly RoleManager<IdentityRole<Guid>> _roleManager = roleManager;

        public async Task<List<IdentityRole<Guid>>> GetAllRoles()
        {
            return await _roleManager.Roles.ToListAsync();
        }

        public async Task<IdentityRole<Guid>> GetRoleById(Guid id)
        {
            return await _roleManager.FindByIdAsync(id.ToString());
        }

        public async Task<IdentityRole<Guid>> GetRoleByName(string name)
        {
            return await _roleManager.FindByNameAsync(name);
        }

        public async Task CreateRole(IdentityRole<Guid> role)
        {
            await _roleManager.CreateAsync(role);
        }
    }
}
