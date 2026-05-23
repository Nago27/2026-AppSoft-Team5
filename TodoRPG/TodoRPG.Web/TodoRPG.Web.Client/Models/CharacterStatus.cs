namespace TodoRPG.Web.Client.Models;

public sealed class CharacterStatus
{
    public int Level { get; set; } = 1;

    public int Hp { get; set; } = 100;
    public int MaxHp { get; set; } = 100;

    public int Exp { get; set; } = 30;
    public int MaxExp { get; set; } = 100;

    public int Supply { get; set; } = 247;

    public int Strength { get; set; } = 42;
    public int Intelligence { get; set; } = 31;
    public int Luck { get; set; } = 28;

    public string Title { get; set; } = "벙커 신입 생존자";

    public int HpPercent => MaxHp == 0 ? 0 : Math.Clamp(Hp * 100 / MaxHp, 0, 100);
    public int ExpPercent => MaxExp == 0 ? 0 : Math.Clamp(Exp * 100 / MaxExp, 0, 100);
}
