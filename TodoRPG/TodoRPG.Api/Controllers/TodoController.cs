using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoRPG.Api.Data;
using TodoRPG.Api.Models;

namespace TodoRPG.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoController : ControllerBase
    {
        private readonly AppDbContext _context;

        private static readonly HashSet<string> AllowedCategories = new(StringComparer.Ordinal)
        {
            "운동",
            "업무",
            "자기개발",
            "일상",
            "기타"
        };

        public TodoController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Todo/user/testuser
        // GET: api/Todo/user/testuser?category=운동
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<TodoItem>>> GetTodoItems(
            string userId,
            [FromQuery] string? category = null)
        {
            userId = userId.Trim();

            if (string.IsNullOrWhiteSpace(userId))
            {
                return BadRequest("사용자 ID가 필요합니다.");
            }

            var userExists = await _context.Users.AnyAsync(user => user.Id == userId);

            if (!userExists)
            {
                return NotFound("해당 사용자를 찾을 수 없습니다.");
            }

            var query = _context.TodoItems
                .AsNoTracking()
                .Where(todo => todo.UserId == userId);

            if (!string.IsNullOrWhiteSpace(category) && category != "전체")
            {
                query = query.Where(todo => todo.Category == category.Trim());
            }

            var todos = await query
                .OrderBy(todo => todo.IsCompleted)
                .ThenByDescending(todo => todo.CreatedAt)
                .ToListAsync();

            return Ok(todos);
        }

        // POST: api/Todo
        [HttpPost]
        public async Task<ActionResult<TodoItem>> PostTodoItem(CreateTodoRequest request)
        {
            var userId = request.UserId.Trim();
            var title = request.Title.Trim();
            var category = NormalizeCategory(request.Category);

            if (string.IsNullOrWhiteSpace(userId))
            {
                return BadRequest("사용자 ID가 필요합니다.");
            }

            if (string.IsNullOrWhiteSpace(title))
            {
                return BadRequest("할 일 제목을 입력하세요.");
            }

            var userExists = await _context.Users.AnyAsync(user => user.Id == userId);

            if (!userExists)
            {
                return NotFound("해당 사용자를 찾을 수 없습니다.");
            }

            var todoItem = new TodoItem
            {
                UserId = userId,
                Title = title,
                Category = category,
                IsCompleted = false,
                CreatedAt = DateTime.UtcNow,
                DueDate = request.DueDate
            };

            _context.TodoItems.Add(todoItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetTodoItems),
                new { userId = todoItem.UserId },
                todoItem
            );
        }

        // PUT: api/Todo/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTodoItem(int id, UpdateTodoRequest request)
        {
            var userId = request.UserId.Trim();
            var title = request.Title.Trim();
            var category = NormalizeCategory(request.Category);

            if (string.IsNullOrWhiteSpace(userId))
            {
                return BadRequest("사용자 ID가 필요합니다.");
            }

            if (string.IsNullOrWhiteSpace(title))
            {
                return BadRequest("할 일 제목을 입력하세요.");
            }

            var todoItem = await _context.TodoItems
                .FirstOrDefaultAsync(todo => todo.Id == id && todo.UserId == userId);

            if (todoItem == null)
            {
                return NotFound("해당 할 일을 찾을 수 없습니다.");
            }

            todoItem.Title = title;
            todoItem.Category = category;
            todoItem.IsCompleted = request.IsCompleted;
            todoItem.DueDate = request.DueDate;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // PATCH: api/Todo/5/completed
        [HttpPatch("{id}/completed")]
        public async Task<IActionResult> SetTodoCompleted(int id, SetTodoCompletedRequest request)
        {
            var userId = request.UserId.Trim();

            if (string.IsNullOrWhiteSpace(userId))
            {
                return BadRequest("사용자 ID가 필요합니다.");
            }

            var todoItem = await _context.TodoItems
                .FirstOrDefaultAsync(todo => todo.Id == id && todo.UserId == userId);

            if (todoItem == null)
            {
                return NotFound("해당 할 일을 찾을 수 없습니다.");
            }

            todoItem.IsCompleted = request.IsCompleted;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Todo/5?userId=testuser
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodoItem(
            int id,
            [FromQuery] string userId)
        {
            userId = userId.Trim();

            if (string.IsNullOrWhiteSpace(userId))
            {
                return BadRequest("사용자 ID가 필요합니다.");
            }

            var todoItem = await _context.TodoItems
                .FirstOrDefaultAsync(todo => todo.Id == id && todo.UserId == userId);

            if (todoItem == null)
            {
                return NotFound("해당 할 일을 찾을 수 없습니다.");
            }

            _context.TodoItems.Remove(todoItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private static string NormalizeCategory(string? category)
        {
            if (string.IsNullOrWhiteSpace(category))
            {
                return "일상";
            }

            var trimmed = category.Trim();

            return AllowedCategories.Contains(trimmed)
                ? trimmed
                : "기타";
        }
    }

    public sealed class CreateTodoRequest
    {
        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string Title { get; set; } = string.Empty;

        public string Category { get; set; } = "일상";

        public DateTime? DueDate { get; set; }
    }

    public sealed class UpdateTodoRequest
    {
        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string Title { get; set; } = string.Empty;

        public string Category { get; set; } = "일상";

        public bool IsCompleted { get; set; }

        public DateTime? DueDate { get; set; }
    }

    public sealed class SetTodoCompletedRequest
    {
        [Required]
        public string UserId { get; set; } = string.Empty;

        public bool IsCompleted { get; set; }
    }
}