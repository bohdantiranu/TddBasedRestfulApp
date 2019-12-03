using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Core.Dto;
using Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TddBasedRestfulApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupsController : ControllerBase
    {
        private readonly IGroupService _groupService;

        public GroupsController(IGroupService groupService)
        {
            _groupService = groupService;
        }

        // GET: api/Groups
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GroupDto>>> GetAllGroupsAsync() => Ok(await _groupService.GetAllAsync());

        // GET: api/Groups/5
        [HttpGet("{name}", Name = "GetGroupByName")]
        public async Task<ActionResult<GroupDto>> GetGroupByName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return BadRequest("Group name is not specified");

            var group = await _groupService.FindOneAsync(g => g.Name == name);
            if (group == null)
                return NotFound("Group with specified name was not found");

            return Ok(group);
        }

        [HttpGet("{country}", Name = "GetGroupSByCountry")]
        public async Task<ActionResult<IEnumerable<GroupDto>>> GetGroupsByCountry(string country)
        {
            if (string.IsNullOrEmpty(country))
                return BadRequest("Country name is not specified");

            var groups = await _groupService.FindAsync(g => g.Country == country);
            if (!groups.Any())
                return NotFound("Groups with specified country was not found");

            return Ok(groups);
        }

        // POST: api/Groups
        [HttpPost]
        public async Task<ActionResult> AddGroupAsync(GroupDto group)
        {
            if (!ModelState.IsValid || group is null)
                return BadRequest(ModelState);

            await _groupService.AddAsync(group);
            return Ok();
        }


        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteGroupAsync(int id)
        {
            if (1 > id)
                return BadRequest();

            try
            {
                await _groupService.DeleteByIdAsync(id);
                return Ok();
            }
            catch (InvalidOperationException e)
            {
                return NotFound(e.Message);
            }
        }
    }
}