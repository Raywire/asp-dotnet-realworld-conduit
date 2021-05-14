using System;
using System.ComponentModel.DataAnnotations;

namespace Conduit.DTOs.Requests
{
    public class UserUpdateRequestDto
    {
        [Required]
        public Guid Id { get; set; }

        [MaxLength(32)]
        public string FirstName { get; set; }

        [MaxLength(32)]
        public string LastName { get; set; }

        [MaxLength(32)]
        public string UserName { get; set; }
    }
}
