using System;
using System.Collections.Generic;
using Conduit.Models;

namespace Conduit.DTOs.Responses
{
    public class UsersResponse
    {
        public bool Success { get; set; }
        public Metadata Metadata { get; set; }
        public IEnumerable<UsersResponseDto> Users { get; set; }
    }
}
