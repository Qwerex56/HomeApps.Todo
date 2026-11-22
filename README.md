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
| POST | `lists/{id}/tasks` `{title, notes, dueDate, dueTime, priority, assigneeId, rrule?, anchor?}` | `tasks.write` |
| PUT | `tasks/{id}?scope=` (cały formularz jak wyżej, z `rrule?`, `anchor?`) | `tasks.write`; zakres w sekcji „Zadania cykliczne” |
| POST | `tasks/{id}/complete`, `tasks/{id}/reopen` | `tasks.write` |
| POST | `tasks/{id}/move` `{listId, beforeTaskId}` (null = na koniec) | `tasks.write` |
| DELETE | `tasks/{id}?scope=` | wspólna lista: autor albo `tasks.manage`; prywatna: właściciel; zakres jak wyżej |
| GET/PUT | `enablement` | z kitu (włącza Admin domu) |

Poza `/api`: `GET /plugins/todo/manifest`, `/plugins/todo/openapi/v1.json`, `/plugins/todo/health/{live,ready}`.

## Kalendarz (pod `/api/households/{hid}/calendar`, etap P2)

Wydarzenia i serie cykliczne (RRULE). Zapisy wymagają roli Członek domu lub wyższej (`tasks.write`).

| Metoda | Ścieżka | Co |
|---|---|---|
| GET | `events?from=&to=&layers=` | widok okna czasu: wydarzenia, wystąpienia serii i zadania z terminem; `layers` = `events,private,tasks` (domyślnie te trzy), `external` = zaimportowane kalendarze ICS (etap P4, patrz niżej) |
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

## Reminders and time zones (stage P5)

- `CalendarReminderJob` (hosted service) runs every 60 seconds. It sends event reminders (per-event and per-user overrides, defaults from the user's planner settings), due-time task reminders (due time minus the user's task offsets) and a morning digest "Zadania na dziś" at 07:30 local time, once per user per day.
- Notifications go through `INotifier`; the job calls `SaveChangesAsync` after notifying so the outbox is written.
- A reminder more than 60 minutes late is dropped and logged, not sent.
- Time zones: each user may set an IANA `timeZone` in planner settings (validated). Without it the server uses `Planner:DefaultTimeZone` (`Europe/Warsaw` in `appsettings.json`). The zone is used for local times (digest, due-time reminders, new events and the agenda). Event and series zones are kept as IANA ids.
- All server-side calculations (recurrence, stored instants, reminder timing) run in UTC. Local times around DST: a skipped local time moves forward one hour, a repeated one takes its first occurrence.

## Planner settings (`/api/households/{hid}/calendar/settings`, stage P6)

| Method | Path | What |
|---|---|---|
| GET | `calendar/settings` | the caller's settings in this household |
| PUT | `calendar/settings` `{eventReminders?, taskReminders?, firstDayOfWeek, timeZone?}` | replace the caller's settings; returns the same view |

- `eventReminders` and `taskReminders` are lists of minutes before the start. `firstDayOfWeek` is a `DayOfWeek` name (for example `Monday`). `timeZone` is an IANA id or null (default zone).
- Every member, Guest included, changes only their own settings. Errors: `400 validation.problem`, `404 household.not_found`.

## Recurring tasks and scope (stage P6)

- A task may repeat: `rrule` (the same RFC 5545 subset as events) and `anchor`:
  - `FromDueDate`: the next due date is the previous due date plus the interval;
  - `FromCompletionDate`: the next due date is the completion date plus the interval.
- The next instance is created only when the open one is completed, so a series has at most one open task.
- `PUT tasks/{id}` and `DELETE tasks/{id}` take `?scope=`: `ThisInstance`, `ThisAndFuture` or `All` (case-insensitive). Missing scope means `All`. An unknown value is `400 planner.invalid_scope`.
- `ThisInstance` changes or skips only the open instance; the next one follows the rule. `ThisAndFuture` and `All` change the series. Deleting with `All` or `ThisAndFuture` removes the open task, and the series does not continue.

## Planner UI and wall widget (stage P6)

- Route `/calendar` shows month, week, day and agenda views (agenda by default on mobile), with layers for household events, private events, tasks, and external calendars. Tasks with a due date have a checkbox.
- The event drawer edits one event (full screen on mobile). Changing or deleting a repeating event or task asks for the scope (`SeriesScopeDialog`).
- The settings dialog holds planner settings, ICS subscriptions and external imports. External imports are hidden for Guests.
- Guests get a read-only view: no create, edit or drag and drop.
- Widget `planner-agenda` ("Planer: dziś i jutro") for the household and `/wall`: today and tomorrow, with events, due tasks and imported calendars. On `/wall` it uses larger type and shows no external-calendar details and no private events.

## No membership copy

The plugin keeps no copy of users or memberships (see `docs/plugins.md` in HomeApps). Roles come from `IHouseholdAccess` with the caller's token. Background jobs and the public ICS feed use only plugin data:
- `CalendarSubscription.IsGuest`, updated from `MemberRoleChanged`/`MemberJoined`, for feed visibility;
- `InactiveMarker` rows, derived from events and not from roles or names: household-level for `HouseholdArchived`/`HouseholdRestored`, user-level for `UserDisabled`/`UserEnabled`/`UserErased` (erased is permanent), and both for `MemberLeft`/`MemberJoined`. The feed and reminders skip marked users.

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

