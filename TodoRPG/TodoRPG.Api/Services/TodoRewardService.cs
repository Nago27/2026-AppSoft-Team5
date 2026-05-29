using Microsoft.EntityFrameworkCore;
using TodoRPG.Api.Data;
using TodoRPG.Api.Models;

namespace TodoRPG.Api.Services;

public sealed class TodoRewardService
{
    private readonly AppDbContext context;
    private readonly Random random = new();

    public TodoRewardService(AppDbContext context)
    {
        this.context = context;
    }

    public async Task<TodoCompletionResult?> SetCompletedAsync(
        int todoId,
        string userId,
        bool isCompleted)
    {
        var todo = await context.TodoItems
            .FirstOrDefaultAsync(item => item.Id == todoId && item.UserId == userId);

        if (todo is null)
        {
            return null;
        }

        var character = await context.Characters
            .FirstOrDefaultAsync(item => item.UserId == userId);

        if (character is null)
        {
            return null;
        }

        if (!isCompleted)
        {
            return new TodoCompletionResult
            {
                TodoItem = todo,
                Character = character,
                Reward = new TodoRewardResult
                {
                    Message = "완료된 Todo는 미완료로 되돌릴 수 없습니다."
                }
            };
        }

        if (todo.IsCompleted)
        {
            return new TodoCompletionResult
            {
                TodoItem = todo,
                Character = character,
                Reward = new TodoRewardResult
                {
                    Message = "이미 완료된 Todo입니다."
                }
            };
        }

        todo.IsCompleted = true;
        todo.CompletedAt = DateTime.UtcNow;

        var reward = ApplyCompletionReward(todo, character);

        await context.SaveChangesAsync();

        return new TodoCompletionResult
        {
            TodoItem = todo,
            Character = character,
            Reward = reward
        };
    }

    // Todo에 대한 보상/패널티
    private TodoRewardResult ApplyCompletionReward(TodoItem todo, Character character)
    {
        var before = CharacterSnapshot.From(character);
        var reward = new TodoRewardResult();

        // 기본 보상안
        var baseExperience = 20 + (character.Level / 2);
        var baseCoin = 10 + (character.Level / 5);

        // 마감 기한 Todo 추가 보상
        reward.ExperienceGained = IsDeadlineSuccess(todo)
            ? RPGCalculator.FloorMultiply(baseExperience, 1.2)
            : baseExperience;

        reward.DeadlineBonusApplied = IsDeadlineSuccess(todo);

        // "업무" 추가 보상
        reward.CoinGained = todo.Category == "업무"
            ? RPGCalculator.FloorMultiply(baseCoin, 1.2)
            : baseCoin;

        character.Experience += reward.ExperienceGained;
        character.Coin += reward.CoinGained;

        ApplyCategoryReward(todo.Category, character, reward);
        ChangeHealth(character, 1);

        // 마감 기한 실패 시 
        if (IsDeadlineFailed(todo))
        {
            var penalty = RPGCalculator.Percent(character.MaxHealth, 0.1);
            ChangeHealth(character, -penalty);
            reward.DeadlinePenaltyApplied = true;
        }

        if (character.Health <= 0)
        {
            ApplyHealthZeroPenalty(character, reward);
        }

        reward.LevelUpCount = RPGCalculator.ApplyLevelUps(character);

        reward.HealthChanged = character.Health - before.Health;
        reward.MaxHealthGained = character.MaxHealth - before.MaxHealth;
        reward.StrengthGained = character.Strength - before.Strength;
        reward.IntelligenceGained = character.Intelligence - before.Intelligence;
        reward.FortuneGained = character.Fortune - before.Fortune;
        reward.Message = BuildRewardMessage(reward);

        return reward;
    }


