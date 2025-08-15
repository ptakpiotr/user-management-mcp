using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;
using ModelContextProtocol.Client;
using UserManagementMcpClient.Models;
using UserManagementMcpClient.Options;

namespace UserManagementMcpClient.Services;

public class ModelRouter(
    IOptions<McpServerOptions> serverOptions,
    [FromKeyedServices(Constants.RouterChatClient)]
    IChatClient routerChatClient,
    [FromKeyedServices(Constants.GeneralChatClient)]
    IChatClient generalChatClient,
    [FromKeyedServices(Constants.ToolChatClient)]
    IChatClient toolChatClient)
{
    private const string RouterChatPrePrompt =
        "Decide based on the following prompt if it is about role management, if so choose Tool client type";

    public async Task<(ChatClientType, IChatClient, ChatOptions)> GetChatClientAsync(string prompt,
        CancellationToken token)
    {
        IList<ChatMessage> chatMessages =
        [
            new()
            {
                Role = ChatRole.System,
                Contents = [new TextContent(RouterChatPrePrompt)]
            },
            new()
            {
                Role = ChatRole.User,
                Contents = [new TextContent(prompt)]
            }
        ];

        var resp = await routerChatClient.GetResponseAsync<RouterChatResponse>(chatMessages, new(), true, token);

        switch (resp.Result.Type)
        {
            case ChatClientType.General:
                return (ChatClientType.General, generalChatClient, new());
            case ChatClientType.Tool:
            {
                var clientTransport = new SseClientTransport(
                    new()
                    {
                        Endpoint = new Uri(serverOptions.Value.Endpoint),
                        AdditionalHeaders = new Dictionary<string, string>()
                        {
                            { serverOptions.Value.AuthHeader, serverOptions.Value.Key }
                        }
                    }
                );

                var mcpClient =
                    await McpClientFactory.CreateAsync(clientTransport, cancellationToken: token);

                var tools = await mcpClient.ListToolsAsync(cancellationToken: token);

                return (ChatClientType.Tool, toolChatClient, new()
                {
                    Tools = [..tools],
                    AllowMultipleToolCalls = true
                });
            }
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}