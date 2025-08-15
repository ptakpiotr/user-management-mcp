using System.Text.Json.Serialization;

namespace UserManagementMcpClient.Models;


[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ChatClientType
{
    General,
    Tool
}