using System;
using System.ComponentModel.DataAnnotations;

namespace Conduit.DTOs.Requests
{
    public class UserLoginRequestDto
    {
        [Required]
        [EmailAddress]
        [MaxLength(64)]
        public string Email { get; set; }

        [Required]
        [MaxLength(32)]
        public string Password { get; set; }
    }
}
