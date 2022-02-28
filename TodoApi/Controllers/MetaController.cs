using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Models;
using Microsoft.Extensions.Logging;

namespace TodoApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class MetaController : ControllerBase
    {
        private readonly TodoContext _context;
        private readonly ILogger<MetaController> _logger;

        public MetaController(ILogger<MetaController> logger, TodoContext context)
        {
            _logger = logger;
            _context = context;
        }

        // GET: meta
        // palauttaa kaikkki tagit
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TagDTO>>> GetTags([FromQuery(Name = "q")] string q)
        {
            var allTags = await _context.Tags
                .Where(t => EF.Functions.Like(t.TagId, $"%{q}%"))
                .Include(t => t.TodoItems)
                .Select(t => t.TagToDTO())
                .ToListAsync();

            return allTags;
        }

        // GET: meta/testi
        // palauttaa montako todo itemia tagilla löytyy
        [HttpGet("{tagId}")]
        public async Task<int> GetTodoItemCountByTag(string tagId)
        {
            var todoItems = _context.TodoItems
                .Include(i => i.Tags)
                .Where(i => i.Tags.Any(t => EF.Functions.Like(t.TagId, $"{tagId}")))
                .Select(t => t.TodoToDTO())
                .Count();

            return todoItems;
        }

        // GET: meta
        // palauttaa käyttäjän käyttämät tagit ja niiden todo itemit
        [HttpGet("user/{user}")]
        public async Task<ActionResult<IEnumerable<TodoTagDTO>>> GetTagsByUser(string user)
        {
            var allTags = await _context.Tags
                .Include(t => t.TodoItems)
                .Where(t => t.TodoItems.Any(i => EF.Functions.Like(i.User, $"{user}")))
                .Select(t => new TodoTagDTO { TagId = t.TagId })
                .ToListAsync();

            return allTags;
        }

        // GET: meta/testi
        // palauttaa käyttäjän tagit ja montako todoitemia tagilla löytyy
        [HttpGet("user/{user}/count")]
        public async Task<ActionResult<IEnumerable<TagDTO>>> GetTagsAndTodoItemCountByUser(string user)
        {
            var allTags = await _context.Tags
                .Include(tag => tag.TodoItems)
                .Where(tag => tag.TodoItems.Any(item => EF.Functions.Like(item.User, $"{user}")))
                .Select(tag => tag.TagToDTO())
                .ToListAsync();

            return allTags;
        }
    }
}
