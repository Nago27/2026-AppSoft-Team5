namespace TodoRPG.Api.Models;

// 각 이벤트에 대한 스탯 변화량 응답값 저장용
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
