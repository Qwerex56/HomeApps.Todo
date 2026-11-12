using System.Collections.Concurrent;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using HomeApps.Contracts.Households;

namespace Todo.Api.Tests;

/// <summary>
/// Stands in for Households' <c>GET /households/{id}/members/me</c>, and for the MCP tools' <c>GET /households</c> and
/// <c>GET /households/{id}/members/names</c>; reads the user from the test token.
/// </summary>
public sealed class FakeHouseholds : HttpMessageHandler
{
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web) { Converters = { new JsonStringEnumConverter() } };
    private readonly ConcurrentDictionary<(Guid, Guid), HouseholdRole> _roles = new();

    public ConcurrentDictionary<Guid, string> HouseholdNames { get; } = new();

    public ConcurrentDictionary<Guid, string> PeopleNames { get; } = new();

    public void SetRole(Guid householdId, Guid userId, HouseholdRole? role)
    {
        if (role is { } value)
        {
            _roles[(householdId, userId)] = value;
        }
        else
        {
            _roles.TryRemove((householdId, userId), out _);
        }
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var segments = request.RequestUri!.AbsolutePath.Trim('/').Split('/');
        var userId = SubjectOf(request.Headers.Authorization?.ToString());
        if (segments.Length == 1)
        {
            var mine = _roles.Keys.Where(k => k.Item2 == userId).Select(k => new { id = k.Item1, name = HouseholdNames.GetValueOrDefault(k.Item1, "?") });
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = JsonContent.Create(mine, options: Json) });
        }

        var householdId = Guid.Parse(segments[1]);
        if (segments[^1] == "names")
        {
            var people = _roles.Keys.Where(k => k.Item1 == householdId).Select(k => new { userId = k.Item2, name = PeopleNames.GetValueOrDefault(k.Item2, "?") });
            return Task.FromResult(_roles.ContainsKey((householdId, userId))
                ? new HttpResponseMessage(HttpStatusCode.OK) { Content = JsonContent.Create(people, options: Json) }
                : new HttpResponseMessage(HttpStatusCode.NotFound));
        }

        return Task.FromResult(_roles.TryGetValue((householdId, userId), out var role)
            ? new HttpResponseMessage(HttpStatusCode.OK) { Content = JsonContent.Create(new { householdId, role }, options: Json) }
            : new HttpResponseMessage(HttpStatusCode.NotFound));
    }

    /// <summary>The <c>sub</c> of a test token, read without validating it (Households would validate it).</summary>
    private static Guid SubjectOf(string? authorization)
    {
        var payload = authorization!["Bearer ".Length..].Split('.')[1];
        var padded = payload.Replace('-', '+').Replace('_', '/').PadRight(payload.Length + ((4 - (payload.Length % 4)) % 4), '=');
        using var json = JsonDocument.Parse(Convert.FromBase64String(padded));
        return Guid.Parse(json.RootElement.GetProperty("sub").GetString()!);
    }
}
