# Project map: HomeApps.Todo

Last updated: 2026-11-22 · HEAD: b99be2d · branch feat/household-planner-p3 (PRs go to integration branch named by owner; master is protected)

## Overview
- Plugin "Zadania" (Todo) for HomeApps, plus in-progress "Planer" (calendar + recurring tasks). Lives outside HomeApps repo.
- Stack: .NET 10 (Minimal API, MediatR 12, FluentValidation, FluentResults, EF Core, Postgres, MassTransit/RabbitMQ), kit `HomeApps.*` packages from `~/.homeapps/packages`; UI Vue 3.5 + TS + Vite + TanStack Query + Axios + `@homeapps/plugin-sdk`.
- Built UI is committed: `src/Todo.Api/wwwroot/ui` (entry.js, entry.css, chunks).
- Planner: P1 (domain + storage) and P2 (calendar events API) in code; P3-P6 todo (status: `.agents/STATUS.md`). Source of truth: `specs/`.

## Layout
| Path | Responsibility |
|---|---|
| `src/Todo.Domain` | `TodoList` (`Lists/TodoList.cs:11`), `TodoTask` (`Tasks/TodoTask.cs:14`, recurrence: `Rule` :69, `RecurrenceAnchor` :72, `EditRecurring` :153), `Tasks/RecurrenceScope.cs`, `Tasks/RecurrenceAnchor.cs`, value objects, `Common/Actor.cs`, `Common/TodoErrors.cs`, `Common/PlannerErrors.cs` |
| `src/Todo.Domain/Events` | planner aggregates: `CalendarEvent.cs:11`, `EventSeries.cs:11` (RRULE), `EventOverride.cs:7`, `EventReminderOverride.cs:6`, `CalendarSubscription.cs:7`, `ExternalCalendarImport.cs:7`, `RecurrenceRule.cs:7` (record), `EventTime.cs`, `EventTitle.cs`, `EventReminder.cs`, enums `SubscriptionTasksMode`, `ExternalCalendarVisibility` |
| `src/Todo.Domain/Settings` | `PlannerSettings.cs:7` |
| `src/Todo.Domain/Households` | `HouseholdMemberProjection.cs:4` (member projection from MemberJoined/Left) |
| `src/Todo.Contracts` | integration message `TaskCompleted` (`TaskCompleted.cs`) |
| `src/Todo.Application` | `TodoPlugin.cs` (manifest, line 12/23), commands/queries per folder, `Abstractions/` ports, `Events/` (calendar commands/queries: Create/Update/DeleteCalendarEvent, GetCalendarEvent, GetCalendarItems, ModifySeriesInstance, DeleteSeriesInstance, `ScopeParser.cs`; plus integration handlers), `Households/` |
| `src/Todo.Infrastructure` | `TodoDbContext` (`Persistence/TodoDbContext.cs:10`; DbSets :15-33 incl. planner), `Persistence/Configurations/` (EF maps per aggregate, `Converters.cs`), repos (`CalendarEventRepository.cs`), `Persistence/CalendarReader.cs` (calendar read model), `Persistence/Migrations/` (Initial, `20261122012643_AddPlannerModule`), `Households/HouseholdsConsumer.cs:15`, `Jobs/LeftListsCleanup.cs:13` (6 h purge), `DependencyInjection.cs:15` |
| `src/Todo.Api` | host: `Program.cs` (entry, MigrateDatabaseOnStartup), `Endpoints/`, `Mcp/TaskTools.cs:30`, `wwwroot/ui` |
| `src/Todo.Web` | Vue UI source: `src/TasksPage.vue`, `TodayWidget.vue`, `TaskEditor.vue`, `ListDialog.vue`, `TaskRow.vue`, `api.ts`, `useTodo.ts`, `dates.ts`, `main.ts` (definePlugin, pl/en texts) |
| `tests/Todo.Domain.Tests` | domain tests (no Docker): `TaskRecurrenceTests`, `CalendarEventTests`, `CalendarEventGuestTests`, `EventSeriesTests`, `EventSeriesOccurrenceTests`, `EventSeriesEdgeTests`, `RecurrenceEvaluatorTests`, `RecurrenceRfcTests`, `EventOverrideTests`, `CalendarSubscriptionTests`, `ExternalCalendarImportTests`, `HouseholdMemberProjectionTests`, `PlannerSettingsTests` |
| `tests/Todo.Api.Tests` | API tests on Testcontainers Postgres (`TodoFactory.cs:12`, `TodoApi.cs`, `FakeHouseholds.cs`, `McpTests.cs:14`, `TodoTests.cs`); no `Todo.Application.Tests` project exists: handler tests go through API tests |
| `specs/` | READ-ONLY for agents: `household-planner.md` (spec), `household-planner-adr.md` (binding ADR) |
| `docs/plan.md` | Todo v1 plan and progress log (section 8) |
| `.claude/skills/` | project skills: `dotnet-conventions`, `vue-conventions`, `testing-standards` (API tests listed NOT RUN if Docker is down), `code-review-checklist` (developer pre-submit section) |
| `.agents/runs/` | run artifacts (gitignored, never commit) |
| `.agents/STATUS.md` | planner feature stage table (P1-P6), decisions, blockers |

