using System;
namespace Conduit.DTOs.Responses
{
    public class UserResponse
    {
        public bool Success { get; set; }
        public UsersResponseDto User { get; set; }
    }
}
