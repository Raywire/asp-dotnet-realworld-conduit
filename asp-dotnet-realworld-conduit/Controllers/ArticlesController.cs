using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Conduit.Models;
using AutoMapper;
using Conduit.DTOs.Responses;
using Microsoft.AspNetCore.Authorization;
using Conduit.DTOs.Requests;
using System.Security.Claims;
using Slugify;
using Conduit.Services;
using Conduit.ResourceParameters;
using System.Text.Json;
using Conduit.Helpers;

namespace Conduit.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ArticlesController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IConduitRepository _repository;

        public ArticlesController(IMapper mapper, IConduitRepository repository)
        {
            _mapper = mapper;
            _repository = repository;
        }

        // GET: api/Articles
        [AllowAnonymous]
        [HttpGet(Name = "GetArticles")]
        public async Task<ActionResult<ArticlesResponse>> GetArticles([FromQuery] ArticlesResourceParameters articlesResourceParameters)
        {
            var articles = await _repository.GetArticlesAsync(articlesResourceParameters);

            var previousPageLink = articles.HasPrevious ?
                CreateArticlesResourceUri(articlesResourceParameters, ResourceUriType.PreviousPage, "GetArticles") : null;

            var nextPageLink = articles.HasNext ?
                CreateArticlesResourceUri(articlesResourceParameters, ResourceUriType.NextPage, "GetArticles") : null;

            var paginationMetadata = new
            {

                totalCount = articles.TotalCount,
                pageSize = articles.PageSize,
                currentPage = articles.CurrentPage,
                totalPages = articles.TotalPages,
                previousPageLink,
                nextPageLink
            };

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata));

            return Ok(new ArticlesResponse()
            {
                Success = true,
                Metadata = new Metadata() {
                    TotalCount = articles.TotalCount,
                    PageSize = articles.PageSize,
                    CurrentPage = articles.CurrentPage,
                    TotalPages = articles.TotalPages,
                    PreviousPageLink = previousPageLink,
                    NextPageLink = nextPageLink
                },
                Articles = _mapper.Map<IEnumerable<ArticlesResponseDto>>(articles)
            });
        }

        // GET: api/Articles/feed
        [HttpGet("feed", Name = "GetArticlesFeed")]
        public async Task<ActionResult<ArticlesResponse>> GetArticlesFeed([FromQuery] ArticlesResourceParameters articlesResourceParameters)
        {
            var currentUserId = Guid.Parse(GetCurrentUserId());
            var articles = await _repository.GetArticlesFeedAsync(articlesResourceParameters, currentUserId);

            var previousPageLink = articles.HasPrevious ?
                CreateArticlesResourceUri(articlesResourceParameters, ResourceUriType.PreviousPage, "GetArticlesFeed") : null;

            var nextPageLink = articles.HasNext ?
                CreateArticlesResourceUri(articlesResourceParameters, ResourceUriType.NextPage, "GetArticlesFeed") : null;

            var paginationMetadata = new
            {

                totalCount = articles.TotalCount,
                pageSize = articles.PageSize,
                currentPage = articles.CurrentPage,
                totalPages = articles.TotalPages,
                previousPageLink,
                nextPageLink
            };

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata));

            return Ok(new ArticlesResponse()
            {
                Success = true,
                Metadata = new Metadata()
                {
                    TotalCount = articles.TotalCount,
                    PageSize = articles.PageSize,
                    CurrentPage = articles.CurrentPage,
                    TotalPages = articles.TotalPages,
                    PreviousPageLink = previousPageLink,
                    NextPageLink = nextPageLink
                },
                Articles = _mapper.Map<IEnumerable<ArticlesResponseDto>>(articles)
            });
        }

        // GET: api/Articles/hello-world
        [AllowAnonymous]
        [HttpGet("{slug}")]
        public async Task<ActionResult<ArticleResponse>> GetArticle(string slug)
        {
            var article = await _repository.GetArticleAsync(slug);

            if (article == null)
            {
                return NotFound(new ErrorResponse()
                {
                    Success = false,
                    Errors = new Errors() { Message = "Article not found" }
                });
            }

            return Ok(new ArticleResponse()
            {
                Success = true,
                Article = _mapper.Map<ArticlesResponseDto>(article)
            });
        }

        // PUT: api/Articles/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{slug}")]
        public async Task<IActionResult> PutArticle(string slug, ArticleUpdateRequestDto articleUpdateRequestDto)
        {
            var article = await _repository.GetArticleAsync(slug);

            if (article == null)
            {
                return NotFound(new ErrorResponse()
                {
                    Success = false,
                    Errors = new Errors() { Message = "Article not found" }
                });
            }

            var currentUserId = GetCurrentUserId();
            if (currentUserId != article.AuthorId.ToString())
            {
                return StatusCode(403, new ErrorResponse()
                {
                    Success = false,
                    Errors = new Errors() { Message = "Only the owner can update this article" }
                });
            }

            if (articleUpdateRequestDto.Title != article.Title)
            {
                article.Slug = GenerateSlug(articleUpdateRequestDto.Title);
            }

            _mapper.Map(articleUpdateRequestDto, article);

            try
            {
                await _repository.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ArticleExists(article.Id))
                {
                    return NotFound(new ErrorResponse()
                    {
                        Success = false,
                        Errors = new Errors() { Message = "Article not found" }
                    });
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Articles
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ArticlesResponse>> PostArticle(ArticleCreateRequestDto articleCreateDto)
        {
            // Get current user Id
            var userId = GetCurrentUserId();

            var user = await _repository.GetUserAsync(Guid.Parse(userId));

            var articleModel = _mapper.Map<Article>(articleCreateDto);
            articleModel.Author = user;

            articleModel.Slug = GenerateSlug(articleModel.Title);

            await _repository.AddArticleAsync(articleModel);
            await _repository.SaveChangesAsync();

            var articleReadDto = _mapper.Map<ArticlesResponseDto>(articleModel);

            return CreatedAtAction("GetArticle", new { slug = articleReadDto.Slug }, new ArticleResponse()
            {
                Success = true,
                Article = articleReadDto
            });
        }

        // DELETE: api/Articles/5
        [HttpDelete("{slug}")]
        public async Task<IActionResult> DeleteArticle(string slug)
        {
            var article = await _repository.GetArticleAsync(slug);

            if (article == null)
            {
                return NotFound(new ErrorResponse()
                {
                    Success = false,
                    Errors = new Errors() { Message = "Article not found" }
                });
            }

            var currentUserId = GetCurrentUserId();
            if (currentUserId != article.AuthorId.ToString())
            {
                return StatusCode(403, new ErrorResponse()
                {
                    Success = false,
                    Errors = new Errors() { Message = "Only the owner can delete this article" }
                });
            }

            _repository.DeleteArticle(article);
            await _repository.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("{slug}/comments")]
        public async Task<IActionResult> GetArticleComments(string slug)
        {
            var article = await _repository.GetArticleAsync(slug);

            if (article == null)
            {
                return NotFound(new ErrorResponse()
                {
                    Success = false,
                    Errors = new Errors() { Message = "Article not found" }
                });
            }

            var comments = await _repository.GetArticleCommentsAsync(article.Id);

            return Ok(new CommentsResponse()
            {
                Success = true,
                Comments = _mapper.Map<IEnumerable<CommentsResponseDto>>(comments)
            });
        }

        [HttpPost]
        [Route("{slug}/comments")]
        public async Task<IActionResult> PostArticleComment(string slug, CommentCreateRequestDto commentCreateDto)
        {
            var article = await _repository.GetArticleAsync(slug);

            if (article == null)
            {
                return NotFound(new ErrorResponse()
                {
                    Success = false,
                    Errors = new Errors() { Message = "Article not found" }
                });
            }

            // Get current user Id
            var userId = GetCurrentUserId();

            var commentModel = _mapper.Map<Comment>(commentCreateDto);
            commentModel.AuthorId = Guid.Parse(userId);
            commentModel.ArticleId = article.Id;

            await _repository.AddArticleCommentAsync(commentModel);
            await _repository.SaveChangesAsync();

            var commentReadDto = _mapper.Map<CommentsResponseDto>(commentModel);

            return StatusCode(201, new CommentResponse()
            {
                Success = true,
                Comment = commentReadDto
            });
        }

        [HttpDelete]
        [Route("{slug}/comments/{id}")]
        public async Task<IActionResult> DeleteComment(string slug, Guid id)
        {
            var article = await _repository.GetArticleAsync(slug);

            if (article == null)
            {
                return NotFound(new ErrorResponse()
                {
                    Success = false,
                    Errors = new Errors() { Message = "Article not found" }
                });
            }

            var currentUserId = GetCurrentUserId();
            if (currentUserId != article.AuthorId.ToString())
            {
                return StatusCode(403, new ErrorResponse()
                {
                    Success = false,
                    Errors = new Errors() { Message = "Only the owner can delete this comment" }
                });
            }

            var comment = await _repository.GetArticleCommentAsync(id);
            if (comment == null)
            {
                return NotFound(new ErrorResponse()
                {
                    Success = false,
                    Errors = new Errors() { Message = "Comment not found" }
                });
            }

            _repository.DeleteArticleComment(comment);
            await _repository.SaveChangesAsync();

            return NoContent();
        }


        [HttpPost]
        [Route("{slug}/favorite")]
        public async Task<IActionResult> PostArticleFavorite(string slug)
        {
            var article = await _repository.GetArticleAsync(slug);

            if (article == null)
            {
                return NotFound(new ErrorResponse()
                {
                    Success = false,
                    Errors = new Errors() { Message = "Article not found" }
                });
            }

            // Get current user Id
            var currentUserId = GetCurrentUserId();

            var favoriteModel = new Favorite() {
                AuthorId = Guid.Parse(currentUserId),
                ArticleId = article.Id
            };

            var favorite = await _repository.GetArticleFavoriteAsync(article.Id, Guid.Parse(currentUserId));

            if (favorite == null)
            {
                await _repository.AddArticleFavoriteAsync(favoriteModel);
                await _repository.SaveChangesAsync();
            }

            var articleReadDto = _mapper.Map<ArticlesResponseDto>(article);

            int statusCode = favorite == null ? 201 : 200;

            return StatusCode(statusCode, new ArticleResponse()
            {
                Success = true,
                Article = articleReadDto
            });
        }

        [HttpDelete]
        [Route("{slug}/favorite")]
        public async Task<IActionResult> DeleteArticleFavorite(string slug, Guid id)
        {
            var article = await _repository.GetArticleAsync(slug);

            if (article == null)
            {
                return NotFound(new ErrorResponse()
                {
                    Success = false,
                    Errors = new Errors() { Message = "Article not found" }
                });
            }

            var currentUserId = GetCurrentUserId();

            var favorite = await _repository.GetArticleFavoriteAsync(article.Id, Guid.Parse(currentUserId));

            if (favorite != null)
            {
                _repository.DeleteArticleFavorite(favorite);
                await _repository.SaveChangesAsync();
            }

            var articleReadDto = _mapper.Map<ArticlesResponseDto>(article);

            return StatusCode(200, new ArticleResponse()
            {
                Success = true,
                Article = articleReadDto
            });
        }


        private bool ArticleExists(Guid id)
        {
            return _repository.ArticleExists(id);
        }

        public static string GenerateSlug (string Title)
        {
            SlugHelper helper = new SlugHelper();
            var slug = $"{helper.GenerateSlug(Title)}-{Guid.NewGuid().ToString().Substring(0, 6)}";
            return slug;
        }

        private string GetCurrentUserId (){
            var identity = User.Identity as ClaimsIdentity;
            IEnumerable<Claim> claims = identity.Claims;
            var userId = claims.Where(p => p.Type == "Id").FirstOrDefault()?.Value;

            return userId;
        }

        private string CreateArticlesResourceUri(ArticlesResourceParameters articlesResourceParameters, ResourceUriType type, string urlLink)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return Url.Link(urlLink,
                      new
                      {
                          pageNumber = articlesResourceParameters.PageNumber - 1,
                          pageSize = articlesResourceParameters.PageSize,
                          author = articlesResourceParameters.Author,
                          search = articlesResourceParameters.Search
                      });
                case ResourceUriType.NextPage:
                    return Url.Link(urlLink,
                      new
                      {
                          pageNumber = articlesResourceParameters.PageNumber + 1,
                          pageSize = articlesResourceParameters.PageSize,
                          author = articlesResourceParameters.Author,
                          search = articlesResourceParameters.Search
                      });
                default:
                    return Url.Link(urlLink,
                    new
                    {
                        pageNumber = articlesResourceParameters.PageNumber,
                        pageSize = articlesResourceParameters.PageSize,
                        author = articlesResourceParameters.Author,
                        search = articlesResourceParameters.Search
                    });
            }
        }
    }
}
