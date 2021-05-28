using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Conduit.Models;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;
using Conduit.DTOs.Responses;
using Conduit.DTOs.Requests;
using Conduit.Services;
using Conduit.ResourceParameters;
using Conduit.Helpers;

namespace Conduit.Controllers
{
    [Authorize(Roles = UserRoles.Admin)]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IConduitRepository _repository;

        public UsersController(IMapper mapper, IConduitRepository repository)
        {
            _mapper = mapper;
            _repository = repository;
        }

        // GET: api/Users
        [HttpGet(Name = "GetUsers")]
        public async Task<ActionResult<UsersResponse>> GetUsers([FromQuery] UsersResourceParameters usersResourceParameters)
        {
            var users = await _repository.GetUsersAsync(usersResourceParameters);

            var previousPageLink = users.HasPrevious ?
                CreateUsersResourceUri(usersResourceParameters, ResourceUriType.PreviousPage, "GetUsers") : null;

            var nextPageLink = users.HasNext ?
                CreateUsersResourceUri(usersResourceParameters, ResourceUriType.NextPage, "GetUsers") : null;

            return Ok(new UsersResponse()
            {
                Success = true,
                Metadata = new Metadata()
                {
                    TotalCount = users.TotalCount,
                    PageSize = users.PageSize,
                    CurrentPage = users.CurrentPage,
                    TotalPages = users.TotalPages,
                    PreviousPageLink = previousPageLink,
                    NextPageLink = nextPageLink
                },
                Users = _mapper.Map<IEnumerable<UsersResponseDto>>(users)
            });
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserResponse>> GetUser(Guid id)
        {
            var user = await _repository.GetUserAsync(id);

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
        public async Task<IActionResult> PutUser(Guid id, UserUpdateRequestWrapper userUpdateRequestDto)
        {
            var user = await _repository.GetUserAsync(id);

            if (user == null)
            {
                return NotFound(new ErrorResponse()
                {
                    Success = false,
                    Errors = new Errors() { Message = "User not found" }
                });
            }

            _mapper.Map(userUpdateRequestDto.User, user);

            try
            {
                await _repository.SaveChangesAsync();
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

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var user = await _repository.GetUserAsync(id);

            if (user == null)
            {
                return NotFound(new ErrorResponse()
                {
                    Success = false,
                    Errors = new Errors() { Message = "User not found" }
                });
            }

            _repository.DeleteUser(user);
            await _repository.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(Guid id)
        {
            return _repository.UserExists(id);
        }

        private string CreateUsersResourceUri(UsersResourceParameters usersResourceParameters, ResourceUriType type, string urlLink)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return Url.Link(urlLink,
                      new
                      {
                          pageNumber = usersResourceParameters.PageNumber - 1,
                          pageSize = usersResourceParameters.PageSize
                      });
                case ResourceUriType.NextPage:
                    return Url.Link(urlLink,
                      new
                      {
                          pageNumber = usersResourceParameters.PageNumber + 1,
                          pageSize = usersResourceParameters.PageSize
                      });
                default:
                    return Url.Link(urlLink,
                    new
                    {
                        pageNumber = usersResourceParameters.PageNumber,
                        pageSize = usersResourceParameters.PageSize
                    });
            }
        }
    }
}
