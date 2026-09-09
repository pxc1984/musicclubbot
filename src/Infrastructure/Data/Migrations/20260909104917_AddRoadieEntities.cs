using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CuMusicClub.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRoadieEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "roadie_ticket",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    song_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: false),
                    accepted_by = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    accepted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roadie_ticket", x => x.id);
                    table.ForeignKey(
                        name: "FK_roadie_ticket_app_user_accepted_by",
                        column: x => x.accepted_by,
                        principalTable: "app_user",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_roadie_ticket_app_user_created_by",
                        column: x => x.created_by,
                        principalTable: "app_user",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_roadie_ticket_song_song_id",
                        column: x => x.song_id,
                        principalTable: "song",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "song_roadie",
                columns: table => new
                {
                    song_id = table.Column<Guid>(type: "uuid", nullable: false),
                    roadie_id = table.Column<Guid>(type: "uuid", nullable: false),
                    assigned_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_song_roadie", x => x.song_id);
                    table.ForeignKey(
                        name: "FK_song_roadie_app_user_roadie_id",
                        column: x => x.roadie_id,
                        principalTable: "app_user",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_song_roadie_song_song_id",
                        column: x => x.song_id,
                        principalTable: "song",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "idx_roadie_ticket_accepted_by",
                table: "roadie_ticket",
                column: "accepted_by");

            migrationBuilder.CreateIndex(
                name: "IX_roadie_ticket_created_by",
                table: "roadie_ticket",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "uq_roadie_ticket_open_song",
                table: "roadie_ticket",
                column: "song_id",
                unique: true,
                filter: "\"accepted_by\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_song_roadie_roadie_id",
                table: "song_roadie",
                column: "roadie_id");

            // Выдать новое право roadie.manage существующим роуди и администраторам
            // (у них уже есть participation.edit_any).
            migrationBuilder.Sql(
                """
                INSERT INTO user_permissions ("UserId", "Permission")
                SELECT "UserId", 'roadie.manage'
                FROM user_permissions
                WHERE "Permission" = 'participation.edit_any'
                ON CONFLICT DO NOTHING;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "roadie_ticket");

            migrationBuilder.DropTable(
                name: "song_roadie");
        }
    }
}
