using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Todo.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPlannerModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LastRecurrenceCompletedAt",
                table: "Tasks",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RecurrenceAnchor",
                table: "Tasks",
                type: "character varying(16)",
                maxLength: 16,
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ReminderAt",
                table: "Tasks",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Rrule",
                table: "Tasks",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SeriesId",
                table: "Tasks",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CalendarSubscriptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    HouseholdId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    TokenHash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Label = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    IncludeHouseholdEvents = table.Column<bool>(type: "boolean", nullable: false),
                    IncludePrivateEvents = table.Column<bool>(type: "boolean", nullable: false),
                    TasksMode = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    LastAccessedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsSuspended = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CalendarSubscriptions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EventSeries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    HouseholdId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    Location = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    TimeZoneId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    LocalStartTime = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    LocalEndTime = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    IsAllDay = table.Column<bool>(type: "boolean", nullable: false),
                    IsPrivate = table.Column<bool>(type: "boolean", nullable: false),
                    Rrule = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    SeriesStartUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    SeriesEndUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    OwnerLeftAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventSeries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ExternalCalendarImports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    HouseholdId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Url = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Color = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    Visibility = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    CachedIcsData = table.Column<string>(type: "text", nullable: true),
                    LastSyncedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LastSyncFailedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LastErrorMessage = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    SyncFailureCount = table.Column<int>(type: "integer", nullable: false),
                    OwnerLeftAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExternalCalendarImports", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HouseholdMemberProjections",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    HouseholdId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Role = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    IsDisabled = table.Column<bool>(type: "boolean", nullable: false),
                    JoinedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LeftAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HouseholdMemberProjections", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PlannerSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    HouseholdId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    DefaultEventReminders = table.Column<string>(type: "jsonb", nullable: false),
                    DefaultTaskReminders = table.Column<string>(type: "jsonb", nullable: false),
                    FirstDayOfWeek = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlannerSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CalendarEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    HouseholdId = table.Column<Guid>(type: "uuid", nullable: false),
                    SeriesId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    Location = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    TimeZoneId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    StartUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    EndUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LocalStartTime = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    LocalEndTime = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    IsAllDay = table.Column<bool>(type: "boolean", nullable: false),
                    IsPrivate = table.Column<bool>(type: "boolean", nullable: false),
                    Attendees = table.Column<string>(type: "jsonb", nullable: false),
                    Reminders = table.Column<string>(type: "jsonb", nullable: false),
                    OwnerLeftAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CalendarEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CalendarEvents_EventSeries_SeriesId",
                        column: x => x.SeriesId,
                        principalTable: "EventSeries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EventOverrides",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SeriesId = table.Column<Guid>(type: "uuid", nullable: false),
                    OriginalInstanceDate = table.Column<DateOnly>(type: "date", nullable: false),
                    IsCancelled = table.Column<bool>(type: "boolean", nullable: false),
                    OverrideTitle = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    OverrideStartUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    OverrideEndUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    OverrideReminders = table.Column<string>(type: "jsonb", nullable: true),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventOverrides", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventOverrides_EventSeries_SeriesId",
                        column: x => x.SeriesId,
                        principalTable: "EventSeries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EventReminderOverrides",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EventId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Reminders = table.Column<string>(type: "jsonb", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventReminderOverrides", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventReminderOverrides_CalendarEvents_EventId",
                        column: x => x.EventId,
                        principalTable: "CalendarEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_SeriesId",
                table: "Tasks",
                column: "SeriesId",
                filter: "\"SeriesId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_calendar_events_created_by_private",
                table: "CalendarEvents",
                columns: new[] { "CreatedBy", "IsPrivate" });

            migrationBuilder.CreateIndex(
                name: "ix_calendar_events_household_start_end",
                table: "CalendarEvents",
                columns: new[] { "HouseholdId", "StartUtc", "EndUtc" });

            migrationBuilder.CreateIndex(
                name: "ix_calendar_events_owner_left",
                table: "CalendarEvents",
                columns: new[] { "HouseholdId", "OwnerLeftAt" });

            migrationBuilder.CreateIndex(
                name: "IX_CalendarEvents_SeriesId",
                table: "CalendarEvents",
                column: "SeriesId");

            migrationBuilder.CreateIndex(
                name: "IX_CalendarSubscriptions_HouseholdId_UserId",
                table: "CalendarSubscriptions",
                columns: new[] { "HouseholdId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "ux_calendar_subscriptions_token_hash",
                table: "CalendarSubscriptions",
                column: "TokenHash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ux_event_overrides_series_date",
                table: "EventOverrides",
                columns: new[] { "SeriesId", "OriginalInstanceDate" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ux_event_reminder_overrides_event_user",
                table: "EventReminderOverrides",
                columns: new[] { "EventId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EventSeries_HouseholdId",
                table: "EventSeries",
                column: "HouseholdId");

            migrationBuilder.CreateIndex(
                name: "IX_EventSeries_HouseholdId_OwnerLeftAt",
                table: "EventSeries",
                columns: new[] { "HouseholdId", "OwnerLeftAt" });

            migrationBuilder.CreateIndex(
                name: "ix_external_calendars_owner_left",
                table: "ExternalCalendarImports",
                columns: new[] { "HouseholdId", "OwnerLeftAt" });

            migrationBuilder.CreateIndex(
                name: "IX_ExternalCalendarImports_HouseholdId_UserId",
                table: "ExternalCalendarImports",
                columns: new[] { "HouseholdId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "ux_household_member_proj_household_user",
                table: "HouseholdMemberProjections",
                columns: new[] { "HouseholdId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ux_planner_settings_household_user",
                table: "PlannerSettings",
                columns: new[] { "HouseholdId", "UserId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CalendarSubscriptions");

            migrationBuilder.DropTable(
                name: "EventOverrides");

            migrationBuilder.DropTable(
                name: "EventReminderOverrides");

            migrationBuilder.DropTable(
                name: "ExternalCalendarImports");

            migrationBuilder.DropTable(
                name: "HouseholdMemberProjections");

            migrationBuilder.DropTable(
                name: "PlannerSettings");

            migrationBuilder.DropTable(
                name: "CalendarEvents");

            migrationBuilder.DropTable(
                name: "EventSeries");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_SeriesId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "LastRecurrenceCompletedAt",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "RecurrenceAnchor",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "ReminderAt",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "Rrule",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "SeriesId",
                table: "Tasks");
        }
    }
}
