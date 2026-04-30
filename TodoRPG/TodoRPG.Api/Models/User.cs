using System.ComponentModel.DataAnnotations;

namespace TodoRPG.Api.Models
{
    public class User
    {
        [Key] // 이 속성이 데이터베이스의 기본 키(PK)가 됩니다.
        public int Id { get; set; }

        [Required] // 닉네임은 필수값입니다.
        public string Nickname { get; set; } = string.Empty;

        [Required] // 비밀번호는 필수값입니다.
        public string Password { get; set; } = string.Empty;

        // RPG 요소를 위해 기본적으로 필요한 컬럼들도 미리 넣어두면 좋습니다.
        // 초기 레벨 경험치 초기화
        public int Level { get; set; } = 1;
        public int Experience { get; set; } = 0;
    }
}