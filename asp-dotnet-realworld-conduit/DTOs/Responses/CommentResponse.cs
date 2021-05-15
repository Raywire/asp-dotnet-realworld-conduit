using System;
namespace Conduit.DTOs.Responses
{
    public class CommentResponse
    {
        public bool Success { get; set; }
        public CommentsResponseDto Comment { get; set; }
    }
}
