using System.Net.Http.Json;

namespace TodoRPG.Web.Client.Services;

public sealed class DungeonApiService
{
    private readonly HttpClient http;

    public DungeonApiService(HttpClient http)
    {
        this.http = http;
    }

    public async Task<int> GetClearCountAsync(string userId)
    {
        var encodedUserId = Uri.EscapeDataString(userId);

        return await http.GetFromJsonAsync<int>(
            $"api/Dungeon/user/{encodedUserId}/clear-count"
        );
    }
}