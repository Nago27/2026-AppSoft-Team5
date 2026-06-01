using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoRPG.Api.Data;
using TodoRPG.Api.Models;

namespace TodoRPG.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DungeonController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DungeonController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /api/Dungeon - 전체 던전 목록 조회
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Dungeon>>> GetDungeons()
        {
            return await _context.Dungeons
                .AsNoTracking()
                .OrderBy(d => d.Index)
                .ToListAsync();
        }

        // GET: /api/Dungeon/{index} - 특정 던전 정보 조회
        [HttpGet("{index}")]
        public async Task<ActionResult<Dungeon>> GetDungeon(int index)
        {
            var dungeon = await _context.Dungeons
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Index == index);

            if (dungeon == null)
            {
                return NotFound("해당 던전 정보를 찾을 수 없습니다.");
            }

            return Ok(dungeon);
        }

        // 현재 진행 던전 인덱스를 기준으로 클리어한 던전 수를 계산.
        // 예: CurrentDungeonIndex = 1 => 0회, 2 => 1회, 3 => 2회
        [HttpGet("user/{userId}/clear-count")]
        public async Task<ActionResult<int>> GetClearCount(string userId)
        {
            var character = await _context.Characters
                .AsNoTracking()
                .FirstOrDefaultAsync(character => character.UserId == userId);

            if (character is null)
            {
                return NotFound("캐릭터 정보를 찾을 수 없습니다.");
            }

            var clearCount = Math.Max(0, character.CurrentDungeonIndex - 1);

            return Ok(clearCount);
        }
    }

    public sealed class DungeonClearCountResponse
    {
        public int Count { get; set; }
    }
}
