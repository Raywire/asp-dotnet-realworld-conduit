using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Conduit.Models;

namespace Conduit.Services
{
    public interface IConduitRepository
    {
        Task<IEnumerable<Article>> GetArticlesAsync(string author);
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
