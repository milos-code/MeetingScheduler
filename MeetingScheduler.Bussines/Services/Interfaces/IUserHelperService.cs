using MeetingScheduler.Bussines.DTOs.User;
using MeetingScheduler.Infrastructure.Models;

namespace MeetingScheduler.Bussines.Services.Interfaces
{
    public interface IUserHelperService
    {
        Task<UserDto> MapUserDtoWithRoles(User user);
        Task<List<UserDto>> MapUsersDtoListWithRoles(List<User> users);
    }
}
