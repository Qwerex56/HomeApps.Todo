import { D as e, E as t, O as n, S as r, _ as i, b as a, d as o, g as s, h as c, k as l, l as u, m as d, n as f, p, t as m, u as h, v as g, w as _, x as v, y } from "./_plugin-vue_export-helper-DX_-5sjr.js";
import { n as b, r as x, s as S, t as C } from "./CalendarChip-BDOUGRpB.js";
import { t as w } from "./TaskRow-BojeEe0Q.js";
// src/PlannerAgendaWidget.vue?vue&type=script&setup=true&lang.ts
var T = {
	key: 0,
	class: "agenda-widget__list",
	"aria-busy": "true"
}, E = {
	key: 1,
	class: "agenda-widget__muted",
	role: "alert"
}, D = {
	key: 2,
	class: "agenda-widget__muted"
}, O = ["aria-label"], k = {
	key: 0,
	class: "agenda-widget__day"
}, A = { class: "agenda-widget__list" }, j = 8, M = /*#__PURE__*/ m(/* @__PURE__ */ v({
	__name: "PlannerAgendaWidget",
	props: {
		householdId: {},
		place: {}
	},
	setup(o) {
		let m = o, h = f(() => m.householdId), { host: v, canWrite: y, change: w } = h, M = /* @__PURE__ */ new Date(), N = d(() => ({
			from: new Date(M.getFullYear(), M.getMonth(), M.getDate()).toISOString(),
			to: b(new Date(M.getFullYear(), M.getMonth(), M.getDate()), 2).toISOString()
		})), P = h.calendar(N, [
			"events",
			"tasks",
			"external"
		]), F = d(() => m.place === "Wall"), I = d(() => [0, 1].map((e) => {
			let t = u(b(M, e));
			return {
				iso: t,
				label: v.t(e === 0 ? "due.today" : "due.tomorrow"),
				items: x(P.data.value ?? [], t).slice(0, j)
			};
		})), L = d(() => I.value.every((e) => e.items.length === 0)), R = (e) => y.value && e.itemType === "Task", z = w((e, t) => t.isDone ? e.reopenTask(t.id) : e.completeTask(t.id)), B = (e) => `${e.itemType}-${e.id}-${e.occurrenceDate ?? S(e)}`;
		return (o, u) => {
			let d = e("Skeleton");
			return _(), i("div", { class: r(["agenda-widget", { "agenda-widget--large": F.value }]) }, [l(P).isPending.value ? (_(), i("div", T, [(_(), i(p, null, t(3, (e) => a(d, {
				key: e,
				height: "2rem"
			})), 64))])) : l(P).isError.value ? (_(), i("p", E, n(l(v).errorMessage(l(P).error.value)), 1)) : L.value ? (_(), i("p", D, n(l(v).t("calendar.widgetEmpty")), 1)) : (_(!0), i(p, { key: 3 }, t(I.value, (e) => (_(), i("section", {
				key: e.iso,
				"aria-label": e.label
			}, [e.items.length ? (_(), i("h4", k, n(e.label), 1)) : s("", !0), g("div", A, [(_(!0), i(p, null, t(e.items, (e) => (_(), c(C, {
				key: B(e),
				item: e,
				"can-toggle": R(e),
				onOpen: (t) => R(e) && l(z).mutate(e),
				onToggle: (t) => l(z).mutate(e)
			}, null, 8, [
				"item",
				"can-toggle",
				"onOpen",
				"onToggle"
			]))), 128))])], 8, O))), 128))], 2);
		};
	}
}), [["__scopeId", "data-v-6ce30508"]]), N = {
	key: 0,
	class: "todo-widget__loading"
}, P = {
	key: 0,
	class: "todo-widget__tasks"
}, F = {
	key: 1,
	class: "todo-widget__empty"
}, I = {
	key: 2,
	class: "todo-widget__more"
}, L = 8, R = o({
	id: "todo",
	routes: {
		"": () => import("./TasksPage-I87hl8f1.js"),
		calendar: () => import("./CalendarPage-BHhlTHGz.js")
	},
	widgets: {
		today: /* @__PURE__ */ m(/* @__PURE__ */ v({
			__name: "TodayWidget",
			props: {
				householdId: {},
				place: {}
			},
			setup(o) {
				let u = o, m = f(() => u.householdId), { host: v, lists: b, canWrite: x, memberName: S, change: C } = m, T = m.tasks(() => ({
					dueBy: h(),
					status: "Open"
				})), E = d(() => (T.data.value ?? []).filter((e) => e.status === "Open")), D = d(() => E.value.slice(0, L)), O = (e) => b.data.value?.find((t) => t.id === e), k = d(() => u.place === "Wall"), A = C((e, t) => e.completeTask(t.id)), j = (e) => {
					let t = O(e.listId);
					return x.value && !!t && !t.archived;
				};
				return (o, u) => {
					let d = e("Skeleton");
					return _(), i("div", { class: r(["todo-widget", { "todo-widget--large": k.value }]) }, [l(T).isPending.value ? (_(), i("div", N, [(_(), i(p, null, t(3, (e) => a(d, {
						key: e,
						height: "2rem"
					})), 64))])) : (_(), i(p, { key: 1 }, [D.value.length ? (_(), i("ul", P, [(_(!0), i(p, null, t(D.value, (e) => (_(), c(w, {
						key: e.id,
						task: e,
						"list-name": O(e.listId)?.name ?? null,
						"assignee-name": l(S)(e.assigneeId),
						"can-toggle": j(e),
						large: k.value,
						onToggle: (t) => l(A).mutate(e)
					}, null, 8, [
						"task",
						"list-name",
						"assignee-name",
						"can-toggle",
						"large",
						"onToggle"
					]))), 128))])) : (_(), i("p", F, [u[0] ||= g("i", {
						class: "pi pi-check-circle",
						"aria-hidden": "true"
					}, null, -1), y(" " + n(l(v).t("empty.today")), 1)])), E.value.length > L ? (_(), i("p", I, n(l(v).t("more", { count: E.value.length - L })), 1)) : s("", !0)], 64))], 2);
				};
			}
		}), [["__scopeId", "data-v-dd1334e4"]]),
		agenda: M
	},
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
			create: "Utwórz",
			calendar: {
				title: "Planer",
				grid: "Kalendarz",
				navigate: "Nawigacja po kalendarzu",
				previous: "Poprzedni okres",
				next: "Następny okres",
				today: "Dziś",
				viewSwitch: "Widok kalendarza",
				views: {
					month: "Miesiąc",
					week: "Tydzień",
					day: "Dzień",
					agenda: "Agenda"
				},
				person: "Osoba",
				everyone: "Wszyscy",
				layersLabel: "Warstwy kalendarza",
				layers: {
					events: "Wydarzenia domu",
					private: "Moje prywatne",
					tasks: "Zadania z terminem",
					external: "Kalendarze zewnętrzne"
				},
				type: {
					Event: "Wydarzenie",
					Task: "Zadanie",
					External: "Kalendarz zewnętrzny"
				},
				newEvent: "Wydarzenie",
				editEvent: "Edycja wydarzenia",
				allDay: "Cały dzień",
				start: "Początek",
				end: "Koniec",
				location: "Miejsce",
				private: "Prywatne (widzisz tylko ty)",
				attendees: "Uczestnicy",
				repeat: {
					label: "Powtarzanie",
					none: "Nie powtarzaj",
					"FREQ=DAILY": "Codziennie",
					"FREQ=WEEKLY": "Co tydzień",
					"FREQ=MONTHLY": "Co miesiąc",
					"FREQ=YEARLY": "Co rok",
					series: "To wydarzenie cykliczne. Zmiany wybierasz dla wystąpień po zapisaniu."
				},
				reminder: {
					label: "Przypomnienia",
					atStart: "W chwili rozpoczęcia",
					before: "{minutes} min przed"
				},
				scope: {
					group: "Zakres zmiany",
					editTitle: "Które wystąpienia zmienić?",
					deleteTitle: "Które wystąpienia usunąć?",
					ThisInstance: "Tylko to wystąpienie",
					ThisAndFuture: "To i kolejne",
					All: "Wszystkie wystąpienia"
				},
				errors: {
					title: "Podaj tytuł wydarzenia.",
					range: "Koniec nie może być przed początkiem."
				},
				empty: "W tym okresie nic nie zaplanowano.",
				agendaEmpty: "Nic w tym okresie.",
				widgetEmpty: "Dziś i jutro nic nie zaplanowano.",
				addFirst: "Dodaj pierwsze wydarzenie",
				retry: "Spróbuj ponownie",
				readOnly: "Jako gość możesz tylko przeglądać kalendarz.",
				deleteConfirm: "Usunąć wydarzenie „{name}”?",
				settings: {
					title: "Ustawienia kalendarza",
					subscriptions: "Subskrypcje kalendarza (ICS)",
					subscriptionsHint: "Adres, który wklejasz do innej aplikacji kalendarza, żeby widzieć w niej wydarzenia domu.",
					noSubscriptions: "Brak subskrypcji.",
					label: "Nazwa subskrypcji",
					lastUsed: "ostatnio użyta",
					never: "nigdy",
					revoke: "Unieważnij subskrypcję {name}",
					feedOnce: "Skopiuj adres teraz. Drugi raz go nie pokażemy.",
					feedUrl: "Adres subskrypcji",
					copy: "Kopiuj",
					copied: "Adres skopiowany.",
					includePrivate: "Z moimi prywatnymi",
					tasksModeLabel: "Zadania w subskrypcji",
					tasksMode: {
						None: "Bez zadań",
						Assigned: "Moje zadania",
						All: "Wszystkie zadania"
					},
					addSubscription: "Dodaj subskrypcję",
					external: "Kalendarze zewnętrzne",
					noExternal: "Nie zaimportowano żadnego kalendarza.",
					url: "Adres ICS",
					name: "Nazwa kalendarza",
					color: "Kolor kalendarza",
					visibilityLabel: "Kto co widzi",
					visibility: {
						OnlyMe: "Tylko ja",
						Busy: "Domownicy widzą „Zajęty”",
						Details: "Domownicy widzą szczegóły"
					},
					addExternal: "Dodaj kalendarz",
					sync: "Odśwież {name}",
					remove: "Usuń {name}",
					stale: "Ostatnia udana synchronizacja: {when}. Sprawdź adres źródłowy"
				}
			}
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
			create: "Create",
			calendar: {
				title: "Planner",
				grid: "Calendar",
				navigate: "Calendar navigation",
				previous: "Previous period",
				next: "Next period",
				today: "Today",
				viewSwitch: "Calendar view",
				views: {
					month: "Month",
					week: "Week",
					day: "Day",
					agenda: "Agenda"
				},
				person: "Person",
				everyone: "Everyone",
				layersLabel: "Calendar layers",
				layers: {
					events: "Household events",
					private: "My private",
					tasks: "Tasks with a due date",
					external: "External calendars"
				},
				type: {
					Event: "Event",
					Task: "Task",
					External: "External calendar"
				},
				newEvent: "Event",
				editEvent: "Edit event",
				allDay: "All day",
				start: "Start",
				end: "End",
				location: "Location",
				private: "Private (only you see it)",
				attendees: "Attendees",
				repeat: {
					label: "Repeat",
					none: "Does not repeat",
					"FREQ=DAILY": "Daily",
					"FREQ=WEEKLY": "Weekly",
					"FREQ=MONTHLY": "Monthly",
					"FREQ=YEARLY": "Yearly",
					series: "A recurring event. You choose which occurrences change when you save."
				},
				reminder: {
					label: "Reminders",
					atStart: "At start",
					before: "{minutes} min before"
				},
				scope: {
					group: "Scope of the change",
					editTitle: "Which occurrences to change?",
					deleteTitle: "Which occurrences to delete?",
					ThisInstance: "This occurrence only",
					ThisAndFuture: "This and following",
					All: "All occurrences"
				},
				errors: {
					title: "Enter an event title.",
					range: "The end cannot be before the start."
				},
				empty: "Nothing planned in this period.",
				agendaEmpty: "Nothing in this period.",
				widgetEmpty: "Nothing planned for today and tomorrow.",
				addFirst: "Add the first event",
				retry: "Try again",
				readOnly: "As a guest you can only view the calendar.",
				deleteConfirm: "Delete the event “{name}”?",
				settings: {
					title: "Calendar settings",
					subscriptions: "Calendar subscriptions (ICS)",
					subscriptionsHint: "An address you paste into another calendar app to see the household events there.",
					noSubscriptions: "No subscriptions.",
					label: "Subscription name",
					lastUsed: "last used",
					never: "never",
					revoke: "Revoke subscription {name}",
					feedOnce: "Copy the address now. It is not shown again.",
					feedUrl: "Subscription address",
					copy: "Copy",
					copied: "Address copied.",
					includePrivate: "With my private events",
					tasksModeLabel: "Tasks in the subscription",
					tasksMode: {
						None: "No tasks",
						Assigned: "My tasks",
						All: "All tasks"
					},
					addSubscription: "Add subscription",
					external: "External calendars",
					noExternal: "No calendar imported.",
					url: "ICS address",
					name: "Calendar name",
					color: "Calendar colour",
					visibilityLabel: "Who sees what",
					visibility: {
						OnlyMe: "Only me",
						Busy: "Others see “Busy”",
						Details: "Others see details"
					},
					addExternal: "Add calendar",
					sync: "Refresh {name}",
					remove: "Remove {name}",
					stale: "Last successful sync: {when}. Check the source address"
				}
			}
		}
	}
});
export { R as default };
