using ChatApp.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChatApp.DataAccess.Configurations;

public class ChatConfiguration: IEntityTypeConfiguration<Chat>
{
    public void Configure(EntityTypeBuilder<Chat> builder)
    {
        builder.HasKey(c => c.Id);

        builder
            .Property(c => c.Name)
            .IsRequired();
        
        builder
            .HasMany(c => c.Users)
            .WithMany(c => c.Chats);
        
        builder
            .HasMany(c => c.Messages)
            .WithOne()
            .HasForeignKey(x => x.ChatId);
    }  
}