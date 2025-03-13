using EntityDal.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EntityDal.EntityConfiguration;

    public class UserConfiguration : IEntityTypeConfiguration<TelegramUser>
    {
        public void Configure(EntityTypeBuilder<TelegramUser> builder)
        {
            builder.ToTable("Users");

            builder.HasKey(u => u.BotUserId);

            builder.HasIndex(u => u.BotUserId).IsUnique();

            builder.HasIndex(u => u.ChatId).IsUnique();
        }
    }
