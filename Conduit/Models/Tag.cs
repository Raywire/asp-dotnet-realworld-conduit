using System;
using System.Collections.Generic;

namespace Conduit.Models
{
    public class Tag
    {
        public string TagId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<ArticleTag> ArticleTags { get; set; }
    }
}