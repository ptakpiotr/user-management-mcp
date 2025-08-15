using System.ComponentModel;
using Microsoft.Extensions.Options;
using ModelContextProtocol.Server;
using UserManagementApp.Models;
using UserManagementMcpServer.Options;

namespace UserManagementMcpServer;

[McpServerToolType]
public static class AddTools
{
    [McpServerTool]
    [Description("Add user to group")]
    public static async Task<bool> AddUser(IHttpClientFactory clientFactory, IOptions<AppOptions> opts, [Description("User data")] UserToGroup userToAddToGroup)
    {
        return (await clientFactory.CreateClient("AppClient").PostAsJsonAsync($"{opts.Value.Url}/userToGroup", userToAddToGroup)).IsSuccessStatusCode;
    }
}