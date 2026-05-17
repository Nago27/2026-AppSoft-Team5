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
        return category switch
        {
            "운동" => "PATROL",
            "업무" => "SUPPLY",
            "자기개발" => "INTEL",
            "일상" => "SHELTER",
            "기타" => "CUSTOM",
            _ => "CUSTOM"
        };
    }

    public static string GetCssClass(string category)
    {
        return category switch
        {
            "운동" => "patrol",
            "업무" => "supply",
            "자기개발" => "intel",
            "일상" => "shelter",
            "기타" => "custom",
            _ => "custom"
        };
    }

    public static string GetCompleteEffectMessage(string category)
    {
        return category switch
        {
            "운동" => "PATROL MISSION COMPLETED",
            "업무" => "SUPPLY TASK COMPLETED",
            "자기개발" => "INTEL MISSION COMPLETED",
            "일상" => "SHELTER MISSION COMPLETED",
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
