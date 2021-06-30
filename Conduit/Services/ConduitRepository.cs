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
            await _context.Articles.AddAsync(article);
        }

        public async Task AddArticleCommentAsync(Comment comment)
        {
            await _context.Comments.AddAsync(comment);
        }

        public bool ArticleExists(Guid articleId)
        {
            return _context.Articles.Any(e => e.Id == articleId);
        }

        public void DeleteArticle(Article article)
        {
            _context.Articles.Remove(article);
        }

        public void DeleteArticleComment(Comment comment)
        {
            _context.Comments.Remove(comment);
        }

        public async Task<Article> GetArticleAsync(string slug, string currentUserId)
        {
            if (currentUserId != null)
            {
                return await _context.Articles
                    .Include(a => a.Author)
                    .Include(a => a.ArticleTags)
                    .Include(a => a.Favorites.Where(f => f.AuthorId == Guid.Parse(currentUserId)))
                    .Where(a => a.Slug == slug)
                    .AsSingleQuery()
                    .FirstOrDefaultAsync();
            }
            return await _context.Articles
                .Include(a => a.Author)
                .Include(a => a.ArticleTags)
                .Where(a => a.Slug == slug)
                .AsSingleQuery()
                .FirstOrDefaultAsync();
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

            var collection = _context.Articles.Include(a => a.Author).Include(a => a.ArticleTags).AsSplitQuery();

            if (currentUserId != null)
            {
                collection = collection.Include(a => a.Favorites.Where(f => f.AuthorId == Guid.Parse(currentUserId)));
            }

            if (!string.IsNullOrWhiteSpace(articlesResourceParameters.Author))
            {
                var author = articlesResourceParameters.Author.Trim();
                collection = collection.Where(a => a.Author.UserName == author || a.Author.Email == author);
            }

            if (!string.IsNullOrWhiteSpace(articlesResourceParameters.Search))
            {
                var search = articlesResourceParameters.Search.Trim();
                collection = collection.Where(a => a.Author.FirstName.Contains(search)
                    || a.Author.LastName.Contains(search)
                    || a.Author.UserName.Contains(search)
                    || a.Title.Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(articlesResourceParameters.Tag))
            {
                var tag = articlesResourceParameters.Tag.Trim();
                var articleTag = await _context.ArticleTags.FirstOrDefaultAsync(x => x.TagId == tag);
                if (articleTag != null)
                {
                    collection = collection.Where(x => x.ArticleTags.Select(c => c.TagId).Contains(articleTag.TagId));
                }
            }

            if (!string.IsNullOrWhiteSpace(articlesResourceParameters.Favorited))
            {
                var favorited = articlesResourceParameters.Favorited.Trim();
                var author = await _context.Users.FirstOrDefaultAsync(u => u.UserName == favorited);

                if (author != null)
                {
                    collection = collection.Where(a => a.Favorites.Any(f => f.AuthorId == author.Id));
                }
            }

            return await PagedList<Article>.Create(collection, articlesResourceParameters.PageNumber, articlesResourceParameters.PageSize);

        }

        public async Task<PagedList<Article>> GetArticlesFeedAsync(ArticlesResourceParameters articlesResourceParameters, Guid currentUserId)
        {
            if (articlesResourceParameters == null)
            {
                throw new ArgumentNullException(nameof(articlesResourceParameters));
            }

            var currentUser = await _context.Users.Include(f => f.Following).FirstOrDefaultAsync(u => u.Id == currentUserId);

            var collection = _context.Articles
                .Where(a => currentUser.Following.Select(y => y.FollowingId).Contains(a.Author.Id))
                .Include(a => a.Author)
                .Include(a => a.ArticleTags)
                .Include(a => a.Favorites.Where(f => f.AuthorId == currentUserId))
                .AsSplitQuery();

            if (!string.IsNullOrWhiteSpace(articlesResourceParameters.Search))
            {
                var search = articlesResourceParameters.Search.Trim();
                collection = collection.Where(a => a.Title.Contains(search));
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

        public async Task<User> GetProfileAsync(Guid userId)
        {
            var user = await _context.Users
                .Include(f => f.Followers).ThenInclude(f => f.Follower)
                .Include(f => f.Following).ThenInclude(f => f.Following)
                .AsSingleQuery()
                .Where(u => u.Id == userId)
                .FirstOrDefaultAsync();
            return user;
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
            return await _context.Users
                .Include(f => f.Followers).ThenInclude(f => f.Follower)
                .Include(f => f.Following).ThenInclude(f => f.Following)
                .AsSingleQuery()
                .FirstOrDefaultAsync(user => user.Email == email || user.UserName == username);
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
            return await _context.Follows.Where(c => c.FollowingId == followingId && c.FollowerId == authorId).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Tag>> GetTagsAsync()
        {
            return await _context.Tags.OrderBy(t => t.TagId).ToListAsync();
        }

        public async Task AddTagAsync(Tag tag)
        {
            await _context.Tags.AddAsync(tag);
        }

        public bool TagExists(string tagId)
        {
            return _context.Tags.Any(t => t.TagId == tagId);
        }

        public async Task<Tag> GetTagAsync(string tagId)
        {
            return await _context.Tags.FindAsync(tagId);
        }


        public async Task<ArticleTag> GetArticleTagAsync(string tagId, Guid articleId)
        {
            return await _context.ArticleTags.Where(at => at.TagId == tagId && at.ArticleId == articleId).FirstOrDefaultAsync();
        }

        public async Task AddArticleTagsAsync(List<Tag> tags, Article article)
        {
            await _context.ArticleTags.AddRangeAsync(tags.Select(x => new ArticleTag()
            {
                Article = article,
                Tag = x
            }));
        }

        public void DeleteArticleTags(List<ArticleTag> articleTags)
        {
            _context.ArticleTags.RemoveRange(articleTags);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
