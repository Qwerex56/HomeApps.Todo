import { A as e, C as t, D as n, E as r, F as i, I as a, M as o, N as s, O as c, P as l, S as u, T as d, _ as f, a as p, b as m, f as h, g, h as _, i as v, j as y, k as b, l as x, m as S, n as C, p as w, t as T, u as E, v as D, w as O, x as k, y as A } from "./_plugin-vue_export-helper-DRFww2dX.js";
import { a as j, c as M, i as N, l as P, o as F, r as I, t as L, u as R } from "./CalendarChip-CsI3k0aj.js";
import { t as z } from "./SeriesScopeDialog-B9qu5_34.js";
// src/AgendaList.vue?vue&type=script&setup=true&lang.ts
var B = { class: "agenda" }, V = {
	key: 0,
	class: "agenda__empty"
}, H = { class: "agenda__heading" }, ee = /*#__PURE__*/ T(/* @__PURE__ */ k({
	__name: "AgendaList",
	props: {
		days: {},
		items: {},
		canToggle: { type: Function }
	},
	emits: ["open", "toggle"],
	setup(e, { emit: t }) {
		let n = e, i = t, a = h(), o = S(() => n.days.map((e) => ({
			iso: x(e),
			label: e.toLocaleDateString("pl-PL", {
				weekday: "long",
				day: "numeric",
				month: "long"
			}),
			items: I(n.items, x(e))
		})).filter((e) => e.items.length > 0));
		return (t, n) => (O(), f("div", B, [o.value.length === 0 ? (O(), f("p", V, c(b(a).t("calendar.agendaEmpty")), 1)) : g("", !0), (O(!0), f(w, null, r(o.value, (t) => (O(), f("section", {
			key: t.iso,
			class: "agenda__day"
		}, [D("h3", H, c(t.label), 1), (O(!0), f(w, null, r(t.items, (t) => (O(), _(L, {
			key: `${t.itemType}-${t.id}-${t.occurrenceDate ?? ""}`,
			item: t,
			"can-toggle": e.canToggle(t),
			onOpen: (e) => i("open", t),
			onToggle: (e) => i("toggle", t)
		}, null, 8, [
			"item",
			"can-toggle",
			"onOpen",
			"onToggle"
		]))), 128))]))), 128))]));
	}
}), [["__scopeId", "data-v-869f9204"]]), U = ["aria-label"], W = { class: "settings__field" }, te = { class: "settings__field" }, ne = { class: "settings__field" }, re = { class: "settings__field" }, ie = ["aria-label"], ae = { class: "settings__hint" }, oe = {
	key: 2,
	class: "settings__list"
}, se = {
	key: 0,
	class: "settings__hint"
}, ce = { role: "status" }, le = { class: "settings__check" }, ue = ["aria-label"], de = {
	key: 2,
	class: "settings__list"
}, fe = {
	key: 0,
	class: "settings__hint"
}, G = ["aria-label"], K = /*#__PURE__*/ T(/* @__PURE__ */ k({
	__name: "CalendarSettingsDialog",
	props: {
		visible: { type: Boolean },
		householdId: {}
	},
	emits: ["update:visible"],
	setup(i, { emit: u }) {
		let v = i, x = u, T = h(), E = C(() => v.householdId), { api: k, change: j } = E, M = S(() => v.visible), N = p({
			queryKey: S(() => [
				"plugins",
				"todo",
				v.householdId,
				"subscriptions"
			]),
			queryFn: () => k.value.subscriptions(),
			enabled: M
		}), P = p({
			queryKey: S(() => [
				"plugins",
				"todo",
				v.householdId,
				"external"
			]),
			queryFn: () => k.value.externals(),
			enabled: M
		}), F = [
			0,
			10,
			30,
			60,
			1440
		], I = d({
			eventReminders: [],
			taskReminders: [],
			firstDayOfWeek: 1,
			timeZone: null
		});
		o(() => E.settings.data.value, (e) => e && (I.value = {
			...e,
			timeZone: e.timeZone ?? E.timeZone.value
		}), { immediate: !0 });
		let L = j((e, t) => e.savePlannerSettings(t)), R = F.map((e) => ({
			value: e,
			label: e === 0 ? T.t("calendar.reminder.atStart") : T.t("calendar.reminder.before", { minutes: e })
		})), z = S(() => {
			let e = Intl.supportedValuesOf?.("timeZone") ?? [];
			return I.value.timeZone && !e.includes(I.value.timeZone) ? [I.value.timeZone, ...e] : e;
		}), B = [
			1,
			0,
			6
		].map((e) => ({
			value: e,
			label: T.t(`calendar.settings.weekDay.${e}`)
		})), V = d(""), H = d("None"), ee = d(!1), K = d(null), q = j((e, t) => e.createSubscription(t)), pe = j((e, t) => e.revokeSubscription(t)), J = [
			"None",
			"Assigned",
			"All"
		].map((e) => ({
			value: e,
			label: T.t(`calendar.settings.tasksMode.${e}`)
		})), me = S(() => K.value ? `${window.location.origin}${K.value.feedPath}` : "");
		function he() {
			V.value.trim() && q.mutate({
				label: V.value.trim(),
				includeHouseholdEvents: !0,
				includePrivateEvents: ee.value,
				tasksMode: H.value
			}, { onSuccess: (e) => {
				K.value = e, V.value = "";
			} });
		}
		async function ge() {
			await navigator.clipboard?.writeText(me.value), T.notify("success", T.t("calendar.settings.copied"));
		}
		let Y = d(""), X = d(""), Z = d("#3b82f6"), Q = d("Busy"), _e = j((e, t) => e.addExternal(t)), ve = j((e, t) => e.syncExternal(t)), ye = j((e, t) => e.removeExternal(t)), $ = [
			"OnlyMe",
			"Busy",
			"Details"
		].map((e) => ({
			value: e,
			label: T.t(`calendar.settings.visibility.${e}`)
		}));
		function be() {
			Y.value.trim() && X.value.trim() && _e.mutate({
				url: Y.value.trim(),
				name: X.value.trim(),
				color: Z.value,
				visibility: Q.value
			}, { onSuccess: () => (Y.value = "", X.value = "") });
		}
		let xe = (e) => e ? new Date(e).toLocaleString("pl-PL") : T.t("calendar.settings.never");
		return (o, u) => {
			let d = n("MultiSelect"), p = n("Select"), h = n("Button"), v = n("Skeleton"), S = n("Message"), C = n("InputText"), E = n("Dialog");
			return O(), _(E, {
				visible: i.visible,
				modal: "",
				header: b(T).t("calendar.settings.title"),
				style: {
					width: "40rem",
					maxWidth: "calc(100vw - 2rem)"
				},
				"onUpdate:visible": u[12] ||= (e) => x("update:visible", e)
			}, {
				default: s(() => [
					D("section", {
						class: "settings",
						"aria-label": b(T).t("calendar.settings.preferences")
					}, [D("h3", null, c(b(T).t("calendar.settings.preferences")), 1), D("form", {
						class: "settings__form",
						onSubmit: u[4] ||= a((e) => b(L).mutate(I.value), ["prevent"])
					}, [
						D("label", W, [D("span", null, c(b(T).t("calendar.settings.eventReminders")), 1), m(d, {
							modelValue: I.value.eventReminders,
							"onUpdate:modelValue": u[0] ||= (e) => I.value.eventReminders = e,
							options: b(R),
							"option-label": "label",
							"option-value": "value",
							display: "chip"
						}, null, 8, ["modelValue", "options"])]),
						D("label", te, [D("span", null, c(b(T).t("calendar.settings.taskReminders")), 1), m(d, {
							modelValue: I.value.taskReminders,
							"onUpdate:modelValue": u[1] ||= (e) => I.value.taskReminders = e,
							options: b(R),
							"option-label": "label",
							"option-value": "value",
							display: "chip"
						}, null, 8, ["modelValue", "options"])]),
						D("label", ne, [D("span", null, c(b(T).t("calendar.settings.timeZone")), 1), m(p, {
							modelValue: I.value.timeZone,
							"onUpdate:modelValue": u[2] ||= (e) => I.value.timeZone = e,
							options: z.value,
							filter: ""
						}, null, 8, ["modelValue", "options"])]),
						D("label", re, [D("span", null, c(b(T).t("calendar.settings.firstDay")), 1), m(p, {
							modelValue: I.value.firstDayOfWeek,
							"onUpdate:modelValue": u[3] ||= (e) => I.value.firstDayOfWeek = e,
							options: b(B),
							"option-label": "label",
							"option-value": "value"
						}, null, 8, ["modelValue", "options"])]),
						m(h, {
							type: "submit",
							label: b(T).t("save"),
							loading: b(L).isPending.value
						}, null, 8, ["label", "loading"])
					], 32)], 8, U),
					D("section", {
						class: "settings",
						"aria-label": b(T).t("calendar.settings.subscriptions")
					}, [
						D("h3", null, c(b(T).t("calendar.settings.subscriptions")), 1),
						D("p", ae, c(b(T).t("calendar.settings.subscriptionsHint")), 1),
						b(N).isPending.value ? (O(), _(v, {
							key: 0,
							height: "2rem"
						})) : b(N).isError.value ? (O(), _(S, {
							key: 1,
							severity: "error"
						}, {
							default: s(() => [A(c(b(T).errorMessage(b(N).error.value)), 1)]),
							_: 1
						})) : (O(), f("ul", oe, [(O(!0), f(w, null, r(b(N).data.value, (e) => (O(), f("li", { key: e.id }, [D("span", null, [A(c(e.label) + " ", 1), D("small", null, "(" + c(b(T).t("calendar.settings.lastUsed")) + ": " + c(xe(e.lastAccessedAt)) + ")", 1)]), m(h, {
							icon: "pi pi-trash",
							text: "",
							severity: "danger",
							"aria-label": b(T).t("calendar.settings.revoke", { name: e.label }),
							onClick: (t) => b(pe).mutate(e.id)
						}, null, 8, ["aria-label", "onClick"])]))), 128)), b(N).data.value?.length ? g("", !0) : (O(), f("li", se, c(b(T).t("calendar.settings.noSubscriptions")), 1))])),
						K.value ? (O(), _(S, {
							key: 3,
							severity: "success",
							closable: !1
						}, {
							default: s(() => [D("div", ce, [
								D("p", null, c(b(T).t("calendar.settings.feedOnce")), 1),
								m(C, {
									"model-value": me.value,
									readonly: "",
									class: "settings__wide",
									"aria-label": b(T).t("calendar.settings.feedUrl")
								}, null, 8, ["model-value", "aria-label"]),
								m(h, {
									label: b(T).t("calendar.settings.copy"),
									icon: "pi pi-copy",
									text: "",
									onClick: ge
								}, null, 8, ["label"])
							])]),
							_: 1
						})) : g("", !0),
						D("form", {
							class: "settings__form",
							onSubmit: a(he, ["prevent"])
						}, [
							m(C, {
								modelValue: V.value,
								"onUpdate:modelValue": u[5] ||= (e) => V.value = e,
								maxlength: "100",
								required: "",
								placeholder: b(T).t("calendar.settings.label"),
								"aria-label": b(T).t("calendar.settings.label")
							}, null, 8, [
								"modelValue",
								"placeholder",
								"aria-label"
							]),
							m(p, {
								modelValue: H.value,
								"onUpdate:modelValue": u[6] ||= (e) => H.value = e,
								options: b(J),
								"option-label": "label",
								"option-value": "value",
								"aria-label": b(T).t("calendar.settings.tasksModeLabel")
							}, null, 8, [
								"modelValue",
								"options",
								"aria-label"
							]),
							D("label", le, [l(D("input", {
								"onUpdate:modelValue": u[7] ||= (e) => ee.value = e,
								type: "checkbox"
							}, null, 512), [[e, ee.value]]), A(" " + c(b(T).t("calendar.settings.includePrivate")), 1)]),
							m(h, {
								type: "submit",
								label: b(T).t("calendar.settings.addSubscription"),
								loading: b(q).isPending.value
							}, null, 8, ["label", "loading"])
						], 32)
					], 8, ie),
					D("section", {
						class: "settings",
						"aria-label": b(T).t("calendar.settings.external")
					}, [
						D("h3", null, c(b(T).t("calendar.settings.external")), 1),
						b(P).isPending.value ? (O(), _(v, {
							key: 0,
							height: "2rem"
						})) : b(P).isError.value ? (O(), _(S, {
							key: 1,
							severity: "error"
						}, {
							default: s(() => [A(c(b(T).errorMessage(b(P).error.value)), 1)]),
							_: 1
						})) : (O(), f("ul", de, [(O(!0), f(w, null, r(b(P).data.value, (e) => (O(), f("li", { key: e.id }, [D("span", null, [
							D("i", {
								class: "settings__dot",
								style: t({ background: e.color }),
								"aria-hidden": "true"
							}, null, 4),
							A(" " + c(e.name) + " ", 1),
							D("small", null, c(b(T).t(`calendar.settings.visibility.${e.visibility}`)), 1),
							e.isStale ? (O(), _(S, {
								key: 0,
								severity: "warn",
								closable: !1,
								role: "alert"
							}, {
								default: s(() => [A(c(b(T).t("calendar.settings.stale", { when: xe(e.lastSyncedAt) })) + " ", 1), e.lastErrorMessage ? (O(), f(w, { key: 0 }, [A(": " + c(e.lastErrorMessage), 1)], 64)) : g("", !0)]),
								_: 2
							}, 1024)) : g("", !0)
						]), D("span", null, [m(h, {
							icon: "pi pi-refresh",
							text: "",
							loading: b(ve).isPending.value,
							"aria-label": b(T).t("calendar.settings.sync", { name: e.name }),
							onClick: (t) => b(ve).mutate(e.id)
						}, null, 8, [
							"loading",
							"aria-label",
							"onClick"
						]), m(h, {
							icon: "pi pi-trash",
							text: "",
							severity: "danger",
							"aria-label": b(T).t("calendar.settings.remove", { name: e.name }),
							onClick: (t) => b(ye).mutate(e.id)
						}, null, 8, ["aria-label", "onClick"])])]))), 128)), b(P).data.value?.length ? g("", !0) : (O(), f("li", fe, c(b(T).t("calendar.settings.noExternal")), 1))])),
						D("form", {
							class: "settings__form",
							onSubmit: a(be, ["prevent"])
						}, [
							m(C, {
								modelValue: Y.value,
								"onUpdate:modelValue": u[8] ||= (e) => Y.value = e,
								type: "url",
								required: "",
								placeholder: b(T).t("calendar.settings.url"),
								"aria-label": b(T).t("calendar.settings.url")
							}, null, 8, [
								"modelValue",
								"placeholder",
								"aria-label"
							]),
							m(C, {
								modelValue: X.value,
								"onUpdate:modelValue": u[9] ||= (e) => X.value = e,
								maxlength: "100",
								required: "",
								placeholder: b(T).t("calendar.settings.name"),
								"aria-label": b(T).t("calendar.settings.name")
							}, null, 8, [
								"modelValue",
								"placeholder",
								"aria-label"
							]),
							l(D("input", {
								"onUpdate:modelValue": u[10] ||= (e) => Z.value = e,
								type: "color",
								"aria-label": b(T).t("calendar.settings.color"),
								class: "settings__color"
							}, null, 8, G), [[y, Z.value]]),
							m(p, {
								modelValue: Q.value,
								"onUpdate:modelValue": u[11] ||= (e) => Q.value = e,
								options: b($),
								"option-label": "label",
								"option-value": "value",
								"aria-label": b(T).t("calendar.settings.visibilityLabel")
							}, null, 8, [
								"modelValue",
								"options",
								"aria-label"
							]),
							m(h, {
								type: "submit",
								label: b(T).t("calendar.settings.addExternal"),
								loading: b(_e).isPending.value
							}, null, 8, ["label", "loading"])
						], 32)
					], 8, ue)
				]),
				_: 1
			}, 8, ["visible", "header"]);
		};
	}
}), [["__scopeId", "data-v-62e35e7a"]]), q = ["aria-label"], pe = {
	key: 0,
	class: "grid__head",
	role: "row"
}, J = [
	"tabindex",
	"aria-label",
	"aria-current",
	"onKeydown",
	"onDrop"
], me = { class: "cell__day" }, he = {
	key: 0,
	class: "cell__more"
}, ge = 3, Y = /*#__PURE__*/ T(/* @__PURE__ */ k({
	__name: "DayGrid",
	props: {
		days: {},
		items: {},
		month: {},
		canWrite: { type: Boolean },
		canToggle: { type: Function },
		columns: {}
	},
	emits: [
		"open",
		"toggle",
		"create",
		"drop"
	],
	setup(e, { emit: n }) {
		let o = e, s = n, l = h(), d = E(), p = S(() => o.days.map((e) => {
			let t = x(e), n = I(o.items, t), r = o.columns === 1 || o.month === null ? n.length : ge;
			return {
				iso: t,
				date: e,
				shown: n.slice(0, r),
				rest: n.length - r,
				dim: o.month !== null && e.getMonth() !== o.month,
				label: e.toLocaleDateString("pl-PL", {
					weekday: "long",
					day: "numeric",
					month: "long"
				})
			};
		})), m = S(() => o.days.slice(0, Math.min(7, o.days.length)).map((e) => e.toLocaleDateString("pl-PL", { weekday: "short" })));
		function v(e, t) {
			let n = {
				ArrowLeft: -1,
				ArrowRight: 1,
				ArrowUp: -o.columns,
				ArrowDown: o.columns
			}[e.key];
			n !== void 0 && (e.preventDefault(), (e.currentTarget.parentElement?.children[t + n])?.focus());
		}
		function y(e, t) {
			e.preventDefault();
			let n = e.dataTransfer?.getData("text/plain");
			n && o.canWrite && s("drop", n, t);
		}
		return (n, o) => (O(), f("div", {
			class: "grid",
			role: "grid",
			"aria-label": b(l).t("calendar.grid")
		}, [e.columns > 1 ? (O(), f("div", pe, [(O(!0), f(w, null, r(m.value, (e) => (O(), f("span", {
			key: e,
			role: "columnheader"
		}, c(e), 1))), 128))])) : g("", !0), D("div", {
			class: "grid__body",
			style: t({ gridTemplateColumns: `repeat(${e.columns}, minmax(0, 1fr))` })
		}, [(O(!0), f(w, null, r(p.value, (t, n) => (O(), f("div", {
			key: t.iso,
			class: u(["cell", {
				"cell--dim": t.dim,
				"cell--today": t.iso === b(d)
			}]),
			role: "gridcell",
			tabindex: n === 0 ? 0 : -1,
			"aria-label": t.label,
			"aria-current": t.iso === b(d) ? "date" : void 0,
			onKeydown: [(e) => v(e, n), i(a((n) => e.canWrite && s("create", t.iso), ["self"]), ["enter"])],
			onDragover: o[0] ||= a(() => {}, ["prevent"]),
			onDrop: (e) => y(e, t.iso)
		}, [
			D("span", me, c(t.date.getDate()), 1),
			(O(!0), f(w, null, r(t.shown, (t) => (O(), _(L, {
				key: `${t.itemType}-${t.id}-${t.occurrenceDate ?? ""}`,
				item: t,
				"can-toggle": e.canToggle(t),
				draggable: e.canWrite && t.itemType === "Task",
				onOpen: (e) => s("open", t),
				onToggle: (e) => s("toggle", t)
			}, null, 8, [
				"item",
				"can-toggle",
				"draggable",
				"onOpen",
				"onToggle"
			]))), 128)),
			t.rest > 0 ? (O(), f("span", he, c(b(l).t("more", { count: t.rest })), 1)) : g("", !0)
		], 42, J))), 128))], 4)], 8, q));
	}
}), [["__scopeId", "data-v-2ee0d671"]]), X = { class: "editor__field" }, Z = {
	key: 0,
	id: "ev-title-err",
	class: "editor__error",
	role: "alert"
}, Q = { class: "editor__check" }, _e = ["disabled"], ve = { class: "editor__row" }, ye = { class: "editor__field" }, $ = ["disabled"], be = {
	key: 0,
	class: "editor__field"
}, xe = { class: "sr-only" }, Se = ["disabled"], Ce = { class: "editor__row" }, we = { class: "editor__field" }, Te = ["disabled"], Ee = {
	key: 0,
	class: "editor__field"
}, De = { class: "sr-only" }, Oe = ["disabled"], ke = {
	key: 0,
	class: "editor__error",
	role: "alert"
}, Ae = { class: "editor__field" }, je = { class: "editor__field" }, Me = { class: "editor__check" }, Ne = ["disabled"], Pe = {
	key: 1,
	class: "editor__field"
}, Fe = {
	key: 2,
	class: "editor__hint"
}, Ie = {
	key: 3,
	class: "editor__group"
}, Le = ["value", "disabled"], Re = {
	key: 4,
	class: "editor__group"
}, ze = ["value", "disabled"], Be = { class: "editor__actions" }, Ve = /*#__PURE__*/ T(/* @__PURE__ */ k({
	__name: "EventEditorDrawer",
	props: {
		visible: { type: Boolean },
		event: {},
		day: {},
		timeZone: {},
		members: {},
		canEdit: { type: Boolean },
		canDelete: { type: Boolean },
		saving: { type: Boolean }
	},
	emits: [
		"update:visible",
		"save",
		"delete"
	],
	setup(t, { emit: i }) {
		let u = t, p = i, v = h(), x = [
			0,
			10,
			30,
			60,
			1440
		], C = [
			"",
			"FREQ=DAILY",
			"FREQ=WEEKLY",
			"FREQ=MONTHLY",
			"FREQ=YEARLY"
		];
		function T(e) {
			return {
				title: "",
				description: "",
				location: "",
				startDate: e,
				startTime: "09:00",
				endDate: e,
				endTime: "10:00",
				isAllDay: !1,
				isPrivate: !1,
				attendees: [],
				reminderMinutes: [],
				rrule: ""
			};
		}
		function E(e) {
			return {
				title: e.title,
				description: e.description ?? "",
				location: e.location ?? "",
				startDate: N(e.startUtc),
				startTime: j(e.startUtc),
				endDate: N(e.endUtc),
				endTime: j(e.endUtc),
				isAllDay: e.isAllDay,
				isPrivate: e.isPrivate,
				attendees: [...e.attendees],
				reminderMinutes: [...e.reminderMinutes],
				rrule: e.rrule ?? ""
			};
		}
		let k = d(T(u.day)), A = d(!1);
		o(() => [
			u.visible,
			u.event,
			u.day
		], ([e]) => {
			e && (k.value = u.event ? E(u.event) : T(u.day), A.value = !1);
		}, { immediate: !0 });
		let P = S(() => !!u.event?.seriesId), F = S(() => P.value || k.value.rrule !== ""), I = S(() => !u.canEdit), L = S(() => M(k.value.startDate, k.value.isAllDay ? "00:00" : k.value.startTime)), R = S(() => M(k.value.endDate, k.value.isAllDay ? "23:59" : k.value.endTime)), z = S(() => A.value && !k.value.title.trim() ? v.t("calendar.errors.title") : null), B = S(() => A.value && R.value < L.value ? v.t("calendar.errors.range") : null), V = C.map((e) => ({
			value: e,
			label: v.t(`calendar.repeat.${e || "none"}`)
		}));
		function H() {
			if (A.value = !0, !k.value.title.trim() || R.value < L.value) return;
			let e = k.value;
			p("save", {
				title: e.title.trim(),
				description: e.description.trim() || null,
				location: e.location.trim() || null,
				timeZoneId: u.event?.timeZoneId ?? u.timeZone,
				startUtc: L.value,
				endUtc: R.value,
				isAllDay: e.isAllDay,
				isPrivate: e.isPrivate,
				attendees: F.value ? [] : e.attendees,
				reminderMinutes: F.value ? [] : e.reminderMinutes,
				rrule: P.value ? null : e.rrule || null
			});
		}
		function ee(e) {
			return e === 0 ? v.t("calendar.reminder.atStart") : v.t("calendar.reminder.before", { minutes: e });
		}
		return (i, o) => {
			let u = n("InputText"), d = n("Textarea"), h = n("Select"), S = n("Button"), C = n("Drawer");
			return O(), _(C, {
				visible: t.visible,
				position: "right",
				header: b(v).t(t.event ? "calendar.editEvent" : "calendar.newEvent"),
				class: "editor-drawer",
				"onUpdate:visible": o[14] ||= (e) => p("update:visible", e)
			}, {
				default: s(() => [D("form", {
					class: "editor",
					novalidate: "",
					onSubmit: a(H, ["prevent"])
				}, [
					D("label", X, [
						D("span", null, c(b(v).t("fields.title")), 1),
						m(u, {
							modelValue: k.value.title,
							"onUpdate:modelValue": o[0] ||= (e) => k.value.title = e,
							maxlength: "200",
							required: "",
							disabled: I.value,
							invalid: !!z.value,
							"aria-describedby": z.value ? "ev-title-err" : void 0
						}, null, 8, [
							"modelValue",
							"disabled",
							"invalid",
							"aria-describedby"
						]),
						z.value ? (O(), f("small", Z, c(z.value), 1)) : g("", !0)
					]),
					D("label", Q, [l(D("input", {
						"onUpdate:modelValue": o[1] ||= (e) => k.value.isAllDay = e,
						type: "checkbox",
						disabled: I.value
					}, null, 8, _e), [[e, k.value.isAllDay]]), D("span", null, c(b(v).t("calendar.allDay")), 1)]),
					D("div", ve, [D("label", ye, [D("span", null, c(b(v).t("calendar.start")), 1), l(D("input", {
						"onUpdate:modelValue": o[2] ||= (e) => k.value.startDate = e,
						type: "date",
						class: "editor__input",
						required: "",
						disabled: I.value
					}, null, 8, $), [[y, k.value.startDate]])]), k.value.isAllDay ? g("", !0) : (O(), f("label", be, [D("span", xe, c(b(v).t("fields.time")), 1), l(D("input", {
						"onUpdate:modelValue": o[3] ||= (e) => k.value.startTime = e,
						type: "time",
						class: "editor__input",
						required: "",
						disabled: I.value
					}, null, 8, Se), [[y, k.value.startTime]])]))]),
					D("div", Ce, [D("label", we, [D("span", null, c(b(v).t("calendar.end")), 1), l(D("input", {
						"onUpdate:modelValue": o[4] ||= (e) => k.value.endDate = e,
						type: "date",
						class: "editor__input",
						required: "",
						disabled: I.value
					}, null, 8, Te), [[y, k.value.endDate]])]), k.value.isAllDay ? g("", !0) : (O(), f("label", Ee, [D("span", De, c(b(v).t("fields.time")), 1), l(D("input", {
						"onUpdate:modelValue": o[5] ||= (e) => k.value.endTime = e,
						type: "time",
						class: "editor__input",
						required: "",
						disabled: I.value
					}, null, 8, Oe), [[y, k.value.endTime]])]))]),
					B.value ? (O(), f("small", ke, c(B.value), 1)) : g("", !0),
					D("label", Ae, [D("span", null, c(b(v).t("calendar.location")), 1), m(u, {
						modelValue: k.value.location,
						"onUpdate:modelValue": o[6] ||= (e) => k.value.location = e,
						maxlength: "500",
						disabled: I.value
					}, null, 8, ["modelValue", "disabled"])]),
					D("label", je, [D("span", null, c(b(v).t("fields.notes")), 1), m(d, {
						modelValue: k.value.description,
						"onUpdate:modelValue": o[7] ||= (e) => k.value.description = e,
						rows: "3",
						"auto-resize": "",
						maxlength: "4000",
						disabled: I.value
					}, null, 8, ["modelValue", "disabled"])]),
					D("label", Me, [l(D("input", {
						"onUpdate:modelValue": o[8] ||= (e) => k.value.isPrivate = e,
						type: "checkbox",
						disabled: I.value
					}, null, 8, Ne), [[e, k.value.isPrivate]]), D("span", null, c(b(v).t("calendar.private")), 1)]),
					P.value ? (O(), f("p", Fe, c(b(v).t("calendar.repeat.series")), 1)) : (O(), f("label", Pe, [D("span", null, c(b(v).t("calendar.repeat.label")), 1), m(h, {
						modelValue: k.value.rrule,
						"onUpdate:modelValue": o[9] ||= (e) => k.value.rrule = e,
						options: b(V),
						"option-label": "label",
						"option-value": "value",
						disabled: I.value || !!t.event
					}, null, 8, [
						"modelValue",
						"options",
						"disabled"
					])])),
					!F.value && t.members.length ? (O(), f("fieldset", Ie, [D("legend", null, c(b(v).t("calendar.attendees")), 1), (O(!0), f(w, null, r(t.members, (t) => (O(), f("label", {
						key: t.userId,
						class: "editor__check"
					}, [l(D("input", {
						"onUpdate:modelValue": o[10] ||= (e) => k.value.attendees = e,
						type: "checkbox",
						value: t.userId,
						disabled: I.value
					}, null, 8, Le), [[e, k.value.attendees]]), D("span", null, c(t.displayName), 1)]))), 128))])) : g("", !0),
					F.value ? g("", !0) : (O(), f("fieldset", Re, [D("legend", null, c(b(v).t("calendar.reminder.label")), 1), (O(), f(w, null, r(x, (t) => D("label", {
						key: t,
						class: "editor__check"
					}, [l(D("input", {
						"onUpdate:modelValue": o[11] ||= (e) => k.value.reminderMinutes = e,
						type: "checkbox",
						value: t,
						disabled: I.value
					}, null, 8, ze), [[e, k.value.reminderMinutes]]), D("span", null, c(ee(t)), 1)])), 64))])),
					D("div", Be, [
						t.event && t.canDelete ? (O(), _(S, {
							key: 0,
							type: "button",
							label: b(v).t("delete"),
							severity: "danger",
							text: "",
							class: "editor__button",
							onClick: o[12] ||= (e) => p("delete")
						}, null, 8, ["label"])) : g("", !0),
						o[15] ||= D("span", { class: "editor__spacer" }, null, -1),
						m(S, {
							type: "button",
							label: b(v).t("cancel"),
							severity: "secondary",
							text: "",
							class: "editor__button",
							onClick: o[13] ||= (e) => p("update:visible", !1)
						}, null, 8, ["label"]),
						t.canEdit ? (O(), _(S, {
							key: 1,
							type: "submit",
							label: b(v).t(t.event ? "save" : "create"),
							loading: t.saving,
							class: "editor__button"
						}, null, 8, ["label", "loading"])) : g("", !0)
					])
				], 32)]),
				_: 1
			}, 8, ["visible", "header"]);
		};
	}
}), [["__scopeId", "data-v-1b5f9771"]]), He = { class: "planner" }, Ue = { class: "planner__bar" }, We = ["aria-label"], Ge = {
	class: "planner__title",
	"aria-live": "polite"
}, Ke = { class: "planner__layers" }, qe = { class: "planner__legend" }, Je = ["value"], Ye = {
	key: 0,
	class: "planner__hint"
}, Xe = {
	key: 1,
	class: "planner__loading",
	"aria-busy": "true"
}, Ze = {
	key: 3,
	class: "planner__empty"
}, Qe = /*#__PURE__*/ T(/* @__PURE__ */ k({
	__name: "CalendarPage",
	setup(t) {
		let i = h(), a = S(() => i.household.value.id), o = C(a), { api: u, userId: y, canWrite: x, canManage: T, members: k, change: j } = o, M = [
			"month",
			"week",
			"day",
			"agenda"
		], N = d(window.matchMedia("(max-width: 640px)").matches ? "agenda" : "month"), I = d(/* @__PURE__ */ new Date()), L = d([
			"events",
			"private",
			"tasks",
			"external"
		]), B = d(null), V = S(() => P(N.value, I.value)), H = S(() => R(N.value, I.value)), U = o.calendar(H, L), W = S(() => {
			let e = U.data.value ?? [], t = B.value;
			return t ? e.filter((e) => e.createdBy === t || e.attendees.includes(t)) : e;
		}), te = S(() => {
			let e = I.value;
			if (N.value === "month") return e.toLocaleDateString("pl-PL", {
				month: "long",
				year: "numeric"
			});
			if (N.value === "day") return e.toLocaleDateString("pl-PL", {
				weekday: "long",
				day: "numeric",
				month: "long",
				year: "numeric"
			});
			let t = V.value[0], n = V.value[V.value.length - 1], r = (e) => e.toLocaleDateString("pl-PL", {
				day: "numeric",
				month: "short"
			});
			return `${r(t)} – ${r(n)}`;
		}), ne = S(() => N.value === "day" ? 1 : 7), re = S(() => M.map((e) => ({
			value: e,
			label: i.t(`calendar.views.${e}`)
		}))), ie = S(() => [
			"events",
			"private",
			"tasks",
			"external"
		].map((e) => ({
			value: e,
			label: i.t(`calendar.layers.${e}`)
		}))), ae = S(() => [{
			userId: null,
			displayName: i.t("calendar.everyone")
		}, ...(k.data.value ?? []).map((e) => ({
			userId: e.userId,
			displayName: e.displayName
		}))]), oe = p({
			queryKey: S(() => [
				"plugins",
				"todo",
				a.value,
				"external"
			]),
			queryFn: () => u.value.externals(),
			enabled: S(() => L.value.includes("external"))
		}), se = S(() => (oe.data.value ?? []).filter((e) => e.isStale)), ce = (e) => e ? new Date(e).toLocaleString("pl-PL") : i.t("calendar.settings.never");
		function le(e) {
			I.value = F(N.value, I.value, e);
		}
		let ue = (e) => x.value && e.itemType === "Task", de = j((e, t) => t.isDone ? e.reopenTask(t.id) : e.completeTask(t.id)), fe = j(async (e, { id: t, day: n }) => {
			let r = W.value.find((e) => e.itemType === "Task" && e.id === t);
			if (!r?.date || r.date === n) return;
			let i = (await e.tasks({
				dueBy: r.date,
				status: "Open"
			})).find((e) => e.id === t);
			i && await e.updateTask(t, {
				...v(i),
				dueDate: n
			});
		}), G = d(!1), q = d(null), pe = d(E()), J = d(null), me = S(() => x.value && (!q.value || q.value.createdBy === y.value || T.value));
		function he(e = E()) {
			q.value = null, pe.value = e, G.value = !0;
		}
		async function ge(e) {
			if (e.itemType === "Task") ue(e) && de.mutate(e);
			else if (e.itemType !== "External") try {
				let t = await u.value.event(e.id);
				q.value = e.isOccurrence ? {
					...t,
					occurrenceDate: e.occurrenceDate,
					title: e.title,
					description: e.description,
					location: e.location,
					startUtc: e.startUtc,
					endUtc: e.endUtc
				} : t, G.value = !0;
			} catch (e) {
				i.notify("error", i.errorMessage(e));
			}
		}
		let X = j((e, t) => e.createEvent(t)), Z = j((e, { id: t, input: n }) => e.updateEvent(t, n)), Q = j((e, t) => e.modifyInstance(t.seriesId, t.date, {
			scope: t.scope,
			title: t.input.title,
			description: t.input.description,
			location: t.input.location,
			startUtc: t.input.startUtc,
			endUtc: t.input.endUtc
		})), _e = j((e, t) => e.deleteEvent(t)), ve = j((e, t) => e.deleteInstance(t.seriesId, t.date, t.scope)), ye = S(() => X.isPending.value || Z.isPending.value || Q.isPending.value), $ = { onSuccess: () => G.value = !1 };
		function be(e) {
			let t = q.value;
			t ? t.seriesId ? J.value = {
				mode: "edit",
				input: e
			} : Z.mutate({
				id: t.id,
				input: e
			}, $) : X.mutate(e, $);
		}
		function xe() {
			let e = q.value;
			e && (e.seriesId ? J.value = { mode: "delete" } : window.confirm(i.t("calendar.deleteConfirm", { name: e.title })) && _e.mutate(e.id, $));
		}
		function Se(e) {
			let t = q.value, n = J.value;
			if (J.value = null, !t?.seriesId || !t.occurrenceDate || !n) return;
			let r = t.occurrenceDate;
			n.mode === "delete" ? ve.mutate({
				seriesId: t.seriesId,
				date: r,
				scope: e
			}, $) : Q.mutate({
				seriesId: t.seriesId,
				date: r,
				scope: e,
				input: n.input
			}, $);
		}
		let Ce = d(!1);
		return (t, u) => {
			let d = n("Button"), p = n("SelectButton"), h = n("Select"), v = n("Message"), y = n("Skeleton");
			return O(), f("div", He, [
				D("header", Ue, [
					D("div", {
						class: "planner__nav",
						role: "group",
						"aria-label": b(i).t("calendar.navigate")
					}, [
						m(d, {
							icon: "pi pi-chevron-left",
							text: "",
							"aria-label": b(i).t("calendar.previous"),
							class: "planner__touch",
							onClick: u[0] ||= (e) => le(-1)
						}, null, 8, ["aria-label"]),
						m(d, {
							label: b(i).t("calendar.today"),
							text: "",
							class: "planner__touch",
							onClick: u[1] ||= (e) => I.value = /* @__PURE__ */ new Date()
						}, null, 8, ["label"]),
						m(d, {
							icon: "pi pi-chevron-right",
							text: "",
							"aria-label": b(i).t("calendar.next"),
							class: "planner__touch",
							onClick: u[2] ||= (e) => le(1)
						}, null, 8, ["aria-label"])
					], 8, We),
					D("h2", Ge, c(te.value), 1),
					m(p, {
						modelValue: N.value,
						"onUpdate:modelValue": u[3] ||= (e) => N.value = e,
						options: re.value,
						"option-label": "label",
						"option-value": "value",
						"allow-empty": !1,
						"aria-label": b(i).t("calendar.viewSwitch")
					}, null, 8, [
						"modelValue",
						"options",
						"aria-label"
					]),
					b(k).data.value?.length ? (O(), _(h, {
						key: 0,
						modelValue: B.value,
						"onUpdate:modelValue": u[4] ||= (e) => B.value = e,
						options: ae.value,
						"option-label": "displayName",
						"option-value": "userId",
						"aria-label": b(i).t("calendar.person")
					}, null, 8, [
						"modelValue",
						"options",
						"aria-label"
					])) : g("", !0),
					b(x) ? (O(), _(d, {
						key: 1,
						icon: "pi pi-plus",
						label: b(i).t("calendar.newEvent"),
						class: "planner__touch",
						onClick: u[5] ||= (e) => he()
					}, null, 8, ["label"])) : g("", !0),
					m(d, {
						icon: "pi pi-cog",
						text: "",
						"aria-label": b(i).t("calendar.settings.title"),
						class: "planner__touch",
						onClick: u[6] ||= (e) => Ce.value = !0
					}, null, 8, ["aria-label"])
				]),
				D("fieldset", Ke, [D("legend", qe, c(b(i).t("calendar.layersLabel")), 1), (O(!0), f(w, null, r(ie.value, (t) => (O(), f("label", {
					key: t.value,
					class: "planner__layer"
				}, [l(D("input", {
					"onUpdate:modelValue": u[7] ||= (e) => L.value = e,
					type: "checkbox",
					value: t.value
				}, null, 8, Je), [[e, L.value]]), A(" " + c(t.label), 1)]))), 128))]),
				(O(!0), f(w, null, r(se.value, (e) => (O(), _(v, {
					key: e.id,
					severity: "warn",
					closable: !1,
					role: "alert"
				}, {
					default: s(() => [A(c(b(i).t("calendar.settings.stale", { when: ce(e.lastSyncedAt) })) + " (" + c(e.name) + ") ", 1)]),
					_: 2
				}, 1024))), 128)),
				b(x) ? g("", !0) : (O(), f("p", Ye, c(b(i).t("calendar.readOnly")), 1)),
				b(U).isPending.value ? (O(), f("div", Xe, [(O(!0), f(w, null, r(N.value === "agenda" ? 5 : 6, (e) => (O(), _(y, {
					key: e,
					height: N.value === "agenda" ? "3rem" : "6rem"
				}, null, 8, ["height"]))), 128))])) : b(U).isError.value ? (O(), _(v, {
					key: 2,
					severity: "error",
					role: "alert"
				}, {
					default: s(() => [A(c(b(i).errorMessage(b(U).error.value)) + " ", 1), m(d, {
						label: b(i).t("calendar.retry"),
						text: "",
						onClick: u[8] ||= (e) => b(U).refetch()
					}, null, 8, ["label"])]),
					_: 1
				})) : W.value.length === 0 && !B.value ? (O(), f("div", Ze, [
					u[14] ||= D("i", {
						class: "pi pi-calendar",
						"aria-hidden": "true"
					}, null, -1),
					D("p", null, c(b(i).t("calendar.empty")), 1),
					b(x) ? (O(), _(d, {
						key: 0,
						label: b(i).t("calendar.addFirst"),
						onClick: u[9] ||= (e) => he()
					}, null, 8, ["label"])) : g("", !0)
				])) : N.value === "agenda" ? (O(), _(ee, {
					key: 4,
					days: V.value,
					items: W.value,
					"can-toggle": ue,
					onOpen: ge,
					onToggle: b(de).mutate
				}, null, 8, [
					"days",
					"items",
					"onToggle"
				])) : (O(), _(Y, {
					key: 5,
					days: V.value,
					items: W.value,
					month: N.value === "month" ? I.value.getMonth() : null,
					columns: ne.value,
					"can-write": b(x),
					"can-toggle": ue,
					onOpen: ge,
					onToggle: b(de).mutate,
					onCreate: he,
					onDrop: u[10] ||= (e, t) => b(fe).mutate({
						id: e,
						day: t
					})
				}, null, 8, [
					"days",
					"items",
					"month",
					"columns",
					"can-write",
					"onToggle"
				])),
				m(Ve, {
					visible: G.value,
					"onUpdate:visible": u[11] ||= (e) => G.value = e,
					event: q.value,
					day: pe.value,
					"time-zone": b(o).timeZone.value,
					members: b(k).data.value ?? [],
					"can-edit": me.value,
					"can-delete": me.value,
					saving: ye.value,
					onSave: be,
					onDelete: xe
				}, null, 8, [
					"visible",
					"event",
					"day",
					"time-zone",
					"members",
					"can-edit",
					"can-delete",
					"saving"
				]),
				m(z, {
					visible: J.value !== null,
					mode: J.value?.mode ?? "edit",
					"onUpdate:visible": u[12] ||= (e) => J.value = null,
					onChoose: Se
				}, null, 8, ["visible", "mode"]),
				m(K, {
					visible: Ce.value,
					"onUpdate:visible": u[13] ||= (e) => Ce.value = e,
					"household-id": a.value
				}, null, 8, ["visible", "household-id"])
			]);
		};
	}
}), [["__scopeId", "data-v-64118055"]]);
export { Qe as default };
