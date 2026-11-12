# Plugin Todo: plan v1 (zaakceptowany 2026-10-29, T1–T8 wg propozycji)

Data: 2026-10-29. Baza: HomeApps 2.0.0 (lokalny master 8ffe7d6). Kod dopiero po akceptacji.

## 0. W skrócie

- Todo to **pierwszy plugin HomeApps** i jednocześnie test całego wzorca "plugin": osobne repo poza HomeApps, własna baza, własny kontener, wpięty przez Traefika, opisany manifestem.
- **Manifestu dziś nie ma w kodzie.** Jest tylko szkic w `plans/ideas/jarvis-hub.md` (9b.6). W HomeApps nie ma ani typów manifestu, ani `IHouseholdAccess`, ani ładowania pluginów w Vue (`/wall` ma tylko puste miejsca na kafelki). Ten plan więc najpierw domyka kontrakt (sekcja 2), a potem dopisuje do HomeApps minimum, które plugin z zewnątrz potrzebuje (sekcja 3).
- Kolejność: **HomeApps: SDK pluginów (backend + front)** → **repo pluginu: domena → API → UI → ekran i MCP → wdrożenie**.

## 1. Co już jest w HomeApps i czego brakuje

| Potrzeba pluginu | Stan w 2.0.0 | Braki |
|---|---|---|
| Kit (MediatR, FluentResults, outbox, JWT, `ICurrentUser`, `IEndpoint`) | `src/BuildingBlocks/*` | dostępny tylko jako `ProjectReference` w repo; plugin z zewnątrz nie ma jak go użyć |
| Kontrakty zdarzeń domów (`MemberLeft`, `MemberRoleChanged`, `HouseholdArchived`) | `src/Contracts/Households` | jw., brak paczki |
| "Czy ten user jest w domu i z jaką rolą" | `GET /households/{id}/members/me` (z JWT użytkownika) | brak klienta z cache w kicie (P7/K2 z jarvis-hub.md) |
| Polityki per zakres (`IsHouseholdMember`, `CanManageHousehold`...) | brak | do napisania |
| Manifest | tylko szkic | typy, endpoint, walidacja |
| Lista pluginów w instancji | brak | konfiguracja + odczyt w GUI |
| UI pluginu w Vue | brak | ładowanie modułu pluginu, API hosta, menu, kafelki, `/wall` |
| Trasy w Traefiku i PWA | ręczne listy (`dynamic/dev-routes.yaml`, `servicePaths` w `vite.config.ts`) | wspólny prefiks dla wszystkich pluginów, żeby nie dopisywać każdego |

## 2. Kontrakt manifestu (propozycja v1)

Manifest opisuje plugin i wskazuje, gdzie jest jego API, UI i MCP. Sam nie jest API. Serwuje go każdy plugin pod `GET /plugins/{id}/manifest` (anonimowo, bez danych domów). W .NET to rekord `PluginManifest` w nowej paczce `HomeApps.Plugins` i jest składany z kodu + `appsettings` pluginu, a w TS ten sam typ generowany z OpenAPI.

```json
{
  "manifestVersion": 1,
  "id": "todo",
  "version": "1.0.0",
  "requires": { "homeApps": ">=2.1.0" },
  "name": { "pl": "Zadania", "en": "Tasks" },
  "description": { "pl": "Wspólne i prywatne zadania domu", "en": "..." },
  "icon": "pi-check-square",
  "scopes": ["HouseholdUser"],
  "enablement": { "scope": "Household", "minRole": "Admin" },
  "permissions": [
    { "id": "tasks.read",   "minRole": "Guest"  },
    { "id": "tasks.write",  "minRole": "Member" },
    { "id": "tasks.manage", "minRole": "Admin"  }
  ],
  "endpoints": {
    "api": "/plugins/todo/api",
    "openapi": "/plugins/todo/openapi/v1.json",
    "mcp": "/plugins/todo/mcp",
    "realtime": null,
    "health": "/plugins/todo/health"
  },
  "ui": {
    "entry": "/plugins/todo/ui/entry.js",
    "style": "/plugins/todo/ui/entry.css",
    "routes": [ { "path": "", "title": { "pl": "Zadania", "en": "Tasks" }, "menu": true } ],
    "widgets": [
      { "id": "today", "title": { "pl": "Na dziś", "en": "Today" }, "size": "medium", "places": ["household", "wall"] }
    ],
    "settings": true
  },
  "display": [ { "id": "today", "path": "/plugins/todo/api/households/{householdId}/display/today", "refreshSeconds": 300 } ],
  "mcp": { "tools": ["add_task", "my_tasks", "complete_task"] },
  "events": {
    "publishes": ["HomeApps.Todo.Contracts.TaskCompleted", "HomeApps.Todo.Contracts.TaskDue"],
    "consumes":  ["HomeApps.Contracts.Households.MemberLeft", "HomeApps.Contracts.Households.HouseholdArchived"]
  }
}
```

