using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using TodoRPG.Api.Data;
using TodoRPG.Api.Models;

namespace TodoRPG.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly AppDbContext _context;

        public InventoryController(AppDbContext context)
        {
            _context = context;
        }

        // 1. 특정 유저의 인벤토리 전체 목록 조회 (GET /api/Inventory/user/{userId})
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<Inventory>>> GetUserInventory(string userId)
        {
            var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
            if (!userExists)
            {
                return NotFound("존재하지 않는 사용자입니다.");
            }

            var inventoryItems = await _context.Inventories
                .Where(i => i.UserId == userId)
                .Include(i => i.ShopItem)
                .ToListAsync();

            return Ok(inventoryItems);
        }

        // ========================================================
        // 💡 2. [기능 변경] 장착 / 해제 상태 토글 API (POST /api/Inventory/toggle-equip)
        // ========================================================
        [HttpPost("toggle-equip")]
        public async Task<IActionResult> ToggleEquip([FromBody] EquipRequest request)
        {
            // [1] 인벤토리 고유 식별자(Id)를 통해 가방 데이터와 상점 마스터 정보 로드
            var inventory = await _context.Inventories
                .Include(i => i.ShopItem)
                .FirstOrDefaultAsync(i => i.Id == request.InventoryId && i.UserId == request.UserId);

            if (inventory == null || inventory.ShopItem == null)
            {
                return NotFound("가방에서 해당 아이템 기록을 추적할 수 없습니다.");
            }

            // [검산] 소모성 아이템("Consume")은 장착 연산 영역에 진입하지 못하도록 차단
            if (inventory.ShopItem.ItemType.Equals("Consume", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("소모성 아이템(포션 등)은 장착할 수 없습니다.");
            }

            // [2] 스탯 연산을 위해 유저의 캐릭터 정보 로드
            var character = await _context.Characters.FindAsync(request.UserId);
            if (character == null)
            {
                return NotFound("캐릭터 정보를 찾을 수 없습니다.");
            }

            string statType = inventory.ShopItem.PlusStat.ToLower().Trim();
            int statValue = inventory.ShopItem.PlusStatValue;

            // [3] 장착 여부 상태 플래그 판별 연산
            if (!inventory.IsEquipped)
            {
                // 미장착 -> 장착 상태 변환 (캐릭터 스탯 누적 가산 처리)
                inventory.IsEquipped = true;
                UpdateCharacterStat(character, statType, statValue);
            }
            else
            {
                // 장착 -> 미장착 상태 변환 (장착 해제에 따른 스탯 회수 역연산 처리)
                inventory.IsEquipped = false;
                UpdateCharacterStat(character, statType, -statValue); // 마이너스 부호 연산
            }

            // [4] 가방 상태 및 캐릭터 엔티티 상태 수정 마킹
            _context.Entry(inventory).State = EntityState.Modified;
            _context.Entry(character).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = inventory.IsEquipped ? $"{inventory.ShopItem.Name}을(를) 장착했습니다." : $"{inventory.ShopItem.Name}을(를) 해제했습니다.",
                IsEquipped = inventory.IsEquipped,
                UpdatedCharacter = character
            });
        }

        // ========================================================
        // 💡 3. [기능 변경] 소모성 아이템 사용 API (POST /api/Inventory/consume)
        // ========================================================
        [HttpPost("consume")]
        public async Task<IActionResult> ConsumeItem([FromBody] ConsumeRequest request)
        {
            var inventory = await _context.Inventories
                .Include(i => i.ShopItem)
                .FirstOrDefaultAsync(i => i.UserId == request.UserId && i.ShopItemId == request.ShopItemId);

            if (inventory == null || inventory.Count <= 0 || inventory.ShopItem == null)
            {
                return BadRequest("인벤토리에 해당 아이템을 소지하고 있지 않습니다.");
            }

            // [검산] 장착형 장비 아이템("Equip", "NoneEquip")은 소모 영역에서 가감 연산 차단
            if (!inventory.ShopItem.ItemType.Equals("Consume", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("장착용 장비 또는 비장착 전용 장비는 마셔서 소모할 수 없습니다.");
            }

            var character = await _context.Characters.FindAsync(request.UserId);
            if (character == null)
            {
                return NotFound("캐릭터 정보를 찾을 수 없습니다.");
            }

            // [1] 스탯 즉시 반영 처리
            string statType = inventory.ShopItem.PlusStat.ToLower().Trim();
            UpdateCharacterStat(character, statType, inventory.ShopItem.PlusStatValue);

            // [2] 수량 감산 계산
            inventory.Count -= 1;

            // [3] 0개 도달 시 찌꺼기 레코드 제거, 남아있으면 수정 상태 처리
            if (inventory.Count == 0)
            {
                _context.Inventories.Remove(inventory);
            }
            else
            {
                _context.Entry(inventory).State = EntityState.Modified;
            }

            _context.Entry(character).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = $"{inventory.ShopItem.Name}을(를) 1개 소모했습니다.",
                RemainingCount = inventory.Count,
                UpdatedCharacter = character
            });
        }

        // 4. 인벤토리에서 아이템 완전히 버리기 (DELETE /api/Inventory/discard)
        [HttpDelete("discard")]
        public async Task<IActionResult> DiscardItem([FromQuery] string userId, [FromQuery] int shopItemId)
        {
            var inventory = await _context.Inventories
                .FirstOrDefaultAsync(i => i.UserId == userId && i.ShopItemId == shopItemId);

            if (inventory == null)
            {
                return NotFound("버릴 아이템이 가방에 존재하지 않습니다.");
            }

            _context.Inventories.Remove(inventory);
            await _context.SaveChangesAsync();

            return Ok("아이템을 인벤토리에서 완전히 폐기했습니다.");
        }

        // 💡 [서브 연산 헬퍼 함수] 공통 수치 제어 연산식 규칙 정의
        private void UpdateCharacterStat(Character character, string statType, int value)
        {
            switch (statType)
            {
                case "strength":
                case "근력":
                    character.Strength += value;
                    break;
                case "intelligence":
                case "지능":
                    character.Intelligence += value;
                    break;
                case "fortune":
                case "행운":
                    character.Fortune += value;
                    break;
                case "health":
                case "체력":
                    character.Health += value;
                    break;
            }
        }
    }

    // 💡 장착 요청 용 DTO 규격 정의
    public sealed class EquipRequest
    {
        [Required]
        public string UserId { get; set; } = string.Empty;
        [Required]
        public int InventoryId { get; set; } // 인벤토리 고유 기본키 매핑
    }

    // 💡 소모 요청 용 DTO 규격 정의
    public sealed class ConsumeRequest
    {
        [Required]
        public string UserId { get; set; } = string.Empty;
        [Required]
        public int ShopItemId { get; set; }
    }
}