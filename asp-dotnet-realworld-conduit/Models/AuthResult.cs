using System;
using asp_dotnet_realworld_conduit.DTOs.Responses;

namespace asp_dotnet_realworld_conduit.Models
{
    public class AuthResult
    {
        public bool Success { get; set; }
        public string Token { get; set; }
        public UserLoginInfoDto User { get; set; }
    }
}
