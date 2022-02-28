using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Models;
using Microsoft.Extensions.Logging;

namespace TodoApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ManageController : ControllerBase
    {
        private readonly TodoContext _context;
        private readonly ILogger<ManageController> _logger;

        public ManageController(ILogger<ManageController> logger, TodoContext context)
        {
            _logger = logger;
            _context = context;
        }

        // GET: manage
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoItem>>> GetTodoItems()
        {
            return await _context.TodoItems.ToListAsync();
        }

        // GET: manage/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TodoItem>> GetTodoItem(long id)
        {
            var todoItem = await _context.TodoItems.FindAsync(id);

            if (todoItem == null)
            {
                return NotFound();
            }

            return todoItem;
        }

        // PUT: manage/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTodoItem(long id, TodoItem todoItem)
        {
            if (id != todoItem.TodoItemId)
            {
                return BadRequest();
            }
            if (false == ModelState.IsValid)
            {
                return BadRequest();
            }

            var dbItem = await _context.TodoItems.FindAsync(id);

            if (null == dbItem)
            {
                return NotFound();
            }

            dbItem.IsComplete = todoItem.IsComplete;
            dbItem.Name = todoItem.Name;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: manage
        [HttpPost]
        public async Task<ActionResult<TodoItem>> PostTodoItem(TodoItem todoItem)
        {
            if (false == ModelState.IsValid)
            {
                return BadRequest();
            }
            _context.TodoItems.Add(todoItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTodoItem", new { id = todoItem.TodoItemId }, todoItem);
        }

        // DELETE: manage/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodoItem(long id)
        {
            var todoItem = await _context.TodoItems.FindAsync(id);
            if (todoItem == null)
            {
                return NotFound();
            }

            _context.TodoItems.Remove(todoItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}
