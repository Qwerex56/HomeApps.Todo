using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using HomeApps.Contracts.Households;
using Todo.Application.Abstractions;
using Todo.Application.Common;

namespace Todo.Api.Tests;

/// <summary>One household with an Owner, an Admin, two Members and a Guest, and calls to the plugin as any of them.</summary>
internal sealed class AHousehold(TodoFactory factory)
{
    public static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web) { Converters = { new JsonStringEnumConverter() } };

    public Guid Id { get; } = Guid.NewGuid();

    public Guid Owner { get; } = Guid.NewGuid();

    public Guid Admin { get; } = Guid.NewGuid();

    public Guid Member { get; } = Guid.NewGuid();

    public Guid Other { get; } = Guid.NewGuid();

    public Guid Guest { get; } = Guid.NewGuid();

    public string Api => $"/plugins/todo/api/households/{Id}";

    public static async Task<AHousehold> SwitchedOnAsync(TodoFactory factory)
    {
        var household = new AHousehold(factory);
        factory.Households.SetRole(household.Id, household.Owner, HouseholdRole.Owner);
        factory.Households.SetRole(household.Id, household.Admin, HouseholdRole.Admin);
        factory.Households.SetRole(household.Id, household.Member, HouseholdRole.Member);
        factory.Households.SetRole(household.Id, household.Other, HouseholdRole.Member);
        factory.Households.SetRole(household.Id, household.Guest, HouseholdRole.Guest);
        (await household.As(household.Owner).PutAsJsonAsync($"{household.Api}/enablement", new { enabled = true })).EnsureSuccessStatusCode();
        return household;
    }

    public HttpClient As(Guid userId) => factory.CreateAuthenticatedClient(userId);

    public async Task<List<ListView>> ListsAsync(Guid userId) =>
        (await As(userId).GetFromJsonAsync<List<ListView>>($"{Api}/lists", Json))!;

    public async Task<ListView> HomeListAsync() => (await ListsAsync(Owner)).Single(l => l.Name == "Dom");

    public async Task<List<TaskView>> TasksAsync(Guid userId, string query = "") =>
        (await As(userId).GetFromJsonAsync<List<TaskView>>($"{Api}/tasks{query}", Json))!;

    public async Task<Guid> CreateListAsync(Guid userId, string name, string visibility)
    {
        var response = await As(userId).PostAsJsonAsync($"{Api}/lists", new { name, visibility });
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<JsonElement>()).GetProperty("id").GetGuid();
    }

    public async Task<Guid> AddTaskAsync(Guid userId, Guid listId, string title, object? more = null)
    {
        var response = await PostTaskAsync(userId, listId, title, more);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<JsonElement>()).GetProperty("id").GetGuid();
    }

    public Task<HttpResponseMessage> PostTaskAsync(Guid userId, Guid listId, string title, object? more = null)
    {
        var body = JsonSerializer.SerializeToNode(more ?? new { }, Json)!.AsObject();
        body["title"] = title;
        body["priority"] ??= "Normal";
        return As(userId).PostAsJsonAsync($"{Api}/lists/{listId}/tasks", body);
    }

    public static async Task<string?> CodeOf(HttpResponseMessage response)
    {
        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        return json.TryGetProperty("code", out var code) ? code.GetString() : json.ToString();
    }
}
