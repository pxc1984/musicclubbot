using CuMusicClub.Domain.Entities;
using Microsoft.AspNetCore.Hosting.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CuMusicClub.Infrastructure.Data.Configurations;

public class RoleTitleConfiguration : IEntityTypeConfiguration<RoleTitle>
{
    public void Configure(EntityTypeBuilder<RoleTitle> builder)
    {
        builder.ToTable("role_title");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id).HasColumnName("id").IsRequired();
        builder.Property(t => t.Title).HasColumnName("title").IsRequired();
        builder.Property(t => t.Svg).HasColumnName("svg").HasComment("Именно вот HTML записанный svg иконки, изображающей роль");

        builder.HasIndex(t => t.Title).IsUnique().HasDatabaseName("idx_role_title_unique_title");

        builder.HasMany(t => t.Users)
            .WithMany(u => u.PreferredRoles)
            .UsingEntity(
                "UserRolePreference",
                t => t.HasOne(typeof(ApplicationUser)).WithMany().HasForeignKey("ApplicationUserId"),
                t => t.HasOne(typeof(RoleTitle)).WithMany().HasForeignKey("PreferredRoleId"),
                j =>
                {
                    j.ToTable("users_preferred_roles");
                    j.HasKey("ApplicationUserId", "PreferredRoleId");
                });
    }
}
