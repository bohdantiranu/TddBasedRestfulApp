using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Dto;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using TddBasedRestfulApp.Controllers;
using Xunit;

namespace AppTests
{
    public class GroupsControllerTests
    {
        private readonly IGroupService _groupServiceMock;
        private readonly GroupsController _sut;
        private readonly List<GroupDto> _expectedList;

        public GroupsControllerTests()
        {
            _groupServiceMock = Substitute.For<IGroupService>();
            var expected = GroupDto.GetGroupDtoWithId(1, "someName", "someCountry", 2000);
            _expectedList = new List<GroupDto> { expected };
            ConfigureSub();

            _sut = new GroupsController(_groupServiceMock);
        }


        [Fact]
        public async Task GetAllGroupsAsync_ShouldReturnStatusOkAndListOfGroups()
        {
            //Act
            var response = await _sut.GetAllGroupsAsync();
            //Assert
            //await groupServiceMock.Received().GetAllAsync();
            var responseResult = (ObjectResult)response.Result;

            Assert.Equal(200, responseResult.StatusCode);
            Assert.Equal(_expectedList, responseResult.Value);
            Assert.Equal(_expectedList.First(), ((List<GroupDto>)responseResult.Value).First());
        }

        [Fact]
        public async Task AddGroupAsync_ShouldReturnStatusOk()
        {
            var groupDtoToAdd = GroupDto.GetGroupDtoWithoutId("someName", "someCountry", 2000);
            //Act
            var response = await _sut.AddGroupAsync(groupDtoToAdd);

            //Assert
            //await groupServiceMock.Received().AddAsync(groupDtoToAdd);
            Assert.Equal(200, ((StatusCodeResult) response).StatusCode);
        }

        [Fact]
        public async Task AddGroupAsync_NullPassedShouldReturnStatusBadRequest()
        {
            //Act
            var response = await _sut.AddGroupAsync(null);

            //Assert
            Assert.Equal(400, ((BadRequestObjectResult) response).StatusCode);
        }

        [Fact]
        public async Task DeleteGroupAsync_ShouldReturnStatusOk()
        {
            //Act
            var response = await _sut.DeleteGroupAsync(1);

            //Assert
            //await groupServiceMock.Received().DeleteByIdAsync(1);
            Assert.Equal(200, ((StatusCodeResult) response).StatusCode);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        public async Task DeleteGroupAsync_WrongIdShouldReturnStatusBadRequest(int id)
        {
            //Act
            var response = await _sut.DeleteGroupAsync(id);

            //Assert
            Assert.Equal(400, ((StatusCodeResult) response).StatusCode);
        }

        [Fact]
        public async Task DeleteGroupAsync_NotExistingIdShouldReturnStatusNotFound()
        {
            //Act
            await Assert.ThrowsAsync<Exception>(() => _sut.DeleteGroupAsync(100500));
            //Assert
            await _groupServiceMock.Received().DeleteByIdAsync(100500);
        }

        [Fact]
        public async Task GetGroupByName_ShouldReturnStatusOkAndGroupDto()
        {
            //Act
            var response = await _sut.GetGroupByName("someName");

            //Assert
            //await groupServiceMock.Received().FindOneAsync(g => g.Name == "someName");
            Assert.Equal(200, ((ObjectResult) response.Result).StatusCode);
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
            //Act
            var response = await _sut.GetGroupByName("notExistingName");

            //Assert
            //await groupServiceMock.Received().FindOneAsync(g => g.Name == "notExistingName");
            Assert.Equal(404, ((ObjectResult) response.Result).StatusCode);
        }

        [Fact]
        public async Task GetGroupsByCountry_ShouldReturnStatusOkAndIEnumerableOfGroupDto()
        {
            //Act
            var response = await _sut.GetGroupsByCountry("someCountry");

            //Assert
            //await groupServiceMock.Received().FindOneAsync(g => g.Country == "someCountry");
            Assert.Equal(200, ((ObjectResult) response.Result).StatusCode);
            //Assert.Equal(expected, response.Value);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async Task GetGroupsByCountry_NotSpecifiedNameStatusBadRequest(string name)
        {

            //Act
            var response = await _sut.GetGroupsByCountry(name);

            //Assert
            Assert.Equal(400, ((ObjectResult) response.Result).StatusCode);
        }

        [Fact]
        public async Task GetGroupsByCountry_NotExistingNameNameStatusBadRequest()
        {
            //Act
            var response = await _sut.GetGroupsByCountry("notExistingName");

            //Assert
            //await groupServiceMock.Received().FindOneAsync(g => g.Name == "notExistingName");
            Assert.Equal(404, ((ObjectResult) response.Result).StatusCode);
        }

        #region Misc

        private void ConfigureSub()
        {
            _groupServiceMock.GetAllAsync().Returns(info => { return _expectedList; });
            _groupServiceMock.FindOneAsync(Arg.Any<Predicate<GroupDto>>()).Returns(info =>
            {
                var predicate = (Predicate<GroupDto>)info.Args()[0];
                return _expectedList.FirstOrDefault(dto => predicate(dto));
            });
            _groupServiceMock.FindAsync(Arg.Any<Predicate<GroupDto>>()).Returns(info =>
            {
                var predicate = (Predicate<GroupDto>)info.Args()[0];
                return _expectedList.Where(dto => predicate(dto));
            });

            _groupServiceMock.DeleteByIdAsync(Arg.Any<int>()).Returns(info =>
            {
                var id = (int)info.Args()[0];

                var groupDto = _expectedList.FirstOrDefault(dto => dto.Id == id);
                if (groupDto != null)
                {
                    _expectedList.Remove(groupDto);
                    return Task.CompletedTask;
                }

                throw new Exception("SHIT!");
            });
        }


        #endregion
    }
}