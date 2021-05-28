using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Conduit.Data;
using Conduit.Models;
using Conduit.Services;
using Conduit.DTOs.Responses.Tag;
using AutoMapper;

namespace Conduit.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TagsController : ControllerBase
    {
        private readonly IConduitRepository _repository;

        public TagsController(IConduitRepository repository)
        {
            _repository = repository;
        }

        // GET: api/Tags
        [HttpGet]
        public async Task<ActionResult<TagsResponse>> GetTags()
        {
            var tags = await _repository.GetTagsAsync();

            return Ok(new TagsResponse()
            {
                Tags = tags.Select(x => x.TagId).ToList()
            });
        }
    }
}
