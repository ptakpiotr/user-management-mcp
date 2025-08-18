using Microsoft.Extensions.Configuration;

var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.UserManagementApp>("app")
    .WithEnvironment((ctx) =>
    {
        var keys = builder.Configuration.GetSection("AllowedApiKeys").Get<string[]>() ?? [];
        foreach (var (item , idx) in keys.Select((item, idx)=>(item ,idx)))
        {
            ctx.EnvironmentVariables[$"AllowedApiKeys__{idx}"] = item;
        }
    });

builder.AddProject<Projects.UserManagementMcpServer>("server")
    .WithEnvironment("AppConn:ApiKey", builder.Configuration["ApiKey"])
    .WithEnvironment("AppConn:Url", builder.Configuration["AppConnUrl"]);

builder.AddProject<Projects.UserManagementMcpClient>("client")
    .WithEnvironment("McpServer:ApiKey", builder.Configuration["ApiKey"])
    .WithEnvironment("McpServer:AuthHeader", "X-API-KEY")
    .WithEnvironment("McpServer:Endpoint", builder.Configuration["McpServerUrl"])
    .WithEnvironment("ChatConnections:Router:Name", builder.Configuration["ChatConnections:Router:Name"])
    .WithEnvironment("ChatConnections:Router:Url", builder.Configuration["ChatConnections:Router:Url"])
    .WithEnvironment("ChatConnections:Router:Model", builder.Configuration["ChatConnections:Router:Model"])
    .WithEnvironment("ChatConnections:General:Name", builder.Configuration["ChatConnections:General:Name"])
    .WithEnvironment("ChatConnections:General:Url", builder.Configuration["ChatConnections:General:Url"])
    .WithEnvironment("ChatConnections:General:Key", builder.Configuration["ChatConnections:General:Key"])
    .WithEnvironment("ChatConnections:Tool:Name", builder.Configuration["ChatConnections:Tool:Name"])
    .WithEnvironment("ChatConnections:Tool:Url", builder.Configuration["ChatConnections:Tool:Url"])
    .WithEnvironment("ChatConnections:Tool:Key", builder.Configuration["ChatConnections:Tool:Key"]);

builder.Build().Run();