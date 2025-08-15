using System.ComponentModel.DataAnnotations;

namespace UserManagementMcpClient.Options;

public abstract class LlamaOptions : CommonOptions
{
    [Required]
    public string Url { get; set; } = null!;

    [Required]
    public string Model { get; set; } = null!;
}