## Entry points and routes
- Program: `src/Todo.Api/Program.cs:9` (`AddHomeAppsPlugin`), `:15` (`UseHomeAppsPlugin`), `:17` (migrate). Port 5400 in dev (`Properties/launchSettings.json`).
- Endpoints (all under `/plugins/todo/api`, households `/households/{householdId:guid}`):
  - `Endpoints/ListEndpoints.cs:19` lists: GET/POST `/lists`, PUT `/lists/{id}`, POST `archive|restore`, DELETE.
  - `Endpoints/CalendarEndpoints.cs` calendar events/items and series instances (P2; contract in ADR).
  - `Endpoints/TaskEndpoints.cs:22-49` tasks: GET `/tasks`, POST `/lists/{id}/tasks`, PUT/DELETE `/tasks/{id}`, POST `complete|reopen|move`.
- Also: `/plugins/todo/manifest`, `/openapi/v1.json`, `/health/{live,ready}`, MCP at `/plugins/todo/mcp` (tools `add_task`, `my_tasks`, `complete_task` in `Mcp/TaskTools.cs`).
- Events: publishes `TaskCompleted` (shared lists only; `Application/Events/PublishTaskCompleted.cs:9`); consumes `MemberLeft`/`MemberJoined` (`HouseholdsConsumer.cs:15`); `SeedDefaultList.cs:9` on `PluginSwitchedOn`.
- Web UI entry: `src/Todo.Web/src/main.ts`; vite config `src/Todo.Web/vite.config.ts` (outDir `../Todo.Api/wwwroot/ui`).

## Build, lint, test, run
| What | Command |
|---|---|
| Build (warnings are errors) | `dotnet build HomeApps.Todo.slnx` |
| Domain tests | `dotnet test tests/Todo.Domain.Tests` |
| API tests (needs Docker) | `dotnet test tests/Todo.Api.Tests` |
| UI type-check | `cd src/Todo.Web && npm run type-check` |
| UI build (commit output) | `cd src/Todo.Web && npm run build` (vue-tsc + vite) |
| Kit packages | in `../HomeApps`: `scripts/pack.sh` (required before restore/build/UI install) |
| Migration | `dotnet ef migrations add <Name> -p src/Todo.Infrastructure -o Persistence/Migrations` |
| Run with IDE | HomeApps dev stack up; run `src/Todo.Api` profile `http`; route in HomeApps `dynamic/plugin-todo.yaml` |
| Run with Docker | `ln -s ../HomeApps/.env .env` once; `docker compose -f compose.yaml up -d --build` |

## Config, Docker, env
- `Directory.Build.props`, `Directory.Packages.props` (central package versions; never bump MediatR, MassTransit or kit), `global.json` (SDK 10, latestMajor), `nuget.config` (feed `%HOME%/.homeapps/packages`), `.editorconfig`.
- `Dockerfile`: multi-stage; kit via extra build context `homeapps`; UI not built in image.
- `compose.yaml`: service `todo`, image `homeapps/todo:1.0.0`, external network `homeapps_default`, Traefik labels for `/plugins/todo`. Env from HomeApps `.env`: `POSTGRES_PASSWORD`, `RABBITMQ_PASSWORD`, `HOMEAPPS_DOMAIN`, etc.
- `src/Todo.Api/appsettings*.json`: Jwt, RabbitMq, ConnectionStrings, Plugin__HouseholdsUrl.
- `.gitignore`: bin, obj, .idea, node_modules, dist, .env, `.claude/runs/`, `.agents/runs/`.

## Rules pointers
- Root `CLAUDE.md`: one type per file; one folder per command/query; no new abstractions/packages without need; ADR contract binding; never touch master; commits English, small.
- Owner communicates in Polish; code/comments/commits/docs in English; UI texts in Polish.
