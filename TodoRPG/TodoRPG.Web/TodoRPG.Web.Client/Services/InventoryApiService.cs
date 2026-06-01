using System.Net.Http.Json;
using TodoRPG.Web.Client.Models;

namespace TodoRPG.Web.Client.Services;

public sealed class InventoryApiService
{
    private readonly HttpClient http;

    public InventoryApiService(HttpClient http)
    {
        this.http = http;
    }

    public async Task<List<InventoryItemDto>> GetByUserAsync(string userId)
    {
        var encodedUserId = Uri.EscapeDataString(userId);

        return await http.GetFromJsonAsync<List<InventoryItemDto>>(
            $"api/Inventory/user/{encodedUserId}"
        ) ?? new List<InventoryItemDto>();
    }

    public async Task<EquipResponse?> ToggleEquipAsync(string userId, int inventoryId)
    {
        var request = new EquipRequest
        {
            UserId = userId,
            InventoryId = inventoryId
        };

        var response = await http.PostAsJsonAsync("api/Inventory/toggle-equip", request);

        await EnsureSuccessAsync(response, "아이템 효과 상태 변경에 실패했습니다.");

        return await response.Content.ReadFromJsonAsync<EquipResponse>();
    }

    public async Task<ConsumeResponse?> ConsumeAsync(string userId, int shopItemId)
    {
        var request = new ConsumeRequest
        {
            UserId = userId,
            ShopItemId = shopItemId
        };

        var response = await http.PostAsJsonAsync("api/Inventory/consume", request);

        await EnsureSuccessAsync(response, "아이템 사용에 실패했습니다.");

        return await response.Content.ReadFromJsonAsync<ConsumeResponse>();
    }

    private static async Task EnsureSuccessAsync(
        HttpResponseMessage response,
        string fallbackMessage)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        var responseText = await response.Content.ReadAsStringAsync();

        var message = string.IsNullOrWhiteSpace(responseText)
            ? fallbackMessage
            : responseText.Trim().Trim('"');

        throw new InvalidOperationException(message);
    }
}