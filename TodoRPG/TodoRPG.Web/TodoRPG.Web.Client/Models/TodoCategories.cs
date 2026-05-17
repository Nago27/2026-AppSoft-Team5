namespace TodoRPG.Web.Client.Models;

public static class TodoCategories
{
    public static readonly string[] Values =
    {
        "운동",
        "업무",
        "자기개발",
        "일상",
        "기타"
    };

    public static string GetLabel(string category)
    {
        return Values.Contains(category)
            ? category
            : "기타";
    }

    public static string GetCssClass(string category)
    {
        return category switch
        {
            "운동" => "patrol",
            "업무" => "supply",
            "자기개발" => "intel",
            "일상" => "shelter",
            _ => "custom"
        };
    }

    public static string GetCompleteEffectMessage(string category)
    {
        return category switch
        {
            "운동" => "운동 COMPLETED",
            "업무" => "업무 COMPLETED",
            "자기개발" => "자기개발 COMPLETED",
            "일상" => "일상 COMPLETED",
            _ => "MISSION COMPLETED"
        };
    }

    public static string GetRewardText(string category)
    {
        return category switch
        {
            "운동" => "+STR",
            "업무" => "+SUP / -HP",
            "자기개발" => "+INT",
            "일상" => "+HP",
            _ => "+LUK"
        };
    }
}
