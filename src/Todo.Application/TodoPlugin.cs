using HomeApps.Contracts.Households;
using HomeApps.Plugins.Manifest;
using HomeApps.Todo.Contracts;

namespace Todo.Application;

/// <summary>
/// The Todo plugin's manifest (docs/plugins.md in HomeApps). The UI (src/Todo.Web, built into Todo.Api/wwwroot/ui) is
/// here since T3; the display screen and MCP join it in T4, so the web app never tries to load what is not there yet.
/// </summary>
public static class TodoPlugin
{
    /// <summary>Read shared lists and one's own private lists.</summary>
    public const string Read = "tasks.read";

    /// <summary>Add, edit, tick off and move tasks; keep private lists.</summary>
    public const string Write = "tasks.write";

    /// <summary>Rename, archive and delete shared lists; delete anyone's task on them.</summary>
    public const string Manage = "tasks.manage";

    public static PluginManifest Manifest { get; } = new()
    {
        Id = "todo",
        Version = typeof(TodoPlugin).Assembly.GetName().Version is { } v ? $"{v.Major}.{v.Minor}.{v.Build}" : "1.0.0",
        MinHomeAppsVersion = "2.1.0",
        Name = new LocalizedText("Zadania", "Tasks"),
        Description = new LocalizedText("Wspólne i prywatne listy zadań domu", "Shared and private task lists of a household"),
        Icon = "pi-check-square",
        Scopes = [DataScope.HouseholdUser],
        Enablement = new PluginEnablementRule(HouseholdRole.Admin),
        Permissions =
        [
            new PluginPermission(Read, HouseholdRole.Guest, new LocalizedText("Przeglądanie wspólnych list", "View shared lists")),
            new PluginPermission(Write, HouseholdRole.Member, new LocalizedText("Dodawanie i odhaczanie zadań, prywatne listy", "Add and tick off tasks, private lists")),
            new PluginPermission(Manage, HouseholdRole.Admin, new LocalizedText("Zarządzanie wspólnymi listami", "Manage shared lists")),
        ],
        Ui = new PluginUi
        {
            Routes = [new PluginRoute("", new LocalizedText("Zadania", "Tasks"))],
            Widgets = [new PluginWidget("today", new LocalizedText("Na dziś", "Today"), WidgetSize.Medium, [WidgetPlace.Household, WidgetPlace.Wall])],
        },
        Events = new PluginEvents([typeof(TaskCompleted)], [typeof(MemberLeft), typeof(MemberJoined)]),
    };
}
