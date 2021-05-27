using System;
using System.ComponentModel.DataAnnotations;

namespace Conduit.DTOs.Requests
{
    public class CommentCreateRequestDto
    {
        [Required]
        [MaxLength(256)]
        public string Body { get; set; }
    }
}
