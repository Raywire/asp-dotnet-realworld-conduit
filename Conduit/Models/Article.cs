﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Conduit.Models
{
    public class Article : BaseEntity
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(164)]
        public string Slug { get; set; }

        [Required]
        [MaxLength(128)]
        public string Title { get; set; }

        [MaxLength(256)]
        public string Description { get; set; }

        [MaxLength(1200)]
        public string Body { get; set; }

        [NotMapped]
        public bool Favorited => Favorites?.Any() ?? false;

        [NotMapped]
        public int FavoritesCount => Favorites?.Count ?? 0;

        [ForeignKey("AuthorId")]
        public User Author { get; set; }

        public Guid AuthorId { get; set; }

        public List<Favorite> Favorites { get; set; }

        public List<ArticleTag> ArticleTags { get; set; }

        public List<string> TagList => (ArticleTags?.Select(x => x.TagId) ?? Enumerable.Empty<string>()).ToList();
    }
}
