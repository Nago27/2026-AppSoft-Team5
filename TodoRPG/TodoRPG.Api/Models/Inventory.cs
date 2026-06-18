using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace TodoRPG.Api.Models
{
    public class Inventory
    {
        // 1. 인벤토리 내역 고유 식별 번호 (기본키, 자동 증가)
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        // 2. 가방 소유 유저 ID (외래키 설정)
        [Required]
        [Column(TypeName = "VARCHAR")]
        [StringLength(10)]
        public string UserId { get; set; } = string.Empty;

        // 💡 [외래키 연결] UserId 필드가 아래 User 객체 정보와 맵핑됨을 직접 명시합니다.
        [ForeignKey("UserId")]
        [JsonIgnore]
        public virtual User? User { get; set; }

        // 🌟 [추가/수정] Character와의 맵핑 관계를 명시하여 AppDbContext와의 뼈대 싱크를 맞춥니다.
        [ForeignKey("UserId")]
        [JsonIgnore]
        public virtual Character? Character { get; set; }

        // 3. 소지 중인 상점 아이템 ID (외래키 설정)
        [Required]
        public int ShopItemId { get; set; }

        // 💡 [외래키 연결] ShopItemId 필드가 위의 ShopItem 마스터 테이블 테이블과 맵핑됨을 명시합니다.
        [ForeignKey("ShopItemId")]
        public virtual ShopItem? ShopItem { get; set; }

        // 4. 아이템 소지 개수
        [Required]
        [Range(1, int.MaxValue)]
        public int Count { get; set; } = 1;

        // 아이템 장착 여부
        [Required]
        public bool IsEquipped { get; set; } = false;
    }
}