using System.Net.Http.Json;
using TodoRPG.Web.Client.Models;

namespace TodoRPG.Web.Client.Services;

public sealed class ShopApiService
{
    private readonly HttpClient http;

    public ShopApiService(HttpClient http)
    {
        this.http = http;
    }

    public async Task<List<ShopItemDto>> GetItemsAsync()
    {
        return await http.GetFromJsonAsync<List<ShopItemDto>>("api/Shop")
            ?? new List<ShopItemDto>();
    }

    public async Task<PurchaseResponse?> PurchaseAsync(string userId, int itemId)
    {
        var request = new PurchaseRequest
        {
            UserId = userId,
            ItemId = itemId
        };

        var response = await http.PostAsJsonAsync("api/Shop/purchase", request);

        await EnsureSuccessAsync(response, "아이템 구매에 실패했습니다.");

        return await response.Content.ReadFromJsonAsync<PurchaseResponse>();
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