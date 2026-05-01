using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TodoRPG.Api.Models
{
    public class TodoItem
    {
        [Key] // 이 속성이 데이터베이스의 기본 키(PK)가 됩니다.
        public int Id { get; set; }

        [Required]
        [StringLength(10)]
        public string UserId { get; set; } = string.Empty;

        [Required] // 제목은 필수값입니다.
        public string Title { get; set; } = string.Empty;

        public bool IsCompleted { get; set; } = false;

        // RPG 요소: 카테고리에 따라 오르는 스탯을 다르게 설정하기 위함
        public string Category { get; set; } = "일상";

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? DueDate { get; set; } // 던전 시스템용 (nullable)

        [JsonIgnore]
        public User? User { get; set; }
    }
}