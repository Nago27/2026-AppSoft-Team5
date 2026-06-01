namespace TodoRPG.Web.Client.Models;

public sealed class InventoryItemDto
{
    public int Id { get; set; }

    public string UserId { get; set; } = string.Empty;

    public int ShopItemId { get; set; }

    public ShopItemDto? ShopItem { get; set; }

    public int Count { get; set; }

    public bool IsEquipped { get; set; }
}

public sealed class EquipRequest
{
    public string UserId { get; set; } = string.Empty;

    public int InventoryId { get; set; }
}

public sealed class EquipResponse
{
    public string Message { get; set; } = string.Empty;

    public bool IsEquipped { get; set; }

    public CharacterDto? UpdatedCharacter { get; set; }
}

public sealed class ConsumeRequest
{
    public string UserId { get; set; } = string.Empty;

    public int ShopItemId { get; set; }
}

public sealed class ConsumeResponse
{
    public string Message { get; set; } = string.Empty;

    public int RemainingCount { get; set; }

    public CharacterDto? UpdatedCharacter { get; set; }
}