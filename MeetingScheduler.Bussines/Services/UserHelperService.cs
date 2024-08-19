using AutoMapper;
using MeetingScheduler.Bussines.DTOs.User;
using MeetingScheduler.Bussines.Services.Interfaces;
using MeetingScheduler.Infrastructure.Models;
using MeetingScheduler.Infrastructure.Repositories.Interfaces;

namespace MeetingScheduler.Bussines.Services
{
    public class UserHelperService(IMapper mapper,
        IUserRepository userRepository) : IUserHelperService
    {
        private readonly IMapper _mapper = mapper;
        private readonly IUserRepository _userRepository = userRepository;

        public async Task<UserDto> MapUserDtoWithRoles(User user)
        {
            UserDto userDto = _mapper.Map<UserDto>(user);

            userDto.RoleNames = await _userRepository.GetRoleNames(user);

            return userDto;
        }

        public async Task<List<UserDto>> MapUsersDtoListWithRoles(List<User> users)
        {
            List<UserDto> usersDto = new();

            foreach (var user in users)
            {
                usersDto.Add(await MapUserDtoWithRoles(user));
            }

            return usersDto;
        }
    }
}
