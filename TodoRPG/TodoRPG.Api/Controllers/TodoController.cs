using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoRPG.Api.Data;
using TodoRPG.Api.Models;
using TodoRPG.Api.Services;

namespace TodoRPG.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoController : ControllerBase
    {
        private readonly AppDbContext context;
        private readonly TodoRewardService todoRewardService;

        private static readonly HashSet<string> AllowedCategories = new(StringComparer.Ordinal)
        {
            "운동",
            "업무",
            "자기개발",
            "일상",
            "기타"
        };

        public TodoController(AppDbContext context, TodoRewardService todoRewardService)
        {
            this.context = context;
            this.todoRewardService = todoRewardService;
        }

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

            var userExists = await context.Users.AnyAsync(user => user.Id == userId);

            if (!userExists)
            {
                return NotFound("해당 사용자를 찾을 수 없습니다.");
            }

            var query = context.TodoItems
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


        [HttpGet("user/{userId}/completed-count")]
        public async Task<ActionResult<int>> GetCompletedCount(string userId)
        {
            var completedCount = await context.TodoItems
                .AsNoTracking()
                .CountAsync(todo => todo.UserId == userId && todo.IsCompleted);

            return Ok(completedCount);
        }

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

            var userExists = await context.Users.AnyAsync(user => user.Id == userId);

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
                CompletedAt = null,
                DueDate = request.DueDate
            };

            context.TodoItems.Add(todoItem);
            await context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetTodoItems),
                new { userId = todoItem.UserId },
                todoItem
            );
        }

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

            var todoItem = await context.TodoItems
                .FirstOrDefaultAsync(todo => todo.Id == id && todo.UserId == userId);

            if (todoItem == null)
            {
                return NotFound("해당 할 일을 찾을 수 없습니다.");
            }

            if (todoItem.IsCompleted)
            {
                return BadRequest("완료된 할 일은 수정할 수 없습니다.");
            }

            todoItem.Title = title;
            todoItem.Category = category;
            todoItem.DueDate = request.DueDate;

            await context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("{id}/completed")]
        public async Task<ActionResult<TodoCompletionResult>> SetTodoCompleted(
            int id,
            SetTodoCompletedRequest request)
        {
            var userId = request.UserId.Trim();

            if (string.IsNullOrWhiteSpace(userId))
            {
                return BadRequest("사용자 ID가 필요합니다.");
            }

            if (!request.IsCompleted)
            {
                return BadRequest("완료된 할 일은 미완료로 되돌릴 수 없습니다.");
            }

            var result = await todoRewardService.SetCompletedAsync(
                id,
                userId,
                request.IsCompleted
            );

            if (result is null)
            {
                return NotFound("해당 할 일 또는 캐릭터를 찾을 수 없습니다.");
            }

            return Ok(result);
        }

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

            var todoItem = await context.TodoItems
                .FirstOrDefaultAsync(todo => todo.Id == id && todo.UserId == userId);

            if (todoItem == null)
            {
                return NotFound("해당 할 일을 찾을 수 없습니다.");
            }

            if (todoItem.IsCompleted)
            {
                return BadRequest("완료된 할 일은 삭제할 수 없습니다.");
            }

            context.TodoItems.Remove(todoItem);
            await context.SaveChangesAsync();

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

    public sealed class TodoCompletedCountResponse
    {
        public int Count { get; set; }
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
