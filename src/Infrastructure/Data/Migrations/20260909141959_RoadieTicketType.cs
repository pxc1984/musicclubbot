using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CuMusicClub.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class RoadieTicketType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "uq_roadie_ticket_open_song",
                table: "roadie_ticket");

            migrationBuilder.AddColumn<int>(
                name: "ticket_type",
                table: "roadie_ticket",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "idx_roadie_ticket_song_id",
                table: "roadie_ticket",
                column: "song_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "idx_roadie_ticket_song_id",
                table: "roadie_ticket");

            migrationBuilder.DropColumn(
                name: "ticket_type",
                table: "roadie_ticket");

            migrationBuilder.CreateIndex(
                name: "uq_roadie_ticket_open_song",
                table: "roadie_ticket",
                column: "song_id",
                unique: true,
                filter: "\"accepted_by\" IS NULL");
        }
    }
}
