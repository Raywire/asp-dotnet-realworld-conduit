using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Conduit.Helpers;
using Conduit.Models;
using Conduit.ResourceParameters;

namespace Conduit.Services
{
    public interface IConduitRepository
    {
        Task<PagedList<Article>> GetArticlesAsync(ArticlesResourceParameters articlesResourceParameters);
        Task<Article> GetArticleAsync(string slug);
        Task AddArticleAsync(Article article);
        void DeleteArticle(Article article);
        Task<IEnumerable<Comment>> GetArticleCommentsAsync(Guid articleId);
        Task<Comment> GetArticleCommentAsync(Guid commentId);
        Task AddArticleCommentAsync(Comment comment);
        void DeleteArticleComment(Comment comment);
        bool ArticleExists(Guid articleId);
        Task<int> SaveChangesAsync();
    }
}
