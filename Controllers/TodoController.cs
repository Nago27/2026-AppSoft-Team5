using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoRPGApp.Data;
using TodoRPGApp.Models;

namespace TodoRPGApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoController : ControllerBase
    {
        private readonly AppDbContext _context;

        // 생성자: Program.cs에서 만들어둔 DB 다리(AppDbContext)를 가져와서 연결합니다.
        public TodoController(AppDbContext context)
        {
            _context = context;
        }

        // 1. 모든 할 일 가져오기 (GET 요청)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoItem>>> GetTodoItems()
        {
            // DB의 TodoItems 테이블에 있는 모든 데이터를 리스트 형태로 반환합니다.
            return await _context.TodoItems.ToListAsync();
        }

        // 2. 새로운 할 일 추가하기 (POST 요청)
        [HttpPost]
        public async Task<ActionResult<TodoItem>> PostTodoItem(TodoItem todoItem)
        {
            // 프론트엔드에서 보낸 할 일 데이터를 DB에 추가합니다.
            _context.TodoItems.Add(todoItem);

            // 변경사항을 실제 DB 파일에 저장합니다.
            await _context.SaveChangesAsync();

            // 저장 성공 시, 저장된 데이터와 함께 성공 상태 코드(201 Created)를 반환합니다.
            return CreatedAtAction(nameof(GetTodoItems), new { id = todoItem.Id }, todoItem);
        }

        // 3. 할 일 수정하기 (PUT: api/Todo/5)
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTodoItem(int id, TodoItem todoItem)
        {
            // URL로 전달된 id와 본문(Body)에 담긴 데이터의 id가 다르면 잘못된 요청입니다.
            if (id != todoItem.Id)
            {
                return BadRequest();
            }

            // DB에게 이 데이터가 수정되었다고 알려줍니다.
            _context.Entry(todoItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // 수정하려는 사이에 데이터가 사라졌는지 확인합니다.
                if (!_context.TodoItems.Any(e => e.Id == id))
                {
                    return NotFound();
                }
                else { throw; }
            }

            return NoContent(); // 성공했지만 딱히 돌려줄 데이터는 없을 때 보냅니다.
        }

        // 4. 할 일 삭제하기 (DELETE: api/Todo/5)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodoItem(int id)
        {
            // 먼저 삭제할 대상을 DB에서 찾습니다.
            var todoItem = await _context.TodoItems.FindAsync(id);

            if (todoItem == null)
            {
                return NotFound(); // 찾는 번호가 없으면 404 에러!
            }

            // 찾은 데이터를 삭제 목록에 넣고 저장합니다.
            _context.TodoItems.Remove(todoItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}