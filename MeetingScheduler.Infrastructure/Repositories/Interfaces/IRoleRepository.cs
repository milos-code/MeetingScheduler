using Microsoft.AspNetCore.Identity;

namespace MeetingScheduler.Infrastructure.Repositories.Interfaces
{
    public interface IRoleRepository
    {
        Task<List<IdentityRole<Guid>>> GetAllRoles();
        Task<IdentityRole<Guid>> GetRoleById(Guid id);
        Task<IdentityRole<Guid>> GetRoleByName(string name);
        Task CreateRole(IdentityRole<Guid> role);
    }
}
