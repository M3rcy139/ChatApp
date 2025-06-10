using ChatApp.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChatApp.DataAccess.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);

        builder
            .Property(u => u.UserName)
            .IsRequired();
        
        builder
            .Property(u => u.PhoneNumber)
            .IsRequired();
        
        builder
            .Property(u => u.PasswordHash)
            .IsRequired();
        
        builder
            .HasMany(u => u.Chats)
            .WithMany(u => u.Users);
    }
}