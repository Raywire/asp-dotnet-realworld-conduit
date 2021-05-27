using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Conduit.Models;
using Conduit.Services;
using Conduit.DTOs.Responses;
using AutoMapper;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Conduit.Constants;

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
            var currentUserId = IsAuthenticated() ? GetCurrentUserId().ToString() : null;

            if (user == null)
            {
                return NotFound(new ErrorResponse()
                {
                    Success = false,
                    Errors = new Errors() { Message = "Profile not found" }
                });
            }

            var profile = _mapper.Map<ProfileResponseDto>(user);

            if (currentUserId != null)
            {
                var isFollowing = await _repository.GetProfileFollowAsync(user.Id, Guid.Parse(currentUserId));
                if (isFollowing != null)
                {
                    profile.IsFollowing = true;
                }
            }

            return new ProfileResponse()
            {
                Success = true,
                Profile = profile
            };
        }

        // GET: api/Profiles/me
        [Authorize]
        [HttpGet("me")]
        public async Task<ActionResult<ProfileResponse>> GetCurrentUserProfile()
        {
            var currentUserId = GetCurrentUserId();

            var user = await _repository.GetProfileAsync(currentUserId);

            if (user == null)
            {
                return NotFound(new ErrorResponse()
                {
                    Success = false,
                    Errors = new Errors() { Message = "Profile not found" }
                });
            }

            var profile = _mapper.Map<ProfileResponseDto>(user);

            return new ProfileResponse()
            {
                Success = true,
                Profile = profile
            };
        }

        // POST: api/Profiles/:username/follow
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPost("{username}/follow")]
        public async Task<ActionResult<User>> FollowUser(string username)
        {
            var followingUser = await _repository.GetUserByEmailOrUsernameAsync(username, username);

            if (followingUser == null)
            {
                return NotFound(new ErrorResponse()
                {
                    Success = false,
                    Errors = new Errors() { Message = "User not found" }
                });
            }

            // Get current user Id
            var currentUserId = GetCurrentUserId();
            var currentUser = await _repository.GetUserAsync(currentUserId);

            if (currentUser.UserName == username)
            {
                return StatusCode(StatusCodes.Forbidden, new ErrorResponse()
                {
                    Success = false,
                    Errors = new Errors() { Message = "You cannot follow yourself" }
                });
            }

            var followModel = new Follow()
            {
                FollowerId = currentUserId,
                FollowingId = followingUser.Id
            };

            var follow = await _repository.GetProfileFollowAsync(followingUser.Id, currentUserId);

            if (follow == null)
            {
                await _repository.AddProfileFollowAsync(followModel);
                await _repository.SaveChangesAsync();
            }

            var profileReadDto = _mapper.Map<ProfileResponseDto>(followingUser);

            profileReadDto.IsFollowing = true;

            int statusCode = follow == null ? StatusCodes.Created : StatusCodes.OK;

            return StatusCode(statusCode, new ProfileResponse()
            {
                Success = true,
                Profile = profileReadDto
            });
        }

        // DELETE: api/Profiles/:username/follow
        [Authorize]
        [HttpDelete("{username}/follow")]
        public async Task<IActionResult> DeleteUser(string username)
        {
            var followingUser = await _repository.GetUserByEmailOrUsernameAsync(username, username);

            if (followingUser == null)
            {
                return NotFound(new ErrorResponse()
                {
                    Success = false,
                    Errors = new Errors() { Message = "User not found" }
                });
            }

            // Get current user Id
            var currentUserId = GetCurrentUserId();

            var follow = await _repository.GetProfileFollowAsync(followingUser.Id, currentUserId);

            if (follow != null)
            {
                _repository.DeleteProfileFollow(follow);
                await _repository.SaveChangesAsync();
            }

            var profileReadDto = _mapper.Map<ProfileResponseDto>(followingUser);

            return StatusCode(StatusCodes.OK, new ProfileResponse()
            {
                Success = true,
                Profile = profileReadDto
            });
        }

        private Guid GetCurrentUserId()
        {
            var identity = User.Identity as ClaimsIdentity;
            IEnumerable<Claim> claims = identity.Claims;
            var userId = claims.Where(p => p.Type == "Id").FirstOrDefault()?.Value;

            return Guid.Parse(userId);
        }

        private bool IsAuthenticated()
        {
            return User.Identity.IsAuthenticated;
        }
    }
}
