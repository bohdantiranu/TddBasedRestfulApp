using Core.Dto;
using Core.Interfaces;
using Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace RepositoryServices
{
    public class GroupService : ServiceBase<GroupDto, Group, IGroupRepository>, IGroupService
    {
        public GroupService(IGroupRepository repository) : base(repository)
        {
        }

        protected override Group CastToDbType(GroupDto dto) =>
            new Group {Id = dto.Id, Name = dto.Name, Country = dto.Country, CreationYear = dto.CreationYear};

        protected override GroupDto CastToDto(Group dbTypeEntity) =>
            GroupDto.GetGroupDtoWithId(dbTypeEntity.Id, dbTypeEntity.Name, dbTypeEntity.Country, dbTypeEntity.CreationYear);
    }
}