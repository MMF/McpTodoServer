using McpTodo.Data;

namespace McpTodo.Contracts;

public class TodoItemDto
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public bool IsDone { get; set; }
    public DateTimeOffset CreatedAt { get; set; }

    public static TodoItemDto FromModel(TodoItem model)
    {
        return new TodoItemDto
        {
            Id = model.Id,
            Title = model.Title,
            IsDone = model.IsDone,
            CreatedAt = model.CreatedAt
        };
    }
}
