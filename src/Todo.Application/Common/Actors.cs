using HomeApps.BuildingBlocks.Application.Abstractions;
using HomeApps.Plugins;
using HomeApps.Plugins.Manifest;
using Todo.Domain.Common;

namespace Todo.Application.Common;

/// <summary>
/// The caller as the domain sees them: the id from the token and whether their household role reaches
/// <see cref="TodoPlugin.Manage"/>. The role comes from <see cref="IHouseholdAccess"/>, which the permission check
/// has just asked, so this is a cache hit.
/// </summary>
public sealed class Actors(IHouseholdAccess access, ICurrentUser user, PluginManifest manifest)
{
    public async Task<Actor> GetAsync(Guid householdId, CancellationToken cancellationToken)
    {
        var role = await access.GetRoleAsync(householdId, cancellationToken);
        var manage = manifest.FindPermission(TodoPlugin.Manage)
            ?? throw new InvalidOperationException($"The manifest does not declare '{TodoPlugin.Manage}'.");
        return new Actor(user.RequiredUserId, role >= manage.MinRole);
    }
}
