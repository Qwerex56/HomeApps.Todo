import { C as e, E as t, T as n, _ as r, b as i, c as a, d as o, f as s, g as c, h as l, l as u, m as d, n as f, p, r as m, t as h, v as g, w as _, x as v, y } from "./TaskRow-6h-DxQkL.js";
// src/TodayWidget.vue?vue&type=script&setup=true&lang.ts
var b = {
	key: 0,
	class: "todo-widget__loading"
}, x = {
	key: 0,
	class: "todo-widget__tasks"
}, S = {
	key: 1,
	class: "todo-widget__empty"
}, C = {
	key: 2,
	class: "todo-widget__more"
}, w = 8, T = u({
	id: "todo",
	routes: { "": () => import("./TasksPage-v5r77IwB.js") },
	widgets: { today: /* @__PURE__ */ f(/* @__PURE__ */ y({
		__name: "TodayWidget",
		props: {
			householdId: {},
			place: {}
		},
		setup(u) {
			let f = u, y = m(() => f.householdId), { host: T, lists: E, canWrite: D, memberName: O, change: k } = y, A = y.tasks(() => ({
				dueBy: a(),
				status: "Open"
			})), j = s(() => (A.data.value ?? []).filter((e) => e.status === "Open")), M = s(() => j.value.slice(0, w)), N = (e) => E.data.value?.find((t) => t.id === e), P = s(() => f.place === "Wall"), F = k((e, t) => e.completeTask(t.id)), I = (e) => {
				let t = N(e.listId);
				return D.value && !!t && !t.archived;
			};
			return (a, s) => {
				let u = _("Skeleton");
				return v(), l("div", { class: i(["todo-widget", { "todo-widget--large": P.value }]) }, [t(A).isPending.value ? (v(), l("div", b, [(v(), l(o, null, e(3, (e) => g(u, {
					key: e,
					height: "2rem"
				})), 64))])) : (v(), l(o, { key: 1 }, [M.value.length ? (v(), l("ul", x, [(v(!0), l(o, null, e(M.value, (e) => (v(), p(h, {
					key: e.id,
					task: e,
					"list-name": N(e.listId)?.name ?? null,
					"assignee-name": t(O)(e.assigneeId),
					"can-toggle": I(e),
					large: P.value,
					onToggle: (n) => t(F).mutate(e)
				}, null, 8, [
					"task",
					"list-name",
					"assignee-name",
					"can-toggle",
					"large",
					"onToggle"
				]))), 128))])) : (v(), l("p", S, [s[0] ||= c("i", {
					class: "pi pi-check-circle",
					"aria-hidden": "true"
				}, null, -1), r(" " + n(t(T).t("empty.today")), 1)])), j.value.length > w ? (v(), l("p", C, n(t(T).t("more", { count: j.value.length - w })), 1)) : d("", !0)], 64))], 2);
			};
		}
	}), [["__scopeId", "data-v-dd1334e4"]]) },
	messages: {
		pl: {
			lists: "Listy",
			views: {
				today: "Na dziś",
				mine: "Moje"
			},
			archive: "Archiwum",
			archived: "zarchiwizowana",
			newList: "Nowa lista",
			renameList: "Zmień nazwę",
			archiveList: "Archiwizuj",
			restoreList: "Przywróć",
			deleteList: "Usuń listę",
			deleteListConfirm: "Usunąć listę „{name}” razem z jej zadaniami? Tego nie da się cofnąć.",
			listActions: "Działania na liście",
			visibility: {
				Shared: "Wspólna",
				Private: "Prywatna"
			},
			visibilityHint: {
				Shared: "Widzi ją cały dom, zadania dodają domownicy.",
				Private: "Widzisz ją tylko ty."
			},
			addPlaceholder: "Dodaj zadanie i naciśnij Enter",
			add: "Dodaj",
			readOnly: "Jako gość możesz tylko przeglądać listy.",
			empty: {
				today: "Na dziś nic nie zostało.",
				mine: "Nie masz przypisanych zadań.",
				list: "Lista jest pusta."
			},
			doneCount: "Zrobione ({count})",
			more: "i jeszcze {count}…",
			editTask: "Zadanie",
			complete: "Odhacz",
			reopen: "Przywróć zadanie",
			notes: "Ma notatki",
			nobody: "Nikt",
			due: {
				today: "dziś",
				tomorrow: "jutro",
				yesterday: "wczoraj"
			},
			priority: {
				Low: "Niski",
				Normal: "Normalny",
				High: "Wysoki"
			},
			fields: {
				title: "Tytuł",
				notes: "Notatki",
				due: "Termin",
				time: "Godzina",
				priority: "Priorytet",
				assignee: "Kto",
				list: "Lista",
				listName: "Nazwa listy"
			},
			delete: "Usuń",
			cancel: "Anuluj",
			save: "Zapisz",
			create: "Utwórz"
		},
		en: {
			lists: "Lists",
			views: {
				today: "Today",
				mine: "Mine"
			},
			archive: "Archive",
			archived: "archived",
			newList: "New list",
			renameList: "Rename",
			archiveList: "Archive",
			restoreList: "Restore",
			deleteList: "Delete list",
			deleteListConfirm: "Delete the list “{name}” with all its tasks? This cannot be undone.",
			listActions: "List actions",
			visibility: {
				Shared: "Shared",
				Private: "Private"
			},
			visibilityHint: {
				Shared: "The whole household sees it; members add tasks.",
				Private: "Only you see it."
			},
			addPlaceholder: "Add a task and press Enter",
			add: "Add",
			readOnly: "As a guest you can only view the lists.",
			empty: {
				today: "Nothing left for today.",
				mine: "No tasks assigned to you.",
				list: "The list is empty."
			},
			doneCount: "Done ({count})",
			more: "and {count} more…",
			editTask: "Task",
			complete: "Tick off",
			reopen: "Reopen",
			notes: "Has notes",
			nobody: "Nobody",
			due: {
				today: "today",
				tomorrow: "tomorrow",
				yesterday: "yesterday"
			},
			priority: {
				Low: "Low",
				Normal: "Normal",
				High: "High"
			},
			fields: {
				title: "Title",
				notes: "Notes",
				due: "Due",
				time: "Time",
				priority: "Priority",
				assignee: "Who",
				list: "List",
				listName: "List name"
			},
			delete: "Delete",
			cancel: "Cancel",
			save: "Save",
			create: "Create"
		}
	}
});
export { T as default };
