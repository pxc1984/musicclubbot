using CuMusicClub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CuMusicClub.Infrastructure.Data.Configurations;

/// <summary>EF-конфигурация закрепления роуди за песней (таблица <c>song_roadie</c>).</summary>
public class SongRoadieConfiguration : IEntityTypeConfiguration<SongRoadie>
{
    public void Configure(EntityTypeBuilder<SongRoadie> builder)
    {
        builder.ToTable("song_roadie");

        builder.HasKey(r => r.SongId);
        builder
            .Property(r => r.SongId)
            .HasColumnName("song_id");

        builder
            .Property(r => r.RoadieId)
            .HasColumnName("roadie_id");

        builder
            .Property(r => r.AssignedAt)
            .HasColumnName("assigned_at")
            .IsRequired();

        builder
            .HasOne(r => r.Song)
            .WithMany()
            .HasForeignKey(r => r.SongId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(r => r.Roadie)
            .WithMany()
            .HasForeignKey(r => r.RoadieId);
    }
}