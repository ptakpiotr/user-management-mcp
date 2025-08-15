namespace UserManagementApp.Models;

public class Group
{
    public Guid Id { get; set; }

    public GroupType GroupType { get; set; }

    public string GroupName { get; set; } = null!;
}