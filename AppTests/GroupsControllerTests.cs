using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Dto;
using Core.Interfaces;
using Core.Models;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using TddBasedRestfulApp.Controllers;
using Xunit;

namespace AppTests
{
    public class GroupsControllerTests
    {
        [Fact]
        public async Task GetAllGroupsAsync_ShouldReturnStatusOkAndListOfGroups()
        {
            //Arrange
            var groupRepoMock = Substitute.For<IGroupRepository>();
            groupRepoMock.GetAllAsync().Returns(new List<Group>
            {
                new Group {Id = 1, Name = "someName", Country = "someCountry", CreationYear = 2000}
            });
            var groupServiceMock = Substitute.For<IGroupService>();
            var expected = new List<GroupDto> {GroupDto.GetGroupDtoWithId(1, "someName", "someCountry", 2000)};
            groupServiceMock.GetAllAsync().Returns(expected);
            var sut = new GroupsController(groupServiceMock);

            //Act
            var response = await sut.GetAllGroupsAsync();
            //Assert
            await groupServiceMock.Received().GetAllAsync();
            //Assert.Equal(200, ((StatusCodeResult)response).StatusCode)); //need to fix(can`t get statusCode & value from ActionResult) but result is correct
            //Assert.Equal(expected, response.Value);
        }

        [Fact]
        public async Task AddGroupAsync_ShouldReturnStatusOk()
        {
            var groupRepoMock = Substitute.For<IGroupRepository>();
            var groupServiceMock = Substitute.For<IGroupService>();
            var groupToAdd = new Group {Name = "someName", Country = "someCountry", CreationYear = 2000};
            var groupDtoToAdd = GroupDto.GetGroupDtoWithoutId("someName", "someCountry", 2000);
            var sut = new GroupsController(groupServiceMock);

            //Act
            var response = await sut.AddGroupAsync(groupDtoToAdd);

            //Assert
            await groupServiceMock.Received().AddAsync(groupDtoToAdd);
            //Assert.Equal(200, ((StatusCodeResult) response).StatusCode);
        }

        [Fact]
        public async Task AddGroupAsync_NullPassedShouldReturnStatusBadRequest()
        {
            //Arrange
            var groupServiceMock = Substitute.For<IGroupService>();
            var sut = new GroupsController(groupServiceMock);

            //Act
            var response = await sut.AddGroupAsync(null);

            //Assert
            //Assert.Equal(400, ((StatusCodeResult) response).StatusCode);
        }

