using System;
using Conduit.Models;

namespace Conduit.DTOs.Responses
{
    public class FollowResponseDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
    }
}