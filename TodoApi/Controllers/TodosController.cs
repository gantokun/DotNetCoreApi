using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Models;

namespace TodoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodosController : ControllerBase
    {
        private readonly TodoContext _todoContext;

        public TodosController(TodoContext todoContext)
        {
            _todoContext = todoContext;

            if (_todoContext.TodoItems.Count() == 0)
            {
                SeedDb();
            }
        }

        private void SeedDb()
        {
            _todoContext.TodoItems.AddRange(new[] {
                new TodoItem{ Name = "Todo1" },
                new TodoItem{ Name = "Todo2" },
                new TodoItem{ Name = "Todo3" }
            });
            _todoContext.SaveChanges();
        }

        // GET api/todos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoItem>>> Get()
        {
            return await _todoContext.TodoItems.ToListAsync();
        }

        // GET api/todos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TodoItem>> Get(long id)
        {
            var todoItem = await _todoContext.TodoItems.FindAsync(id);

            if (todoItem == null)
            {
                return NotFound();
            }
            return todoItem;
        }

        // POST api/todos
        [HttpPost]
        public async Task<ActionResult<TodoItem>> Create(TodoItem todoItem)
        {
            _todoContext.TodoItems.Add(todoItem);
            await _todoContext.SaveChangesAsync();

            return CreatedAtAction("Get", new { id = todoItem.Id }, todoItem);
        }

        // PUT api/todos/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, TodoItem todoItem)
        {
            if (id != todoItem.Id)
            {
                return BadRequest("URL id must be the same of the item id");
            }

            _todoContext.Entry(todoItem).State = EntityState.Modified;

            try
            {
                await _todoContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TodoItemExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // Check if a TodoItem exists with the specific id
        private bool TodoItemExists(long id)
        {
            return _todoContext.TodoItems.Any(item => item.Id == id);
        }

        // DELETE api/todos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var todoItem = await _todoContext.TodoItems.FindAsync(id);

            if (todoItem == null)
            {
                return NotFound();
            }

            _todoContext.TodoItems.Remove(todoItem);
            await _todoContext.SaveChangesAsync();

            return NoContent();
        }
    }
}