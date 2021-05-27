using System;
using Conduit.Models;

namespace Conduit.DTOs.Responses
{
    public class CommentsResponseDto
    {
        public Guid Id { get; set; }
        public string Body { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public AuthorResponseDto Author { get; set; }
    }
}
