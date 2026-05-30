using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq; // 💡 3. Sum, First 등을 사용하기 위해 추가
using System.Threading.Tasks;
using TodoRPG.Api.Data;
using TodoRPG.Api.Models;

namespace TodoRPG.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GachaController : ControllerBase
    {
        private readonly AppDbContext _context;
        private static readonly Random _random = new();
        private const int GachaCost = 100; // 1회 뽑기 비용 (100코인)

        public GachaController(AppDbContext context)
        {
            _context = context;
        }

        // POST: api/Gacha/draw
        [HttpPost("draw")]
        public async Task<IActionResult> DrawItem([FromBody] GachaRequest request)
        {
            var userId = request.UserId.Trim();

            // 1. 유저 및 캐릭터 존재 확인
            var character = await _context.Characters
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (character == null)
            {
                return NotFound("캐릭터를 찾을 수 없습니다.");
            }

            // 2. 코인 충분한지 확인
            if (character.Coin < GachaCost)
            {
                return BadRequest("코인이 부족합니다.");
            }

            // 3. 뽑기 가능한 아이템 목록 가져오기
            var items = await _context.ShopItems.ToListAsync();
            if (!items.Any())
            {
                return BadRequest("뽑을 수 있는 아이템이 존재하지 않습니다.");
            }

            // 4. 가중치 기반 무작위 아이템 선정 (Roulette Wheel Selection)
            int totalWeight = items.Sum(i => i.DropWeight);
            int roll = _random.Next(0, totalWeight);
            int currentSum = 0;

            // 💡 1. Item? 에서 ShopItem? 으로 타입을 명확히 정정
            ShopItem? selectedItem = null;

            foreach (var item in items)
            {
                currentSum += item.DropWeight;
                if (roll < currentSum)
                {
                    selectedItem = item;
                    break;
                }
            }

            if (selectedItem == null) selectedItem = items.First(); // 방어 코드

            // 5. coin 차감 및 인벤토리 저장
            character.Coin -= GachaCost;

            // 💡 2. ItemId를 AppDbContext 규격인 ShopItemId로 변경
            var inventoryItem = await _context.Inventories
                .FirstOrDefaultAsync(i => i.UserId == userId && i.ShopItemId == selectedItem.Id);

            if (inventoryItem != null)
            {
                // 이미 가지고 있는 아이템이면 개수 증가
                inventoryItem.Count += 1;
            }
            else
            {
                // 새로 획득한 아이템이면 인벤토리에 추가
                var newInventory = new Inventory
                {
                    UserId = userId,
                    ShopItemId = selectedItem.Id, // 💡 규격 일치화
                    Count = 1,
                    IsEquipped = false
                };
                _context.Inventories.Add(newInventory);
            }

            // 6. DB 반영
            await _context.SaveChangesAsync();

            // 7. 결과 반환
            return Ok(new
            {
                Message = $"{selectedItem.Name}({selectedItem.Rarity})을(를) 뽑았습니다!",
                Item = new
                {
                    selectedItem.Id,
                    selectedItem.Name,
                    selectedItem.Rarity,
                    selectedItem.Description
                },
                RemainingCoin = character.Coin
            });
        }
    }

    public class GachaRequest
    {
        public string UserId { get; set; } = string.Empty;
    }
}