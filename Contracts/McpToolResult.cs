namespace McpTodoServer.Contracts;

public class McpToolResult<T>
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public  List<string> Errors { get; set; } = new();

    public static McpToolResult<T> Success(T data, string msg = "Completed Successfully")
    {
        return new McpToolResult<T>
        {
            IsSuccess = true,
            Data = data,
            Message = msg,
            Errors = new List<string>()
        };
    }

    public static McpToolResult<T> Failure(List<string> errors, string msg = "Error Occurred")
    {
        return new McpToolResult<T>
        {
            IsSuccess = false,
            Data = default,
            Message = msg,
            Errors = errors
        };
    }

    public static McpToolResult<T> Failure(string error, string msg = "Error Occurred")
    {
        return new McpToolResult<T>
        {
            IsSuccess = false,
            Data = default,
            Message = msg,
            Errors = new List<string> { error }
        };
    }
}
