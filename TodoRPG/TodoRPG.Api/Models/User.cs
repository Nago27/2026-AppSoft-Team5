using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TodoRPG.Api.Models
{
    public class User
    {
        [Key] // 이 필드를 기본키로 설정합니다.
        [Column(TypeName = "VARCHAR")] // DB에서 타입을 VARCHAR로 지정합니다.
        [StringLength(10, MinimumLength = 2, ErrorMessage = "ID는 2~10자여야 합니다.")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)] // 자동 증가를 끄고 직접 입력하게 합니다.
        public string Id { get; set; } = string.Empty; // 타입을 int에서 string으로 변경했습니다.

        [Required(ErrorMessage = "닉네임을 입력하세요.")]
        [StringLength(10, MinimumLength = 2, ErrorMessage = "닉네임은 2~10자여야 합니다.")]
        public string Nickname { get; set; } = string.Empty;

        [Required(ErrorMessage = "비밀번호를 입력하세요.")]
        [StringLength(20, MinimumLength = 4, ErrorMessage = "비밀번호는 4~20자여야 합니다.")]
        public string Password { get; set; } = string.Empty;

        // RPG 요소를 위해 기본적으로 필요한 컬럼들도 미리 넣어두면 좋습니다.
        // 초기 레벨 경험치 초기화
        public int Level { get; set; } = 1;
        public int Experience { get; set; } = 0;
    }
}