# HomeApps.Todo

Plugin „Zadania” dla [HomeApps](../HomeApps): wspólne i prywatne listy zadań domu. Żyje poza repo HomeApps
i korzysta z jego kitu (paczki `HomeApps.*` z lokalnego feedu) oraz kontraktu manifestu pluginów
(`docs/plugins.md` w HomeApps). Plan i decyzje: [docs/plan.md](docs/plan.md).

## Wymagania

- .NET SDK 10
- Kit HomeApps w lokalnym feedzie: w repo HomeApps uruchom `scripts/pack.sh`
  (pakuje do `~/.homeapps/packages`, z którego czyta `nuget.config` tego repo).

## Budowanie i testy

```bash
dotnet test
```

## Układ

| Projekt | Co |
|---|---|
| `src/Todo.Domain` | listy (`TodoList`) i zadania (`TodoTask`): kto co widzi i zmienia, archiwum, wyjście z domu (T5) |
| `tests/Todo.Domain.Tests` | testy domeny |

Kolejne etapy (API, UI, ekran i MCP, wdrożenie) dochodzą na osobnych gałęziach, patrz plan, sekcja 5.
