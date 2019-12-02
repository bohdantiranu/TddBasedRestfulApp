using Core.Interfaces;
using Core.Models;
using Repository;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace AppTests
{
    public class GroupRepositoryTests
    {
        private readonly GroupsContext _context;

        public GroupRepositoryTests()
        {
            _context = new GroupsContext(); //need to mock but i don`t understand how
        }

        [Fact]
        public async Task Add_ShouldAddNewRowInTableGroup()
        {
            //Arrange
            IGroupRepository sut = new GroupRepository(_context);
            var expected = new Group {Name = "someName", Country = "someCountry", CreationYear = 2000};
            //Act
            await sut.AddAsync(expected);
            await sut.SaveAsync();
            var actual = (await sut.GetAllAsync()).LastOrDefault();

            //Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task GetAll_ShouldReturnIEnumerableOfAllGroups()
        {
            //Arrange 
            IGroupRepository sut = new GroupRepository(_context);

            //Act
            var result = (await sut.GetAllAsync()).ToList();

            //Assert
            Assert.IsType<List<Group>>(result);
        }


        [Fact]
        public async Task  GetById_ShouldReturnTypeGroupWithSpecifiedId()
        {
            //Arrange
            IGroupRepository sut = new GroupRepository(_context);
            var group = new Group {Name = "someName", Country = "someCountry", CreationYear = 2000};
            //Act
            await sut.AddAsync(group);
            await sut.SaveAsync();
            var actual = await sut.GetByIdAsync(1);
            var expected = (await sut.GetAllAsync()).FirstOrDefault();

            //Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task Delete_ShouldRemove()
        {
            //Arrange
            IGroupRepository sut = new GroupRepository(_context);

            //Act
            await sut.AddAsync(new Group
                {Name = "someNameToDelete", Country = "someCountryToDelete", CreationYear = 2000});
            await sut.SaveAsync();
            var expected = (await sut.GetAllAsync()).LastOrDefault();
            await sut.DeleteAsync(expected);
            await sut.SaveAsync();
            var actual = (await sut.GetAllAsync()).LastOrDefault();

            //Assert
            Assert.NotEqual(expected, actual);
        }
    }
}