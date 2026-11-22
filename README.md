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

## Kalendarz (pod `/api/households/{hid}/calendar`, etap P2)

Wydarzenia i serie cykliczne (RRULE). Zapisy wymagają roli Członek domu lub wyższej (`tasks.write`).

| Metoda | Ścieżka | Co |
|---|---|---|
| GET | `events?from=&to=&layers=` | widok okna czasu: wydarzenia, wystąpienia serii i zadania z terminem; `layers` = `events,private,tasks` (domyślnie te trzy), `external` jest przyjmowane, ale bez danych do etapu P4 |
| POST | `events` | nowe wydarzenie albo seria (gdy podano `rrule`); 201 |
| GET | `events/{id}` | szczegóły wydarzenia |
| PUT | `events/{id}` | zmiana całego pojedynczego wydarzenia |
| DELETE | `events/{id}` | usunięcie wydarzenia |
| PUT | `events/{seriesId}/instances/{date}` `{scope, title?, description?, location?, startUtc?, endUtc?, reminderMinutes?, rrule?}` | zmiana jednego wystąpienia serii (`date` = lokalna data oryginalnego wystąpienia) |
| DELETE | `events/{seriesId}/instances/{date}?scope=` | usunięcie wystąpienia |

- `scope` przy zmianie i usuwaniu wystąpienia jest wymagany: `ThisInstance`, `ThisAndFuture` albo `All` (bez względu na wielkość liter). Brak lub zła wartość to `400 planner.invalid_scope`.
- `ThisInstance` nadpisuje tytuł, czas i przypomnienia tylko tego wystąpienia; `ThisAndFuture` dzieli serię od tego dnia (nowa seria zachowuje autora, licznik `COUNT` zaczyna się od nowa); `All` zmienia całą serię.
- Pola pominięte w `PUT` wystąpienia zostają bez zmian.
- Wydarzenia prywatne widzi tylko autor. Gość widzi wyłącznie wydarzenia, w których uczestniczy, i te dla całego domu.
- Serie zapisane w tym etapie nie przechowują uczestników ani przypomnień: `rrule` razem z uczestnikami lub przypomnieniami daje błąd walidacji.
- Obsługiwany podzbiór RRULE: `FREQ` DAILY/WEEKLY/MONTHLY/YEARLY z `INTERVAL`, `COUNT`, `UNTIL`, `BYDAY` (z porządkowymi w miesiącu/roku), `BYMONTHDAY` (tylko MONTHLY/YEARLY), `BYMONTH`; `WKST` tylko domyślny. Reszta (np. `BYSETPOS`, `INTERVAL=0`) jest odrzucana przy zapisie.
- Kody błędów: `400 planner.invalid_date_range`, `400 planner.empty_title`, `400 planner.invalid_scope`, `403 planner.cannot_edit_event`, `403 planner.cannot_edit_private`, `403 planner.cannot_delete_event`, `404 planner.event_not_found`, `404 planner.series_not_found`.

## ICS subscriptions (under `/api/households/{hid}/calendar/subscriptions`, stage P3)

Read-only ICS link so a phone calendar can show the household calendar. Each subscription has its own random token.

| Method | Path | What |
|---|---|---|
| GET | `subscriptions` | the caller's subscriptions in this household (no token) |
| POST | `subscriptions` `{label, includeHouseholdEvents?, includePrivateEvents?, tasksMode?}` | new subscription; 201 with `token` and `feedPath` |
| DELETE | `subscriptions/{id}` | revoke the link; 204 |
| GET | `/plugins/todo/feed/{token}.ics` (outside `/api`, anonymous) | `text/calendar` feed (RFC 5545) |

- The token (43 Base64Url characters, 256 bits) is returned **only once**, in the `POST` response. The database stores only its SHA-256 hash. A lost token cannot be recovered: create a new subscription and revoke the old one.
- `feedPath` is `/plugins/todo/feed/{token}.ics`; prepend your HomeApps domain when adding it to the calendar app.
- `includeHouseholdEvents` (default `true`) adds household-wide events. `includePrivateEvents` (default `false`) adds private events, but only for the subscription owner's own events.
- `tasksMode`: `None` (default, no tasks), `Assigned` (only tasks assigned to the caller) or `All` (all household tasks with a due date). Tasks are taken from a window of 30 days back to 366 days ahead.
- The feed contains events and series occurrences in that window, with overrides applied. Guests get only events they attend and household-wide events.
- Errors: `400 validation.problem` (empty or too long label, invalid `tasksMode`), `404 household.not_found`, `404 planner.subscription_not_found` (on `DELETE`, a subscription that is missing or belongs to someone else).
- The feed returns a bare `404 Not Found` when the token is unknown, the subscription is suspended, the member has left the household, the household is archived, or the account is disabled. Responses carry `Cache-Control: private, no-store` and `Referrer-Policy: no-referrer`.
- Account lifecycle: `UserDisabled` suspends all of the user's subscriptions (the feed returns 404 at once), `UserEnabled` restores them without changing the link, and `UserErased` deletes them permanently.
- A role change in the household is reflected in the feed immediately. Leaving the household (`MemberLeft`) and archiving it (`HouseholdArchived`) block the feed until membership is restored (`HouseholdRestored`).

## MCP (pod `/plugins/todo/mcp`, HomeApps 2.3.0+)

Dla JARVIS-a i innych klientów MCP, z tokenem osoby (też delegowanym), te same sprawdzenia uprawnień co API. Dom,
listę i osobę podaje się nazwą (bez względu na wielkość liter) albo id; gdy nazwa pasuje do kilku, narzędzie zwraca
listę do wyboru zamiast zgadywać. Pominięty dom = jedyny dom osoby (`add_task`) albo wszystkie jej domy z włączonym
Todo (`my_tasks`, `complete_task`).

| Narzędzie | Argumenty | Co |
|---|---|---|
| `add_task` | `title`, `list?`, `household?`, `due?` (`2026-11-13` albo `2026-11-13 18:00`), `assignee?` (imię, id albo `me`) | dodaje zadanie; bez `list` tylko gdy w domu jest jedna lista |
| `my_tasks` | `when?` (`today`, `week`, `all`, data), `household?` | otwarte zadania przypisane do osoby, z id; „dziś” = lokalna data serwera |
| `complete_task` | `task` (tytuł albo id), `household?` | odhacza otwarte zadanie |

Asystent (token z `act`) zmienia zadania, ale nie włącza ani nie wyłącza pluginu. Ręcznie: MCP Inspector,
Streamable HTTP `http://localhost:5400/plugins/todo/mcp`, nagłówek `Authorization: Bearer <token z przeglądarki>`.

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