## External calendars (under `/api/households/{hid}/calendar/external`, stage P4)

Imports a calendar from an ICS address (for example a work or school calendar) into the household view. Each import belongs to the member who added it. Writes require the Member role or higher (`tasks.write`); guests see no imported events.

| Method | Path | What |
|---|---|---|
| GET | `external` | the caller's imports in this household (no address) |
| POST | `external` `{url, name, color?, visibility}` | new import; 201 with `Location` and the import view; the first download runs at once |
| POST | `external/{id}/sync` | download the calendar now; 200 with the import view |
| DELETE | `external/{id}` | remove the import; 204 |

- The address (`url`) is secret: it is stored on the server, returned to no one, and never appears in responses, logs of the view or the feed. Only the owner can sync or remove an import; somebody else's import is `404 planner.calendar_not_found` (on `DELETE`, `403 planner.not_owner` for an import the caller can see but did not add).
- `url` must be `http` or `https`, up to 2048 characters, without user name and password. `name` is 1-100 characters without control characters. `color` (up to 16 characters) defaults to `#3b82f6`.
- `visibility` controls what other household members see (the owner always sees everything):
  - `OnlyMe`: nobody else sees the import;
  - `Busy`: others see the time blocks with the title `Zajęty` and no details;
  - `Details`: others see titles, descriptions and locations.
- The import view has `id`, `name`, `color`, `visibility`, `lastSyncedAt`, `lastSyncFailedAt`, `lastErrorMessage` and `isStale`. `isStale` is `true` while the last attempt failed; the data shown is then the last one downloaded successfully.
- Imported events appear in `GET events` under the `external` layer (`layers=events,private,tasks,external`), with the import's `color`.
- Errors: `400 planner.invalid_url` (address not http(s), too long or with credentials), `400 planner.invalid_import` (bad name or color), `400 planner.ssrf_rejected` (the address points inside the server's network), `400 planner.parse_error` (the download is not an iCalendar file), `400 planner.sync_failed` (on `sync`, the calendar could not be downloaded), `404 household.not_found`, `404 planner.calendar_not_found`, `403 planner.not_owner`, `403 plugin.forbidden`.
- On `POST`, an address that is refused (SSRF, not a calendar) is rejected and not saved. An address that is only unreachable right now is saved: the job tries again, and the view shows `lastErrorMessage` and `isStale`.

### Synchronisation

- A background job runs every 5 minutes and refreshes each import whose last attempt is at least 30 minutes old. Failures count as attempts, so a broken address is retried every 30 minutes.
- After 144 consecutive failures (three days at the 30-minute rhythm) the owner gets one push notification, "Kalendarz nie odświeża się" (the calendar does not refresh). Further failures do not repeat it; the next successful sync resets the count.

### Address checks (SSRF guard)

Every download, including the first one, goes through the same guard:

- **Scheme:** `https` only in Production; `http` is also accepted outside Production (development).
- **Destination:** the host name is resolved and every address is checked before connecting. Loopback, private (RFC 1918), link-local (including cloud metadata `169.254.169.254`), CGNAT `100.64.0.0/10`, multicast and unspecified addresses are refused, including IPv4-mapped IPv6 forms. The connection is made to the checked address (the TLS certificate is still checked against the host name), so the address cannot change between the check and the connection.
- **Time:** 10 seconds in total for one download.
- **Size:** at most 2 MB; a larger response is cut off and counts as a failure.
- **Redirects:** followed by hand, at most 3 hops; each target goes through the full check again. A redirect to a refused address or to a non-https scheme in Production fails the download.

### Retention after the owner leaves

- On `MemberLeft` the import is hidden at once, its sync stops, and it is kept with the owner's leaving time.
- If the member joins the household again within 30 days, the import comes back as it was (sync resumes).
- After 30 days the import is deleted by the cleanup job (the same job that purges left private lists). Deleting the account (`UserErased`) removes all of the user's imports at once.

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
| `src/Todo.Infrastructure` | `TodoDbContext` (`PluginDbContext`), repozytoria, migracje, konsumenci Households, sprzątanie T5, `CalendarReminderJob`, `ExternalCalendarSyncJob` |
| `src/Todo.Api` | host pluginu i endpointy; `wwwroot/ui` to zbudowane UI |
| `src/Todo.Web` | UI (Vue 3 + `@homeapps/plugin-sdk`): strona „Zadania” (`TasksPage`), widżet „Na dziś” (`TodayWidget`), planer `/calendar` (`CalendarPage`), widżet `planner-agenda`, teksty pl/en w `main.ts` |
| `tests/Todo.Domain.Tests` | testy domeny |
| `tests/Todo.Api.Tests` | testy end to end na Postgresie, z Households udawanym po HTTP |

Migracja: `dotnet ef migrations add <Nazwa> -p src/Todo.Infrastructure -o Persistence/Migrations`.
