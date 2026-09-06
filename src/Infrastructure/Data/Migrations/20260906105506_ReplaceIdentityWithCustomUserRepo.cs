using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CuMusicClub.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceIdentityWithCustomUserRepo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Отвязываем внешние ключи прочих таблиц, указывавшие на AspNetUsers
            //    (ниже переназначим их на app_user).
            migrationBuilder.DropForeignKey(
                name: "FK_calendar_AspNetUsers_user_id",
                table: "calendar");

            migrationBuilder.DropForeignKey(
                name: "FK_event_AspNetUsers_created_by",
                table: "event");

            migrationBuilder.DropForeignKey(
                name: "FK_event_participant_AspNetUsers_user_id",
                table: "event_participant");

            migrationBuilder.DropForeignKey(
                name: "FK_refresh_tokens_AspNetUsers_sub",
                table: "refresh_tokens");

            migrationBuilder.DropForeignKey(
                name: "FK_song_AspNetUsers_created_by",
                table: "song");

            migrationBuilder.DropForeignKey(
                name: "FK_song_role_assignment_AspNetUsers_user_id",
                table: "song_role_assignment");

            migrationBuilder.DropForeignKey(
                name: "FK_song_topic_member_AspNetUsers_user_id",
                table: "song_topic_member");

            migrationBuilder.DropForeignKey(
                name: "FK_user_preferences_AspNetUsers_user_id",
                table: "user_preferences");

            migrationBuilder.DropForeignKey(
                name: "FK_user_session_AspNetUsers_user_id",
                table: "user_session");

            migrationBuilder.DropForeignKey(
                name: "FK_users_preferred_roles_AspNetUsers_ApplicationUserId",
                table: "users_preferred_roles");

            // 2. Чистим Identity-колонки/индексы у AspNetUsers (данные пользователей сохраняются).
            //    PK сбрасываем через CASCADE — это снимает и FK от Identity-таблиц (AspNetUserClaims,
            //    AspNetUserRoles и др.), которые нужны ниже для переноса прав.
            migrationBuilder.Sql(
                "ALTER TABLE \"AspNetUsers\" DROP CONSTRAINT \"PK_AspNetUsers\" CASCADE;");

            migrationBuilder.DropIndex(
                name: "EmailIndex",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "UserNameIndex",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "AccessFailedCount",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ConcurrencyStamp",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "EmailConfirmed",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LockoutEnabled",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LockoutEnd",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "NormalizedEmail",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "NormalizedUserName",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "PhoneNumberConfirmed",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "SecurityStamp",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "TwoFactorEnabled",
                table: "AspNetUsers");

            // 3. Переименовываем таблицу в go-совместимую app_user.
            migrationBuilder.RenameTable(
                name: "AspNetUsers",
                newName: "app_user");

            migrationBuilder.AlterColumn<string>(
                name: "UserName",
                table: "app_user",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "app_user",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_app_user",
                table: "app_user",
                column: "Id");

            // 4. Таблица прав пользователя (аналог go-based user_permissions, права строковые).
            migrationBuilder.CreateTable(
                name: "user_permissions",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Permission = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_permissions", x => new { x.UserId, x.Permission });
                    table.ForeignKey(
                        name: "FK_user_permissions_app_user_UserId",
                        column: x => x.UserId,
                        principalTable: "app_user",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // 5. Переносим права: permission-claims из AspNetUserClaims и пакеты ролей
            //    (AspNetUserRoles -> AspNetRoles) в user_permissions.
            migrationBuilder.Sql(
                """
                INSERT INTO "user_permissions" ("UserId", "Permission")
                SELECT c."UserId", c."ClaimValue"
                FROM "AspNetUserClaims" c
                WHERE c."ClaimType" = 'permission'
                  AND c."ClaimValue" IS NOT NULL
                ON CONFLICT ("UserId", "Permission") DO NOTHING;
                """);

            migrationBuilder.Sql(
                """
                INSERT INTO "user_permissions" ("UserId", "Permission")
                SELECT DISTINCT ur."UserId", m."Permission"
                FROM "AspNetUserRoles" ur
                JOIN "AspNetRoles" r ON r."Id" = ur."RoleId"
                JOIN (VALUES
                    ('Administrator', 'participation.edit_own'),
                    ('Administrator', 'participation.edit_any'),
                    ('Administrator', 'songs.edit_own'),
                    ('Administrator', 'songs.edit_any'),
                    ('Administrator', 'songs.edit_featured'),
                    ('Administrator', 'events.edit'),
                    ('Administrator', 'tracklists.edit'),
                    ('Roadie', 'participation.edit_own'),
                    ('Roadie', 'participation.edit_any'),
                    ('Roadie', 'songs.edit_own'),
                    ('Default', 'participation.edit_own'),
                    ('Default', 'songs.edit_own')
                ) AS m("RoleName", "Permission") ON m."RoleName" = r."Name"
                ON CONFLICT ("UserId", "Permission") DO NOTHING;
                """);

            // 6. Удаляем Identity-таблицы (после переноса прав).
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.AddForeignKey(
                name: "FK_calendar_app_user_user_id",
                table: "calendar",
                column: "user_id",
                principalTable: "app_user",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_event_app_user_created_by",
                table: "event",
                column: "created_by",
                principalTable: "app_user",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_event_participant_app_user_user_id",
                table: "event_participant",
                column: "user_id",
                principalTable: "app_user",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_refresh_tokens_app_user_sub",
                table: "refresh_tokens",
                column: "sub",
                principalTable: "app_user",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_song_app_user_created_by",
                table: "song",
                column: "created_by",
                principalTable: "app_user",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_song_role_assignment_app_user_user_id",
                table: "song_role_assignment",
                column: "user_id",
                principalTable: "app_user",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_song_topic_member_app_user_user_id",
                table: "song_topic_member",
                column: "user_id",
                principalTable: "app_user",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_user_preferences_app_user_user_id",
                table: "user_preferences",
                column: "user_id",
                principalTable: "app_user",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_user_session_app_user_user_id",
                table: "user_session",
                column: "user_id",
                principalTable: "app_user",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_users_preferred_roles_app_user_ApplicationUserId",
                table: "users_preferred_roles",
                column: "ApplicationUserId",
                principalTable: "app_user",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_calendar_app_user_user_id",
                table: "calendar");

            migrationBuilder.DropForeignKey(
                name: "FK_event_app_user_created_by",
                table: "event");

            migrationBuilder.DropForeignKey(
                name: "FK_event_participant_app_user_user_id",
                table: "event_participant");

            migrationBuilder.DropForeignKey(
                name: "FK_refresh_tokens_app_user_sub",
                table: "refresh_tokens");

            migrationBuilder.DropForeignKey(
                name: "FK_song_app_user_created_by",
                table: "song");

            migrationBuilder.DropForeignKey(
                name: "FK_song_role_assignment_app_user_user_id",
                table: "song_role_assignment");

            migrationBuilder.DropForeignKey(
                name: "FK_song_topic_member_app_user_user_id",
                table: "song_topic_member");

            migrationBuilder.DropForeignKey(
                name: "FK_user_preferences_app_user_user_id",
                table: "user_preferences");

            migrationBuilder.DropForeignKey(
                name: "FK_user_session_app_user_user_id",
                table: "user_session");

            migrationBuilder.DropForeignKey(
                name: "FK_users_preferred_roles_app_user_ApplicationUserId",
                table: "users_preferred_roles");

            migrationBuilder.DropTable(
                name: "user_permissions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_app_user",
                table: "app_user");

            migrationBuilder.RenameTable(
                name: "app_user",
                newName: "AspNetUsers");

            migrationBuilder.AlterColumn<string>(
                name: "UserName",
                table: "AspNetUsers",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "AspNetUsers",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AccessFailedCount",
                table: "AspNetUsers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ConcurrencyStamp",
                table: "AspNetUsers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EmailConfirmed",
                table: "AspNetUsers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "LockoutEnabled",
                table: "AspNetUsers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LockoutEnd",
                table: "AspNetUsers",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NormalizedEmail",
                table: "AspNetUsers",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NormalizedUserName",
                table: "AspNetUsers",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "AspNetUsers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PhoneNumberConfirmed",
                table: "AspNetUsers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "SecurityStamp",
                table: "AspNetUsers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "TwoFactorEnabled",
                table: "AspNetUsers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetUsers",
                table: "AspNetUsers",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    ProviderKey = table.Column<string>(type: "text", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_calendar_AspNetUsers_user_id",
                table: "calendar",
                column: "user_id",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_event_AspNetUsers_created_by",
                table: "event",
                column: "created_by",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_event_participant_AspNetUsers_user_id",
                table: "event_participant",
                column: "user_id",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_refresh_tokens_AspNetUsers_sub",
                table: "refresh_tokens",
                column: "sub",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_song_AspNetUsers_created_by",
                table: "song",
                column: "created_by",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_song_role_assignment_AspNetUsers_user_id",
                table: "song_role_assignment",
                column: "user_id",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_song_topic_member_AspNetUsers_user_id",
                table: "song_topic_member",
                column: "user_id",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_user_preferences_AspNetUsers_user_id",
                table: "user_preferences",
                column: "user_id",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_user_session_AspNetUsers_user_id",
                table: "user_session",
                column: "user_id",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_users_preferred_roles_AspNetUsers_ApplicationUserId",
                table: "users_preferred_roles",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
