using System;
using System.Collections.Generic;
using Conduit.Models;

namespace Conduit.DTOs.Responses
{
    public class ProfileResponseDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Bio { get; set; }
        public string Photo { get; set; }
        public bool IsFollowing { get; set; }
        public List<FollowResponseDto> Following { get; set; }
        public List<FollowResponseDto> Followers { get; set; }
    }
}
