using System;
using System.Collections.Generic;
using asp_dotnet_realworld_conduit.Models;

namespace asp_dotnet_realworld_conduit.DTOs.Responses
{
    public class UsersResponse
    {
        public bool Success { get; set; }
        public IEnumerable<UsersResponseDto> Users { get; set; }
    }
}
