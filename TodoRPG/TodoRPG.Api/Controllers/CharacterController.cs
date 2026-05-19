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
				CurrentDungeonIndex = 1
			};

			_context.Characters.Add(character);
			await _context.SaveChangesAsync();

			return CreatedAtAction(nameof(GetCharacter), new { userId = character.UserId }, character);
		}

		// PATCH: /api/Character/{userId}/stats - 캐릭터 스탯 업데이트
		[HttpPatch("{userId}/stats")]
		public async Task<IActionResult> UpdateStats(string userId, UpdateStatsRequest request)
		{
			var character = await _context.Characters.FirstOrDefaultAsync(c => c.UserId == userId);
			if (character == null)
			{
				return NotFound("캐릭터를 찾을 수 없습니다.");
			}

			// 전달받은 스탯 값만큼 누적 가산 (혹은 필요에 따라 덮어쓰기로 변경 가능)
			character.Strength += request.Strength;
			character.Intelligence += request.Intelligence;
			character.Fortune += request.Fortune;
			character.Health += request.Health;

			if (request.CoinDelta != 0)
				character.Coin = Math.Max(0, character.Coin + request.CoinDelta);

			await _context.SaveChangesAsync();
			return NoContent();
		}

		// PATCH: /api/Character/{userId}/dungeon - 현재 던전 위치 갱신
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

			// 입장 레벨 제한 체크
			if (character.Level < dungeon.RequiredCondition)
			{
				return BadRequest($"해당 던전에 입장하기 위한 레벨({dungeon.RequiredCondition})이 부족합니다.");
			}

			character.CurrentDungeonIndex = request.DungeonIndex;
			await _context.SaveChangesAsync();

			return NoContent();
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
	}

	public class UpdateDungeonRequest
	{
		[Required]
		public int DungeonIndex { get; set; }
	}
}