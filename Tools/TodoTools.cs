using McpTodoServer.Contracts;
using McpTodoServer.Data;
using Microsoft.EntityFrameworkCore;
using ModelContextProtocol.Server;
using System.ComponentModel;

namespace McpTodoServer.Tools;

internal class TodoTools(TodoDbContext db)
{
    [McpServerTool]
    [Description("Adds a new todo item with the specified title and returns its ID.")]
    public async Task<McpToolResult<int>> AddTodoItem(
        [Description("title of the todo item")] string title,
        CancellationToken ct)
    {
        title = title?.Trim() ?? "";

        var validationErrors = await ValidateAsync(title, ct);
        if (validationErrors.Any())
            return McpToolResult<int>.Failure(validationErrors);

        var todoItem = new TodoItem
        {
            Title = title,
            IsDone = false,
            CreatedAt = DateTimeOffset.UtcNow
        };

        db.TodoItems.Add(todoItem);
        await db.SaveChangesAsync(ct);

        return McpToolResult<int>.Success(todoItem.Id);
    }

    private async Task<List<string>> ValidateAsync(string title, CancellationToken ct)
    {
        var errors = new List<string>();

        // has value
        if (string.IsNullOrEmpty(title))
        {
            errors.Add("Title cannot be empty.");
            return errors;
        }

        // length 
        if (title.Length > 300)
        {
            errors.Add("Title cannot exceed 300 characters.");
            return errors;
        }

        // duplicate
        var existingItem = await db.TodoItems.AnyAsync(t => t.Title.ToLower() == title.ToLower(), ct);
        if (existingItem)
            errors.Add("A todo item with the same title already exists.");

        return errors;
    }

    [McpServerTool]
    [Description("List all todo items.")]
    public async Task<McpToolResult<List<TodoItemDto>>> GetTodoItems(CancellationToken ct)
    {
        var data = await db
            .TodoItems
            .OrderByDescending(t => t.CreatedAt)
            .Select(t => TodoItemDto.FromModel(t))
            .ToListAsync(ct);
        return McpToolResult<List<TodoItemDto>>.Success(data);
    }

    [McpServerTool]
    [Description("Marks the specified todo item as complete.")]
    public async Task<McpToolResult<bool>> MarkTodoItemAsComplete(
        [Description("Id of the todo item")] int id,
        CancellationToken ct)
    {
        var todoItem = await db.TodoItems.FindAsync(id, ct);
        if (todoItem == null)
            return McpToolResult<bool>.Failure($"Todo item with ID {id} not found.");

        if (todoItem.IsDone)
            return McpToolResult<bool>.Success(true, $"Todo item with ID {id} is already marked as complete.");

        todoItem.IsDone = true;
        await db.SaveChangesAsync(ct);

        return McpToolResult<bool>.Success(true, $"Todo item with ID {id} marked as complete.");
    }
}
