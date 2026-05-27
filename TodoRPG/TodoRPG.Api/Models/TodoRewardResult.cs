namespace TodoRPG.Api.Models;

public sealed class TodoRewardResult
{
    public int ExperienceGained { get; set; }
    public int CoinGained { get; set; }
    public int StrengthGained { get; set; }
    public int IntelligenceGained { get; set; }
    public int FortuneGained { get; set; }
    public int HealthChanged { get; set; }
    public int MaxHealthGained { get; set; }
    public int LevelUpCount { get; set; }

    public bool DeadlineBonusApplied { get; set; }
    public bool DeadlinePenaltyApplied { get; set; }
    public bool HealthPenaltyApplied { get; set; }

    public string Message { get; set; } = string.Empty;
}