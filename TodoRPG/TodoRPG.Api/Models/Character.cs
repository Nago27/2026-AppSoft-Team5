using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace TodoRPG.Api.Models
{
	public class Character
	{
		[Key]
		[ForeignKey("User")]
		[StringLength(10)]
		public string UserId { get; set; } = string.Empty;

		[Required]
		[Range(1, int.MaxValue, ErrorMessage = "레벨은 1 이상이어야 합니다.")]
		public int Level { get; set; } = 1;

		[Required]
		[Range(0, int.MaxValue, ErrorMessage = "경험치는 0 이상이어야 합니다.")]
		public int Experience { get; set; } = 0;

		[Required]
		[Range(0, int.MaxValue, ErrorMessage = "코인은 0 이상이어야 합니다.")]
		public int Coin { get; set; } = 10;

		[Required]
		[Range(0, int.MaxValue)]
		public int Strength { get; set; } = 0;

		[Required]
		[Range(0, int.MaxValue)]
		public int Intelligence { get; set; } = 0;

		[Required]
		[Range(0, int.MaxValue)]
		public int Fortune { get; set; } = 0;

		[Required]
		[Range(0, int.MaxValue)]
		public int Health { get; set; } = 100;

		[Required]
        [Range(1, int.MaxValue)]
        public int MaxHealth { get; set; } = 100;

		[Required]
		public int CurrentDungeonIndex { get; set; } = 1;

        // 던전 입장을 위한 티켓
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "던전 티켓은 0개 이상이어야 합니다.")]
        public int DungeonTickets { get; set; } = 0;

		// Todo 미생성 패널티
        public DateTime? LastTodoInactivityPenaltyAt { get; set; }

        public DateTime? RewardReductionExpiresAt { get; set; }

        // EF Core 관계 설정을 위한 네비게이션 프로퍼티
        [JsonIgnore]
		public User? User { get; set; }

		[JsonIgnore]
		public Dungeon? CurrentDungeon { get; set; }
	}
}