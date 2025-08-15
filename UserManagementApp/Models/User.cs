namespace UserManagementApp.Models;

public class User
{
    public Guid Id { get; set; }

    public string UserName { get; set; } = null!;

    public string UserEmail { get; set; } = null!;
}