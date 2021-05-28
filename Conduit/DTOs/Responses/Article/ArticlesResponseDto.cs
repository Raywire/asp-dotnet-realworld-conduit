using System;
using System.Collections.Generic;

namespace Conduit.DTOs.Responses
{
    public class ArticlesResponseDto
    {
        public Guid Id { get; set; }

        public string Slug { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public List<string> TagList { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public bool Favorited { get; set; }

        public int FavoritesCount { get; set; }

        public AuthorResponseDto Author { get; set; }
    }
}
