| Feature | Stage | State (todo/doing/done/blocked) | Branch | ADR | Updated |
|---|---|---|---|---|---|
| household-planner | P1 Domain & Storage | done (Api tests with Docker pending) | feat/household-planner-p1 | specs/household-planner-adr.md | 2026-11-22 |
| household-planner | P2-P6 | todo | - | specs/household-planner-adr.md | 2026-11-22 |

## Decisions
- 2026-11-22: MemberLeft emptying an event's Attendees keeps the author as attendee (when the author is not the leaver), so the event does not become "whole household".
- 2026-11-22: reviewer/DA via agy needed `--dangerously-skip-permissions` in headless mode (reviewer rounds only); DA ran as Claude agent after the classifier denied the agy run.

## Blockers
- Run `dotnet test tests/Todo.Api.Tests` on a Docker host before merging P1 (migration apply, jsonb Attendees, xmin untested).
