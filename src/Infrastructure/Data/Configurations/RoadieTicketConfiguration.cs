using CuMusicClub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CuMusicClub.Infrastructure.Data.Configurations;

/// <summary>EF-конфигурация заявки группы на помощь роуди (таблица <c>roadie_ticket</c>).</summary>
public class RoadieTicketConfiguration : IEntityTypeConfiguration<RoadieTicket>
{
    public void Configure(EntityTypeBuilder<RoadieTicket> builder)
    {
        builder.ToTable("roadie_ticket");

        builder.HasKey(t => t.Id);
        builder
            .Property(t => t.Id)
            .HasColumnName("id")
            .HasDefaultValueSql("gen_random_uuid()");

        builder
            .Property(t => t.SongId)
            .HasColumnName("song_id");

        builder
            .Property(t => t.CreatedById)
            .HasColumnName("created_by");

        builder
            .Property(t => t.AcceptedById)
            .HasColumnName("accepted_by");

        builder
            .Property(t => t.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("NOW()");
        builder
            .Property(t => t.AcceptedAt)
            .HasColumnName("accepted_at");

        builder
            .HasOne(t => t.Song)
            .WithMany()
            .HasForeignKey(t => t.SongId);

        builder
            .HasOne(t => t.CreatedBy)
            .WithMany()
            .HasForeignKey(t => t.CreatedById);

        builder
            .HasOne(t => t.AcceptedBy)
            .WithMany()
            .HasForeignKey(t => t.AcceptedById);

        builder
            .HasIndex(t => t.SongId)
            .HasDatabaseName("idx_roadie_ticket_song_id");
        // Гарантия «одна открытая заявка на песню» на уровне БД (защита от гонки).
        builder
            .HasIndex(t => t.SongId)
            .IsUnique()
            .HasFilter("\"accepted_by\" IS NULL")
            .HasDatabaseName("uq_roadie_ticket_open_song");
        builder
            .HasIndex(t => t.AcceptedById)
            .HasDatabaseName("idx_roadie_ticket_accepted_by");
    }
}