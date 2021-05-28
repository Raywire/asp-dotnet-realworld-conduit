using System.ComponentModel.DataAnnotations;

namespace Conduit.DTOs.Requests
{
    public class UserRegisterRequestWrapper
    {
        [Required]
        public UserRegisterRequestDto User { get; set; }
    }
    public class UserRegisterRequestDto
    {
        [Required]
        [MaxLength(32)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(32)]
        public string LastName { get; set; }

        [Required]
        [MaxLength(32)]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(128)]
        public string Email { get; set; }

        [Required]
        [MaxLength(32)]
        public string Password { get; set; }
    }
}
