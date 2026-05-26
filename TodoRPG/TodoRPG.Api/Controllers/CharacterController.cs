using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using TodoRPG.Api.Data;
using TodoRPG.Api.Models;

namespace TodoRPG.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CharacterController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CharacterController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /api/Character/{userId} - 캐릭터 정보 조회
        [HttpGet("{userId}")]
        public async Task<ActionResult<Character>> GetCharacter(string userId)
        {
            var character = await _context.Characters
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (character == null)
            {
                return NotFound("해당 사용자의 캐릭터 정보를 찾을 수 없습니다.");
            }

            return Ok(character);
        }

        // POST: /api/Character - 캐릭터 생성 (초기화)
        [HttpPost]
        public async Task<ActionResult<Character>> PostCharacter(CreateCharacterRequest request)
        {
            var userId = request.UserId.Trim();

            var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
            if (!userExists)
            {
                return NotFound("해당 사용자가 존재하지 않습니다.");
            }

            var characterExists = await _context.Characters.AnyAsync(c => c.UserId == userId);
            if (characterExists)
            {
                return BadRequest("이미 캐릭터가 생성된 사용자입니다.");
            }

            var character = new Character
            {
                UserId = userId,
                Level = 1,
                Experience = 0,
                Coin = 0,
                Strength = 0,
                Intelligence = 0,
                Fortune = 0,
                Health = 0,
                CurrentDungeonIndex = 1,
                // 캐릭터 생성 시 던전 티켓 0장으로 데이터 초기화 연산
                DungeonTickets = 0
            };

            _context.Characters.Add(character);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCharacter), new { userId = character.UserId }, character);
        }

        // PATCH: /api/Character/{userId}/stats - 캐릭터 스탯 및 재화 업데이트
        [HttpPatch("{userId}/stats")]
        public async Task<IActionResult> UpdateStats(string userId, UpdateStatsRequest request)
        {
            var character = await _context.Characters.FirstOrDefaultAsync(c => c.UserId == userId);
            if (character == null)
            {
                return NotFound("캐릭터를 찾을 수 없습니다.");
            }

            // 전달받은 스탯 값만큼 누적 가산
            character.Strength += request.Strength;
            character.Intelligence += request.Intelligence;
            character.Fortune += request.Fortune;
            character.Health += request.Health;

            if (request.CoinDelta != 0)
                character.Coin = Math.Max(0, character.Coin + request.CoinDelta);

            // 던전 티켓 개수 가감 연산 처리 추가 (티켓이 마이너스가 되지 않도록 방어 계산)
            if (request.DungeonTicketsDelta != 0)
                character.DungeonTickets = Math.Max(0, character.DungeonTickets + request.DungeonTicketsDelta);

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // PATCH: /api/Character/{userId}/dungeon - 현재 던전 위치 갱신 (티켓 검산 및 차감 로직 반영)
        [HttpPatch("{userId}/dungeon")]
        public async Task<IActionResult> UpdateDungeon(string userId, UpdateDungeonRequest request)
        {
            var character = await _context.Characters.FirstOrDefaultAsync(c => c.UserId == userId);
            if (character == null)
            {
                return NotFound("캐릭터를 찾을 수 없습니다.");
            }

            var dungeon = await _context.Dungeons.FindAsync(request.DungeonIndex);
            if (dungeon == null)
            {
                return NotFound("존재하지 않는 던전입니다.");
            }

            // 1. 입장 레벨 제한 체크
            if (character.Level < dungeon.RequiredCondition)
            {
                return BadRequest($"해당 던전에 입장하기 위한 레벨({dungeon.RequiredCondition})이 부족합니다.");
            }

            // 2. 던전 티켓 보유 수량 검산 (0장 체크)
            if (character.DungeonTickets < 1)
            {
                return BadRequest("던전에 입장하기 위한 던전 티켓이 부족합니다. 상점에서 구매해 주세요.");
            }

            // 3. 입장 제한조건 충족 확인 후 티켓 1장 차감 연산
            character.DungeonTickets -= 1;

            // 4. 던전 위치 인덱스 갱신 및 DB 저장
            character.CurrentDungeonIndex = request.DungeonIndex;
            await _context.SaveChangesAsync();

            // 스웨거 및 클라이언트에서 바뀐 잔여 티켓 연산 상태를 확인할 수 있도록 변경 양식 반환
            return Ok(new
            {
                message = $"{dungeon.Name}에 성공적으로 입장했습니다.",
                currentTickets = character.DungeonTickets,
                currentDungeonIndex = character.CurrentDungeonIndex
            });
        }
    }

    public class CreateCharacterRequest
    {
        [Required]
        public string UserId { get; set; } = string.Empty;
    }

    public class UpdateStatsRequest
    {
        public int Strength { get; set; }
        public int Intelligence { get; set; }
        public int Fortune { get; set; }
        public int Health { get; set; }
        public int CoinDelta { get; set; } // 코인 가감용 (+50, -20 등)

        // 티켓 증가 및 차감 요청을 수립하기 위한 전송 변수 추가
        public int DungeonTicketsDelta { get; set; } // 던전 티켓 가감용 (+1, -1 등)
    }

    public class UpdateDungeonRequest
    {
        [Required]
        public int DungeonIndex { get; set; }
    }
}