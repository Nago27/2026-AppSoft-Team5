using System.Net.Http.Json;
using TodoRPG.Web.Client.Models;

namespace TodoRPG.Web.Client.Services;

public sealed class CharacterApiService
{
    private readonly HttpClient http;

    public CharacterApiService(HttpClient http)
    {
        this.http = http;
    }

    public async Task<CharacterDto?> GetByUserAsync(string userId)
    {
        var encodedUserId = Uri.EscapeDataString(userId);

        return await http.GetFromJsonAsync<CharacterDto>(
            $"api/Character/{encodedUserId}"
        );
    }
}