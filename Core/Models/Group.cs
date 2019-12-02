using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models
{
    public class Group
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }
        public int CreationYear { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            var group = (Group) obj;

            return (Id == group.Id) && (Name == group.Name) &&
                   (Country == group.Country) && (CreationYear == group.CreationYear);
        }

        public override int GetHashCode() => Id.GetHashCode();
    }
}