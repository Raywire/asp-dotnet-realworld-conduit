using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Conduit.Data;
using Conduit.Models;
using AutoMapper;
using Conduit.DTOs.Responses;
using Microsoft.AspNetCore.Authorization;
using Conduit.DTOs.Requests;
using System.Security.Claims;
using Slugify;

namespace Conduit.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ArticlesController : ControllerBase
    {
        private readonly ConduitContext _context;
        private readonly IMapper _mapper;

        public ArticlesController(ConduitContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Articles
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<ArticlesResponse>> GetArticle()
        {
            var articles = await _context.Article.Include(a => a.Author).ToListAsync();

            return Ok(new ArticlesResponse()
            {
                Success = true,
                Articles = _mapper.Map<IEnumerable<ArticlesResponseDto>>(articles)
            });
        }

        // GET: api/Articles/5
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<ArticleResponse>> GetArticle(Guid id)
        {
            var article = await _context.Article.Include(a => a.Author).Where(a => a.Id == id).FirstOrDefaultAsync();

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
        [HttpPut("{id}")]
        public async Task<IActionResult> PutArticle(Guid id, ArticleUpdateRequestDto articleUpdateRequestDto)
        {
            var article = await _context.Article.FindAsync(id);

            if (article == null)
            {
                return NotFound(new ErrorResponse()
                {
                    Success = false,
                    Errors = new Errors() { Message = "Article not found" }
                });
            }

            if (articleUpdateRequestDto.Title != article.Title)
            {
                article.Slug = GenerateSlug(articleUpdateRequestDto.Title);
            }

            _mapper.Map(articleUpdateRequestDto, article);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ArticleExists(id))
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
            var identity = User.Identity as ClaimsIdentity;
            IEnumerable<Claim> claims = identity.Claims;
            var userId = claims.Where(p => p.Type == "Id").FirstOrDefault()?.Value;

            var articleModel = _mapper.Map<Article>(articleCreateDto);
            articleModel.AuthorId = Guid.Parse(userId);

            articleModel.Slug = GenerateSlug(articleModel.Title);

            _context.Article.Add(articleModel);
            await _context.SaveChangesAsync();

            var articleReadDto = _mapper.Map<ArticlesResponseDto>(articleModel);

            return CreatedAtAction("GetArticle", new { id = articleReadDto.Id }, new ArticleResponse()
            {
                Success = true,
                Article = articleReadDto
            });
        }

        // DELETE: api/Articles/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteArticle(Guid id)
        {
            var article = await _context.Article.FindAsync(id);
            if (article == null)
            {
                return NotFound(new ErrorResponse()
                {
                    Success = false,
                    Errors = new Errors() { Message = "Article not found" }
                });
            }

            _context.Article.Remove(article);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ArticleExists(Guid id)
        {
            return _context.Article.Any(e => e.Id == id);
        }

        public static string GenerateSlug (string Title)
        {
            SlugHelper helper = new SlugHelper();
            var slug = $"{helper.GenerateSlug(Title)}-{Guid.NewGuid().ToString().Substring(0, 6)}";
            return slug;
        }
    }
}
