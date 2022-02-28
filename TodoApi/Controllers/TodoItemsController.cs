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
    public class TodoItemsController : ControllerBase
    {
        private readonly TodoContext _context;
        private readonly ILogger<TodoItemsController> _logger;

        public TodoItemsController(ILogger<TodoItemsController> logger, TodoContext context)
        {
            _logger = logger;
            _context = context;
        }

        // GET: TodoItems
        [HttpGet]
        [HttpPost]
        [HttpPut]
        [HttpDelete]
        public ActionResult NotValidUrls()
        {
            return BadRequest();
        }

        // GET: pekka
        // palauttaa k‰ytt‰j‰n kaikki todo itemit
        [HttpGet("{user}")]
        public async Task<ActionResult<IEnumerable<TodoItemDTO>>> GetTodoItems(string user)
        {
            var todoItems = await _context.TodoItems
                .Where(i => EF.Functions.Like(i.User, $"{user}"))
                .Include(i => i.Tags)
                .Select(i => i.TodoToDTO())
                .ToListAsync();

            return todoItems;
        }

        // GET: pekka/5
        // palauttaa k‰ytt‰j‰n kyseisen todo itemin
        [HttpGet("{user}/{id:int}")]
        public async Task<ActionResult<TodoItemDTO>> GetTodoItem(long id, string user)
        {
            var todoItem = await _context.TodoItems
                .Where(i => i.TodoItemId == id && EF.Functions.Like(i.User, $"{user}"))
                .Include(i => i.Tags)
                .Select(i => i.TodoToDTO())
                .FirstOrDefaultAsync();

            if (todoItem == null)
            {
                return NotFound();
            }

            return todoItem;
        }

        // GET: noora/testi
        // palauttaa k‰ytt‰j‰n kaikki todo itemit joissa kyseinen tagi
        [HttpGet("{user}/{tagId}")]
        public async Task<ActionResult<IEnumerable<TodoItemDTO>>> GetTodoItemsByTags(string user, string tagId)
        {
            var todoItems = await _context.TodoItems
                .Where(i => EF.Functions.Like(i.User, $"{user}"))
                .Include(i => i.Tags)
                .Where(i => i.Tags.Any(t => EF.Functions.Like(t.TagId, $"{tagId}")))
                .Select(t => t.TodoToDTO())
                .ToListAsync();

            return todoItems;
        }
        
        // PUT: pekka/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        // p‰ivitt‰‰ k‰ytt‰j‰n todo itemin tiedot
        [HttpPut("{user}/{id}")]
        public async Task<IActionResult> PutTodoItem(long id, string user, TodoItem todoItem)
        {
            // bodyssa olevan todo itemin id ei ole sama kuin urlin id
            if (id != todoItem.TodoItemId)
            {
                return BadRequest();
            }

            // NOTE Mit‰ t‰m‰ tarkoittaa?
            if (false == ModelState.IsValid)
            {
                return BadRequest();
            }

            var dbItem = await _context.TodoItems
                .Include(i => i.Tags)
                .FirstOrDefaultAsync(i => i.TodoItemId == id && EF.Functions.Like(i.User, $"{user}"));

            if (dbItem == null)
            {
                return NotFound();
            }

            dbItem.IsComplete = todoItem.IsComplete;
            dbItem.Name = todoItem.Name;

            var existingTags = await _context.Tags // t‰ss‰ haetaan olemassa olevat tagit
                .Select(t => t.TagId)
                .ToListAsync();

            var tagsToBeAdded = new List<Tag>(); // uusi lista, johon siirret‰‰n lis‰tt‰v‰t tagit

            foreach (Tag tag in todoItem.Tags)
            {
                if (existingTags.Contains(tag.TagId)) // tarkistetaan onko kyseinen tagi jo tietokannassa
                {
                    var existingTag = _context.Tags.Find(tag.TagId); // jos on, haetaan kyseinen entry
                    tagsToBeAdded.Add(existingTag); // ja lis‰t‰‰n se lis‰tt‰viin
                }
                else
                {
                    tagsToBeAdded.Add(tag); // muuten vaan lis‰t‰‰n lis‰tt‰viin AKA tehd‰‰n uusi entry
                }
            }

            dbItem.Tags.Clear();
            dbItem.Tags = tagsToBeAdded;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: pekka
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        // lis‰‰ uuden todo itemin
        [HttpPost("{user}")]
        public async Task<ActionResult<TodoItemDTO>> PostTodoItem(string user, TodoItem todoItem)
        {
            // NOTE Mit‰ t‰m‰ tarkoittaa
            if (false == ModelState.IsValid)
            {
                return BadRequest();
            }

            todoItem.User = user; // todoitemin tekij‰, on urlin user

            var existingTags = await _context.Tags  // t‰ss‰ haetaan olemassa olevat tagit
                .Select(t => t.TagId)
                .ToListAsync();

            var tagsToBeAdded = new List<Tag>(); // uusi lista, johon siirret‰‰n lis‰tt‰v‰t tagit

            foreach (Tag tag in todoItem.Tags)
            {
                if (existingTags.Contains(tag.TagId)) // tarkistetaan onko kyseinen tagi jo tietokannassa
                {
                    var existingTag = _context.Tags.Find(tag.TagId); // jos on, haetaan kyseinen entry
                    tagsToBeAdded.Add(existingTag); // ja lis‰t‰‰n se lis‰tt‰viin
                }
                else
                {
                    tagsToBeAdded.Add(tag); // muuten vaan lis‰t‰‰n lis‰tt‰viin AKA tehd‰‰n uusi entry
                }
            }

            todoItem.Tags = tagsToBeAdded; // vaihdetaan alkuper‰inen lista t‰h‰n uuteen listaan

            _context.TodoItems.Add(todoItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTodoItem", new { id = todoItem.TodoItemId, user }, todoItem.TodoToDTO());
        }

        // DELETE: pekka/5
        // poistaa kyseisen todo itemin
        [HttpDelete("{user}/{id}")]
        public async Task<IActionResult> DeleteTodoItem(long id, string user)
        {
            var todoItem = await _context.TodoItems
                .FirstOrDefaultAsync(i => i.TodoItemId == id && EF.Functions.Like(i.User, $"{user}"));
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
