namespace TodoRPG.Web.Client.Models;

public sealed class CharacterDto
{
    public string UserId { get; set; } = string.Empty;

    public int Level { get; set; }

    public int Experience { get; set; }

    public int Coin { get; set; }

    public int Strength { get; set; }

    public int Intelligence { get; set; }

    public int Fortune { get; set; }

    public int Health { get; set; }

    public int CurrentDungeonIndex { get; set; }
}