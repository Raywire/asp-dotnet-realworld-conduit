using System;
using System.Collections.Generic;

namespace Conduit.DTOs.Responses.Tag
{
    public class TagsResponse
    {
        public bool Success { get; set; } = true;
        public IEnumerable<string> Tags { get; set; }
    }
}
