using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TodoRPG.Api.Models
{
    public class Dungeon
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)] // 자동 증가 꺼짐 (수동 지정)
        public int Index { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "던전 이름은 최대 50자입니다.")] // 임의 지정 제약조건
        public string Name { get; set; } = string.Empty;

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "입장 가능 레벨은 0 이상이어야 합니다.")]
        public int RequiredCondition { get; set; } = 0;
    }
}