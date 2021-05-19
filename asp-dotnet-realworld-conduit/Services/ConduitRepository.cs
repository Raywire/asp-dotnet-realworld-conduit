using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Conduit.Data;
using Conduit.Helpers;
using Conduit.Models;
using Conduit.ResourceParameters;
using Microsoft.EntityFrameworkCore;

namespace Conduit.Services
{
    public class ConduitRepository : IConduitRepository
    {
        private readonly ConduitContext _context;

        public ConduitRepository(ConduitContext context)
        {
            _context = context;
        }

        public async Task AddArticleAsync(Article article)
        {
            await _context.Article.AddAsync(article);
        }

        public async Task AddArticleCommentAsync(Comment comment)
        {
            await _context.Comments.AddAsync(comment);
        }

        public bool ArticleExists(Guid articleId)
        {
            return _context.Article.Any(e => e.Id == articleId);
        }

        public void DeleteArticle(Article article)
        {
            _context.Article.Remove(article);
        }

        public void DeleteArticleComment(Comment comment)
        {
            _context.Comments.Remove(comment);
        }

        public async Task<Article> GetArticleAsync(string slug, string currentUserId)
        {
            if (currentUserId != null)
            {
                return await _context.Article.Include(a => a.Author).Include(a => a.Favorites.Where(f => f.AuthorId == Guid.Parse(currentUserId))).Where(a => a.Slug == slug).FirstOrDefaultAsync();
            }
            return await _context.Article.Include(a => a.Author).Where(a => a.Slug == slug).FirstOrDefaultAsync();
        }

        public async Task<Comment> GetArticleCommentAsync(Guid commentId)
        {
            return await _context.Comments.FindAsync(commentId);
        }

        public async Task<IEnumerable<Comment>> GetArticleCommentsAsync(Guid articleId)
        {
            return await _context.Comments.Include(c => c.Author).Where(c => c.ArticleId == articleId).ToListAsync();
        }

        public async Task<PagedList<Article>> GetArticlesAsync(ArticlesResourceParameters articlesResourceParameters, string currentUserId)
        {
            if(articlesResourceParameters == null)
            {
                throw new ArgumentNullException(nameof(articlesResourceParameters));
            }

            var collection = _context.Article.Include(a => a.Author) as IQueryable<Article>;

            if (currentUserId != null)
            {
                collection = collection.Include(a => a.Favorites.Where(f => f.AuthorId == Guid.Parse(currentUserId)));
            }

            if (!string.IsNullOrWhiteSpace(articlesResourceParameters.Author))
            {
                var author = articlesResourceParameters.Author.Trim();
                collection = collection.Include(a => a.Author).Where(a => a.Author.UserName == author || a.Author.Email == author);
            }

            if (!string.IsNullOrWhiteSpace(articlesResourceParameters.Search))
            {
                var search = articlesResourceParameters.Search.Trim();
                collection = collection.Include(a => a.Author).Where(a => a.Author.FirstName.Contains(search)
                    || a.Author.LastName.Contains(search)
                    || a.Author.UserName.Contains(search)
                    || a.Title.Contains(search));
            }

            return await PagedList<Article>.Create(collection, articlesResourceParameters.PageNumber, articlesResourceParameters.PageSize);

        }

        public async Task<PagedList<Article>> GetArticlesFeedAsync(ArticlesResourceParameters articlesResourceParameters, Guid currentUserId)
        {
            if (articlesResourceParameters == null)
            {
                throw new ArgumentNullException(nameof(articlesResourceParameters));
            }

            var collection = _context.Article.Include(a => a.Author).Include(a => a.Favorites.Where(f => f.AuthorId == currentUserId)).Where(c => c.AuthorId == currentUserId);

            if (!string.IsNullOrWhiteSpace(articlesResourceParameters.Search))
            {
                var search = articlesResourceParameters.Search.Trim();
                collection = collection.Include(a => a.Author).Where(a => a.Title.Contains(search));
            }

            return await PagedList<Article>.Create(collection, articlesResourceParameters.PageNumber, articlesResourceParameters.PageSize);
        }

        public async Task AddArticleFavoriteAsync(Favorite favorite)
        {
            await _context.Favorites.AddAsync(favorite);
        }

        public void DeleteArticleFavorite(Favorite favorite)
        {
            _context.Favorites.Remove(favorite);
        }

        public async Task<Favorite> GetArticleFavoriteAsync(Guid articleId, Guid userId)
        {
            return await _context.Favorites.Where(c => c.ArticleId == articleId && c.AuthorId == userId).FirstOrDefaultAsync();
        }

        public async Task<User> GetUserAsync(Guid id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<PagedList<User>> GetUsersAsync(UsersResourceParameters usersResourceParameters)
        {
            if (usersResourceParameters == null)
            {
                throw new ArgumentNullException(nameof(usersResourceParameters));
            }
            var collection = _context.Users as IQueryable<User>;

            return await PagedList<User>.Create(collection, usersResourceParameters.PageNumber, usersResourceParameters.PageSize);
        }

        public void DeleteUser(User user)
        {
            _context.Users.Remove(user);
        }

        public bool UserExists(Guid userId)
        {
            return _context.Users.Any(e => e.Id == userId);
        }

        public async Task<User> GetUserByEmailOrUsernameAsync(string email, string username)
        {
            return await _context.Users.FirstOrDefaultAsync(user => user.Email == email || user.UserName == username);
        }

        public async Task AddUserAsync(User user)
        {
            await _context.Users.AddAsync(user);
        }

        public async Task AddProfileFollowAsync(Follow follow)
        {
            await _context.Follows.AddAsync(follow);
        }

        public void DeleteProfileFollow(Follow follow)
        {
            _context.Follows.Remove(follow);
        }

        public async Task<Follow> GetProfileFollowAsync(Guid followingId, Guid authorId)
        {
            return await _context.Follows.Where(c => c.FollowingId == followingId && c.AuthorId == authorId).FirstOrDefaultAsync();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
