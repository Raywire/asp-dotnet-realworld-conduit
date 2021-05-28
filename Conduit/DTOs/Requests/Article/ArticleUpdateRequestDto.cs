using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Conduit.DTOs.Requests
{
    public class ArticleUpdateRequestWrapper
    {
        [Required]
        public ArticleUpdateRequestDto Article { get; set; }
    }
    public class ArticleUpdateRequestDto
    {
        [Required]
        [MaxLength(128)]
        public string Title { get; set; }

        [Required]
        [MaxLength(1200)]
        public string Description { get; set; }

        [Required]
        [MaxLength(64)]
        public List<string> TagList { get; set; } = new List<string>();
    }
}
