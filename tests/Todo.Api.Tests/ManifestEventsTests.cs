using HomeApps.Contracts.Accounts;
using HomeApps.Contracts.Households;
using Todo.Application;

namespace Todo.Api.Tests;

public sealed class ManifestEventsTests
{
    [Fact]
    public void Manifest_lists_every_event_the_plugin_consumes()
    {
        var consumed = TodoPlugin.Manifest.Events!.Consumes;

        foreach (var type in new[]
        {
            typeof(MemberLeft), typeof(MemberJoined), typeof(MemberRoleChanged), typeof(HouseholdArchived), typeof(HouseholdRestored),
            typeof(UserDisabled), typeof(UserEnabled), typeof(UserErased),
        })
        {
            Assert.Contains(type, consumed);
        }
    }
}
