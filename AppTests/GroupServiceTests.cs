using Core.Dto;
using Core.Interfaces;
using Core.Models;
using NSubstitute;
using RepositoryServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace AppTests
{
    public class GroupServiceTests
    {
        [Fact]
        public async Task GetAll_ShouldReturnIEnumerableOfGroupDto()
        {
            //Arrange
            var groupRepoMock = Substitute.For<IGroupRepository>();
            var expected = GroupDto.GetGroupDtoWithoutId("someName", "someCountry", 1234);
            groupRepoMock.GetAllAsync().Returns(new List<Group>
                {new Group {Name = "someName", Country = "someCountry", CreationYear = 1234}});
            IGroupService sut = new GroupService(groupRepoMock);

            //Act
            var allGroups = (await sut.GetAllAsync()).ToList();

            //Assert
            await groupRepoMock.Received().GetAllAsync();
            Assert.NotEmpty(allGroups);
            Assert.Equal(expected, allGroups[0]);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnGroupDto()
        {
            //Arrange
            var groupRepoMock = Substitute.For<IGroupRepository>();
            var expected = GroupDto.GetGroupDtoWithId(1, "someName", "someCountry", 1234);
            groupRepoMock.GetByIdAsync(1).Returns(new Group{Id = 1, Name = "someName", Country = "someCountry", CreationYear = 1234});
            IGroupService sut = new GroupService(groupRepoMock);

            //Act
            var actual = await sut.GetByIdAsync(1);

            //Assert
            await groupRepoMock.Received().GetByIdAsync(1);
            Assert.Equal(actual, expected);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        public async Task GetByIdAsync_NotExistingIdShouldFail(int id)
        {
            //Arrange
            var groupRepoMock = Substitute.For<IGroupRepository>();
            IGroupService sut = new GroupService(groupRepoMock);

            //Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await sut.GetByIdAsync(id));
        }

        [Fact]
        public async Task AddAsync_ShouldCallMethodAddAsyncFromRepository()
        {
            //Arrange
            var groupRepoMock = Substitute.For<IGroupRepository>();
            var groupToAdd = new Group {Id = 1, Name = "someName", Country = "someCountry", CreationYear = 1234};
            IGroupService sut = new GroupService(groupRepoMock);

            //Act
            await sut.AddAsync(GroupDto.GetGroupDtoWithId(1, "someName", "someCountry", 1234));

            //Assert
            await groupRepoMock.Received().AddAsync(groupToAdd);
        }

        [Fact]
        public async Task AddAsync_passNullShouldFail()
        {
            //Arrange
            var groupRepoMock = Substitute.For<IGroupRepository>();
            IGroupService sut = new GroupService(groupRepoMock);

            //Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => sut.AddAsync(null));
        }

        [Fact]
        public async Task DeleteByIdAsync_ShouldCallRepositoryMethodGetByIdAsyncDeleteAsync()
        {
            //Arrange
            var groupRepoMock = Substitute.For<IGroupRepository>();
            var groupToRemove = new Group {Id = 1, Name = "someName", Country = "someCountry", CreationYear = 1234};
            groupRepoMock.GetByIdAsync(1).Returns(groupToRemove);
            IGroupService sut = new GroupService(groupRepoMock);

            //Act 
            await sut.DeleteByIdAsync(1);

            //Assert
            await groupRepoMock.Received().GetByIdAsync(1);
            await groupRepoMock.Received().DeleteAsync(groupToRemove);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(100500)]
        public async Task DeleteByIdAsync_NotExistingIdShouldFail(int id)
        {
            //Arrange
            var groupRepoMock = Substitute.For<IGroupRepository>();
            IGroupService sut = new GroupService(groupRepoMock);

            //Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await sut.DeleteByIdAsync(id));
        }

        [Fact]
        public async Task FindAsync_ShouldReturnIEnumerableOfGroupDtoWithSpecifiedPredicate()
        {
            //Arrange
            var groupRepoMock = Substitute.For<IGroupRepository>();
            var groupToFind = new Group {Id = 1, Name = "someName", Country = "someCountry", CreationYear = 1234};
            var expected = GroupDto.GetGroupDtoWithId(1, "someName", "someCountry", 1234);
            ;
            groupRepoMock.GetAllAsync().Returns(new List<Group> {groupToFind});
            IGroupService sut = new GroupService(groupRepoMock);

            //Act
            var actualGroup = (await sut.FindAsync(g => g.Name == groupToFind.Name).ConfigureAwait(false)).ToList();

            //Asset
            await groupRepoMock.Received().GetAllAsync();
            Assert.NotEmpty(actualGroup);
            Assert.Equal(expected, actualGroup[0]);
        }

        [Fact]
        public async Task FindOneAsync_ShouldReturnGroupDtoWithSpecifiedPredicate()
        {
            //Arrange
            var groupRepoMock = Substitute.For<IGroupRepository>();
            var groupToFind = new Group {Id = 1, Name = "someName", Country = "someCountry", CreationYear = 1234};
            var expected = GroupDto.GetGroupDtoWithId(1, "someName", "someCountry", 1234);
            ;
            groupRepoMock.GetAllAsync().Returns(new List<Group> {groupToFind});
            IGroupService sut = new GroupService(groupRepoMock);

            //Act
            var actualGroup = await sut.FindOneAsync(g => g.Name == groupToFind.Name).ConfigureAwait(false);

            //Asset
            await groupRepoMock.Received().GetAllAsync();
            Assert.Equal(expected, actualGroup);
        }
    }
}