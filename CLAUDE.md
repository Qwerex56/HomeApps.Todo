# HomeApps.Todo for Claude Code

Plugin "Zadania" (and, in progress, "Planer": calendar + recurring tasks) for HomeApps. Lives outside the HomeApps repo and uses
its kit (`HomeApps.*` packages from `~/.homeapps/packages`; build them in `../HomeApps` with `scripts/pack.sh`).
See `README.md` and `docs/plan.md`. Specs: `specs/household-planner.md`, decisions: `specs/household-planner-adr.md`.

## Commands

| What | Command |
|---|---|
| Build (warnings are errors) | `dotnet build HomeApps.Todo.slnx` |
| Domain tests (no Docker) | `dotnet test tests/Todo.Domain.Tests` |
| API tests (Docker: Testcontainers Postgres) | `dotnet test tests/Todo.Api.Tests` |
| UI checks | `cd src/Todo.Web && npm run type-check && npm run build` (commit the built `wwwroot/ui`) |

## Rules

- Project skills in `.claude/skills`: `dotnet-conventions`, `vue-conventions`, `testing-standards`, `code-review-checklist`.
  Roles, `run-feature` and `spec-debate` come from `~/.agent-hub` / `~/.claude`: do not copy them here.
- One type per file; one folder per command/query with `Command`/`Validator`/`Handler`. Lean: no new abstractions or packages
  without a real need; never bump MediatR, MassTransit or kit versions.
- The ADR contract (routes, field names, status and error codes) is binding. `specs/` is read-only for agents.
- Never touch `master` directly and never force-push. Work on the integration branch the owner names; PRs go against it.
  Never skip, disable or delete tests to get green.
- Commits: small, topical, English (`Todo: recurring tasks with scope`). Never commit `.claude/runs/`, `.agents/runs/`, `.env`, `bin/`, `obj/`, `node_modules/`.
- Answer the owner in Polish. Code, comments, commits and docs stay in English (UI texts in Polish).
- Keep tokens low: grep and read line ranges, build/test only affected projects.
