| Feature | Stage | State (todo/doing/done/blocked) | Branch | ADR | Updated |
|---|---|---|---|---|---|
| household-planner | P1 Domain & Storage | done (Api tests with Docker pending) | feat/household-planner-p1 | specs/household-planner-adr.md | 2026-11-22 |
| household-planner | P2 Events & Calendar API | done (Api tests with Docker pending) | feat/household-planner-p2 | specs/household-planner-adr.md | 2026-11-22 |
| household-planner | P3 ICS Subscriptions | done (Api tests with Docker pending) | feat/household-planner-p3 | specs/household-planner-adr.md | 2026-11-22 |
| household-planner | P4 External Calendar Import & SSRF | done (Api tests with Docker pending) | feat/household-planner-p4 |
| household-planner | P5-P6 | todo | - | specs/household-planner-adr.md | 2026-11-22 |

## Decisions
- 2026-11-22: MemberLeft emptying an event's Attendees keeps the author as attendee (when the author is not the leaver), so the event does not become "whole household".
- 2026-11-22: reviewer/DA via agy needed `--dangerously-skip-permissions` in headless mode (reviewer rounds only); DA ran as Claude agent after the classifier denied the agy run.
- 2026-11-22: P2 reviewer and DA ran as Claude agents: agy headless was auto-denied the `command` permission (no --dangerously-skip-permissions used).

- 2026-11-22: P3 deferred to owner: per-user cap on ICS links; UserErased hard-deleting private events (ADR 3.5 pt 4); rows archived before the IsArchived migration stay unarchived.

- 2026-11-22: P4 reviewer/DA ran via agy OK. Open for owner: add NAT64/6to4 to SsrfIpPolicy?; MemberLeft does not set OwnerLeftAt on private events/series (P2 scope); failed manual sync is not persisted (intentional); notification URL `/homes/{hid}/apps/todo` is a guess.

## Blockers
- Run `dotnet test tests/Todo.Api.Tests` (ExternalCalendarApiTests, new) on a Docker host before merging P4.
- Run `dotnet test tests/Todo.Api.Tests` (SubscriptionTests, new) on a Docker host before merging P3.
- Run `dotnet test tests/Todo.Api.Tests` (incl. new CalendarTests) on a Docker host before merging P2.
- Run `dotnet test tests/Todo.Api.Tests` on a Docker host before merging P1 (migration apply, jsonb Attendees, xmin untested).
