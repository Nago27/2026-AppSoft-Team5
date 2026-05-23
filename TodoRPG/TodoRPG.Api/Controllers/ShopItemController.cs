using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoRPG.Api.Data;
using TodoRPG.Api.Models;

namespace TodoRPG.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShopController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ShopController(AppDbContext context)
        {
            _context = context;
        }

        // 상점 아이템 구매 처리 API (POST /api/Shop/purchase)
        [HttpPost("purchase")]
        public async Task<IActionResult> PurchaseItem([FromBody] PurchaseRequest request)
        {
            // 1. 구매할 상점 아이템 데이터 스캔
            var shopItem = await _context.ShopItems.FindAsync(request.ItemId);
            if (shopItem == null)
            {
                return NotFound("상점에 등록되지 않은 상품 항목입니다.");
            }
            // ========================================================
            // 💡 2. [사전 검산] 상점 아이템 소진(재고 부족) 여부 체크 및 차단
            // ========================================================
            if (shopItem.Stock <= 0)
            {
                return BadRequest($"{shopItem.Name} 상품이 모두 소진(매진)되어 구매할 수 없습니다.");
            }

            // 2. 구매를 시도하는 유저의 캐릭터 데이터 로드
            var character = await _context.Characters.FindAsync(request.UserId);
            if (character == null)
            {
                return NotFound("해당 사용자의 캐릭터를 추적할 수 없습니다.");
            }

            // 3. [사전 검산] 유저 잔여 코인이 상품의 Cost 요구량보다 크거나 같은지 검증
            if (character.Coin < shopItem.Cost)
            {
                return BadRequest("코인이 부족하여 상품을 획득할 수 없습니다.");
            }

            // 상점 내 아이템 개수 1개 감소
            shopItem.Stock -= 1;
            // 4. [재화 차감 연산] 코인 감소 처리
            character.Coin -= shopItem.Cost;

            // ========================================================
            // 💡 6. [인벤토리 테이블 가방 누적 및 생성 연산 추가]
            // ========================================================
            // 유저 가방에 이미 동일한 상품이 들어있는지 데이터베이스에서 스캔
            var inventoryItem = await _context.Inventories
    .FirstOrDefaultAsync(i => i.UserId == request.UserId && i.ShopItemId == request.ItemId && i.IsEquipped == false);

            if (inventoryItem != null && shopItem.ItemType == "Consume")
            {
                // 소모품이고 가방에 있으면 개수만 +1 계산
                inventoryItem.Count += 1;
                _context.Entry(inventoryItem).State = EntityState.Modified;
            }
            else
            {
                // 장착 아이템이거나 가방에 없는 물건이면 가방에 새로 할당
                var newInventory = new Inventory
                {
                    UserId = request.UserId,
                    ShopItemId = request.ItemId,
                    Count = 1,
                    IsEquipped = false // 처음 살 때는 미장착 상태가 기본값
                };
                _context.Inventories.Add(newInventory);
            }

            // 7. 캐릭터 정보 변경 상태 마킹
            _context.Entry(shopItem).State = EntityState.Modified;
            _context.Entry(character).State = EntityState.Modified;

            // 8. 코인 차감 + 스탯 가산 + 인벤토리 갱신 트랜잭션을 영구 저장소에 일괄 반영
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = $"{shopItem.Name} 상품의 구매 및 인벤토리 저장이 완료되었습니다.",
                CurrentCoin = character.Coin,
                UpdatedCharacter = character
            });
        }
    }

    // 명세서 요청 형식 맵핑 DTO
    public sealed class PurchaseRequest
    {
        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public int ItemId { get; set; }
    }
}