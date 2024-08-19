using AutoMapper;
using MeetingScheduler.Bussines.DTOs.User;
using MeetingScheduler.Bussines.Services;
using MeetingScheduler.Bussines.Services.Interfaces;
using MeetingScheduler.Infrastructure.Models;
using MeetingScheduler.Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace MeetingScheduler.Bussines.Test.Services
{
    public class UserServiceTests
    {
        readonly Mock<IUserRepository> userRepositoryMock = new();
        readonly Mock<IMapper> userMapperMock = new();
        readonly Mock<IEmailService> userEmailServiceMock = new();
        readonly Mock<IRoleRepository> userRoleRepositoryMock = new();
        readonly Mock<UserManager<User>> userUserManagerMock = new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
        readonly Mock<IUserHelperService> userUserHelperServiceMock = new();
        readonly Mock<IHttpContextAccessor> userHttpContextAccessorMock = new();

        [Fact]
        public async Task GetAllUsers_Should_ReturnsListOfUsersDto()
        {
            //Arrange
            var users = new List<User>
            {
                new User{
                    Id = Guid.NewGuid(),
                    FirstName = "Milos",
                    LastName = "Jovanovic",
                    Email = "Milos@Jovanovic.com"
                }
            };
            var usersDto = new List<UserDto>
            {
                new UserDto{
                    Id = Guid.NewGuid(),
                    FirstName = "Milos",
                    LastName = "Jovanovic",
                    Email = "Milos@Jovanovic.com"
                }
            };

            userRepositoryMock
                .Setup(x => x.GetAllUsers())
                .ReturnsAsync(users);

            userUserHelperServiceMock
                .Setup(x => x.MapUsersDtoListWithRoles(users))
                .ReturnsAsync(usersDto);

            var service = new UserService(userRepositoryMock.Object,
                userMapperMock.Object,
                userEmailServiceMock.Object,
                userRoleRepositoryMock.Object,
                userUserManagerMock.Object,
                userUserHelperServiceMock.Object,
                userHttpContextAccessorMock.Object);

            //Act
            var result = await service.GetAllUsers();

            //Assert
            userRepositoryMock.Verify(x => x.GetAllUsers(), Times.Once);

            var returnValue = Assert.IsType<List<UserDto>>(result);
            Assert.Equal(usersDto, returnValue);
        }

        [Fact]
        public async Task CreateUser_Should_ReturnUserDto()
        {
            //Arrange
            var createUserDto = new CreateUserDto()
            {
                FirstName = "Milos",
                LastName = "Jovanovic",
                Email = "Milos@Jovanovic.com",
                Position = "Junior BE Developer",
                RoleName = "Employee"
            };

            var user = new User
            {
                FirstName = createUserDto.FirstName,
                LastName = createUserDto.LastName,
                Email = createUserDto.Email,
                UserName = createUserDto.FirstName + createUserDto.LastName
            };

            var identityRole = new IdentityRole<Guid>
            {
                Id = Guid.NewGuid(),
                Name = createUserDto.RoleName
            };

            userMapperMock
                .Setup(m => m.Map<User>(createUserDto))
                .Returns(user);          

            userRoleRepositoryMock
                .Setup(r => r.GetRoleByName(createUserDto.RoleName))
                .ReturnsAsync(identityRole);

            userRepositoryMock
                .Setup(r => r.AddUser(It.IsAny<User>()))
                .ReturnsAsync(user);

            userRepositoryMock
                .Setup(r => r.AddRoleToUser(It.IsAny<User>(), createUserDto.RoleName))
                .Returns(Task.CompletedTask);

            userUserManagerMock
                .Setup(m => m.GenerateEmailConfirmationTokenAsync(user))
                .ReturnsAsync("fake-token");

            userEmailServiceMock
                .Setup(s => s.SendEmail(It.IsAny<List<string>>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            userUserHelperServiceMock
                .Setup(h => h.MapUserDtoWithRoles(user))
                .ReturnsAsync(new UserDto
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email
                });

            var service = new UserService(userRepositoryMock.Object,
                userMapperMock.Object,
                userEmailServiceMock.Object,
                userRoleRepositoryMock.Object,
                userUserManagerMock.Object,
                userUserHelperServiceMock.Object,
                userHttpContextAccessorMock.Object);

            //Act
            var result = await service.CreateUser(createUserDto);

            //Assert
            var returnValue = Assert.IsType<UserDto>(result);
            Assert.Equal(createUserDto.FirstName, returnValue.FirstName);
            Assert.Equal(createUserDto.LastName, returnValue.LastName);
            Assert.Equal(createUserDto.Email, returnValue.Email);
        }
    }
}
