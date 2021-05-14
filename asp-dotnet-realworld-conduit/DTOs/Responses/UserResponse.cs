using System;
namespace asp_dotnet_realworld_conduit.DTOs.Responses
{
    public class UserResponse
    {
        public bool Success { get; set; }
        public UsersResponseDto User { get; set; }
    }
}
