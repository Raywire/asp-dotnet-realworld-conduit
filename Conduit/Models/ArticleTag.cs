using System;

namespace Conduit.Models
{
    public class ArticleTag : BaseEntity
    {
        public Guid ArticleId { get; set; }
        public Article Article { get; set; }
        public string TagId { get; set; }
        public Tag Tag { get; set; }
    }
}