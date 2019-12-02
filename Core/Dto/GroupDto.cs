using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Dto
{
    public class GroupDto
    {
        public int Id { get; }
        public string Name { get; }
        public string Country { get; }
        public int CreationYear { get; }

        private GroupDto(int id, string name, string country, int year)
        {
            Id = id;
            Name = name;
            Country = country;
            CreationYear = year;
        }

        public static GroupDto GetGroupDtoWithId(int id, string name, string country, int year) =>
            new GroupDto(id, name, country, year);
        public static GroupDto GetGroupDtoWithoutId(string name, string country, int year) =>
            new GroupDto(0, name, country, year);

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            var group = (GroupDto)obj;

            return (Id == group.Id) && (Name == group.Name) && (Country == group.Country) && CreationYear == (group.CreationYear);
        }

        public override int GetHashCode() => Id.GetHashCode();
    }
}
