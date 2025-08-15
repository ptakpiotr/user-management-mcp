using System.ComponentModel.DataAnnotations;

namespace UserManagementMcpClient.Options;

public sealed class McpServerOptions
{
    [Required]
    public string Endpoint { get; set; } = null!;

    [Required]
    public string Key { get; set; } = null!;

    [Required]
    public string AuthHeader { get; set; } = null!;
}