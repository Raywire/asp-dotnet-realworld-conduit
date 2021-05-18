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
        Task<PagedList<Article>> GetArticlesAsync(ArticlesResourceParameters articlesResourceParameters, string currentUserId = null);
        Task<PagedList<Article>> GetArticlesFeedAsync(ArticlesResourceParameters articlesResourceParameters, Guid currentUserId);
        Task<Article> GetArticleAsync(string slug, string currentUserId = null);
        Task AddArticleAsync(Article article);
        void DeleteArticle(Article article);
        Task<IEnumerable<Comment>> GetArticleCommentsAsync(Guid articleId);
        Task<Comment> GetArticleCommentAsync(Guid commentId);
        Task AddArticleCommentAsync(Comment comment);
        void DeleteArticleComment(Comment comment);
        Task AddArticleFavoriteAsync(Favorite favorite);
        void DeleteArticleFavorite(Favorite favorite);
        Task<Favorite> GetArticleFavoriteAsync(Guid articleId, Guid userId);
        bool ArticleExists(Guid articleId);

        Task<PagedList<User>> GetUsersAsync(UsersResourceParameters usersResourceParameters);
        Task<User> GetUserAsync(Guid id);
        void DeleteUser(User user);
        bool UserExists(Guid userId);
        Task<User> GetUserByEmailOrUsernameAsync(string email, string username);
        Task AddUserAsync(User user);

        Task<int> SaveChangesAsync();
    }
}
