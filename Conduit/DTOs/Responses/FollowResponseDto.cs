using System;
using Conduit.Models;

namespace Conduit.DTOs.Responses
{
    public class FollowingResponseDto
    {
        public Guid FollowingId { get; set; }
        // public AuthorResponseDto Follower { get; set; }
    }

    public class FollowerResponseDto
    {
        public Guid FollowerId { get; set; }

        // public AuthorResponseDto Follower { get; set; }
    }
}