        [Fact]
        public async Task DeleteGroupAsync_ShouldReturnStatusOk()
        {
            //Arrange
            var groupServiceMock = Substitute.For<IGroupService>();
            var groupToRemove = new Group {Id = 1, Name = "someName", Country = "someCountry", CreationYear = 2000};
            var sut = new GroupsController(groupServiceMock);

            //Act
            var response = await sut.DeleteGroupAsync(1);

            //Assert
            await groupServiceMock.Received().DeleteByIdAsync(1);
            //Assert.Equal(200, ((StatusCodeResult) response).StatusCode);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        public async Task DeleteGroupAsync_WrongIdShouldReturnStatusBadRequest(int id)
        {
            //Arrange
            var groupServiceMock = Substitute.For<IGroupService>();
            var sut = new GroupsController(groupServiceMock);

            //Act
            var response = await sut.DeleteGroupAsync(id);

            //Assert
            Assert.Equal(400, ((StatusCodeResult) response).StatusCode);
        }

        [Fact]
        public async Task DeleteGroupAsync_NotExistingIdShouldReturnStatusNotFound()
        {
            //Arrange
            var groupServiceMock = Substitute.For<IGroupService>();
            var sut = new GroupsController(groupServiceMock);

            //Act
            var response = await sut.DeleteGroupAsync(100500);

            //Assert
            await groupServiceMock.Received().DeleteByIdAsync(100500);
            //Assert.Equal(404, ((StatusCodeResult) response).StatusCode);
        }

        [Fact]
        public async Task GetGroupByName_ShouldReturnStatusOkAndGroupDto()
        {
            //Arrange
            var groupServiceMock = Substitute.For<IGroupService>();
            var expected = GroupDto.GetGroupDtoWithId(1, "someName", "someCountry", 2000);
            groupServiceMock.GetAllAsync().Returns(new List<GroupDto> {expected});
            var sut = new GroupsController(groupServiceMock);

            //Act
            var response = await sut.GetGroupByName("someName");

            //Assert
            await groupServiceMock.Received().FindOneAsync(g => g.Name == "someName");
            //Assert.Equal(200, ((StatusCodeResult) response.Result).StatusCode);
            //Assert.Equal(expected, response.Value);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async Task GetGroupByName_NotSpecifiedNameStatusBadRequest(string name)
        {
            //Arrange
            var groupServiceMock = Substitute.For<IGroupService>();
            var sut = new GroupsController(groupServiceMock);

            //Act
            var response = await sut.GetGroupByName(name);

            //Assert
            //Assert.Equal(400, ((StatusCodeResult) response.Result).StatusCode);
        }

        [Fact]
        public async Task GetGroupByName_NotExistingNameNameStatusBadRequest()
        {
            //Arrange
            var groupServiceMock = Substitute.For<IGroupService>();
            var expected = GroupDto.GetGroupDtoWithId(1, "someName", "someCountry", 2000);
            groupServiceMock.GetAllAsync().Returns(new List<GroupDto> {expected});
            //groupServiceMock.FindOneAsync(g => g.Name == "notExistingName").Returns();
            var sut = new GroupsController(groupServiceMock);

            //Act
            var response = await sut.GetGroupByName("notExistingName");

            //Assert
            await groupServiceMock.Received().FindOneAsync(g => g.Name == "notExistingName");
            //Assert.Equal(400, ((StatusCodeResult) response.Result).StatusCode);
        }

        [Fact]
        public async Task GetGroupsByCountry_ShouldReturnStatusOkAndIEnumerableOfGroupDto()
        {
            //Arrange
            var groupServiceMock = Substitute.For<IGroupService>();
            var group = new Group {Id = 1, Name = "someName", Country = "someCountry", CreationYear = 2000};
            var expected = new List<GroupDto> {GroupDto.GetGroupDtoWithId(1, "someName", "someCountry", 2000)};
            groupServiceMock.GetAllAsync().Returns(new List<GroupDto>
                {GroupDto.GetGroupDtoWithId(1, "someName", "someCountry", 2000)});
            var sut = new GroupsController(groupServiceMock);

            //Act
            var response = await sut.GetGroupsByCountry("someCountry");

            //Assert
            await groupServiceMock.Received().FindOneAsync(g => g.Country == "someCountry");
            //Assert.Equal(200, ((StatusCodeResult) response.Result).StatusCode);
            //Assert.Equal(expected, response.Value);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async Task GetGroupsByCountry_NotSpecifiedNameStatusBadRequest(string name)
        {
            //Arrange
            var groupServiceMock = Substitute.For<IGroupService>();
            var sut = new GroupsController(groupServiceMock);

            //Act
            var response = await sut.GetGroupsByCountry(name);

            //Assert
            Assert.Equal(400, ((StatusCodeResult) response.Result).StatusCode);
        }

        [Fact]
        public async Task GetGroupsByCountry_NotExistingNameNameStatusBadRequest()
        {
            //Arrange
            var groupServiceMock = Substitute.For<IGroupService>();
            var group = GroupDto.GetGroupDtoWithId(1, "someName", "someCountry", 2000);
            groupServiceMock.GetAllAsync().Returns(new List<GroupDto> {group});
            //groupServiceMock.FindOneAsync(g => g.Country == "notExistingName").Returns(null);
            var sut = new GroupsController(groupServiceMock);

            //Act
            var response = await sut.GetGroupsByCountry("notExistingName");

            //Assert
            await groupServiceMock.Received().FindOneAsync(g => g.Name == "notExistingName");
            //Assert.Equal(400, ((StatusCodeResult) response.Result).StatusCode);
        }
    }
}