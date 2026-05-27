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

		// GET: /api/Character/{userId} - ©¦???? ???? ???
		[HttpGet("{userId}")]
		public async Task<ActionResult<Character>> GetCharacter(string userId)
		{
			var character = await _context.Characters
				.FirstOrDefaultAsync(c => c.UserId == userId);

			if (character == null)
			{
				return NotFound("??? ??????? ©¦???? ?????? ??? ?? ???????.");
			}

			return Ok(character);
		}

		// POST: /api/Character - ©¦???? ???? (????)
		[HttpPost]
		public async Task<ActionResult<Character>> PostCharacter(CreateCharacterRequest request)
		{
			var userId = request.UserId.Trim();

			var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
			if (!userExists)
			{
				return NotFound("??? ?????? ???????? ??????.");
			}

			var characterExists = await _context.Characters.AnyAsync(c => c.UserId == userId);
			if (characterExists)
			{
				return BadRequest("??? ©¦????? ?????? ?????????.");
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
				Health = 100,
				CurrentDungeonIndex = 1
			};

			_context.Characters.Add(character);
			await _context.SaveChangesAsync();

			return CreatedAtAction(nameof(GetCharacter), new { userId = character.UserId }, character);
		}

		// PATCH: /api/Character/{userId}/stats - ©¦???? ???? ???????
		[HttpPatch("{userId}/stats")]
		public async Task<IActionResult> UpdateStats(string userId, UpdateStatsRequest request)
		{
			var character = await _context.Characters.FirstOrDefaultAsync(c => c.UserId == userId);
			if (character == null)
			{
				return NotFound("©¦????? ??? ?? ???????.");
			}

			// ??????? ???? ????? ???? ???? (??? ??γα ???? ??????? ???? ????)
			character.Strength += request.Strength;
			character.Intelligence += request.Intelligence;
			character.Fortune += request.Fortune;
			character.Health += request.Health;

			if (request.CoinDelta != 0)
				character.Coin = Math.Max(0, character.Coin + request.CoinDelta);

			await _context.SaveChangesAsync();
			return NoContent();
		}

		// PATCH: /api/Character/{userId}/dungeon - ???? ???? ??? ????
		[HttpPatch("{userId}/dungeon")]
		public async Task<IActionResult> UpdateDungeon(string userId, UpdateDungeonRequest request)
		{
			var character = await _context.Characters.FirstOrDefaultAsync(c => c.UserId == userId);
			if (character == null)
			{
				return NotFound("©¦????? ??? ?? ???????.");
			}

			var dungeon = await _context.Dungeons.FindAsync(request.DungeonIndex);
			if (dungeon == null)
			{
				return NotFound("???????? ??? ????????.");
			}

			// ???? ???? ???? ??
			if (character.Level < dungeon.RequiredCondition)
			{
				return BadRequest($"??? ?????? ??????? ???? ????({dungeon.RequiredCondition})?? ????????.");
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
		public int CoinDelta { get; set; } // ???? ?????? (+50, -20 ??)
	}

	public class UpdateDungeonRequest
	{
		[Required]
		public int DungeonIndex { get; set; }
	}
}