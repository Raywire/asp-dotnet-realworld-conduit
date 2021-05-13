﻿using System;
using System.ComponentModel.DataAnnotations;

namespace asp_dotnet_realworld_conduit.Models
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }

        [MaxLength(32)]
        [Required]
        public string FirstName { get; set; }

        [MaxLength(32)]
        [Required]
        public string LastName { get; set; }

        [MaxLength(32)]
        public string UserName { get; set; }

        [MaxLength(64)]
        [Required]
        public string Email { get; set; }

        [MaxLength(64)]
        [Required]
        public string Password { get; set; }

        [MaxLength(256)]
        public string Bio { get; set; }

        [MaxLength(256)]
        public string Photo { get; set; }

        public bool Admin { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