    // 카테고리 별 스탯 보상안
    private void ApplyCategoryReward(
        string category,
        Character character,
        TodoRewardResult reward)
    {
        switch (category)
        {
            case "운동":
                character.Strength += 2;
                break;

            case "자기개발":
                character.Intelligence += 2;
                break;

            case "업무":
                var workPenalty = RPGCalculator.Percent(character.MaxHealth, 0.05);
                ChangeHealth(character, -workPenalty);
                break;

            case "일상":
                character.MaxHealth += 2;
                break;

            case "기타":
                if (random.NextDouble() < 0.3)
                {
                    character.Fortune += 1;
                }
                break;
        }
    }

    private static void ChangeHealth(Character character, int amount)
    {
        character.Health = Math.Min(character.MaxHealth, character.Health + amount);
    }

    // 마감기한 성공/실패 확인
    private static bool IsDeadlineSuccess(TodoItem todo)
    {
        return todo.DueDate is not null
            && todo.CompletedAt is not null
            && todo.CompletedAt.Value.Date <= todo.DueDate.Value.Date;
    }
    private static bool IsDeadlineFailed(TodoItem todo)
    {
        return todo.DueDate is not null
            && todo.CompletedAt is not null
            && todo.CompletedAt.Value.Date > todo.DueDate.Value.Date;
    }

    // 캐릭터 체력 0에 대한 패널티 
    private static void ApplyHealthZeroPenalty(Character character, TodoRewardResult reward)
    {
        var experiencePenalty = CalculateExperiencePenalty(character.Experience);
        var coinPenalty = 10 + (character.Level * 2);
        var actualCoinPenalty = Math.Min(character.Coin, coinPenalty);
        var recoveredHealth = Math.Max(
            1,
            RPGCalculator.Percent(character.MaxHealth, 0.5)
        );

        character.Experience = Math.Max(0, character.Experience - experiencePenalty);
        character.Coin = Math.Max(0, character.Coin - actualCoinPenalty);
        character.Health = recoveredHealth;

        reward.ExperienceLost += experiencePenalty;
        reward.CoinLost += actualCoinPenalty;
        reward.HealthRecovered = recoveredHealth;
        reward.HealthPenaltyApplied = true;
    }

    // 경험치 감소 로직
    private static int CalculateExperiencePenalty(int currentExperience)
    {
        if (currentExperience <= 0)
        {
            return 0;
        }

        var penalty = Math.Max(
            1,
            RPGCalculator.Percent(currentExperience, 0.1)
        );

        return Math.Min(currentExperience, penalty);
    }

    // 보상 이벤트 메세지
    private static string BuildRewardMessage(TodoRewardResult reward)
    {
        var messages = new List<string>
        {
            $"EXP +{reward.ExperienceGained}",
            $"COIN +{reward.CoinGained}"
        };

        AddPositive(messages, "STR", reward.StrengthGained);
        AddPositive(messages, "INT", reward.IntelligenceGained);
        AddPositive(messages, "LUK", reward.FortuneGained);
        AddPositive(messages, "MAX HP", reward.MaxHealthGained);
        AddSigned(messages, "HP", reward.HealthChanged);

        if (reward.LevelUpCount > 0)
        {
            messages.Add(reward.LevelUpCount == 1
                ? "LEVEL UP"
                : $"LEVEL UP x{reward.LevelUpCount}");
        }

        if (reward.HealthPenaltyApplied)
        {
            messages.Add("HEALTH PENALTY");
        }

        return string.Join(" / ", messages);
    }

    private static void AddPositive(List<string> messages, string label, int value)
    {
        if (value > 0)
        {
            messages.Add($"{label} +{value}");
        }
    }

    private static void AddSigned(List<string> messages, string label, int value)
    {
        if (value > 0)
        {
            messages.Add($"{label} +{value}");
        }
        else if (value < 0)
        {
            messages.Add($"{label} {value}");
        }
    }

    private readonly record struct CharacterSnapshot(
        int Health,
        int MaxHealth,
        int Strength,
        int Intelligence,
        int Fortune)
    {
        public static CharacterSnapshot From(Character character)
        {
            return new CharacterSnapshot(
                character.Health,
                character.MaxHealth,
                character.Strength,
                character.Intelligence,
                character.Fortune);
        }
    }
}
