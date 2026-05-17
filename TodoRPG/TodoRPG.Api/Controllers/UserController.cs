using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoRPG.Api.Data;
using TodoRPG.Api.Models;

namespace TodoRPG.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;

        // 생성자: DB 연결 다리를 가져옵니다.
        public UserController(AppDbContext context)
        {
            _context = context;
        }

        // 1. 모든 사용자 목록 조회 (Read)
        // 관리자로서 사용할 기능
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        // 2. 특정 사용자 정보 조회 (Read)
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(string id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // 3. 회원가입: 새로운 사용자 추가 (Create)
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            user.Id = user.Id.Trim();
            user.Nickname = user.Nickname.Trim();

            // Trim 후 길이 변경 가능성이 있어 모델 검증을 다시 돌립니다.
            // (ID/닉네임/비밀번호 길이 규칙은 User 모델의 DataAnnotations에 정의)
            ModelState.Clear();
            if (!TryValidateModel(user))
            {
                return ValidationProblem(ModelState);
            }

            var exists = await _context.Users.AnyAsync(u => u.Id == user.Id);

            if (exists)
            {
                return BadRequest("이미 존재하는 아이디입니다.");
            }

            user.Level = 1;
            user.Experience = 0;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }

        // 4. 로그인: ID와 비밀번호가 DB에 존재하는지 확인
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Dictionary<string, string>? request)
        {
            if (request == null)
            {
                return BadRequest("로그인 정보가 없습니다.");
            }

            var id = request
                .FirstOrDefault(x => string.Equals(x.Key, "id", StringComparison.OrdinalIgnoreCase))
                .Value?
                .Trim();

            var password = request
                .FirstOrDefault(x => string.Equals(x.Key, "password", StringComparison.OrdinalIgnoreCase))
                .Value;

            if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(password))
            {
                return BadRequest("ID와 비밀번호를 입력하세요.");
            }

            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id && u.Password == password);

            if (user == null)
            {
                return Unauthorized("ID 또는 비밀번호가 올바르지 않습니다.");
            }

            return Ok(new
            {
                user.Id,
                user.Nickname,
                user.Level,
                user.Experience
            });
        }

        private static string? GetRequestValue(Dictionary<string, string> request, string key)
        {
            return request
                .FirstOrDefault(x => string.Equals(x.Key, key, StringComparison.OrdinalIgnoreCase))
                .Value;
        }          

        // 5. 비밀번호 수정 (Update)
        [HttpPut("{id}/change-password")]
        public async Task<IActionResult> UpdatePassword(string id, [FromBody] ChangePasswordRequest request)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound("해당 사용자를 찾을 수 없습니다.");
            }

            if (user.Password != request.CurrentPassword)
            {
                return Unauthorized("현재 비밀번호가 올바르지 않습니다.");
            }

            if (request.NewPassword == request.CurrentPassword)
            {
                return BadRequest("새 비밀번호는 현재 비밀번호와 달라야 합니다.");
            }

            user.Password = request.NewPassword;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id)) return NotFound();
                else throw;
            }

            return Ok("비밀번호가 성공적으로 변경되었습니다.");
        }

        // 6. 회원 탈퇴 (Delete)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            // 1. 삭제할 유저를 찾습니다.
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound("삭제할 사용자가 존재하지 않습니다.");
            }

            // 2. DB에서 해당 유저를 삭제합니다.
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok($"{user.Nickname}님의 회원 탈퇴가 완료되었습니다.");
        }

        // 유저 존재 여부 확인용 헬퍼 메서드
        private bool UserExists(string id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }

    public sealed class ChangePasswordRequest
    {
        [Required(ErrorMessage = "현재 비밀번호를 입력하세요.")]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "새 비밀번호를 입력하세요.")]
        [StringLength(20, MinimumLength = 4, ErrorMessage = "비밀번호는 4~20자여야 합니다.")]
        public string NewPassword { get; set; } = string.Empty;
    }
}