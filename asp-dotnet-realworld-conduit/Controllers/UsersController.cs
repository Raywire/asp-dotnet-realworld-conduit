using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using asp_dotnet_realworld_conduit.Data;
using asp_dotnet_realworld_conduit.Models;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;
using asp_dotnet_realworld_conduit.DTOs.Responses;
using asp_dotnet_realworld_conduit.DTOs.Requests;

namespace asp_dotnet_realworld_conduit.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ConduitContext _context;
        private readonly IMapper _mapper;

        public UsersController(ConduitContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<UsersResponse>> GetUsers()
        {
            var users = await _context.Users.ToListAsync();

            return Ok(new UsersResponse()
            {
                Success = true,
                Users = _mapper.Map<IEnumerable<UsersResponseDto>>(users)
            });
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserResponse>> GetUser(Guid id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound(new ErrorResponse()
                {
                    Success = false,
                    Errors = new Errors() { Message = "User not found" }
                });
            }

            return Ok(new UserResponse()
            {
                Success = true,
                User = _mapper.Map<UsersResponseDto>(user)
            });
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(Guid id, UserUpdateRequestDto userUpdateRequestDto)
        {
            if (id != userUpdateRequestDto.Id)
            {
                return BadRequest(new ErrorResponse()
                {
                    Success = false,
                    Errors = new Errors() { Message = "User ids do not match" }
                });
            }

            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound(new ErrorResponse()
                {
                    Success = false,
                    Errors = new Errors() { Message = "User not found" }
                });
            }

            _mapper.Map(userUpdateRequestDto, user);

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound(new ErrorResponse()
                    {
                        Success = false,
                        Errors = new Errors() { Message = "User not found" }
                    });
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPost]
        //public async Task<ActionResult<Users>> PostUser(Users user)
        //{
        //    _context.Users.Add(user);
        //    await _context.SaveChangesAsync();

        //    return CreatedAtAction("GetUser", new { id = user.Id }, user);
        //}

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound(new ErrorResponse()
                {
                    Success = false,
                    Errors = new Errors() { Message = "User not found" }
                });
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(Guid id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
