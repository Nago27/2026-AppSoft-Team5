using System.Net.Http.Json;
using TodoRPG.Web.Client.Models;

namespace TodoRPG.Web.Client.Services;

public sealed class TodoApiService
{
    private readonly HttpClient http;

    public TodoApiService(HttpClient http)
    {
        this.http = http;
    }

    public async Task<List<TodoItemDto>> GetByUserAsync(string userId)
    {
        var encodedUserId = Uri.EscapeDataString(userId);

        return await http.GetFromJsonAsync<List<TodoItemDto>>(
            $"api/Todo/user/{encodedUserId}"
        ) ?? new List<TodoItemDto>();
    }

    public async Task<int> GetCompletedCountAsync(string userId)
    {
        var encodedUserId = Uri.EscapeDataString(userId);

        return await http.GetFromJsonAsync<int>(
            $"api/Todo/user/{encodedUserId}/completed-count"
        );
    }
    
    public async Task<TodoItemDto?> CreateAsync(CreateTodoRequest request)
    {
        var response = await http.PostAsJsonAsync("api/Todo", request);

        await EnsureSuccessAsync(response, "Todo 추가에 실패했습니다.");

        return await response.Content.ReadFromJsonAsync<TodoItemDto>();
    }

    public async Task UpdateAsync(int id, UpdateTodoRequest request)
    {
        var response = await http.PutAsJsonAsync($"api/Todo/{id}", request);

        await EnsureSuccessAsync(response, "Todo 수정에 실패했습니다.");
    }

    public async Task<TodoCompletionResultDto?> SetCompletedAsync(
        int id,
        SetTodoCompletedRequest request)
    {
        var response = await http.PatchAsJsonAsync(
            $"api/Todo/{id}/completed",
            request
        );

        await EnsureSuccessAsync(response, "Todo 상태 변경에 실패했습니다.");

        return await response.Content.ReadFromJsonAsync<TodoCompletionResultDto>();
    }

    public async Task DeleteAsync(int id, string userId)
    {
        var encodedUserId = Uri.EscapeDataString(userId);

        var response = await http.DeleteAsync(
            $"api/Todo/{id}?userId={encodedUserId}"
        );

        await EnsureSuccessAsync(response, "Todo 삭제에 실패했습니다.");
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