Co dodaję względem szkicu z jarvis-hub.md 9b.6 i dlaczego:

| Pole | Po co |
|---|---|
| `manifestVersion`, `requires.homeApps` | GUI pomija plugin z nieznaną wersją manifestu albo za nowy dla tej instancji, zamiast się wywalać |
| `name`/`description` jako `{pl, en}` | front ma i18n (pl/en) |
| `enablement` | kto i w jakim zakresie włącza plugin (dla Todo: Admin domu włącza w swoim domu) |
| `permissions` | uprawnienia pluginu nazwane i przypięte do ról Household; GUI chowa przyciski, backend sprawdza to samo przez politykę z kitu |
| `endpoints.health` | Traefik/compose i GUI ("plugin chwilowo niedostępny") |
| `ui.entry/style/routes/widgets/settings` | **jak plugin dostarcza swoje ekrany** (sekcja 3.2); w szkicu były tylko widgety |
| `widgets[].places` | gdzie kafelek może stanąć: strona domu, `/wall`, pulpit |
| `display[]` z `path` i `refreshSeconds` | centrala/ramka wie, co pobierać i jak często, bez zgadywania trasy |
| `mcp.tools` | JARVIS wie, czego się spodziewać, zanim połączy się z MCP |
| `events` | dokumentacja zależności między serwisami (nie jest wymuszane w runtime) |

Włączenie w domu: każdy plugin ma `GET/PUT /plugins/{id}/api/households/{householdId}/enablement` (kit daje gotowe: tabela `PluginEnablement`, polityka z `enablement.minRole`). Brak rekordu = wyłączony (zgodnie z 9b.1).

## 3. Co dopisuję do HomeApps (etap A, przed pluginem)

### 3.1 Backend: paczki dla pluginów

- **Dystrybucja kitu: lokalny feed NuGet** (propozycja T2). `scripts/pack.sh` w HomeApps pakuje `HomeApps.BuildingBlocks.*`, `HomeApps.Contracts` i nowe `HomeApps.Plugins` do `~/.homeapps/packages` (wersja = wersja HomeApps, np. 2.1.0). Repo pluginu ma `nuget.config` wskazujący ten folder. Zero zmian w kodzie usług HomeApps, w repo dalej `ProjectReference`.
- **Nowy projekt `src/BuildingBlocks/HomeApps.Plugins`** (cienki):
  - `PluginManifest` (rekordy) + `MapPluginManifest()` i walidacja przy starcie (id = prefiks tras, wersja, role istnieją).
  - `IHouseholdAccess.GetMembershipAsync(householdId, userId)` → `HouseholdRole?`; HTTP do `http://households:8080/households/{id}/members/me` z JWT wywołującego, cache w pamięci 60 s, czyszczony konsumentem `MemberLeft`/`MemberRoleChanged`/`HouseholdArchived` (K2).
  - Filtr endpointu `.RequireHouseholdPermission("tasks.write")` (rola z `IHouseholdAccess` vs `permissions` z manifestu) i `.RequireHouseholdUserScope()` gdzie potrzeba.
  - `PluginEnablement` (encja + konfiguracja EF + dwa endpointy) do użycia w DbContext pluginu.
  - `MapPluginUi()`: serwuje zbudowany UI pluginu z `wwwroot/ui` pod `/plugins/{id}/ui`.
