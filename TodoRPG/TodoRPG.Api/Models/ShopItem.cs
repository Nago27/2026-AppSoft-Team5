using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TodoRPG.Api.Models
{
    public class ShopItem
    {
        // 1. 아이템 고유 식별 번호 (기본키, 자동 증가)
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        // 2. 상점 아이템 이름
        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;

        // 3. 아이템 설명
        public string Description { get; set; } = string.Empty;

        // 4. 구매 가격 (Cost)
        [Required]
        [Range(0, int.MaxValue)]
        public int Cost { get; set; } = 0;

        // 5. 상승시킬 스탯 종류 (예: "strength", "intelligence", "fortune", "health")
        [Required]
        public string PlusStat { get; set; } = string.Empty;

        // 6. 스탯 증가 수치량
        [Required]
        [Range(0, int.MaxValue)]
        public int PlusStatValue { get; set; } = 0;

        // 7. 상점 재고
        [Required]
        [Range(0, int.MaxValue)]
        public int Stock { get; set; } = 0;

        // 아이템 분류 "Equip" (장착아이템), "NoneEquip" (비장착아이템), "Consume" (소모성아이템)
        [Required]
        public string ItemType { get; set; } = "Consume";

        // [뽑기용 추가 컬럼]
        [Required]
        public string Rarity { get; set; } = "Common"; // "Common", "Rare", "Epic", "Legendary"

        [Required]
        [Range(1, int.MaxValue)] // 가중치는 최소 1 이상이어야 확률 계산에 포함됩니다.
        public int DropWeight { get; set; } = 100;
    }
}