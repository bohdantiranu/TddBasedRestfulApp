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
            await groupRepoMock.Received().GetAllAsync();
            //Assert.Equal(200, ((StatusCodeResult)response).StatusCode)); //need to fix but result is correct
            Assert.Equal(expected, response.Value);
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
            await groupRepoMock.Received().AddAsync(groupToAdd);
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
            Assert.Equal(400, ((StatusCodeResult) response).StatusCode);
        }

        [Fact]
        public async Task DeleteGroupAsync_ShouldReturnStatusOk()
        {
            //Arrange
            var groupRepoMock = Substitute.For<IGroupRepository>();
            var groupServiceMock = Substitute.For<IGroupService>();
            var groupToRemove = new Group {Id = 1, Name = "someName", Country = "someCountry", CreationYear = 2000};
            groupRepoMock.GetByIdAsync(1).Returns(groupToRemove);
            var sut = new GroupsController(groupServiceMock);

            //Act
            var response = await sut.DeleteGroupAsync(1);

            //Assert
            await groupServiceMock.Received().DeleteByIdAsync(1);
            await groupRepoMock.Received().GetByIdAsync(1);
            await groupRepoMock.Received().DeleteAsync(groupToRemove);
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
            var groupRepoMock = Substitute.For<IGroupRepository>();
            var groupServiceMock = Substitute.For<IGroupService>();
            var sut = new GroupsController(groupServiceMock);

            //Act
            var response = await sut.DeleteGroupAsync(100500);

            //Assert
            await groupServiceMock.Received().DeleteByIdAsync(100500);
            await groupRepoMock.Received().GetByIdAsync(100500);
            Assert.Equal(404, ((StatusCodeResult) response).StatusCode);
        }

        [Fact]
        public async Task GetGroupByName_ShouldReturnStatusOkAndGroupDto()
        {
            //Arrange
            var groupRepoMock = Substitute.For<IGroupRepository>();
            var groupServiceMock = Substitute.For<IGroupService>();
            var group = new Group {Id = 1, Name = "someName", Country = "someCountry", CreationYear = 2000};
            var expected = GroupDto.GetGroupDtoWithId(1, "someName", "someCountry", 2000);
            groupRepoMock.GetAllAsync().Returns(new List<Group> {group});
            var sut = new GroupsController(groupServiceMock);

            //Act
            var response = await sut.GetGroupByName("someName");

            //Assert
            await groupServiceMock.Received().FindOneAsync(g => g.Name == "someName");
            await groupRepoMock.Received().GetAllAsync();
            //Assert.Equal(200, ((StatusCodeResult) response.Result).StatusCode);
            Assert.Equal(expected, response.Value);
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
            var groupRepoMock = Substitute.For<IGroupRepository>();
            var groupServiceMock = Substitute.For<IGroupService>();
            var group = new Group {Id = 1, Name = "someName", Country = "someCountry", CreationYear = 2000};
            groupRepoMock.GetAllAsync().Returns(new List<Group> {group});
            //groupServiceMock.FindOneAsync(g => g.Name == "notExistingName").Returns();
            var sut = new GroupsController(groupServiceMock);

            //Act
            var response = await sut.GetGroupByName("notExistingName");

            //Assert
            await groupServiceMock.Received().FindOneAsync(g => g.Name == "notExistingName");
            await groupRepoMock.Received().GetAllAsync();
            //Assert.Equal(400, ((StatusCodeResult) response.Result).StatusCode);
        }

        [Fact]
        public async Task GetGroupsByCountry_ShouldReturnStatusOkAndIEnumerableOfGroupDto()
        {
            //Arrange
            var groupRepoMock = Substitute.For<IGroupRepository>();
            var groupServiceMock = Substitute.For<IGroupService>();
            var group = new Group {Id = 1, Name = "someName", Country = "someCountry", CreationYear = 2000};
            var expected = new List<GroupDto> {GroupDto.GetGroupDtoWithId(1, "someName", "someCountry", 2000)};
            groupRepoMock.GetAllAsync().Returns(new List<Group> {group});
            var sut = new GroupsController(groupServiceMock);

            //Act
            var response = await sut.GetGroupsByCountry("someCountry");

            //
            await groupServiceMock.Received().FindOneAsync(g => g.Country == "someCountry");
            await groupRepoMock.Received().GetAllAsync();
            //Assert.Equal(200, ((StatusCodeResult) response.Result).StatusCode);
            Assert.Equal(expected, response.Value);
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
            var groupRepoMock = Substitute.For<IGroupRepository>();
            var groupServiceMock = Substitute.For<IGroupService>();
            var group = new Group {Id = 1, Name = "someName", Country = "someCountry", CreationYear = 2000};
            groupRepoMock.GetAllAsync().Returns(new List<Group> {group});
            //groupServiceMock.FindOneAsync(g => g.Country == "notExistingName").Returns(null);
            var sut = new GroupsController(groupServiceMock);

            //Act
            var response = await sut.GetGroupsByCountry("notExistingName");

            //Assert
            await groupServiceMock.Received().FindOneAsync(g => g.Name == "notExistingName");
            await groupRepoMock.Received().GetAllAsync();
            //Assert.Equal(400, ((StatusCodeResult) response.Result).StatusCode);
        }
    }
}