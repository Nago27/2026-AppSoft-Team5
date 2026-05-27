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
		[Range(1, int.MaxValue, ErrorMessage = "?????? 1 ??????? ????.")]
		public int Level { get; set; } = 1;

		[Required]
		[Range(0, int.MaxValue, ErrorMessage = "??????? 0 ??????? ????.")]
		public int Experience { get; set; } = 0;

		[Required]
		[Range(0, int.MaxValue, ErrorMessage = "?????? 0 ??????? ????.")]
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

		// EF Core ???? ?????? ???? ??????? ???????
		[JsonIgnore]
		public User? User { get; set; }

		[JsonIgnore]
		public Dungeon? CurrentDungeon { get; set; }
	}
}