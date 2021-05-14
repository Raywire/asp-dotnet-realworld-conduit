using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Conduit.Data;
using Conduit.DTOs.Requests;
using Conduit.DTOs.Responses;
using Conduit.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Conduit.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ConduitContext _context;
        private readonly IMapper _mapper;

        public AuthController(IConfiguration configuration, ConduitContext context, IMapper mapper)
        {
            _configuration = configuration;
            _context = context;
            _mapper = mapper;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(UserLoginRequestDto userData)
        {
            User user = await GetUser(userData.Email);

            if (user != null)
            {
                var passwordVerified = VerifyPassword(userData.Password, user.Password);
                if (passwordVerified)
                {
                    var jwtToken = GenerateJwtToken(user);

                    return Ok(new UserLoginResponse()
                    {
                        Success = true,
                        Token = jwtToken,
                        User = _mapper.Map<UserLoginInfoDto>(user)
                    });
                }
                else
                {
                    return Unauthorized(new ErrorResponse()
                    {
                        Errors = new Errors() { Message = "Invalid credentials" },
                        Success = false
                    });
                }
            }
            else
            {
                return Unauthorized(new ErrorResponse()
                {
                    Errors = new Errors() { Message = "Invalid credentials" },
                    Success = false
                });
            }
        }

        [HttpPost]
        [Route("signup")]
        public async Task<IActionResult> Register([FromBody] UserRegisterRequestDto userData)
        {
            User user = await _context.Users.FirstOrDefaultAsync(user => user.Email == userData.Email || user.UserName == userData.UserName);
            if (user != null)
            {
                return BadRequest(new ErrorResponse()
                {
                    Errors = new Errors() { Message = "User exists" },
                    Success = false
                });
            }

            var userDataModel = _mapper.Map<User>(userData);

            userDataModel.Password = BCrypt.Net.BCrypt.HashPassword(userData.Password);
            DateTime now = DateTime.UtcNow;
            userDataModel.CreatedAt = now;
            userDataModel.UpdatedAt = now;
            _context.Users.Add(userDataModel);
            await _context.SaveChangesAsync();

            User createdUser = await GetUser(userData.Email);
            var jwtToken = GenerateJwtToken(createdUser);

            return Ok(new UserRegisterResponse()
            {
                Success = true,
                Token = jwtToken,
                User = _mapper.Map<UserLoginInfoDto>(createdUser)
            });
        }

        private async Task<User> GetUser(string email)
        {
            User user = await _context.Users.FirstOrDefaultAsync(user => user.Email == email || user.UserName == email);

            if (user == null)
            {
                return null;
            }

            return user;
        }

        private string GenerateJwtToken(User user)
        {
            string userRole = user.Admin ? "admin" : "user";
            //create claims details based on the user information
            var claims = new[] {
                    new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                    new Claim("Id", user.Id.ToString()),
                    new Claim("FirstName", user.FirstName),
                    new Claim("LastName", user.LastName),
                    new Claim("UserName", user.UserName),
                    new Claim("Email", user.Email),
                    new Claim(ClaimTypes.Role, userRole)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

            var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_configuration["Jwt:Issuer"], _configuration["Jwt:Audience"], claims, expires: DateTime.UtcNow.AddDays(1), signingCredentials: signIn);

            var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

            return jwtToken;
        }

        private static bool VerifyPassword(string password, string passwordHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }
    }
}
