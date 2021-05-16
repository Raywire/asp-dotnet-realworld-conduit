using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Conduit.Data;
using Conduit.Models;
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

        public async Task<IEnumerable<Article>> GetArticlesAsync(string author)
        {
            if (string.IsNullOrWhiteSpace(author))
            {
                return await _context.Article.Include(a => a.Author).ToListAsync();
            }

            author = author.Trim();

            return await _context.Article.Include(a => a.Author).Where(a => a.Author.FirstName == author).ToListAsync();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
