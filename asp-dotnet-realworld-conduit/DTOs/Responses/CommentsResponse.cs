using System;
using System.Collections.Generic;
using System.Linq;

namespace Conduit.DTOs.Responses
{
    public class CommentsResponse
    {
        public bool Success { get; set; }
        public IEnumerable<CommentsResponseDto> Comments { get; set; }
        public int CommentsCount
        {
            get
            {
                return Comments.Count();
            }
        }
    }
}
