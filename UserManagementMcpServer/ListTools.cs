using System.ComponentModel;
using Microsoft.Extensions.Options;
using ModelContextProtocol.Server;
using UserManagementApp.Models;
using UserManagementMcpServer.Options;

namespace UserManagementMcpServer;

[McpServerToolType]
public static class ListTools
{
    [McpServerTool]
    [Description("Get user data based on their email")]
    public static async Task<User?> GetUsers(IHttpClientFactory clientFactory, IOptions<AppOptions> opts, [Description("User email")] string email)
    {
        return await clientFactory.CreateClient("AppClient").GetFromJsonAsync<User>($"{opts.Value.Url}/user/{email}");
    }
    
    [McpServerTool]
    [Description("Get groups data")]
    public static async Task<List<Group>?> GetGroups(IHttpClientFactory clientFactory, IOptions<AppOptions> opts)
    {
        return await clientFactory.CreateClient("AppClient").GetFromJsonAsync<List<Group>>($"{opts.Value.Url}/groups");
    }
}