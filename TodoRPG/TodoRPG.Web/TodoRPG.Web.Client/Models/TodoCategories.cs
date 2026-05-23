namespace TodoRPG.Web.Client.Models;

public static class TodoCategories
{
    public const string Exercise = "운동";
    public const string Work = "업무";
    public const string Study = "자기개발";
    public const string Daily = "일상";
    public const string Custom = "기타";

    public static readonly string[] Values =
    {
        Exercise,
        Work,
        Study,
        Daily,
        Custom
    };

    public static string GetLabel(string category)
    {
        return Values.Contains(category)
            ? category
            : Custom;
    }

    public static string GetCssClass(string category)
    {
        return category switch
        {
            Exercise => "strength",
            Work => "work",
            Study => "study",
            Daily => "daily",
            Custom => "custom",
            _ => "custom"
        };
    }

    public static string GetCompleteEffectMessage(string category)
    {
        return category switch
        {
            Exercise => "운동 미션 완료",
            Work => "업무 미션 완료",
            Study => "자기개발 미션 완료",
            Daily => "일상 미션 완료",
            Custom => "기타 미션 완료",
            _ => "미션 완료"
        };
    }

    public static string GetRewardText(string category)
    {
        return category switch
        {
            Exercise => "+STR",
            Work => "+SUPPLY / -HP",
            Study => "+INT",
            Daily => "+MAX HP",
            Custom => "+CUSTOM",
            _ => "+CUSTOM"
        };
    }
}