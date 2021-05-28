using System.ComponentModel.DataAnnotations;

namespace Conduit.DTOs.Requests
{
    public class CommentCreateRequestWrapper
    {
        [Required]
        public CommentCreateRequestDto Comment { get; set; }
    }
    public class CommentCreateRequestDto
    {
        [Required]
        [MaxLength(256)]
        public string Body { get; set; }
    }
}
