using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserManagementApp.Models;

namespace UserManagementApp.Data.EntityTypeConfigurators;

public class UserEntityTypeConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.Property(p => p.UserEmail).IsRequired();
        builder.Property(p => p.UserName).IsRequired();
    }
}