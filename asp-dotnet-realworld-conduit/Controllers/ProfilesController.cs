using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Conduit.Data;
using Conduit.Models;
using Conduit.Services;
using Conduit.DTOs.Responses;
using AutoMapper;

namespace Conduit.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfilesController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IConduitRepository _repository;

        public ProfilesController(IMapper mapper, IConduitRepository repository)
        {
            _mapper = mapper;
            _repository = repository;
        }

        // GET: api/Profiles/raywire
        [HttpGet("{username}")]
        public async Task<ActionResult<ProfileResponse>> GetProfile(string username)
        {
            var user = await _repository.GetUserByEmailOrUsernameAsync(username, username);

            if (user == null)
            {
                return NotFound(new ErrorResponse()
                {
                    Success = false,
                    Errors = new Errors() { Message = "Profile not found" }
                });
            }

            return new ProfileResponse()
            {
                Success = true,
                Profile = _mapper.Map<ProfileResponseDto>(user)
            };
        }

        // POST: api/Profiles
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            //_context.Users.Add(user);
            //await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = user.Id }, user);
        }

        // DELETE: api/Profiles/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            //var user = await _context.Users.FindAsync(id);
            //if (user == null)
            //{
            //    return NotFound();
            //}

            //_context.Users.Remove(user);
            //await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(Guid id)
        {
            return _repository.UserExists(id);
        }
    }
}
