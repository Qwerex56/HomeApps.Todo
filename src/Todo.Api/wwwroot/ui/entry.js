import { A as e, D as t, E as n, O as r, S as i, T as a, _ as o, b as s, d as c, g as l, h as u, k as d, l as f, m as p, n as m, p as h, t as g, u as _, v, w as y, x as b, y as x } from "./_plugin-vue_export-helper-Ntn7wuVI.js";
import { n as S, r as C, s as w, t as T } from "./CalendarChip-CPNvSX1x.js";
import { t as E } from "./TaskRow-Cjsj_ntn.js";
// src/PlannerAgendaWidget.vue?vue&type=script&setup=true&lang.ts
var D = {
	key: 0,
	class: "agenda-widget__list",
	"aria-busy": "true"
}, O = {
	key: 1,
	class: "agenda-widget__muted",
	role: "alert"
}, k = {
	key: 2,
	class: "agenda-widget__muted"
}, A = ["aria-label"], j = {
	key: 0,
	class: "agenda-widget__day"
}, M = { class: "agenda-widget__list" }, N = 8, P = /*#__PURE__*/ g(/* @__PURE__ */ b({
	__name: "PlannerAgendaWidget",
	props: {
		householdId: {},
		place: {}
	},
	setup(c) {
		let g = c, _ = m(() => g.householdId), { host: b, canWrite: x, change: E } = _, P = n(/* @__PURE__ */ new Date()), F = setInterval(() => {
			let e = /* @__PURE__ */ new Date();
			f(e) !== f(P.value) && (P.value = e);
		}, 6e4);
		y(() => clearInterval(F));
		let I = p(() => {
			let e = new Date(P.value.getFullYear(), P.value.getMonth(), P.value.getDate());
			return {
				from: e.toISOString(),
				to: S(e, 2).toISOString()
			};
		}), L = p(() => g.place === "Wall"), R = _.calendar(I, () => L.value ? ["events", "tasks"] : [
			"events",
			"tasks",
			"external"
		]), z = p(() => [0, 1].map((e) => {
			let t = f(S(P.value, e));
			return {
				iso: t,
				label: b.t(e === 0 ? "due.today" : "due.tomorrow"),
				items: C(R.data.value ?? [], t).slice(0, N)
			};
		})), B = p(() => z.value.every((e) => e.items.length === 0)), V = (e) => x.value && e.itemType === "Task", H = E((e, t) => t.isDone ? e.reopenTask(t.id) : e.completeTask(t.id)), U = (e) => `${e.itemType}-${e.id}-${e.occurrenceDate ?? w(e)}`;
		return (n, c) => {
			let f = r("Skeleton");
			return a(), o("div", { class: i(["agenda-widget", { "agenda-widget--large": L.value }]) }, [e(R).isPending.value ? (a(), o("div", D, [(a(), o(h, null, t(3, (e) => s(f, {
				key: e,
				height: "2rem"
			})), 64))])) : e(R).isError.value ? (a(), o("p", O, d(e(b).errorMessage(e(R).error.value)), 1)) : B.value ? (a(), o("p", k, d(e(b).t("calendar.widgetEmpty")), 1)) : (a(!0), o(h, { key: 3 }, t(z.value, (n) => (a(), o("section", {
				key: n.iso,
				"aria-label": n.label
			}, [n.items.length ? (a(), o("h4", j, d(n.label), 1)) : l("", !0), v("div", M, [(a(!0), o(h, null, t(n.items, (t) => (a(), u(T, {
				key: U(t),
				item: t,
				"can-toggle": V(t),
				onOpen: (n) => V(t) && e(H).mutate(t),
				onToggle: (n) => e(H).mutate(t)
			}, null, 8, [
				"item",
				"can-toggle",
				"onOpen",
				"onToggle"
			]))), 128))])], 8, A))), 128))], 2);
		};
	}
}), [["__scopeId", "data-v-1badfdc5"]]), F = {
	key: 0,
	class: "todo-widget__loading"
}, I = {
	key: 0,
	class: "todo-widget__tasks"
}, L = {
	key: 1,
	class: "todo-widget__empty"
}, R = {
	key: 2,
	class: "todo-widget__more"
}, z = 8, B = c({
	id: "todo",
	routes: {
		"": () => import("./TasksPage-D67PzAZj.js"),
		calendar: () => import("./CalendarPage-DgPHK3xG.js")
	},
	widgets: {
		today: /* @__PURE__ */ g(/* @__PURE__ */ b({
			__name: "TodayWidget",
			props: {
				householdId: {},
				place: {}
			},
			setup(n) {
				let c = n, f = m(() => c.householdId), { host: g, lists: y, canWrite: b, memberName: S, change: C } = f, w = f.tasks(() => ({
					dueBy: _(),
					status: "Open"
				})), T = p(() => (w.data.value ?? []).filter((e) => e.status === "Open")), D = p(() => T.value.slice(0, z)), O = (e) => y.data.value?.find((t) => t.id === e), k = p(() => c.place === "Wall"), A = C((e, t) => e.completeTask(t.id)), j = (e) => {
					let t = O(e.listId);
					return b.value && !!t && !t.archived;
				};
				return (n, c) => {
					let f = r("Skeleton");
					return a(), o("div", { class: i(["todo-widget", { "todo-widget--large": k.value }]) }, [e(w).isPending.value ? (a(), o("div", F, [(a(), o(h, null, t(3, (e) => s(f, {
						key: e,
						height: "2rem"
					})), 64))])) : (a(), o(h, { key: 1 }, [D.value.length ? (a(), o("ul", I, [(a(!0), o(h, null, t(D.value, (t) => (a(), u(E, {
						key: t.id,
						task: t,
						"list-name": O(t.listId)?.name ?? null,
						"assignee-name": e(S)(t.assigneeId),
						"can-toggle": j(t),
						large: k.value,
						onToggle: (n) => e(A).mutate(t)
					}, null, 8, [
						"task",
						"list-name",
						"assignee-name",
						"can-toggle",
						"large",
						"onToggle"
					]))), 128))])) : (a(), o("p", L, [c[0] ||= v("i", {
						class: "pi pi-check-circle",
						"aria-hidden": "true"
					}, null, -1), x(" " + d(e(g).t("empty.today")), 1)])), T.value.length > z ? (a(), o("p", R, d(e(g).t("more", { count: T.value.length - z })), 1)) : l("", !0)], 64))], 2);
				};
			}
		}), [["__scopeId", "data-v-dd1334e4"]]),
		"planner-agenda": P
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
			recurrence: {
				label: "Powtarzanie",
				none: "Nie powtarza się",
				"FREQ=DAILY": "Codziennie",
				"FREQ=WEEKLY": "Co tydzień",
				"FREQ=MONTHLY": "Co miesiąc",
				"FREQ=YEARLY": "Co rok",
				custom: "Własna reguła",
				anchorLabel: "Kolejny termin liczony od",
				FromDueDate: "terminu zadania",
				FromCompletionDate: "daty wykonania"
			},
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
					preferences: "Moje ustawienia",
					eventReminders: "Domyślne przypomnienia o wydarzeniach",
					taskReminders: "Domyślne przypomnienia o zadaniach",
					timeZone: "Strefa czasowa",
					firstDay: "Pierwszy dzień tygodnia",
					weekDay: {
						0: "Niedziela",
						1: "Poniedziałek",
						6: "Sobota"
					},
					title: "Ustawienia kalendarza",
					subscriptions: "Subskrypcje kalendarza (ICS)",
					subscriptionsHint: "Adres, który wklejasz do innej aplikacji kalendarza, żeby widzieć w niej wydarzenia domu.",
					noSubscriptions: "Brak subskrypcji.",
					label: "Nazwa subskrypcji",
					lastUsed: "ostatnio użyta",
					never: "nigdy",
					revoke: "Unieważnij subskrypcję {name}",
					confirmTitle: "Potwierdź",
					confirmRemove: "Usunąć „{name}”? Tej operacji nie można cofnąć.",
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
			recurrence: {
				label: "Repeat",
				none: "Does not repeat",
				"FREQ=DAILY": "Daily",
				"FREQ=WEEKLY": "Weekly",
				"FREQ=MONTHLY": "Monthly",
				"FREQ=YEARLY": "Yearly",
				custom: "Custom rule",
				anchorLabel: "Next due date counts from",
				FromDueDate: "the due date",
				FromCompletionDate: "the completion date"
			},
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
					preferences: "My settings",
					eventReminders: "Default event reminders",
					taskReminders: "Default task reminders",
					timeZone: "Time zone",
					firstDay: "First day of the week",
					weekDay: {
						0: "Sunday",
						1: "Monday",
						6: "Saturday"
					},
					title: "Calendar settings",
					subscriptions: "Calendar subscriptions (ICS)",
					subscriptionsHint: "An address you paste into another calendar app to see the household events there.",
					noSubscriptions: "No subscriptions.",
					label: "Subscription name",
					lastUsed: "last used",
					never: "never",
					revoke: "Revoke subscription {name}",
					confirmTitle: "Confirm",
					confirmRemove: "Remove \"{name}\"? This cannot be undone.",
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
export { B as default };
