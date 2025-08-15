using Microsoft.Extensions.Options;
using UserManagementMcpServer.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOptionsWithValidateOnStart<AppOptions>()
    .Bind(builder.Configuration.GetSection("AppConn"));

builder.Services.AddMcpServer()
    .WithHttpTransport(opts =>
    {
        opts.IdleTimeout = Timeout.InfiniteTimeSpan;
    })
    .WithToolsFromAssembly();

builder.Services.AddHttpClient("AiClient", (services, client) =>
{
    client.DefaultRequestHeaders.Add("X-API-KEY", services.GetRequiredService<IOptions<AppOptions>>().Value.ApiKey);
});

builder.Services.AddCors(opts =>
{
    opts.AddPolicy("AllowAnyone", (pb) => { pb.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin(); });
});

var app = builder.Build();

app.UseCors("AllowAnyone");

app.MapMcp();

app.Run();