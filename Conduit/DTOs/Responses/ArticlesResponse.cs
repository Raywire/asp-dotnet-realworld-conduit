using System;
using System.Collections.Generic;
using System.Linq;

namespace Conduit.DTOs.Responses
{
    public class ArticlesResponse
    {
        public bool Success { get; set; }
        public Metadata Metadata { get; set; }
        public IEnumerable<ArticlesResponseDto> Articles { get; set; }
        public int ArticlesCount
        {
            get
            {
                return Articles.Count();
            }
        }
    }
}
