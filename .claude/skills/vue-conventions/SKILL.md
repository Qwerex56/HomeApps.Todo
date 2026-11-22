---
name: vue-conventions
description: HomeApps.Todo plugin UI conventions (Vue 3.5 + TypeScript + Vite, TanStack Query + Axios, @homeapps/plugin-sdk definePlugin, Polish messages in main.ts). Use when writing, changing or reviewing code in src/Todo.Web.
---

# Todo plugin UI conventions

`src/Todo.Web` is a **plugin UI**, not the HomeApps web app: Vue 3.5, TypeScript ~5.9 strict (`vue-tsc`), Vite 8,
`@tanstack/vue-query` + `axios` for server data, `@homeapps/plugin-sdk` (`definePlugin`) for routes, widgets and
Polish `messages`. No Pinia, no vue-i18n of its own, no PrimeVue/Tailwind imports unless the plugin SDK exposes them:
**do not add packages** (see the ADR, section on the front end) without the owner's approval.

- Flat layout in `src/`: pages and components (`TasksPage.vue`, `TaskEditor.vue`, `TodayWidget.vue`), `api.ts`
  (HTTP calls), `useTodo.ts` (queries/mutations), `dates.ts`. Follow the existing files; extract a folder only when a
  feature (e.g. calendar) grows past a handful of files.
- Register routes and widgets in `main.ts` (`definePlugin`); user-visible text goes in `messages.pl`, never hardcoded.
- Server errors: show the ProblemDetails message translated through `messages`; handle loading, empty and error states.
- Reference for HomeApps web conventions (read only): `../HomeApps/.claude/skills/vue-conventions/SKILL.md`.

## Before you finish

`cd src/Todo.Web && npm run type-check && npm run build` (the build output in `src/Todo.Api/wwwroot/ui` is committed).
