using System.ComponentModel.DataAnnotations;

namespace UserManagementMcpServer.Options;

public sealed class AppOptions
{
    [Required]
    public string Url { get; set; } = null!;

    [Required]
    public string ApiKey { get; set; } = null!;
}