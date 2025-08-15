using System.ComponentModel.DataAnnotations;

namespace UserManagementMcpClient.Options;

public abstract class AzOpenAIOptions : CommonOptions
{
    [Required]
    public string Url { get; set; } = null!;

    [Required]
    public string Key { get; set; } = null!;
}