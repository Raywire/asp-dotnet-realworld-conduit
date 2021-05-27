using System;
namespace Conduit.DTOs.Responses
{
    public class ArticleResponse
    {
        public bool Success { get; set; }
        public ArticlesResponseDto Article { get; set; }
    }
}
