using System;
using System.ComponentModel.DataAnnotations;

namespace Conduit.DTOs.Requests
{
    public class ArticleCreateRequestDto
    {
        [Required]
        [MaxLength(128)]
        public string Title { get; set; }

        [Required]
        [MaxLength(1200)]
        public string Description { get; set; }

        [MaxLength(64)]
        public string TagList { get; set; }
    }
}
