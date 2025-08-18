using Azure;
using Azure.AI.OpenAI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;
using UserManagementMcpClient;
using UserManagementMcpClient.Options;
using UserManagementMcpClient.Services;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

var cfg = builder.Configuration;

builder.Services.AddOptionsWithValidateOnStart<McpServerOptions>()
    .Bind(builder.Configuration.GetSection("McpServer"));

builder.Services.AddOptionsWithValidateOnStart<RouterOptions>()
    .Bind(builder.Configuration.GetSection("ChatConnections:Router"));

builder.Services.AddOptionsWithValidateOnStart<GeneralOptions>()
    .Bind(builder.Configuration.GetSection("ChatConnections:General"));

builder.Services.AddOptionsWithValidateOnStart<ToolOptions>()
    .Bind(builder.Configuration.GetSection("ChatConnections:Tool"));

builder.Services.AddKeyedChatClient(Constants.RouterChatClient, sp =>
{
    var routerOptions = sp.GetRequiredService<IOptions<RouterOptions>>();
    var client = new OllamaChatClient(routerOptions.Value.Url, routerOptions.Value.Model);

    return client;
});

builder.Services.AddKeyedChatClient(Constants.GeneralChatClient, sp =>
{
    var generalOptions = sp.GetRequiredService<IOptions<GeneralOptions>>();

    var client = new AzureOpenAIClient(new Uri(generalOptions.Value.Url),
        new AzureKeyCredential(generalOptions.Value.Key));

    return client.GetChatClient(generalOptions.Value.Name).AsIChatClient()
        .AsBuilder()
        .UseFunctionInvocation()
        .Build();
});

builder.Services.AddKeyedChatClient(Constants.ToolChatClient, sp =>
{
    var toolOptions = sp.GetRequiredService<IOptions<ToolOptions>>();

    var client = new AzureOpenAIClient(new Uri(toolOptions.Value.Url),
        new AzureKeyCredential(toolOptions.Value.Key));

    return client.GetChatClient(toolOptions.Value.Name).AsIChatClient()
        .AsBuilder()
        .UseFunctionInvocation()
        .Build();
});

builder.Services.AddSingleton<ModelRouter>();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapGet("/prompt",
    async ([FromQuery] string prompt, [FromServices] ModelRouter router, CancellationToken token) =>
    {
        var (type, chatClient, opts) = await router.GetChatClientAsync(prompt, token);

        var resp = await chatClient.GetResponseAsync(prompt, opts, token);

        return Results.Ok(new
        {
            ClientType = type,
            Response = resp.Text
        });
    });

app.Run();