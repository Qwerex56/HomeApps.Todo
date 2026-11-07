# HomeApps.Todo

Plugin „Zadania” dla [HomeApps](../HomeApps): wspólne i prywatne listy zadań domu. Żyje poza repo HomeApps
i korzysta z jego kitu (paczki `HomeApps.*` z lokalnego feedu) oraz kontraktu manifestu pluginów
(`docs/plugins.md` w HomeApps). Plan i decyzje: [docs/plan.md](docs/plan.md).

## Wymagania

- .NET SDK 10, Docker (testy API stawiają Postgresa w Testcontainers)
- Kit HomeApps w lokalnym feedzie: w repo HomeApps uruchom `scripts/pack.sh`
  (pakuje do `~/.homeapps/packages`, z którego czyta `nuget.config` tego repo).

## Budowanie i testy

```bash
dotnet test
```

## Uruchomienie obok HomeApps (z IDE)

1. HomeApps w trybie dev (Postgres, RabbitMQ, Traefik, Auth, Households).
2. `src/Todo.Api`, profil `http` (port **5400**, `Development`). Baza `homeapps_todo` i migracje powstają same przy starcie.
3. Trasa w Traefiku: plik `dynamic/plugin-todo.yaml` w repo HomeApps (ignorowany przez git), treść w
   `docs/plugins.md` HomeApps, sekcja „From an IDE”.
4. Front HomeApps: `HOMEAPPS_PLUGINS=todo npm run dev`. Do etapu T3 plugin nie ma UI, więc widać go tylko
   w ustawieniach domu (włącznik).

## API (pod `/plugins/todo`)

| Metoda | Ścieżka (`/api/households/{hid}/...`) | Uprawnienie |
|---|---|---|
| GET | `lists` | `tasks.read` (wspólne + moje prywatne) |
| POST | `lists` `{name, visibility: Shared\|Private}` | `tasks.write`; wspólna wymaga `tasks.manage` |
| PUT | `lists/{id}` `{name}` | wspólna: `tasks.manage`, prywatna: właściciel |
| POST | `lists/{id}/archive`, `lists/{id}/restore` | jw. |
| DELETE | `lists/{id}` (razem z zadaniami) | jw. |
| GET | `tasks?list=&assignee=me&dueBy=YYYY-MM-DD&status=Open\|Done` | `tasks.read` |
| POST | `lists/{id}/tasks` `{title, notes, dueDate, dueTime, priority, assigneeId}` | `tasks.write` |
| PUT | `tasks/{id}` (cały formularz jak wyżej) | `tasks.write` |
| POST | `tasks/{id}/complete`, `tasks/{id}/reopen` | `tasks.write` |
| POST | `tasks/{id}/move` `{listId, beforeTaskId}` (null = na koniec) | `tasks.write` |
| DELETE | `tasks/{id}` | wspólna lista: autor albo `tasks.manage`; prywatna: właściciel |
| GET/PUT | `enablement` | z kitu (włącza Admin domu) |

Poza `/api`: `GET /plugins/todo/manifest`, `/plugins/todo/openapi/v1.json`, `/plugins/todo/health/{live,ready}`.

Zdarzenia: publikuje `HomeApps.Todo.Contracts.TaskCompleted` (tylko zadania z list wspólnych), słucha
`MemberLeft` i `MemberJoined` z Households.

## Układ

| Projekt | Co |
|---|---|
| `src/Todo.Domain` | listy (`TodoList`) i zadania (`TodoTask`): kto co widzi i zmienia, archiwum, wyjście z domu (T5) |
| `src/Todo.Contracts` | wiadomości dla innych usług (`TaskCompleted`) |
| `src/Todo.Application` | manifest (`TodoPlugin`), komendy i zapytania MediatR (folder na każdą: `Lists/CreateList/CreateListCommand.cs`, `…Validator.cs`, `…Handler.cs`), porty w `Abstractions` |
| `src/Todo.Infrastructure` | `TodoDbContext` (`PluginDbContext`), repozytoria, migracje, konsumenci Households, sprzątanie T5 |
| `src/Todo.Api` | host pluginu i endpointy |
| `tests/Todo.Domain.Tests` | testy domeny |
| `tests/Todo.Api.Tests` | testy end to end na Postgresie, z Households udawanym po HTTP |

Migracja: `dotnet ef migrations add <Nazwa> -p src/Todo.Infrastructure -o Persistence/Migrations`.