- **Traefik:** jeden wzorzec `PathPrefix(/plugins/{id})` z etykiet kontenera pluginu (docker provider już włączony), w dev dodatkowo trasa na hosta w `dynamic/`. `vite.config.ts`: jeden wpis `/^\/plugins\//` w `servicePaths` zamiast wpisu per plugin.
- **Lista pluginów w instancji:** zmienna `HOMEAPPS_PLUGINS=todo` w compose → nginx frontu serwuje `/plugins.json` (`["todo"]`), dev: plik w `public/`. Bez serwisu-rejestru (P1).

### 3.2 Front Vue: host pluginów

Propozycja T3: **plugin dostarcza swoje UI jako moduł ES** budowany Vite w trybie biblioteki, z `vue`, `vue-router`, `primevue`, `@tanstack/vue-query`, `vue-i18n` jako *external*. Host udostępnia je przez import map, więc plugin wygląda jak reszta aplikacji (ten sam motyw PrimeVue + Tailwind) i nie ładuje drugiej kopii Vue.

- Moduł eksportuje `definePlugin({ routes, widgets, settings, messages })` z małej paczki **`@homeapps/plugin-sdk`** (typy manifestu + API hosta). Paczka żyje w `src/Web/plugin-sdk` w HomeApps i trafia do pluginu jako `file:` / tarball z tego samego `scripts/pack.sh`.
- API hosta dla pluginu: `http` (axios z tokenem i odświeżaniem, jak w GUI), `useCurrentHousehold()`, rola w domu, `t()` (i18n z `messages` pluginu), `notify`, `problem` (obsługa błędów RFC 7807 jak w GUI).
- Host: `features/plugins` czyta `/plugins.json` → manifesty → `import(ui.entry)`; dokleja trasy pod `/homes/:id/apps/{pluginId}/...`, pozycje menu w zakładkach domu, kafelki na stronie domu i na `/wall`, ekran "Aplikacje" w ustawieniach domu (włącz/wyłącz, `enablement`). Plugin, który się nie załadował, nie psuje reszty GUI (kafelek "niedostępne").

Odrzucone: iframe (osobny motyw, sesja i routing, toporny na `/wall`), UI generowane z samego manifestu (za mało na listę zadań z przeciąganiem i edycją), wbudowanie ekranów Todo w `src/Web` (łamie "plugin żyje poza HomeApps").

## 4. Plugin Todo

### 4.1 Repo i układ

Propozycja T1: osobne repo git **`~/Documents/RiderProjects/HomeApps.Todo`** (obok HomeApps), własny `.slnx`, `Directory.Packages.props`, `global.json`, `nuget.config` na lokalny feed.

```
HomeApps.Todo/
  src/Todo.Domain  Todo.Application  Todo.Infrastructure  Todo.Api   (clean architecture jak w usługach)
  src/Todo.Contracts      (TaskCompleted, TaskDue)
  src/Todo.Web            (Vite library + @homeapps/plugin-sdk; build → Todo.Api/wwwroot/ui)
  tests/...  compose.plugin.yaml  Dockerfile   (manifest składany z kodu + appsettings, bez osobnego pliku)
  docs/plan.md            (ten plan przeniesiony tutaj po akceptacji)
```

`compose.plugin.yaml` dołącza do sieci `homeapps` (external), używa wspólnego Postgresa (baza `homeapps_todo`) i RabbitMQ.

### 4.2 Domena (zakres `HouseholdUser`)

Propozycja T4: na start **listy + zadania**, bez podzadań.

