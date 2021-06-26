using System;
using System.Collections.Generic;

namespace Conduit.Models
{
    public class Tag : BaseEntity
    {
        public string TagId { get; set; }
        public List<ArticleTag> ArticleTags { get; set; }
    }
}