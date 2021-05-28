using System.ComponentModel.DataAnnotations;

namespace Conduit.DTOs.Requests
{
    public class UserUpdateRequestWrapper
    {
        [Required]
        public UserUpdateRequestDto User { get; set; }
    }
    public class UserUpdateRequestDto
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
        [MaxLength(128)]
        [EmailAddress]
        public string Email { get; set; }
    }
}
