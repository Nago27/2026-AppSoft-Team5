using TodoRPG.Api.Models;

namespace TodoRPG.Api.Services;

public static class RPGCalculator
{
    public const int MaxLevel = 50;

    public static int GetRequiredExperience(int level)
    {
        return (int)Math.Floor(90 + (10 * level) + (0.5 * level * level));
    }

    public static int FloorMultiply(int value, double multiplier)
    {
        return (int)Math.Floor(value * multiplier);
    }

    public static int Percent(int value, double rate)
    {
        return (int)Math.Floor(value * rate);
    }

    public static int ApplyLevelUps(Character character)
    {
        var levelUpCount = 0;

        while (character.Level < MaxLevel
               && character.Experience >= GetRequiredExperience(character.Level))
        {
            character.Experience -= GetRequiredExperience(character.Level);
            character.Level += 1;
            levelUpCount += 1;
        }

        if (character.Level >= MaxLevel)
        {
            character.Level = MaxLevel;
            character.Experience = 0;
        }

        if (levelUpCount > 0)
        {
            character.Health = character.MaxHealth;
        }

        return levelUpCount;
    }
}