- `TodoList` (agregat, per dom): `Name`, `Visibility` (`Shared` dla domu / `Private` dla autora), `OwnerId` (autor), `Archived`. Każdy dom dostaje przy włączeniu listę "Dom" (wspólna).
- `TodoTask` (agregat, osobny, żeby długie listy nie ładowały się w całości): `ListId`, `Title`, `Notes?`, `DueDate?` (data, opcjonalnie godzina), `AssigneeId?` (członek domu), `Priority` (Low/Normal/High), `Status` (Open/Done), `CompletedBy/At`, `CreatedBy/At`, `Position` (kolejność ręczna).
- Zachowania: `Create`, `Rename`, `Edit`, `Assign/Unassign`, `Complete` → `TaskCompleted`, `Reopen`, `Move(list, position)`, `Delete` (twarde, to nie dane konta).
- Uprawnienia (z manifestu): Guest czyta wspólne listy; Member dodaje i odhacza zadania na wspólnych + ma prywatne; Admin/Owner zarządza wspólnymi listami (zmiana nazwy, archiwum) i włącza plugin. Prywatna lista: tylko autor, także Admin jej nie widzi (D2a: dane członków są prywatne).
- Gdy ktoś wychodzi z domu (`MemberLeft`): jego zadania wspólne tracą przypisanie, jego prywatne listy w tym domu zostają ukryte (T5: usuwać czy trzymać na wypadek powrotu, propozycja: trzymać 30 dni, potem usunąć). `HouseholdArchived`: plugin tylko przestaje odpowiadać dla tego domu.
- Imiona osób: plugin trzyma tylko `UserId`; GUI dokleja nazwy z listy członków Household, którą już ma.

### 4.3 API (Minimal APIs + MediatR 12, pod `/plugins/todo/api`)

| Metoda | Trasa | Uprawnienie |
|---|---|---|
| GET | `/households/{hid}/lists` | tasks.read (prywatne tylko swoje) |
| POST/PUT/DELETE | `/households/{hid}/lists[/{listId}]` | write (prywatna) / manage (wspólna) |
| GET | `/households/{hid}/tasks?list=&assignee=me&due=today&status=` | tasks.read |
| POST | `/households/{hid}/lists/{listId}/tasks` | tasks.write |
| PUT | `/households/{hid}/tasks/{taskId}` | tasks.write |
| POST | `/households/{hid}/tasks/{taskId}/complete` / `reopen` / `move` | tasks.write |
| DELETE | `/households/{hid}/tasks/{taskId}` | autor albo manage |
| GET | `/households/{hid}/display/today` | kompaktowy JSON dla ekranu (T6) |
| GET/PUT | `/households/{hid}/enablement` | z kitu |
| GET | `/plugins/todo/manifest` (poza `/api`) | anonimowo |

### 4.4 UI pluginu

Strona "Zadania" w domu (listy z lewej / zakładki na telefonie, zadania z przeciąganiem, szybkie dodawanie), filtr "moje" i "na dziś", edycja w bocznym panelu. Kafelek "Na dziś" na stronie domu i na `/wall` (duże litery, odhaczanie dotykiem). Ustawienia: tylko włącz/wyłącz w tym etapie.

### 4.5 Ekran i MCP (T7)

- `display/today`: do 6 zadań na dziś i zaległych, krótkie teksty, bez HTML. Uwierzytelnienie na razie JWT użytkownika (klucz ramki P3 czeka na decyzję w wątku centrali).
- MCP: `add_task`, `my_tasks`, `complete_task` jako cienkie nakładki na komendy MediatR, oficjalny pakiet `ModelContextProtocol.AspNetCore`, JWT użytkownika. Propozycja: robimy w tym pluginie, bo manifest ma to pokryć "w pełni", a JARVIS-a jeszcze nie ma, więc test to MCP Inspector.

### 4.6 Poza zakresem Todo 1.0

Przypomnienia (powiadomienia push/e-mail), zadania cykliczne, realtime SignalR (lista odświeża się co 30 s i po własnych zmianach), podzadania, załączniki.

## 5. Etapy (każdy na osobnej lokalnej gałęzi do review)

| # | Repo / gałąź | Co | Gotowe, gdy |
|---|---|---|---|
| A1 | HomeApps `plugins/sdk-backend` | `HomeApps.Plugins` (manifest, `IHouseholdAccess`, polityki, enablement, UI hosting), `scripts/pack.sh`, wersja 2.1.0, Traefik `/plugins/*`, `servicePaths`, docs `docs/plugins.md` | testy kitu zielone, paczki w `~/.homeapps/packages` |
| A2 | HomeApps `plugins/sdk-web` | `@homeapps/plugin-sdk`, import map, `features/plugins` (trasy, menu, kafelki, `/wall`, włącz/wyłącz), `/plugins.json` w nginx | testy Vitest + plugin "hello" z fixture ładuje się w GUI |
| T1 | HomeApps.Todo `todo/domain` | repo, solution, domena + testy | testy domeny zielone |
| T2 | `todo/api` | Application + Infrastructure + API + migracje + konsumenci Household + testy integracyjne | API działa za Traefikiem w dev |
| T3 | `todo/web` | UI pluginu, kafelki, i18n pl/en | widać w GUI HomeApps, e2e Playwright |
| T4 | `todo/display-mcp` | `display/today`, serwer MCP | MCP Inspector widzi i wywołuje 3 narzędzia |
| T5 | `todo/release` | Dockerfile, `compose.plugin.yaml`, README, tag `v1.0.0` | smoke na prod compose |

