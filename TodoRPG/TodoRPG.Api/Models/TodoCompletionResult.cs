namespace TodoRPG.Api.Models;

public sealed class TodoCompletionResult
{
    public TodoItem TodoItem { get; set; } = new();

    public Character Character { get; set; } = new();

    public TodoRewardResult Reward { get; set; } = new();
}