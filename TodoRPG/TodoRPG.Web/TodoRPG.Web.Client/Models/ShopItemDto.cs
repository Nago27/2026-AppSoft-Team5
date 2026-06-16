namespace TodoRPG.Web.Client.Models;

public sealed class ShopItemDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public int Cost { get; set; }

    public string PlusStat { get; set; } = string.Empty;

    public int PlusStatValue { get; set; }

    public int Stock { get; set; }

    public string ItemType { get; set; } = string.Empty;

    public string Rarity { get; set; } = string.Empty;

    public int DropWeight { get; set; }

    public bool IsSoldOut => Stock <= 0;

    public bool IsConsumable => Normalize(ItemType) == "CONSUME";

    public bool IsEquipOrPassive => Normalize(ItemType) is "EQUIP" or "NONEEQUIP";

    public string StatLabel => Normalize(PlusStat) switch
    {
        "STR" or "STRENGTH" => "STR",
        "INT" or "INTELLIGENCE" => "INT",
        "LUK" or "LUCK" or "FORTUNE" => "LUK",
        "STA" or "HP" or "HEALTH" or "MAXHP" => "Max HP",
        "DUNGEON" or "DUNGEONTICKET" or "TICKET" => "Dungeon Ticket",
        _ => PlusStat
    };

    public string EffectText => PlusStatValue <= 0
        ? Description
        : $"{StatLabel} +{PlusStatValue}";

    private static string Normalize(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        return value
            .Trim()
            .Replace(" ", string.Empty)
            .Replace("_", string.Empty)
            .Replace("-", string.Empty)
            .ToUpperInvariant();
    }
}

public sealed class PurchaseRequest
{
    public string UserId { get; set; } = string.Empty;

    public int ItemId { get; set; }
}

public sealed class PurchaseResponse
{
    public string Message { get; set; } = string.Empty;

    public int CurrentCoin { get; set; }

    public CharacterDto? UpdatedCharacter { get; set; }
}

public sealed class GachaRequest
{
    public string UserId { get; set; } = string.Empty;
}

public sealed class GachaResponse
{
    public string Message { get; set; } = string.Empty;

    public ShopItemDto? Item { get; set; }

    public int RemainingCoin { get; set; }

    public CharacterDto? UpdatedCharacter { get; set; }
}
