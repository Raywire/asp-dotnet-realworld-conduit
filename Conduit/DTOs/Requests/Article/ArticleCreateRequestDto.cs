using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Conduit.Models;

namespace Conduit.DTOs.Requests
{
    public class ArticleCreateRequestWrapper
    {
        [Required]
        public ArticleCreateRequestDto Article { get; set; }
    }
    public class ArticleCreateRequestDto
    {
        [Required]
        [MaxLength(128)]
        public string Title { get; set; }

        [Required]
        [MaxLength(1200)]
        public string Description { get; set; }

        [MaxLength(64)]
        public List<string> TagList { get; set; } = new List<string>();
    }
}
