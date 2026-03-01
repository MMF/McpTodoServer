namespace McpTodoServer.Middleware;

public class McpAuthenticationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _config;

    public McpAuthenticationMiddleware(RequestDelegate next, IConfiguration config)
    {
        _next = next;
        _config = config;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Headers.TryGetValue("Authorization", out var authHeader))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("MIssing Authorization Header");
            return;
        }

        var headerValue = authHeader.ToString();
        if (!headerValue.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Invalid Authorization Scheme");
            return;
        }

        var token = headerValue.Substring("Bearer ".Length).Trim();
        var expectedToken = _config["Mcp:AuthToken"];
        if (token != expectedToken)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Invalid Token");
            return;
        }

        await _next(context);
    }
}
