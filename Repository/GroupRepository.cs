using Core.Interfaces;
using Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repository
{
    public class GroupRepository : BaseRepository<Group>, IGroupRepository
    {
        public GroupRepository(GroupsContext context) : base(context)
        {
        }
    }
}