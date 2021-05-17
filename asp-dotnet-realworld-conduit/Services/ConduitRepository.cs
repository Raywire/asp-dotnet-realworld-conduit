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

        public async Task<Article> GetArticleAsync(string slug)
        {
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

        public async Task<PagedList<Article>> GetArticlesAsync(ArticlesResourceParameters articlesResourceParameters)
        {
            if(articlesResourceParameters == null)
            {
                throw new ArgumentNullException(nameof(articlesResourceParameters));
            }

            var collection = _context.Article.Include(a => a.Author) as IQueryable<Article>;

            if (!string.IsNullOrWhiteSpace(articlesResourceParameters.Author))
            {
                var author = articlesResourceParameters.Author.Trim();
                collection = collection.Include(a => a.Author).Where(a => a.Author.FirstName == author || a.Author.LastName == author);
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

        public async Task<PagedList<Article>> GetArticlesFeedAsync(ArticlesResourceParameters articlesResourceParameters, Guid authorId)
        {
            if (articlesResourceParameters == null)
            {
                throw new ArgumentNullException(nameof(articlesResourceParameters));
            }

            var collection = _context.Article.Include(a => a.Author).Where(c => c.AuthorId == authorId);

            if (!string.IsNullOrWhiteSpace(articlesResourceParameters.Search))
            {
                var search = articlesResourceParameters.Search.Trim();
                collection = collection.Include(a => a.Author).Where(a => a.Title.Contains(search));
            }

            return await PagedList<Article>.Create(collection, articlesResourceParameters.PageNumber, articlesResourceParameters.PageSize);
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

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
