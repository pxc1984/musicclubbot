using CuMusicClub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CuMusicClub.Infrastructure.Data.Configurations;

public class UserPreferencesConfiguration : IEntityTypeConfiguration<UserPreferences>
{
    public void Configure(EntityTypeBuilder<UserPreferences> builder)
    {
        builder.ToTable("user_preferences");
        builder.HasKey(p => p.UserId);

        builder.Property(p => p.UserId).HasColumnName("user_id").IsRequired();
        builder.Property(p => p.AllowAdding).HasColumnName("allow_adding").IsRequired().HasDefaultValue(true);
        builder.Property(p => p.AllowRemoving).HasColumnName("allow_removing").IsRequired().HasDefaultValue(true);

        builder.HasOne(p => p.User)
            .WithOne(u => u.Preferences)
            .HasForeignKey<UserPreferences>(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
