import { A as e, C as t, D as n, E as r, F as i, I as a, L as o, M as s, N as c, O as l, P as u, S as d, T as f, _ as p, a as m, b as h, f as g, g as _, h as v, i as y, j as b, k as x, l as S, m as C, n as w, p as T, t as E, u as D, v as O, x as k, y as A } from "./_plugin-vue_export-helper-B-kgh5nT.js";
import { a as j, c as ee, i as M, l as N, o as P, r as F, t as I, u as L } from "./CalendarChip-0N5siCjS.js";
import { t as R } from "./SeriesScopeDialog-CopcsF3w.js";
// src/AgendaList.vue?vue&type=script&setup=true&lang.ts
var z = { class: "agenda" }, B = {
	key: 0,
	class: "agenda__empty"
}, V = { class: "agenda__heading" }, H = /*#__PURE__*/ E(/* @__PURE__ */ k({
	__name: "AgendaList",
	props: {
		days: {},
		items: {},
		canToggle: { type: Function }
	},
	emits: ["open", "toggle"],
	setup(t, { emit: r }) {
		let i = t, a = r, o = g(), s = C(() => i.days.map((e) => ({
			iso: S(e),
			label: e.toLocaleDateString("pl-PL", {
				weekday: "long",
				day: "numeric",
				month: "long"
			}),
			items: F(i.items, S(e))
		})).filter((e) => e.items.length > 0));
		return (r, i) => (f(), p("div", z, [s.value.length === 0 ? (f(), p("p", B, x(e(o).t("calendar.agendaEmpty")), 1)) : _("", !0), (f(!0), p(T, null, n(s.value, (e) => (f(), p("section", {
			key: e.iso,
			class: "agenda__day"
		}, [O("h3", V, x(e.label), 1), (f(!0), p(T, null, n(e.items, (e) => (f(), v(I, {
			key: `${e.itemType}-${e.id}-${e.occurrenceDate ?? ""}`,
			item: e,
			"can-toggle": t.canToggle(e),
			onOpen: (t) => a("open", e),
			onToggle: (t) => a("toggle", e)
		}, null, 8, [
			"item",
			"can-toggle",
			"onOpen",
			"onToggle"
		]))), 128))]))), 128))]));
	}
}), [["__scopeId", "data-v-53e744d4"]]), te = ["aria-label"], U = { class: "settings__field" }, W = { class: "settings__field" }, ne = { class: "settings__field" }, re = { class: "settings__field" }, ie = ["aria-label"], ae = { class: "settings__hint" }, oe = {
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
}, pe = ["aria-label"], me = /*#__PURE__*/ E(/* @__PURE__ */ k({
	__name: "CalendarSettingsDialog",
	props: {
		visible: { type: Boolean },
		householdId: {}
	},
	emits: ["update:visible"],
	setup(a, { emit: d }) {
		let y = a, S = d, E = g(), D = w(() => y.householdId), { api: k, change: j } = D, ee = C(() => y.visible), M = m({
			queryKey: C(() => [
				"plugins",
				"todo",
				y.householdId,
				"subscriptions"
			]),
			queryFn: () => k.value.subscriptions(),
			enabled: ee
		}), N = m({
			queryKey: C(() => [
				"plugins",
				"todo",
				y.householdId,
				"external"
			]),
			queryFn: () => k.value.externals(),
			enabled: C(() => ee.value && D.canWrite.value)
		}), P = [
			0,
			10,
			30,
			60,
			1440
		], F = r({
			eventReminders: [],
			taskReminders: [],
			firstDayOfWeek: 1,
			timeZone: null
		});
		c(() => D.settings.data.value, (e) => e && (F.value = {
			...e,
			timeZone: e.timeZone ?? D.timeZone.value
		}), { immediate: !0 });
		let I = j((e, t) => e.savePlannerSettings(t)), L = P.map((e) => ({
			value: e,
			label: e === 0 ? E.t("calendar.reminder.atStart") : E.t("calendar.reminder.before", { minutes: e })
		})), R = C(() => {
			let e = Intl.supportedValuesOf?.("timeZone") ?? [];
			return F.value.timeZone && !e.includes(F.value.timeZone) ? [F.value.timeZone, ...e] : e;
		}), z = [
			1,
			0,
			6
		].map((e) => ({
			value: e,
			label: E.t(`calendar.settings.weekDay.${e}`)
		})), B = r(""), V = r("None"), H = r(!1), me = r(null), G = j((e, t) => e.createSubscription(t)), K = j((e, t) => e.revokeSubscription(t)), he = [
			"None",
			"Assigned",
			"All"
		].map((e) => ({
			value: e,
			label: E.t(`calendar.settings.tasksMode.${e}`)
		})), q = C(() => me.value ? `${window.location.origin}${me.value.feedPath}` : "");
		function ge() {
			B.value.trim() && G.mutate({
				label: B.value.trim(),
				includeHouseholdEvents: !0,
				includePrivateEvents: H.value,
				tasksMode: V.value
			}, { onSuccess: (e) => {
				me.value = e, B.value = "";
			} });
		}
		async function _e() {
			await navigator.clipboard?.writeText(q.value), E.notify("success", E.t("calendar.settings.copied"));
		}
		let J = r(null), Y = C({
			get: () => J.value !== null,
			set: (e) => !e && (J.value = null)
		});
		function ve() {
			let e = J.value;
			J.value = null, e && (e.kind === "subscription" ? K.mutate(e.id) : $.mutate(e.id));
		}
		let X = r(""), Z = r(""), Q = r("#3b82f6"), ye = r("Busy"), be = j((e, t) => e.addExternal(t)), xe = j((e, t) => e.syncExternal(t)), $ = j((e, t) => e.removeExternal(t)), Se = [
			"OnlyMe",
			"Busy",
			"Details"
		].map((e) => ({
			value: e,
			label: E.t(`calendar.settings.visibility.${e}`)
		}));
		function Ce() {
			X.value.trim() && Z.value.trim() && be.mutate({
				url: X.value.trim(),
				name: Z.value.trim(),
				color: Q.value,
				visibility: ye.value
			}, { onSuccess: () => (X.value = "", Z.value = "") });
		}
		let we = (e) => e ? new Date(e).toLocaleString("pl-PL") : E.t("calendar.settings.never");
		return (r, c) => {
			let d = l("MultiSelect"), m = l("Select"), g = l("Button"), y = l("Skeleton"), C = l("Message"), w = l("InputText"), k = l("Dialog");
			return f(), v(k, {
				visible: a.visible,
				modal: "",
				header: e(E).t("calendar.settings.title"),
				style: {
					width: "40rem",
					maxWidth: "calc(100vw - 2rem)"
				},
				"onUpdate:visible": c[14] ||= (e) => S("update:visible", e)
			}, {
				default: u(() => [
					O("section", {
						class: "settings",
						"aria-label": e(E).t("calendar.settings.preferences")
					}, [O("h3", null, x(e(E).t("calendar.settings.preferences")), 1), O("form", {
						class: "settings__form",
						onSubmit: c[4] ||= o((t) => e(I).mutate(F.value), ["prevent"])
					}, [
						O("label", U, [O("span", null, x(e(E).t("calendar.settings.eventReminders")), 1), h(d, {
							modelValue: F.value.eventReminders,
							"onUpdate:modelValue": c[0] ||= (e) => F.value.eventReminders = e,
							options: e(L),
							"option-label": "label",
							"option-value": "value",
							display: "chip"
						}, null, 8, ["modelValue", "options"])]),
						O("label", W, [O("span", null, x(e(E).t("calendar.settings.taskReminders")), 1), h(d, {
							modelValue: F.value.taskReminders,
							"onUpdate:modelValue": c[1] ||= (e) => F.value.taskReminders = e,
							options: e(L),
							"option-label": "label",
							"option-value": "value",
							display: "chip"
						}, null, 8, ["modelValue", "options"])]),
						O("label", ne, [O("span", null, x(e(E).t("calendar.settings.timeZone")), 1), h(m, {
							modelValue: F.value.timeZone,
							"onUpdate:modelValue": c[2] ||= (e) => F.value.timeZone = e,
							options: R.value,
							filter: ""
						}, null, 8, ["modelValue", "options"])]),
						O("label", re, [O("span", null, x(e(E).t("calendar.settings.firstDay")), 1), h(m, {
							modelValue: F.value.firstDayOfWeek,
							"onUpdate:modelValue": c[3] ||= (e) => F.value.firstDayOfWeek = e,
							options: e(z),
							"option-label": "label",
							"option-value": "value"
						}, null, 8, ["modelValue", "options"])]),
						h(g, {
							type: "submit",
							label: e(E).t("save"),
							loading: e(I).isPending.value
						}, null, 8, ["label", "loading"])
					], 32)], 8, te),
					O("section", {
						class: "settings",
						"aria-label": e(E).t("calendar.settings.subscriptions")
					}, [
						O("h3", null, x(e(E).t("calendar.settings.subscriptions")), 1),
						O("p", ae, x(e(E).t("calendar.settings.subscriptionsHint")), 1),
						e(M).isPending.value ? (f(), v(y, {
							key: 0,
							height: "2rem"
						})) : e(M).isError.value ? (f(), v(C, {
							key: 1,
							severity: "error"
						}, {
							default: u(() => [A(x(e(E).errorMessage(e(M).error.value)), 1)]),
							_: 1
						})) : (f(), p("ul", oe, [(f(!0), p(T, null, n(e(M).data.value, (t) => (f(), p("li", { key: t.id }, [O("span", null, [A(x(t.label) + " ", 1), O("small", null, "(" + x(e(E).t("calendar.settings.lastUsed")) + ": " + x(we(t.lastAccessedAt)) + ")", 1)]), h(g, {
							icon: "pi pi-trash",
							text: "",
							severity: "danger",
							"aria-label": e(E).t("calendar.settings.revoke", { name: t.label }),
							onClick: (e) => J.value = {
								kind: "subscription",
								id: t.id,
								name: t.label
							}
						}, null, 8, ["aria-label", "onClick"])]))), 128)), e(M).data.value?.length ? _("", !0) : (f(), p("li", se, x(e(E).t("calendar.settings.noSubscriptions")), 1))])),
						me.value ? (f(), v(C, {
							key: 3,
							severity: "success",
							closable: !1
						}, {
							default: u(() => [O("div", ce, [
								O("p", null, x(e(E).t("calendar.settings.feedOnce")), 1),
								h(w, {
									"model-value": q.value,
									readonly: "",
									class: "settings__wide",
									"aria-label": e(E).t("calendar.settings.feedUrl")
								}, null, 8, ["model-value", "aria-label"]),
								h(g, {
									label: e(E).t("calendar.settings.copy"),
									icon: "pi pi-copy",
									text: "",
									onClick: _e
								}, null, 8, ["label"])
							])]),
							_: 1
						})) : _("", !0),
						O("form", {
							class: "settings__form",
							onSubmit: o(ge, ["prevent"])
						}, [
							h(w, {
								modelValue: B.value,
								"onUpdate:modelValue": c[5] ||= (e) => B.value = e,
								maxlength: "100",
								required: "",
								placeholder: e(E).t("calendar.settings.label"),
								"aria-label": e(E).t("calendar.settings.label")
							}, null, 8, [
								"modelValue",
								"placeholder",
								"aria-label"
							]),
							h(m, {
								modelValue: V.value,
								"onUpdate:modelValue": c[6] ||= (e) => V.value = e,
								options: e(he),
								"option-label": "label",
								"option-value": "value",
								"aria-label": e(E).t("calendar.settings.tasksModeLabel")
							}, null, 8, [
								"modelValue",
								"options",
								"aria-label"
							]),
							O("label", le, [i(O("input", {
								"onUpdate:modelValue": c[7] ||= (e) => H.value = e,
								type: "checkbox"
							}, null, 512), [[b, H.value]]), A(" " + x(e(E).t("calendar.settings.includePrivate")), 1)]),
							h(g, {
								type: "submit",
								label: e(E).t("calendar.settings.addSubscription"),
								loading: e(G).isPending.value
							}, null, 8, ["label", "loading"])
						], 32)
					], 8, ie),
					e(D).canWrite.value ? (f(), p("section", {
						key: 0,
						class: "settings",
						"aria-label": e(E).t("calendar.settings.external")
					}, [
						O("h3", null, x(e(E).t("calendar.settings.external")), 1),
						e(N).isPending.value ? (f(), v(y, {
							key: 0,
							height: "2rem"
						})) : e(N).isError.value ? (f(), v(C, {
							key: 1,
							severity: "error"
						}, {
							default: u(() => [A(x(e(E).errorMessage(e(N).error.value)), 1)]),
							_: 1
						})) : (f(), p("ul", de, [(f(!0), p(T, null, n(e(N).data.value, (n) => (f(), p("li", { key: n.id }, [O("span", null, [
							O("i", {
								class: "settings__dot",
								style: t({ background: n.color }),
								"aria-hidden": "true"
							}, null, 4),
							A(" " + x(n.name) + " ", 1),
							O("small", null, x(e(E).t(`calendar.settings.visibility.${n.visibility}`)), 1),
							n.isStale ? (f(), v(C, {
								key: 0,
								severity: "warn",
								closable: !1,
								role: "alert"
							}, {
								default: u(() => [A(x(e(E).t("calendar.settings.stale", { when: we(n.lastSyncedAt) })) + " ", 1), n.lastErrorMessage ? (f(), p(T, { key: 0 }, [A(": " + x(n.lastErrorMessage), 1)], 64)) : _("", !0)]),
								_: 2
							}, 1024)) : _("", !0)
						]), O("span", null, [h(g, {
							icon: "pi pi-refresh",
							text: "",
							loading: e(xe).isPending.value,
							"aria-label": e(E).t("calendar.settings.sync", { name: n.name }),
							onClick: (t) => e(xe).mutate(n.id)
						}, null, 8, [
							"loading",
							"aria-label",
							"onClick"
						]), h(g, {
							icon: "pi pi-trash",
							text: "",
							severity: "danger",
							"aria-label": e(E).t("calendar.settings.remove", { name: n.name }),
							onClick: (e) => J.value = {
								kind: "external",
								id: n.id,
								name: n.name
							}
						}, null, 8, ["aria-label", "onClick"])])]))), 128)), e(N).data.value?.length ? _("", !0) : (f(), p("li", fe, x(e(E).t("calendar.settings.noExternal")), 1))])),
						O("form", {
							class: "settings__form",
							onSubmit: o(Ce, ["prevent"])
						}, [
							h(w, {
								modelValue: X.value,
								"onUpdate:modelValue": c[8] ||= (e) => X.value = e,
								type: "url",
								required: "",
								placeholder: e(E).t("calendar.settings.url"),
								"aria-label": e(E).t("calendar.settings.url")
							}, null, 8, [
								"modelValue",
								"placeholder",
								"aria-label"
							]),
							h(w, {
								modelValue: Z.value,
								"onUpdate:modelValue": c[9] ||= (e) => Z.value = e,
								maxlength: "100",
								required: "",
								placeholder: e(E).t("calendar.settings.name"),
								"aria-label": e(E).t("calendar.settings.name")
							}, null, 8, [
								"modelValue",
								"placeholder",
								"aria-label"
							]),
							i(O("input", {
								"onUpdate:modelValue": c[10] ||= (e) => Q.value = e,
								type: "color",
								"aria-label": e(E).t("calendar.settings.color"),
								class: "settings__color"
							}, null, 8, pe), [[s, Q.value]]),
							h(m, {
								modelValue: ye.value,
								"onUpdate:modelValue": c[11] ||= (e) => ye.value = e,
								options: e(Se),
								"option-label": "label",
								"option-value": "value",
								"aria-label": e(E).t("calendar.settings.visibilityLabel")
							}, null, 8, [
								"modelValue",
								"options",
								"aria-label"
							]),
							h(g, {
								type: "submit",
								label: e(E).t("calendar.settings.addExternal"),
								loading: e(be).isPending.value
							}, null, 8, ["label", "loading"])
						], 32)
					], 8, ue)) : _("", !0),
					h(k, {
						visible: Y.value,
						"onUpdate:visible": c[13] ||= (e) => Y.value = e,
						modal: "",
						header: e(E).t("calendar.settings.confirmTitle"),
						style: {
							width: "24rem",
							maxWidth: "calc(100vw - 2rem)"
						}
					}, {
						footer: u(() => [h(g, {
							severity: "secondary",
							text: "",
							label: e(E).t("cancel"),
							onClick: c[12] ||= (e) => J.value = null
						}, null, 8, ["label"]), h(g, {
							severity: "danger",
							label: e(E).t("delete"),
							onClick: ve
						}, null, 8, ["label"])]),
						default: u(() => [O("p", null, x(e(E).t("calendar.settings.confirmRemove", { name: J.value?.name ?? "" })), 1)]),
						_: 1
					}, 8, ["visible", "header"])
				]),
				_: 1
			}, 8, ["visible", "header"]);
		};
	}
}), [["__scopeId", "data-v-57dff6fb"]]), G = ["aria-label"], K = {
	key: 0,
	class: "grid__head",
	role: "row"
}, he = [
	"tabindex",
	"aria-label",
	"aria-current",
	"onKeydown",
	"onDrop"
], q = { class: "cell__day" }, ge = {
	key: 0,
	class: "cell__more"
}, _e = 3, J = /*#__PURE__*/ E(/* @__PURE__ */ k({
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
	setup(r, { emit: i }) {
		let s = r, c = i, l = g(), u = D(), m = C(() => s.days.map((e) => {
			let t = S(e), n = F(s.items, t), r = s.columns === 1 || s.month === null ? n.length : _e;
			return {
				iso: t,
				date: e,
				shown: n.slice(0, r),
				rest: n.length - r,
				dim: s.month !== null && e.getMonth() !== s.month,
				label: e.toLocaleDateString("pl-PL", {
					weekday: "long",
					day: "numeric",
					month: "long"
				})
			};
		})), h = C(() => s.days.slice(0, Math.min(7, s.days.length)).map((e) => e.toLocaleDateString("pl-PL", { weekday: "short" })));
		function y(e, t) {
			let n = {
				ArrowLeft: -1,
				ArrowRight: 1,
				ArrowUp: -s.columns,
				ArrowDown: s.columns
			}[e.key];
			n !== void 0 && (e.preventDefault(), (e.currentTarget.parentElement?.children[t + n])?.focus());
		}
		function b(e, t) {
			e.preventDefault();
			let n = e.dataTransfer?.getData("text/plain");
			n && s.canWrite && c("drop", n, t);
		}
		return (i, s) => (f(), p("div", {
			class: "grid",
			role: "grid",
			"aria-label": e(l).t("calendar.grid")
		}, [r.columns > 1 ? (f(), p("div", K, [(f(!0), p(T, null, n(h.value, (e) => (f(), p("span", {
			key: e,
			role: "columnheader"
		}, x(e), 1))), 128))])) : _("", !0), O("div", {
			class: "grid__body",
			style: t({ gridTemplateColumns: `repeat(${r.columns}, minmax(0, 1fr))` })
		}, [(f(!0), p(T, null, n(m.value, (t, i) => (f(), p("div", {
			key: t.iso,
			class: d(["cell", {
				"cell--dim": t.dim,
				"cell--today": t.iso === e(u)
			}]),
			role: "gridcell",
			tabindex: i === 0 ? 0 : -1,
			"aria-label": t.label,
			"aria-current": t.iso === e(u) ? "date" : void 0,
			onKeydown: [(e) => y(e, i), a(o((e) => r.canWrite && c("create", t.iso), ["self"]), ["enter"])],
			onDragover: s[0] ||= o(() => {}, ["prevent"]),
			onDrop: (e) => b(e, t.iso)
		}, [
			O("span", q, x(t.date.getDate()), 1),
			(f(!0), p(T, null, n(t.shown, (e) => (f(), v(I, {
				key: `${e.itemType}-${e.id}-${e.occurrenceDate ?? ""}`,
				item: e,
				"can-toggle": r.canToggle(e),
				draggable: r.canWrite && e.itemType === "Task",
				onOpen: (t) => c("open", e),
				onToggle: (t) => c("toggle", e)
			}, null, 8, [
				"item",
				"can-toggle",
				"draggable",
				"onOpen",
				"onToggle"
			]))), 128)),
			t.rest > 0 ? (f(), p("span", ge, x(e(l).t("more", { count: t.rest })), 1)) : _("", !0)
		], 42, he))), 128))], 4)], 8, G));
	}
}), [["__scopeId", "data-v-a96b0a3c"]]), Y = { class: "editor__field" }, ve = {
	key: 0,
	id: "ev-title-err",
	class: "editor__error",
	role: "alert"
}, X = { class: "editor__check" }, Z = ["disabled"], Q = { class: "editor__row" }, ye = { class: "editor__field" }, be = ["disabled"], xe = {
	key: 0,
	class: "editor__field"
}, $ = { class: "sr-only" }, Se = ["disabled"], Ce = { class: "editor__row" }, we = { class: "editor__field" }, Te = ["disabled"], Ee = {
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
}, ze = ["value", "disabled"], Be = { class: "editor__actions" }, Ve = /*#__PURE__*/ E(/* @__PURE__ */ k({
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
	setup(t, { emit: a }) {
		let d = t, m = a, y = g(), S = [
			0,
			10,
			30,
			60,
			1440
		], w = [
			"",
			"FREQ=DAILY",
			"FREQ=WEEKLY",
			"FREQ=MONTHLY",
			"FREQ=YEARLY"
		];
		function E(e) {
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
		function D(e) {
			let t = d.timeZone;
			return {
				title: e.title,
				description: e.description ?? "",
				location: e.location ?? "",
				startDate: M(e.startUtc, t),
				startTime: j(e.startUtc, t),
				endDate: M(e.endUtc, t),
				endTime: j(e.endUtc, t),
				isAllDay: e.isAllDay,
				isPrivate: e.isPrivate,
				attendees: [...e.attendees],
				reminderMinutes: [...e.reminderMinutes],
				rrule: e.rrule ?? ""
			};
		}
		let k = r(E(d.day)), A = r(!1);
		c(() => [
			d.visible,
			d.event,
			d.day
		], ([e]) => {
			e && (k.value = d.event ? D(d.event) : E(d.day), A.value = !1);
		}, { immediate: !0 });
		let N = C(() => !!d.event?.seriesId), P = C(() => N.value || k.value.rrule !== ""), F = C(() => !d.canEdit), I = C(() => ee(k.value.startDate, k.value.isAllDay ? "00:00" : k.value.startTime, d.timeZone)), L = C(() => ee(k.value.endDate, k.value.isAllDay ? "23:59" : k.value.endTime, d.timeZone)), R = C(() => A.value && !k.value.title.trim() ? y.t("calendar.errors.title") : null), z = C(() => A.value && L.value < I.value ? y.t("calendar.errors.range") : null), B = w.map((e) => ({
			value: e,
			label: y.t(`calendar.repeat.${e || "none"}`)
		}));
		function V() {
			if (A.value = !0, !k.value.title.trim() || L.value < I.value) return;
			let e = k.value;
			m("save", {
				title: e.title.trim(),
				description: e.description.trim() || null,
				location: e.location.trim() || null,
				timeZoneId: d.event?.timeZoneId ?? d.timeZone,
				startUtc: I.value,
				endUtc: L.value,
				isAllDay: e.isAllDay,
				isPrivate: e.isPrivate,
				attendees: P.value ? [] : e.attendees,
				reminderMinutes: P.value ? [] : e.reminderMinutes,
				rrule: N.value ? null : e.rrule || null
			});
		}
		function H(e) {
			return e === 0 ? y.t("calendar.reminder.atStart") : y.t("calendar.reminder.before", { minutes: e });
		}
		return (r, a) => {
			let c = l("InputText"), d = l("Textarea"), g = l("Select"), C = l("Button"), w = l("Drawer");
			return f(), v(w, {
				visible: t.visible,
				position: "right",
				header: e(y).t(t.event ? "calendar.editEvent" : "calendar.newEvent"),
				class: "editor-drawer",
				"onUpdate:visible": a[14] ||= (e) => m("update:visible", e)
			}, {
				default: u(() => [O("form", {
					class: "editor",
					novalidate: "",
					onSubmit: o(V, ["prevent"])
				}, [
					O("label", Y, [
						O("span", null, x(e(y).t("fields.title")), 1),
						h(c, {
							modelValue: k.value.title,
							"onUpdate:modelValue": a[0] ||= (e) => k.value.title = e,
							maxlength: "200",
							required: "",
							disabled: F.value,
							invalid: !!R.value,
							"aria-describedby": R.value ? "ev-title-err" : void 0
						}, null, 8, [
							"modelValue",
							"disabled",
							"invalid",
							"aria-describedby"
						]),
						R.value ? (f(), p("small", ve, x(R.value), 1)) : _("", !0)
					]),
					O("label", X, [i(O("input", {
						"onUpdate:modelValue": a[1] ||= (e) => k.value.isAllDay = e,
						type: "checkbox",
						disabled: F.value
					}, null, 8, Z), [[b, k.value.isAllDay]]), O("span", null, x(e(y).t("calendar.allDay")), 1)]),
					O("div", Q, [O("label", ye, [O("span", null, x(e(y).t("calendar.start")), 1), i(O("input", {
						"onUpdate:modelValue": a[2] ||= (e) => k.value.startDate = e,
						type: "date",
						class: "editor__input",
						required: "",
						disabled: F.value
					}, null, 8, be), [[s, k.value.startDate]])]), k.value.isAllDay ? _("", !0) : (f(), p("label", xe, [O("span", $, x(e(y).t("fields.time")), 1), i(O("input", {
						"onUpdate:modelValue": a[3] ||= (e) => k.value.startTime = e,
						type: "time",
						class: "editor__input",
						required: "",
						disabled: F.value
					}, null, 8, Se), [[s, k.value.startTime]])]))]),
					O("div", Ce, [O("label", we, [O("span", null, x(e(y).t("calendar.end")), 1), i(O("input", {
						"onUpdate:modelValue": a[4] ||= (e) => k.value.endDate = e,
						type: "date",
						class: "editor__input",
						required: "",
						disabled: F.value
					}, null, 8, Te), [[s, k.value.endDate]])]), k.value.isAllDay ? _("", !0) : (f(), p("label", Ee, [O("span", De, x(e(y).t("fields.time")), 1), i(O("input", {
						"onUpdate:modelValue": a[5] ||= (e) => k.value.endTime = e,
						type: "time",
						class: "editor__input",
						required: "",
						disabled: F.value
					}, null, 8, Oe), [[s, k.value.endTime]])]))]),
					z.value ? (f(), p("small", ke, x(z.value), 1)) : _("", !0),
					O("label", Ae, [O("span", null, x(e(y).t("calendar.location")), 1), h(c, {
						modelValue: k.value.location,
						"onUpdate:modelValue": a[6] ||= (e) => k.value.location = e,
						maxlength: "500",
						disabled: F.value
					}, null, 8, ["modelValue", "disabled"])]),
					O("label", je, [O("span", null, x(e(y).t("fields.notes")), 1), h(d, {
						modelValue: k.value.description,
						"onUpdate:modelValue": a[7] ||= (e) => k.value.description = e,
						rows: "3",
						"auto-resize": "",
						maxlength: "4000",
						disabled: F.value
					}, null, 8, ["modelValue", "disabled"])]),
					O("label", Me, [i(O("input", {
						"onUpdate:modelValue": a[8] ||= (e) => k.value.isPrivate = e,
						type: "checkbox",
						disabled: F.value
					}, null, 8, Ne), [[b, k.value.isPrivate]]), O("span", null, x(e(y).t("calendar.private")), 1)]),
					N.value ? (f(), p("p", Fe, x(e(y).t("calendar.repeat.series")), 1)) : (f(), p("label", Pe, [O("span", null, x(e(y).t("calendar.repeat.label")), 1), h(g, {
						modelValue: k.value.rrule,
						"onUpdate:modelValue": a[9] ||= (e) => k.value.rrule = e,
						options: e(B),
						"option-label": "label",
						"option-value": "value",
						disabled: F.value || !!t.event
					}, null, 8, [
						"modelValue",
						"options",
						"disabled"
					])])),
					!P.value && t.members.length ? (f(), p("fieldset", Ie, [O("legend", null, x(e(y).t("calendar.attendees")), 1), (f(!0), p(T, null, n(t.members, (e) => (f(), p("label", {
						key: e.userId,
						class: "editor__check"
					}, [i(O("input", {
						"onUpdate:modelValue": a[10] ||= (e) => k.value.attendees = e,
						type: "checkbox",
						value: e.userId,
						disabled: F.value
					}, null, 8, Le), [[b, k.value.attendees]]), O("span", null, x(e.displayName), 1)]))), 128))])) : _("", !0),
					P.value ? _("", !0) : (f(), p("fieldset", Re, [O("legend", null, x(e(y).t("calendar.reminder.label")), 1), (f(), p(T, null, n(S, (e) => O("label", {
						key: e,
						class: "editor__check"
					}, [i(O("input", {
						"onUpdate:modelValue": a[11] ||= (e) => k.value.reminderMinutes = e,
						type: "checkbox",
						value: e,
						disabled: F.value
					}, null, 8, ze), [[b, k.value.reminderMinutes]]), O("span", null, x(H(e)), 1)])), 64))])),
					O("div", Be, [
						t.event && t.canDelete ? (f(), v(C, {
							key: 0,
							type: "button",
							label: e(y).t("delete"),
							severity: "danger",
							text: "",
							class: "editor__button",
							onClick: a[12] ||= (e) => m("delete")
						}, null, 8, ["label"])) : _("", !0),
						a[15] ||= O("span", { class: "editor__spacer" }, null, -1),
						h(C, {
							type: "button",
							label: e(y).t("cancel"),
							severity: "secondary",
							text: "",
							class: "editor__button",
							onClick: a[13] ||= (e) => m("update:visible", !1)
						}, null, 8, ["label"]),
						t.canEdit ? (f(), v(C, {
							key: 1,
							type: "submit",
							label: e(y).t(t.event ? "save" : "create"),
							loading: t.saving,
							class: "editor__button"
						}, null, 8, ["label", "loading"])) : _("", !0)
					])
				], 32)]),
				_: 1
			}, 8, ["visible", "header"]);
		};
	}
}), [["__scopeId", "data-v-b366e60c"]]), He = { class: "planner" }, Ue = { class: "planner__bar" }, We = ["aria-label"], Ge = {
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
}, Qe = /*#__PURE__*/ E(/* @__PURE__ */ k({
	__name: "CalendarPage",
	setup(t) {
		let a = g(), o = C(() => a.household.value.id), s = w(o), { api: c, userId: d, canWrite: S, canManage: E, members: k, change: j } = s, ee = [
			"month",
			"week",
			"day",
			"agenda"
		], M = r(window.matchMedia("(max-width: 640px)").matches ? "agenda" : "month"), F = r(/* @__PURE__ */ new Date()), I = r([
			"events",
			"private",
			"tasks",
			"external"
		]), z = r(null), B = C(() => s.settings.data.value?.firstDayOfWeek ?? 1), V = C(() => N(M.value, F.value, B.value)), te = C(() => L(M.value, F.value, B.value)), U = s.calendar(te, I), W = C(() => {
			let e = U.data.value ?? [], t = z.value;
			return t ? e.filter((e) => e.createdBy === t || e.attendees.includes(t)) : e;
		}), ne = C(() => {
			let e = F.value;
			if (M.value === "month") return e.toLocaleDateString("pl-PL", {
				month: "long",
				year: "numeric"
			});
			if (M.value === "day") return e.toLocaleDateString("pl-PL", {
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
		}), re = C(() => M.value === "day" ? 1 : 7), ie = C(() => ee.map((e) => ({
			value: e,
			label: a.t(`calendar.views.${e}`)
		}))), ae = C(() => [
			"events",
			"private",
			"tasks",
			"external"
		].map((e) => ({
			value: e,
			label: a.t(`calendar.layers.${e}`)
		}))), oe = C(() => [{
			userId: null,
			displayName: a.t("calendar.everyone")
		}, ...(k.data.value ?? []).map((e) => ({
			userId: e.userId,
			displayName: e.displayName
		}))]), se = m({
			queryKey: C(() => [
				"plugins",
				"todo",
				o.value,
				"external"
			]),
			queryFn: () => c.value.externals(),
			enabled: C(() => S.value && I.value.includes("external"))
		}), ce = C(() => (se.data.value ?? []).filter((e) => e.isStale)), le = (e) => e ? new Date(e).toLocaleString("pl-PL") : a.t("calendar.settings.never");
		function ue(e) {
			F.value = P(M.value, F.value, e);
		}
		let de = (e) => S.value && e.itemType === "Task", fe = j((e, t) => t.isDone ? e.reopenTask(t.id) : e.completeTask(t.id)), pe = j(async (e, { id: t, day: n }) => {
			let r = W.value.find((e) => e.itemType === "Task" && e.id === t);
			if (!r?.date || r.date === n) return;
			let i = (await e.tasks({
				dueBy: r.date,
				status: "Open"
			})).find((e) => e.id === t);
			i && await e.updateTask(t, {
				...y(i),
				dueDate: n
			});
		}), G = r(!1), K = r(null), he = r(D()), q = r(null), ge = C(() => {
			let e = K.value;
			return S.value ? !e || e.createdBy === d.value || E.value ? !0 : !e.isPrivate && (e.attendees.length === 0 || e.attendees.includes(d.value ?? "")) : !1;
		}), _e = C(() => S.value && (!K.value || K.value.createdBy === d.value || E.value));
		function Y(e = D()) {
			K.value = null, he.value = e, G.value = !0;
		}
		async function ve(e) {
			if (e.itemType === "Task") de(e) && fe.mutate(e);
			else if (e.itemType !== "External") try {
				let t = await c.value.event(e.id);
				K.value = e.isOccurrence ? {
					...t,
					occurrenceDate: e.occurrenceDate,
					title: e.title,
					description: e.description,
					location: e.location,
					startUtc: e.startUtc,
					endUtc: e.endUtc
				} : t, G.value = !0;
			} catch (e) {
				a.notify("error", a.errorMessage(e));
			}
		}
		let X = j((e, t) => e.createEvent(t)), Z = j((e, { id: t, input: n }) => e.updateEvent(t, n)), Q = j((e, t) => e.modifyInstance(t.seriesId, t.date, {
			scope: t.scope,
			title: t.input.title,
			description: t.input.description,
			location: t.input.location,
			startUtc: t.input.startUtc,
			endUtc: t.input.endUtc
		})), ye = j((e, t) => e.deleteEvent(t)), be = j((e, t) => e.deleteInstance(t.seriesId, t.date, t.scope)), xe = C(() => X.isPending.value || Z.isPending.value || Q.isPending.value), $ = { onSuccess: () => G.value = !1 };
		function Se(e) {
			let t = K.value;
			t ? t.seriesId ? q.value = {
				mode: "edit",
				input: e
			} : Z.mutate({
				id: t.id,
				input: e
			}, $) : X.mutate(e, $);
		}
		function Ce() {
			let e = K.value;
			e && (e.seriesId ? q.value = { mode: "delete" } : window.confirm(a.t("calendar.deleteConfirm", { name: e.title })) && ye.mutate(e.id, $));
		}
		function we(e) {
			let t = K.value, n = q.value;
			if (q.value = null, !t?.seriesId || !t.occurrenceDate || !n) return;
			let r = t.occurrenceDate;
			n.mode === "delete" ? be.mutate({
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
		let Te = r(!1);
		return (t, r) => {
			let c = l("Button"), d = l("SelectButton"), m = l("Select"), g = l("Message"), y = l("Skeleton");
			return f(), p("div", He, [
				O("header", Ue, [
					O("div", {
						class: "planner__nav",
						role: "group",
						"aria-label": e(a).t("calendar.navigate")
					}, [
						h(c, {
							icon: "pi pi-chevron-left",
							text: "",
							"aria-label": e(a).t("calendar.previous"),
							class: "planner__touch",
							onClick: r[0] ||= (e) => ue(-1)
						}, null, 8, ["aria-label"]),
						h(c, {
							label: e(a).t("calendar.today"),
							text: "",
							class: "planner__touch",
							onClick: r[1] ||= (e) => F.value = /* @__PURE__ */ new Date()
						}, null, 8, ["label"]),
						h(c, {
							icon: "pi pi-chevron-right",
							text: "",
							"aria-label": e(a).t("calendar.next"),
							class: "planner__touch",
							onClick: r[2] ||= (e) => ue(1)
						}, null, 8, ["aria-label"])
					], 8, We),
					O("h2", Ge, x(ne.value), 1),
					h(d, {
						modelValue: M.value,
						"onUpdate:modelValue": r[3] ||= (e) => M.value = e,
						options: ie.value,
						"option-label": "label",
						"option-value": "value",
						"allow-empty": !1,
						"aria-label": e(a).t("calendar.viewSwitch")
					}, null, 8, [
						"modelValue",
						"options",
						"aria-label"
					]),
					e(k).data.value?.length ? (f(), v(m, {
						key: 0,
						modelValue: z.value,
						"onUpdate:modelValue": r[4] ||= (e) => z.value = e,
						options: oe.value,
						"option-label": "displayName",
						"option-value": "userId",
						"aria-label": e(a).t("calendar.person")
					}, null, 8, [
						"modelValue",
						"options",
						"aria-label"
					])) : _("", !0),
					e(S) ? (f(), v(c, {
						key: 1,
						icon: "pi pi-plus",
						label: e(a).t("calendar.newEvent"),
						class: "planner__touch",
						onClick: r[5] ||= (e) => Y()
					}, null, 8, ["label"])) : _("", !0),
					h(c, {
						icon: "pi pi-cog",
						text: "",
						"aria-label": e(a).t("calendar.settings.title"),
						class: "planner__touch",
						onClick: r[6] ||= (e) => Te.value = !0
					}, null, 8, ["aria-label"])
				]),
				O("fieldset", Ke, [O("legend", qe, x(e(a).t("calendar.layersLabel")), 1), (f(!0), p(T, null, n(ae.value, (e) => (f(), p("label", {
					key: e.value,
					class: "planner__layer"
				}, [i(O("input", {
					"onUpdate:modelValue": r[7] ||= (e) => I.value = e,
					type: "checkbox",
					value: e.value
				}, null, 8, Je), [[b, I.value]]), A(" " + x(e.label), 1)]))), 128))]),
				(f(!0), p(T, null, n(ce.value, (t) => (f(), v(g, {
					key: t.id,
					severity: "warn",
					closable: !1,
					role: "alert"
				}, {
					default: u(() => [A(x(e(a).t("calendar.settings.stale", { when: le(t.lastSyncedAt) })) + " (" + x(t.name) + ") ", 1)]),
					_: 2
				}, 1024))), 128)),
				e(S) ? _("", !0) : (f(), p("p", Ye, x(e(a).t("calendar.readOnly")), 1)),
				e(U).isPending.value ? (f(), p("div", Xe, [(f(!0), p(T, null, n(M.value === "agenda" ? 5 : 6, (e) => (f(), v(y, {
					key: e,
					height: M.value === "agenda" ? "3rem" : "6rem"
				}, null, 8, ["height"]))), 128))])) : e(U).isError.value ? (f(), v(g, {
					key: 2,
					severity: "error",
					role: "alert"
				}, {
					default: u(() => [A(x(e(a).errorMessage(e(U).error.value)) + " ", 1), h(c, {
						label: e(a).t("calendar.retry"),
						text: "",
						onClick: r[8] ||= (t) => e(U).refetch()
					}, null, 8, ["label"])]),
					_: 1
				})) : W.value.length === 0 && !z.value ? (f(), p("div", Ze, [
					r[14] ||= O("i", {
						class: "pi pi-calendar",
						"aria-hidden": "true"
					}, null, -1),
					O("p", null, x(e(a).t("calendar.empty")), 1),
					e(S) ? (f(), v(c, {
						key: 0,
						label: e(a).t("calendar.addFirst"),
						onClick: r[9] ||= (e) => Y()
					}, null, 8, ["label"])) : _("", !0)
				])) : M.value === "agenda" ? (f(), v(H, {
					key: 4,
					days: V.value,
					items: W.value,
					"can-toggle": de,
					onOpen: ve,
					onToggle: e(fe).mutate
				}, null, 8, [
					"days",
					"items",
					"onToggle"
				])) : (f(), v(J, {
					key: 5,
					days: V.value,
					items: W.value,
					month: M.value === "month" ? F.value.getMonth() : null,
					columns: re.value,
					"can-write": e(S),
					"can-toggle": de,
					onOpen: ve,
					onToggle: e(fe).mutate,
					onCreate: Y,
					onDrop: r[10] ||= (t, n) => e(pe).mutate({
						id: t,
						day: n
					})
				}, null, 8, [
					"days",
					"items",
					"month",
					"columns",
					"can-write",
					"onToggle"
				])),
				h(Ve, {
					visible: G.value,
					"onUpdate:visible": r[11] ||= (e) => G.value = e,
					event: K.value,
					day: he.value,
					"time-zone": e(s).timeZone.value,
					members: e(k).data.value ?? [],
					"can-edit": ge.value,
					"can-delete": _e.value,
					saving: xe.value,
					onSave: Se,
					onDelete: Ce
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
				h(R, {
					visible: q.value !== null,
					mode: q.value?.mode ?? "edit",
					"onUpdate:visible": r[12] ||= (e) => q.value = null,
					onChoose: we
				}, null, 8, ["visible", "mode"]),
				h(me, {
					visible: Te.value,
					"onUpdate:visible": r[13] ||= (e) => Te.value = e,
					"household-id": o.value
				}, null, 8, ["visible", "household-id"])
			]);
		};
	}
}), [["__scopeId", "data-v-21637182"]]);
export { Qe as default };
