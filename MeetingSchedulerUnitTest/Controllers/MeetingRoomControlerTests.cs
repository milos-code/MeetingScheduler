using MeetingScheduler.Api.Controllers;
using MeetingScheduler.Bussines.DTOs.MeetingRoom;
using MeetingScheduler.Bussines.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace MeetingScheduler.Api.Test.Controllers
{
    public class MeetingRoomControlerTests
    {
        readonly Mock<IMeetingRoomService> meetingRoomServiceMock = new();
        readonly Guid meetingRoomId = Guid.NewGuid();
        const string roomName = nameof(roomName);

        [Fact]
        public async Task GetAllMeetingRooms_ReturnsOkResult_WithMeetingRoomsDto()
        {
            // Arrange
            var meetingRooms = new List<MeetingRoomDto>
            {
                new MeetingRoomDto { Id = meetingRoomId, RoomName = roomName }
            };

            meetingRoomServiceMock
                .Setup(service => service.GetAllMeetingRooms())
                .ReturnsAsync(meetingRooms);

            var controller = new MeetingRoomController(meetingRoomServiceMock.Object);

            // Act
            var result = await controller.GetAllMeetingRooms();

            // Assert
            meetingRoomServiceMock.Verify(x => x.GetAllMeetingRooms(), Times.Once);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<List<MeetingRoomDto>>(okResult.Value);
            Assert.Equal(meetingRooms, returnValue);
        }

        [Fact]
        public async Task GetMeetingRoomById_ReturnsOkResult_WithMeetingRoomDto()
        {
            //Arrange
            var meetingRoom = new MeetingRoomDto()
            {
                Id = meetingRoomId,
                RoomName = roomName
            };

            meetingRoomServiceMock
                .Setup(service => service.GetMeetingRoomById(meetingRoomId))
                .ReturnsAsync(meetingRoom);

            var controller = new MeetingRoomController(meetingRoomServiceMock.Object);

            //Act
            var result = await controller.GetMeetingRoomById(meetingRoomId);

            //Assert
            meetingRoomServiceMock.Verify(x => x.GetMeetingRoomById(meetingRoomId), Times.Once);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<MeetingRoomDto>(okResult.Value);
            Assert.Equal(meetingRoom, returnValue);
        }

        [Fact]
        public async Task CreateMeetingRoom_ReturnsMeetingsRoomDto()
        {
            //Arrange
            var createMeetingRoomDto = new CreateMeetingRoomDto()
            {
                RoomName = roomName
            };

            var meetingRoom = new MeetingRoomDto()
            {
                Id = meetingRoomId,
                RoomName = createMeetingRoomDto.RoomName
            };

            meetingRoomServiceMock
                .Setup(service => service.CreateMeetingRoom(createMeetingRoomDto))
                .ReturnsAsync(meetingRoom);

            var controller = new MeetingRoomController(meetingRoomServiceMock.Object);

            //Act
            var result = await controller.CreateMeetingRoom(createMeetingRoomDto);

            //Assert
            meetingRoomServiceMock.Verify(x => x.CreateMeetingRoom(createMeetingRoomDto), Times.Once);

            var returnValue = Assert.IsType<MeetingRoomDto>(result.Value);
            Assert.Equal(meetingRoom, returnValue);
        }

        [Fact]
        public async Task UpdateMeetingRoom_ReturnsUpdatedMeetingRoomDto()
        {
            //Arrange
            var updateMeetingRoomDto = new UpdateMeetingRoomDto()
            {
                RoomName = roomName
            };

            var meetingRoom = new MeetingRoomDto()
            {
                Id = meetingRoomId,
                RoomName = updateMeetingRoomDto.RoomName
            };

            meetingRoomServiceMock
                .Setup(service => service.UpdateMeetingRoom(updateMeetingRoomDto))
                .ReturnsAsync(meetingRoom);

            var controller = new MeetingRoomController(meetingRoomServiceMock.Object);

            //Act
            var result = await controller.UpdateMeetingRoom(updateMeetingRoomDto);

            //Assert
            meetingRoomServiceMock.Verify(x => x.UpdateMeetingRoom(updateMeetingRoomDto), Times.Once);

            var returnValue = Assert.IsType<MeetingRoomDto>(result.Value);
        }

        [Fact]
        public async Task DeleteMeetingRoom_ReturnsOkResult()
        {
            //Arrange

            meetingRoomServiceMock
                .Setup(service => service.DeleteMeetingRoom(roomName))
                .ReturnsAsync(true);

            var controller = new MeetingRoomController(meetingRoomServiceMock.Object);

            //Act
            var result = await controller.DeleteMeetingRoom(roomName);

            //Assert
            meetingRoomServiceMock.Verify(x => x.DeleteMeetingRoom(roomName), Times.Once);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Meeting room has been deleted.", okResult.Value);
        }
    }
}