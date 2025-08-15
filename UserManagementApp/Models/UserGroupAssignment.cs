namespace UserManagementApp.Models;

public class UserGroupAssignment
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public Guid GroupId { get; set; }
}