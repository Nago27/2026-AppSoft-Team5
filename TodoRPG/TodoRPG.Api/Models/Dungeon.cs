using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TodoRPG.Api.Models
{
    public class Dungeon
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)] // �ڵ� ���� ���� (���� ����)
        public int Index { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "���� �̸��� �ִ� 50���Դϴ�.")] // ���� ���� ��������
        public string Name { get; set; } = string.Empty;

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "���� ���� ������ 0 �̻��̾�� �մϴ�.")]
        public int RequiredCondition { get; set; } = 0;
    }
}