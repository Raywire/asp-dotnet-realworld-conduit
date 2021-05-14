using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Conduit.Models
{
    public class Article
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Slug { get; set; }

        [Required]
        [MaxLength(128)]
        public string Title { get; set; }

        [MaxLength(1200)]
        public string Description { get; set; }

        public List<string> TagList { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public bool Favorited { get; set; }

        public int FavoritesCount { get; set; }

        [ForeignKey("AuthorId")]
        public Users Author { get; set; }

        public Guid AuthorId { get; set; }
    }
}
