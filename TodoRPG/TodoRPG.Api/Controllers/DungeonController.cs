using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
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
            return await _context.Dungeons.OrderBy(d => d.Index).ToListAsync();
        }

        // GET: /api/Dungeon/{index} - 특정 던전 정보 조회
        [HttpGet("{index}")]
        public async Task<ActionResult<Dungeon>> GetDungeon(int index)
        {
            var dungeon = await _context.Dungeons.FindAsync(index);

            if (dungeon == null)
            {
                return NotFound("해당 던전 정보를 찾을 수 없습니다.");
            }

            return Ok(dungeon);
        }
    }
}