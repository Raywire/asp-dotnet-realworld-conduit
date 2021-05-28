using System;

namespace Conduit.Models
{
    public class ArticleTag
    {
        public Guid ArticleId { get; set; }
        public Article Article { get; set; }
        public string TagId { get; set; }
        public Tag Tag { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}