# HomeApps.Todo

Plugin „Zadania” dla [HomeApps](../HomeApps): wspólne i prywatne listy zadań domu. Żyje poza repo HomeApps
i korzysta z jego kitu (paczki `HomeApps.*` z lokalnego feedu) oraz kontraktu manifestu pluginów
(`docs/plugins.md` w HomeApps). Plan i decyzje: [docs/plan.md](docs/plan.md).

## Wymagania

- .NET SDK 10, Docker (testy API stawiają Postgresa w Testcontainers)
- Node 22 (tylko do zmian w UI)
- Kit HomeApps w lokalnym feedzie: w repo HomeApps uruchom `scripts/pack.sh`
  (pakuje do `~/.homeapps/packages`, z którego czyta `nuget.config` tego repo, a `src/Todo.Web` bierze stamtąd
  `@homeapps/plugin-sdk`).

## Budowanie i testy

```bash
dotnet test

# UI (src/Todo.Web): buduje się do src/Todo.Api/wwwroot/ui, a build jest w repo jak w pluginie Hello,
# więc sam `dotnet run` serwuje gotowe UI. Po zmianie w .vue/.ts zbuduj i zacommituj wynik.
cd src/Todo.Web && npm ci && npm run build
```

## Uruchomienie obok HomeApps (z IDE)

1. HomeApps w trybie dev (Postgres, RabbitMQ, Traefik, Auth, Households).
2. `src/Todo.Api`, profil `http` (port **5400**, `Development`). Baza `homeapps_todo` i migracje powstają same przy starcie.
3. Trasa w Traefiku: plik `dynamic/plugin-todo.yaml` w repo HomeApps (ignorowany przez git), treść w
   `docs/plugins.md` HomeApps, sekcja „From an IDE”.
4. Front HomeApps: `HOMEAPPS_PLUGINS=todo npm run dev`. Po włączeniu pluginu w ustawieniach domu pojawia się
   zakładka **Zadania** i widżet **Na dziś** (strona domu i `/wall`). Przy zmianach w UI: `npm run build -- --watch`
   w `src/Todo.Web` i odśwież stronę.

## Uruchomienie obok HomeApps (Docker, produkcja)

Pełny opis, sprawdzenie i rozwiązywanie problemów: `docs/plugin-todo.md` w repo HomeApps. W skrócie:

```bash
ln -s ../HomeApps/.env .env      # raz: domena i hasła z .env HomeApps
docker compose -f compose.yaml up -d --build   # -f, bo .env HomeApps może ustawiać COMPOSE_FILE; obraz homeapps/todo:1.0.0, kit z ~/.homeapps/packages (HomeApps: scripts/pack.sh)
# w HomeApps: HOMEAPPS_PLUGINS=todo w .env, potem docker compose -f compose.prod.yaml up -d web
```

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
| `src/Todo.Api` | host pluginu i endpointy; `wwwroot/ui` to zbudowane UI |
| `src/Todo.Web` | UI (Vue 3 + `@homeapps/plugin-sdk`): strona „Zadania” (`TasksPage`), widżet „Na dziś” (`TodayWidget`), teksty pl/en w `main.ts` |
| `tests/Todo.Domain.Tests` | testy domeny |
| `tests/Todo.Api.Tests` | testy end to end na Postgresie, z Households udawanym po HTTP |

Migracja: `dotnet ef migrations add <Nazwa> -p src/Todo.Infrastructure -o Persistence/Migrations`.
