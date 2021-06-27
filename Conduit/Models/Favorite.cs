﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Conduit.Models
{
    public class Favorite : BaseEntity
    {
        [Key]
        public Guid Id { get; set; }

        [ForeignKey("AuthorId")]
        public User Author { get; set; }

        public Guid AuthorId { get; set; }

        [ForeignKey("ArticleId")]
        public Article Article { get; set; }

        public Guid ArticleId { get; set; }
    }
}
