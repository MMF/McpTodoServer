using McpTodoServer.Data;
using McpTodoServer.Middleware;
using McpTodoServer.Tools;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// DB
builder.Services.AddDbContext<TodoDbContext>(opt => opt.UseInMemoryDatabase("TodoDb"));

// Add the MCP services: the transport to use (http) and the tools to register.
builder.Services
    .AddMcpServer()
    .WithHttpTransport()
    .WithTools<TodoTools>();

var app = builder.Build();

app.MapGet("/", () => "OK");
app.MapGet("/health", () => "Healthy");

app.UseWhen(
    ctx => ctx.Request.Path.StartsWithSegments("/api/mcp"), 
    appBuilder =>
    {
        appBuilder.UseMiddleware<McpAuthenticationMiddleware>();
    });

app.MapMcp("/api/mcp");
//app.UseHttpsRedirection();

app.Run();
