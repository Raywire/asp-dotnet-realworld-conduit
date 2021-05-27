using System;
using Conduit.DTOs.Responses;

namespace Conduit.Models
{
    public class AuthResult
    {
        public bool Success { get; set; }
        public string Token { get; set; }
        public UserLoginInfoDto User { get; set; }
    }
}
