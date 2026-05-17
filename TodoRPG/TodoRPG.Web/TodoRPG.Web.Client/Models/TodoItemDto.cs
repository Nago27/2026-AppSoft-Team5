namespace TodoRPG.Web.Client.Models;

public sealed class TodoItemDto
{
    public int Id { get; set; }

    public string UserId { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public bool IsCompleted { get; set; }

    public string Category { get; set; } = "일상";

    public DateTime CreatedAt { get; set; }

    public DateTime? DueDate { get; set; }
}
