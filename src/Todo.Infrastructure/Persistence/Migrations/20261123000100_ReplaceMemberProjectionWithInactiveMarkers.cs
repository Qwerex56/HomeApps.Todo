using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Todo.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceMemberProjectionWithInactiveMarkers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsGuest",
                table: "CalendarSubscriptions",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "InactiveMarkers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    HouseholdId = table.Column<Guid>(type: "uuid", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InactiveMarkers", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InactiveMarkers_HouseholdId_UserId",
                table: "InactiveMarkers",
                columns: new[] { "HouseholdId", "UserId" });

            // Carry what the dropped copy knew into the plugin's own data before it goes (roles and names are not kept).
            migrationBuilder.Sql("UPDATE \"CalendarSubscriptions\" s SET \"IsGuest\" = TRUE WHERE NOT EXISTS (SELECT 1 FROM \"HouseholdMemberProjections\" m WHERE m.\"HouseholdId\" = s.\"HouseholdId\" AND m.\"UserId\" = s.\"UserId\" AND m.\"Role\" <> 'Guest')");
            migrationBuilder.Sql("INSERT INTO \"InactiveMarkers\" (\"Id\", \"HouseholdId\", \"UserId\", \"CreatedAt\") SELECT gen_random_uuid(), m.\"HouseholdId\", m.\"UserId\", now() FROM \"HouseholdMemberProjections\" m WHERE m.\"LeftAt\" IS NOT NULL");
            migrationBuilder.Sql("INSERT INTO \"InactiveMarkers\" (\"Id\", \"HouseholdId\", \"UserId\", \"CreatedAt\") SELECT gen_random_uuid(), NULL, m.\"UserId\", now() FROM \"HouseholdMemberProjections\" m WHERE m.\"IsDisabled\" GROUP BY m.\"UserId\"");
            migrationBuilder.Sql("INSERT INTO \"InactiveMarkers\" (\"Id\", \"HouseholdId\", \"UserId\", \"CreatedAt\") SELECT gen_random_uuid(), m.\"HouseholdId\", NULL, now() FROM \"HouseholdMemberProjections\" m WHERE m.\"IsArchived\" GROUP BY m.\"HouseholdId\"");

            migrationBuilder.DropTable(
                name: "HouseholdMemberProjections");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InactiveMarkers");

            migrationBuilder.DropColumn(
                name: "IsGuest",
                table: "CalendarSubscriptions");

            migrationBuilder.CreateTable(
                name: "HouseholdMemberProjections",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    HouseholdId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsArchived = table.Column<bool>(type: "boolean", nullable: false),
                    IsDisabled = table.Column<bool>(type: "boolean", nullable: false),
                    JoinedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LeftAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Role = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HouseholdMemberProjections", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "ux_household_member_proj_household_user",
                table: "HouseholdMemberProjections",
                columns: new[] { "HouseholdId", "UserId" },
                unique: true);
        }
    }
}
