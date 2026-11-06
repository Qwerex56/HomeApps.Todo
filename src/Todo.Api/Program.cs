using HomeApps.BuildingBlocks.Infrastructure.Persistence;
using HomeApps.Plugins.Hosting;
using Todo.Application;
using Todo.Infrastructure;
using Todo.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.AddHomeAppsPlugin(TodoPlugin.Manifest, typeof(Program).Assembly);
builder.Services.AddTodoApplication();
builder.Services.AddTodoInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseHomeAppsPlugin();

await app.MigrateDatabaseOnStartupAsync<TodoDbContext>();

await app.RunAsync();

public partial class Program;
