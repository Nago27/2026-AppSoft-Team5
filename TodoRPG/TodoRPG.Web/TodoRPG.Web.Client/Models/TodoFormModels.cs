using System.ComponentModel.DataAnnotations;

namespace TodoRPG.Web.Client.Models;

public sealed class TodoCreateModel
{
    [Required(ErrorMessage = "내용을 입력하세요.")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "내용은 1~100자여야 합니다.")]
    public string Title { get; set; } = string.Empty;

    public string Category { get; set; } = "일상";

    public DateTime? DueDate { get; set; }
}

public sealed class TodoEditModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "내용을 입력하세요.")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "내용은 1~100자여야 합니다.")]
    public string Title { get; set; } = string.Empty;

    public string Category { get; set; } = "일상";

    public bool IsCompleted { get; set; }

    public DateTime? DueDate { get; set; }

    public static TodoEditModel FromTodo(TodoItemDto todo)
    {
        return new TodoEditModel
        {
            Id = todo.Id,
            Title = todo.Title,
            Category = todo.Category,
            IsCompleted = todo.IsCompleted,
            DueDate = todo.DueDate
        };
    }
}

public sealed class CreateTodoRequest
{
    public string UserId { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Category { get; set; } = "일상";

    public DateTime? DueDate { get; set; }
}

public sealed class UpdateTodoRequest
{
    public string UserId { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Category { get; set; } = "일상";

    public bool IsCompleted { get; set; }

    public DateTime? DueDate { get; set; }
}

public sealed class SetTodoCompletedRequest
{
    public string UserId { get; set; } = string.Empty;

    public bool IsCompleted { get; set; }
}

public sealed class TodoCompletedChange
{
    public int TodoId { get; set; }

    public bool IsCompleted { get; set; }
}

public sealed class TodoRewardResultDto
{
    public int ExperienceGained { get; set; }

    public int CoinGained { get; set; }

    public int StrengthGained { get; set; }

    public int IntelligenceGained { get; set; }

    public int FortuneGained { get; set; }

    public int HealthChanged { get; set; }

    public int MaxHealthGained { get; set; }

    public int LevelUpCount { get; set; }

    public int ExperienceLost { get; set; }
    public int CoinLost { get; set; }
    public int HealthRecovered { get; set; }
    public int TodoInactivityHealthLost { get; set; }

    public bool DeadlineBonusApplied { get; set; }

    public bool DeadlinePenaltyApplied { get; set; }

    public bool HealthPenaltyApplied { get; set; }

    public bool TodoInactivityPenaltyApplied { get; set; }

    public bool RewardReductionApplied { get; set; }

    public string Message { get; set; } = string.Empty;
}

public sealed class TodoCompletionResultDto
{
    public TodoItemDto TodoItem { get; set; } = new();

    public CharacterDto Character { get; set; } = new();

    public TodoRewardResultDto Reward { get; set; } = new();
}