## 6. Decyzje (✅ właściciel, 2026-10-29: wszystkie wg propozycji)

| # | Pytanie | Propozycja |
|---|---|---|
| T1 | Gdzie folder pluginu | `~/Documents/RiderProjects/HomeApps.Todo`, osobne repo git |
| T2 | Jak plugin dostaje kit | lokalny feed NuGet `~/.homeapps/packages` budowany `scripts/pack.sh` (alternatywa: `ProjectReference` do `../HomeApps/src/...`, prościej, ale wiąże plugin z układem folderów i wersją z dysku) |
| T3 | Jak plugin dostarcza UI | moduł ES + import map + `@homeapps/plugin-sdk` (sekcja 3.2) |
| T4 | Model danych | listy + zadania (wspólne i prywatne listy), bez podzadań |
| T5 | Prywatne zadania osoby, która wyszła z domu | ukryć, usunąć po 30 dniach |
| T6 | Ekran `display/today` już teraz | tak, z JWT; klucz ramki po decyzji P3 |
| T7 | MCP w Todo 1.0 | tak (test MCP Inspectorem) |
| T8 | Kontrakt manifestu z sekcji 2 | przyjąć jako v1; zmiany po Todo trafiają do `manifestVersion` 1 tylko jako pola opcjonalne |

## 7. Postęp

- **A1 gotowe (2026-10-29):** lokalna gałąź `plugins/sdk-backend` w HomeApps (4 commity od master 8ffe7d6, ostatni 6840cbe), do review. Opis w `docs/plugins.md`. Różnice względem sekcji 2: `requires.homeApps` to po prostu minimalna wersja (`"2.1.0"`), ścieżka włączania jest w `enablement.path`, `realtime` pominięte do czasu SignalR, OpenAPI pluginu serwowane zawsze. Uprawnienia sprawdza zachowanie MediatR (`IHouseholdRequest`), więc obejmie też MCP.
- **A2 gotowe (2026-10-29):** lokalna gałąź `plugins/sdk-web` (4 commity na master 6840cbe, ostatni 8d2c1ad), do review. `@homeapps/plugin-sdk` w `src/Web/plugin-sdk` (typy, `definePlugin`, `useHost`, `homeAppsPlugin()` dla Vite: `vue`, `vue-router`, `vue-i18n`, `pinia`, `@tanstack/vue-query` brane z aplikacji przez `globalThis.__HOMEAPPS_SHARED__`), komponenty PrimeVue po nazwie z aplikacji, strony pluginu pod `/homes/{id}/apps/{plugin}/{path}`, kafelki w domu i na `/wall`, karta „Aplikacje” w ustawieniach domu. Lista pluginów: `HOMEAPPS_PLUGINS` → `/plugins.json`. Sprawdzone w Chromium z UI pluginu Hello (Owner i Guest).

## 8. Postęp w repo HomeApps.Todo

