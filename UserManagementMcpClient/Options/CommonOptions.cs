using System.ComponentModel.DataAnnotations;

namespace UserManagementMcpClient.Options;

public abstract class CommonOptions
{
    [Required]
    public string Name { get; set; } = null!;
}