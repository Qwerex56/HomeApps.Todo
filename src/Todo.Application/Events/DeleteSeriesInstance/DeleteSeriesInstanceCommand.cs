using HomeApps.BuildingBlocks.Application.Messaging;
using HomeApps.Plugins;

namespace Todo.Application.Events.DeleteSeriesInstance;

/// <summary>Deletes the occurrence on <paramref name="Date"/> (<c>ThisInstance</c>), it and the later ones, or the whole series.</summary>
public sealed record DeleteSeriesInstanceCommand(Guid HouseholdId, Guid SeriesId, DateOnly Date, string? Scope) : ICommand, IHouseholdRequest
{
    public string Permission => TodoPlugin.Write;
}
