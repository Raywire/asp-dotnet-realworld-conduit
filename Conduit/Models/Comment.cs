﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Conduit.Models
{
    public class Comment
    {
        [Key]
        public Guid Id { get; set; }

        [MaxLength(256)]
        [Required]
        public string Body { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        [ForeignKey("AuthorId")]
        public User Author { get; set; }

        public Guid AuthorId { get; set; }

        [ForeignKey("ArticleId")]
        public Article Article { get; set; }

        public Guid ArticleId { get; set; }
    }
}