- **T1 gotowe (2026-11-06):** gałąź `todo/domain`. `TodoList` (wspólna/prywatna, archiwum, T5: `OwnerLeft`/`OwnerReturned`/`IsDueForPurge` po 30 dniach) i `TodoTask` (tytuł, notatki, termin dzień + opcjonalnie godzina, przypisanie, priorytet, `Position` jako ułamek, `Complete`/`Reopen` idempotentne, `Move` między listami, `CanDelete`, `AssigneeLeft`, `IsDueBy`). Rola wchodzi do domeny jako `Actor(UserId, Manages)`, gdzie `Manages` = rola sięga `tasks.manage` z manifestu; członkostwo i `tasks.read/write` sprawdza wcześniej `HouseholdPermissionBehavior`. Zdarzenie domenowe tylko `TaskCompleted` (z flagą `OnSharedList`, bo prywatne nie wychodzą na zewnątrz).
- **T2 gotowe (2026-11-07):** gałąź `todo/api`. Contracts (`TaskCompleted`), Application (manifest `TodoPlugin` bez UI/ekranu/MCP, dochodzą w T3/T4; komendy i zapytania z `IHouseholdRequest`), Infrastructure (`TodoDbContext`, migracja `Initial`, konsumenci `MemberLeft`/`MemberJoined`, sprzątanie T5 co 6 h), Api (port 5400). Różnice względem 4.3: filtr „na dziś” to `dueBy=YYYY-MM-DD` od klienta (serwer nie zna strefy czasowej domu); przenoszenie to `{listId, beforeTaskId}`, pozycja liczona na serwerze; zmiany list i usuwanie proszą o `tasks.write`, a próg `tasks.manage` dla list wspólnych sprawdza domena (`Actor.Manages`). Osoby przypisanej do zadania nie sprawdzamy w Households (GUI podsuwa tylko członków, przypisanie znika przy `MemberLeft`). `TaskDue` z planu odłożone razem z przypomnieniami (poza Todo 1.0).
- **T3 gotowe (2026-11-07):** gałąź `todo/web`. `src/Todo.Web` (Vue 3, `@homeapps/plugin-sdk` z lokalnego feedu), build do `Todo.Api/wwwroot/ui` trzymany w repo. Strona „Zadania”: po lewej widoki „Na dziś” i „Moje” (dla Domownika i wyżej), listy wspólne, prywatne i zwinięte archiwum; na telefonie jedna lista rozwijana. Szybkie dodawanie (tytuł + Enter), odhaczanie, zwinięta sekcja „Zrobione”, edycja w panelu z boku (tytuł, notatki, termin z godziną, priorytet, kto, przeniesienie na inną listę), przeciąganie myszą w obrębie listy i na listę w menu, menu listy (zmiana nazwy, archiwum, usunięcie z potwierdzeniem) tylko dla tego, komu API na to pozwoli. Widżet „Na dziś” (Medium, strona domu i tablet): otwarte zadania z terminem do dziś, maks. 8, na tablecie większe. Odświeżanie co 30 s i od razu po własnej zmianie. Imiona domowników z `GET /households/{id}/members`, więc Gość ich nie widzi (D2a) i nie przypisuje. Przeciąganie działa tylko myszą: na telefonie zadanie przenosi się na inną listę w panelu edycji, ale kolejności w liście nie zmienia (do rozważenia później).
- **Integracja (2026-11-07, przed T4):** gałąź `todo/docker`. `Dockerfile` (kit z lokalnego feedu jako dodatkowy kontekst budowania `homeapps`, UI z commita, bez Node), `compose.yaml` w sieci `homeapps_default` z etykietami Traefika i ustawieniami z `.env` HomeApps (link `.env`). Opis dla HomeApps: `docs/plugin-todo.md` tam. Sprawdzone na `compose.prod.yaml` + `compose.smoke.yaml`: smoke HomeApps zielony, włączenie w domu tworzy listę „Dom”, zadania, widżet na stronie domu i na tablecie, Domownik na telefonie.
- **MCP gotowe (2026-11-12, etap J1 planu JARVIS-a):** gałąź `todo/mcp` na kicie 2.3.0. Część MCP z T4 bez ekranu centrali: `add_task`, `my_tasks`, `complete_task` w `Todo.Api/Mcp`, nazwy zamiast id przez `IHouseholdDirectory` i `NameLookup` z kitu, manifest `mcp.tools`, `MinHomeAppsVersion` 2.3.0. Różnice: `my_tasks` = zadania przypisane do osoby (jak widok „Moje”), „dziś” to lokalna data serwera (strefy domu dalej brak); przypisanie przez MCP sprawdza imię w Households, w przeciwieństwie do API. Ekran `display/today` zostaje w T4.
