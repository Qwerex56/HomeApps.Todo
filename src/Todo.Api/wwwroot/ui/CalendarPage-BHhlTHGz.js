import { A as e, C as t, D as n, E as r, F as i, I as a, M as o, N as s, O as c, P as l, S as u, T as d, _ as f, a as p, b as m, f as h, g, h as _, i as v, j as y, k as b, l as x, m as S, n as C, p as w, t as T, u as E, v as D, w as O, x as k, y as A } from "./_plugin-vue_export-helper-DX_-5sjr.js";
import { a as j, c as M, i as N, l as P, o as F, r as I, t as L, u as R } from "./CalendarChip-BDOUGRpB.js";
// src/AgendaList.vue?vue&type=script&setup=true&lang.ts
var z = { class: "agenda" }, B = {
	key: 0,
	class: "agenda__empty"
}, V = { class: "agenda__heading" }, ee = /*#__PURE__*/ T(/* @__PURE__ */ k({
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
		return (t, n) => (O(), f("div", z, [o.value.length === 0 ? (O(), f("p", B, c(b(a).t("calendar.agendaEmpty")), 1)) : g("", !0), (O(!0), f(w, null, r(o.value, (t) => (O(), f("section", {
			key: t.iso,
			class: "agenda__day"
		}, [D("h3", V, c(t.label), 1), (O(!0), f(w, null, r(t.items, (t) => (O(), _(L, {
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
}), [["__scopeId", "data-v-869f9204"]]), H = ["aria-label"], U = { class: "settings__hint" }, te = {
	key: 2,
	class: "settings__list"
}, ne = {
	key: 0,
	class: "settings__hint"
}, re = { role: "status" }, ie = { class: "settings__check" }, ae = ["aria-label"], oe = {
	key: 2,
	class: "settings__list"
}, se = {
	key: 0,
	class: "settings__hint"
}, ce = ["aria-label"], W = /*#__PURE__*/ T(/* @__PURE__ */ k({
	__name: "CalendarSettingsDialog",
	props: {
		visible: { type: Boolean },
		householdId: {}
	},
	emits: ["update:visible"],
	setup(i, { emit: o }) {
		let u = i, v = o, x = h(), { api: T, change: E } = C(() => u.householdId), k = S(() => u.visible), j = p({
			queryKey: S(() => [
				"plugins",
				"todo",
				u.householdId,
				"subscriptions"
			]),
			queryFn: () => T.value.subscriptions(),
			enabled: k
		}), M = p({
			queryKey: S(() => [
				"plugins",
				"todo",
				u.householdId,
				"external"
			]),
			queryFn: () => T.value.externals(),
			enabled: k
		}), N = d(""), P = d("None"), F = d(!1), I = d(null), L = E((e, t) => e.createSubscription(t)), R = E((e, t) => e.revokeSubscription(t)), z = [
			"None",
			"Assigned",
			"All"
		].map((e) => ({
			value: e,
			label: x.t(`calendar.settings.tasksMode.${e}`)
		})), B = S(() => I.value ? `${window.location.origin}${I.value.feedPath}` : "");
		function V() {
			N.value.trim() && L.mutate({
				label: N.value.trim(),
				includeHouseholdEvents: !0,
				includePrivateEvents: F.value,
				tasksMode: P.value
			}, { onSuccess: (e) => {
				I.value = e, N.value = "";
			} });
		}
		async function ee() {
			await navigator.clipboard?.writeText(B.value), x.notify("success", x.t("calendar.settings.copied"));
		}
		let W = d(""), G = d(""), K = d("#3b82f6"), q = d("Busy"), J = E((e, t) => e.addExternal(t)), Y = E((e, t) => e.syncExternal(t)), X = E((e, t) => e.removeExternal(t)), le = [
			"OnlyMe",
			"Busy",
			"Details"
		].map((e) => ({
			value: e,
			label: x.t(`calendar.settings.visibility.${e}`)
		}));
		function Z() {
			W.value.trim() && G.value.trim() && J.mutate({
				url: W.value.trim(),
				name: G.value.trim(),
				color: K.value,
				visibility: q.value
			}, { onSuccess: () => (W.value = "", G.value = "") });
		}
		let Q = (e) => e ? new Date(e).toLocaleString("pl-PL") : x.t("calendar.settings.never");
		return (o, u) => {
			let d = n("Skeleton"), p = n("Message"), h = n("Button"), S = n("InputText"), C = n("Select"), T = n("Dialog");
			return O(), _(T, {
				visible: i.visible,
				modal: "",
				header: b(x).t("calendar.settings.title"),
				style: {
					width: "40rem",
					maxWidth: "calc(100vw - 2rem)"
				},
				"onUpdate:visible": u[7] ||= (e) => v("update:visible", e)
			}, {
				default: s(() => [D("section", {
					class: "settings",
					"aria-label": b(x).t("calendar.settings.subscriptions")
				}, [
					D("h3", null, c(b(x).t("calendar.settings.subscriptions")), 1),
					D("p", U, c(b(x).t("calendar.settings.subscriptionsHint")), 1),
					b(j).isPending.value ? (O(), _(d, {
						key: 0,
						height: "2rem"
					})) : b(j).isError.value ? (O(), _(p, {
						key: 1,
						severity: "error"
					}, {
						default: s(() => [A(c(b(x).errorMessage(b(j).error.value)), 1)]),
						_: 1
					})) : (O(), f("ul", te, [(O(!0), f(w, null, r(b(j).data.value, (e) => (O(), f("li", { key: e.id }, [D("span", null, [A(c(e.label) + " ", 1), D("small", null, "(" + c(b(x).t("calendar.settings.lastUsed")) + ": " + c(Q(e.lastAccessedAt)) + ")", 1)]), m(h, {
						icon: "pi pi-trash",
						text: "",
						severity: "danger",
						"aria-label": b(x).t("calendar.settings.revoke", { name: e.label }),
						onClick: (t) => b(R).mutate(e.id)
					}, null, 8, ["aria-label", "onClick"])]))), 128)), b(j).data.value?.length ? g("", !0) : (O(), f("li", ne, c(b(x).t("calendar.settings.noSubscriptions")), 1))])),
					I.value ? (O(), _(p, {
						key: 3,
						severity: "success",
						closable: !1
					}, {
						default: s(() => [D("div", re, [
							D("p", null, c(b(x).t("calendar.settings.feedOnce")), 1),
							m(S, {
								"model-value": B.value,
								readonly: "",
								class: "settings__wide",
								"aria-label": b(x).t("calendar.settings.feedUrl")
							}, null, 8, ["model-value", "aria-label"]),
							m(h, {
								label: b(x).t("calendar.settings.copy"),
								icon: "pi pi-copy",
								text: "",
								onClick: ee
							}, null, 8, ["label"])
						])]),
						_: 1
					})) : g("", !0),
					D("form", {
						class: "settings__form",
						onSubmit: a(V, ["prevent"])
					}, [
						m(S, {
							modelValue: N.value,
							"onUpdate:modelValue": u[0] ||= (e) => N.value = e,
							maxlength: "100",
							required: "",
							placeholder: b(x).t("calendar.settings.label"),
							"aria-label": b(x).t("calendar.settings.label")
						}, null, 8, [
							"modelValue",
							"placeholder",
							"aria-label"
						]),
						m(C, {
							modelValue: P.value,
							"onUpdate:modelValue": u[1] ||= (e) => P.value = e,
							options: b(z),
							"option-label": "label",
							"option-value": "value",
							"aria-label": b(x).t("calendar.settings.tasksModeLabel")
						}, null, 8, [
							"modelValue",
							"options",
							"aria-label"
						]),
						D("label", ie, [l(D("input", {
							"onUpdate:modelValue": u[2] ||= (e) => F.value = e,
							type: "checkbox"
						}, null, 512), [[e, F.value]]), A(" " + c(b(x).t("calendar.settings.includePrivate")), 1)]),
						m(h, {
							type: "submit",
							label: b(x).t("calendar.settings.addSubscription"),
							loading: b(L).isPending.value
						}, null, 8, ["label", "loading"])
					], 32)
				], 8, H), D("section", {
					class: "settings",
					"aria-label": b(x).t("calendar.settings.external")
				}, [
					D("h3", null, c(b(x).t("calendar.settings.external")), 1),
					b(M).isPending.value ? (O(), _(d, {
						key: 0,
						height: "2rem"
					})) : b(M).isError.value ? (O(), _(p, {
						key: 1,
						severity: "error"
					}, {
						default: s(() => [A(c(b(x).errorMessage(b(M).error.value)), 1)]),
						_: 1
					})) : (O(), f("ul", oe, [(O(!0), f(w, null, r(b(M).data.value, (e) => (O(), f("li", { key: e.id }, [D("span", null, [
						D("i", {
							class: "settings__dot",
							style: t({ background: e.color }),
							"aria-hidden": "true"
						}, null, 4),
						A(" " + c(e.name) + " ", 1),
						D("small", null, c(b(x).t(`calendar.settings.visibility.${e.visibility}`)), 1),
						e.isStale ? (O(), _(p, {
							key: 0,
							severity: "warn",
							closable: !1,
							role: "alert"
						}, {
							default: s(() => [A(c(b(x).t("calendar.settings.stale", { when: Q(e.lastSyncedAt) })) + " ", 1), e.lastErrorMessage ? (O(), f(w, { key: 0 }, [A(": " + c(e.lastErrorMessage), 1)], 64)) : g("", !0)]),
							_: 2
						}, 1024)) : g("", !0)
					]), D("span", null, [m(h, {
						icon: "pi pi-refresh",
						text: "",
						loading: b(Y).isPending.value,
						"aria-label": b(x).t("calendar.settings.sync", { name: e.name }),
						onClick: (t) => b(Y).mutate(e.id)
					}, null, 8, [
						"loading",
						"aria-label",
						"onClick"
					]), m(h, {
						icon: "pi pi-trash",
						text: "",
						severity: "danger",
						"aria-label": b(x).t("calendar.settings.remove", { name: e.name }),
						onClick: (t) => b(X).mutate(e.id)
					}, null, 8, ["aria-label", "onClick"])])]))), 128)), b(M).data.value?.length ? g("", !0) : (O(), f("li", se, c(b(x).t("calendar.settings.noExternal")), 1))])),
					D("form", {
						class: "settings__form",
						onSubmit: a(Z, ["prevent"])
					}, [
						m(S, {
							modelValue: W.value,
							"onUpdate:modelValue": u[3] ||= (e) => W.value = e,
							type: "url",
							required: "",
							placeholder: b(x).t("calendar.settings.url"),
							"aria-label": b(x).t("calendar.settings.url")
						}, null, 8, [
							"modelValue",
							"placeholder",
							"aria-label"
						]),
						m(S, {
							modelValue: G.value,
							"onUpdate:modelValue": u[4] ||= (e) => G.value = e,
							maxlength: "100",
							required: "",
							placeholder: b(x).t("calendar.settings.name"),
							"aria-label": b(x).t("calendar.settings.name")
						}, null, 8, [
							"modelValue",
							"placeholder",
							"aria-label"
						]),
						l(D("input", {
							"onUpdate:modelValue": u[5] ||= (e) => K.value = e,
							type: "color",
							"aria-label": b(x).t("calendar.settings.color"),
							class: "settings__color"
						}, null, 8, ce), [[y, K.value]]),
						m(C, {
							modelValue: q.value,
							"onUpdate:modelValue": u[6] ||= (e) => q.value = e,
							options: b(le),
							"option-label": "label",
							"option-value": "value",
							"aria-label": b(x).t("calendar.settings.visibilityLabel")
						}, null, 8, [
							"modelValue",
							"options",
							"aria-label"
						]),
						m(h, {
							type: "submit",
							label: b(x).t("calendar.settings.addExternal"),
							loading: b(J).isPending.value
						}, null, 8, ["label", "loading"])
					], 32)
				], 8, ae)]),
				_: 1
			}, 8, ["visible", "header"]);
		};
	}
}), [["__scopeId", "data-v-86db5c50"]]), G = ["aria-label"], K = {
	key: 0,
	class: "grid__head",
	role: "row"
}, q = [
	"tabindex",
	"aria-label",
	"aria-current",
	"onKeydown",
	"onDrop"
], J = { class: "cell__day" }, Y = {
	key: 0,
	class: "cell__more"
}, X = 3, le = /*#__PURE__*/ T(/* @__PURE__ */ k({
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
			let t = x(e), n = I(o.items, t), r = o.columns === 1 || o.month === null ? n.length : X;
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
		}, [e.columns > 1 ? (O(), f("div", K, [(O(!0), f(w, null, r(m.value, (e) => (O(), f("span", {
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
			D("span", J, c(t.date.getDate()), 1),
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
			t.rest > 0 ? (O(), f("span", Y, c(b(l).t("more", { count: t.rest })), 1)) : g("", !0)
		], 42, q))), 128))], 4)], 8, G));
	}
}), [["__scopeId", "data-v-2ee0d671"]]), Z = { class: "editor__field" }, Q = {
	key: 0,
	id: "ev-title-err",
	class: "editor__error",
	role: "alert"
}, ue = { class: "editor__check" }, de = ["disabled"], fe = { class: "editor__row" }, pe = { class: "editor__field" }, me = ["disabled"], he = {
	key: 0,
	class: "editor__field"
}, ge = { class: "sr-only" }, _e = ["disabled"], ve = { class: "editor__row" }, $ = { class: "editor__field" }, ye = ["disabled"], be = {
	key: 0,
	class: "editor__field"
}, xe = { class: "sr-only" }, Se = ["disabled"], Ce = {
	key: 0,
	class: "editor__error",
	role: "alert"
}, we = { class: "editor__field" }, Te = { class: "editor__field" }, Ee = { class: "editor__check" }, De = ["disabled"], Oe = {
	key: 1,
	class: "editor__field"
}, ke = {
	key: 2,
	class: "editor__hint"
}, Ae = {
	key: 3,
	class: "editor__group"
}, je = ["value", "disabled"], Me = {
	key: 4,
	class: "editor__group"
}, Ne = ["value", "disabled"], Pe = { class: "editor__actions" }, Fe = /*#__PURE__*/ T(/* @__PURE__ */ k({
	__name: "EventEditorDrawer",
	props: {
		visible: { type: Boolean },
		event: {},
		day: {},
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
		function ee() {
			if (A.value = !0, !k.value.title.trim() || R.value < L.value) return;
			let e = k.value;
			p("save", {
				title: e.title.trim(),
				description: e.description.trim() || null,
				location: e.location.trim() || null,
				timeZoneId: u.event?.timeZoneId ?? Intl.DateTimeFormat().resolvedOptions().timeZone,
				startUtc: L.value,
				endUtc: R.value,
				isAllDay: e.isAllDay,
				isPrivate: e.isPrivate,
				attendees: F.value ? [] : e.attendees,
				reminderMinutes: F.value ? [] : e.reminderMinutes,
				rrule: P.value ? null : e.rrule || null
			});
		}
		function H(e) {
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
					onSubmit: a(ee, ["prevent"])
				}, [
					D("label", Z, [
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
						z.value ? (O(), f("small", Q, c(z.value), 1)) : g("", !0)
					]),
					D("label", ue, [l(D("input", {
						"onUpdate:modelValue": o[1] ||= (e) => k.value.isAllDay = e,
						type: "checkbox",
						disabled: I.value
					}, null, 8, de), [[e, k.value.isAllDay]]), D("span", null, c(b(v).t("calendar.allDay")), 1)]),
					D("div", fe, [D("label", pe, [D("span", null, c(b(v).t("calendar.start")), 1), l(D("input", {
						"onUpdate:modelValue": o[2] ||= (e) => k.value.startDate = e,
						type: "date",
						class: "editor__input",
						required: "",
						disabled: I.value
					}, null, 8, me), [[y, k.value.startDate]])]), k.value.isAllDay ? g("", !0) : (O(), f("label", he, [D("span", ge, c(b(v).t("fields.time")), 1), l(D("input", {
						"onUpdate:modelValue": o[3] ||= (e) => k.value.startTime = e,
						type: "time",
						class: "editor__input",
						required: "",
						disabled: I.value
					}, null, 8, _e), [[y, k.value.startTime]])]))]),
					D("div", ve, [D("label", $, [D("span", null, c(b(v).t("calendar.end")), 1), l(D("input", {
						"onUpdate:modelValue": o[4] ||= (e) => k.value.endDate = e,
						type: "date",
						class: "editor__input",
						required: "",
						disabled: I.value
					}, null, 8, ye), [[y, k.value.endDate]])]), k.value.isAllDay ? g("", !0) : (O(), f("label", be, [D("span", xe, c(b(v).t("fields.time")), 1), l(D("input", {
						"onUpdate:modelValue": o[5] ||= (e) => k.value.endTime = e,
						type: "time",
						class: "editor__input",
						required: "",
						disabled: I.value
					}, null, 8, Se), [[y, k.value.endTime]])]))]),
					B.value ? (O(), f("small", Ce, c(B.value), 1)) : g("", !0),
					D("label", we, [D("span", null, c(b(v).t("calendar.location")), 1), m(u, {
						modelValue: k.value.location,
						"onUpdate:modelValue": o[6] ||= (e) => k.value.location = e,
						maxlength: "500",
						disabled: I.value
					}, null, 8, ["modelValue", "disabled"])]),
					D("label", Te, [D("span", null, c(b(v).t("fields.notes")), 1), m(d, {
						modelValue: k.value.description,
						"onUpdate:modelValue": o[7] ||= (e) => k.value.description = e,
						rows: "3",
						"auto-resize": "",
						maxlength: "4000",
						disabled: I.value
					}, null, 8, ["modelValue", "disabled"])]),
					D("label", Ee, [l(D("input", {
						"onUpdate:modelValue": o[8] ||= (e) => k.value.isPrivate = e,
						type: "checkbox",
						disabled: I.value
					}, null, 8, De), [[e, k.value.isPrivate]]), D("span", null, c(b(v).t("calendar.private")), 1)]),
					P.value ? (O(), f("p", ke, c(b(v).t("calendar.repeat.series")), 1)) : (O(), f("label", Oe, [D("span", null, c(b(v).t("calendar.repeat.label")), 1), m(h, {
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
					!F.value && t.members.length ? (O(), f("fieldset", Ae, [D("legend", null, c(b(v).t("calendar.attendees")), 1), (O(!0), f(w, null, r(t.members, (t) => (O(), f("label", {
						key: t.userId,
						class: "editor__check"
					}, [l(D("input", {
						"onUpdate:modelValue": o[10] ||= (e) => k.value.attendees = e,
						type: "checkbox",
						value: t.userId,
						disabled: I.value
					}, null, 8, je), [[e, k.value.attendees]]), D("span", null, c(t.displayName), 1)]))), 128))])) : g("", !0),
					F.value ? g("", !0) : (O(), f("fieldset", Me, [D("legend", null, c(b(v).t("calendar.reminder.label")), 1), (O(), f(w, null, r(x, (t) => D("label", {
						key: t,
						class: "editor__check"
					}, [l(D("input", {
						"onUpdate:modelValue": o[11] ||= (e) => k.value.reminderMinutes = e,
						type: "checkbox",
						value: t,
						disabled: I.value
					}, null, 8, Ne), [[e, k.value.reminderMinutes]]), D("span", null, c(H(t)), 1)])), 64))])),
					D("div", Pe, [
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
}), [["__scopeId", "data-v-c64544eb"]]), Ie = ["aria-label"], Le = /*#__PURE__*/ T(/* @__PURE__ */ k({
	__name: "SeriesScopeDialog",
	props: {
		visible: { type: Boolean },
		mode: {}
	},
	emits: ["update:visible", "choose"],
	setup(e, { emit: t }) {
		let i = t, a = h(), o = [
			"ThisInstance",
			"ThisAndFuture",
			"All"
		];
		return (t, c) => {
			let l = n("Button"), u = n("Dialog");
			return O(), _(u, {
				visible: e.visible,
				modal: "",
				header: b(a).t(e.mode === "edit" ? "calendar.scope.editTitle" : "calendar.scope.deleteTitle"),
				style: {
					width: "24rem",
					maxWidth: "calc(100vw - 2rem)"
				},
				"onUpdate:visible": c[1] ||= (e) => i("update:visible", e)
			}, {
				default: s(() => [D("div", {
					class: "scope",
					role: "group",
					"aria-label": b(a).t("calendar.scope.group")
				}, [(O(), f(w, null, r(o, (t) => m(l, {
					key: t,
					label: b(a).t(`calendar.scope.${t}`),
					severity: e.mode === "delete" ? "danger" : "primary",
					outlined: t !== "ThisInstance",
					class: "scope__button",
					onClick: (e) => i("choose", t)
				}, null, 8, [
					"label",
					"severity",
					"outlined",
					"onClick"
				])), 64)), m(l, {
					label: b(a).t("cancel"),
					severity: "secondary",
					text: "",
					class: "scope__button",
					onClick: c[0] ||= (e) => i("update:visible", !1)
				}, null, 8, ["label"])], 8, Ie)]),
				_: 1
			}, 8, ["visible", "header"]);
		};
	}
}), [["__scopeId", "data-v-35daf7fc"]]), Re = { class: "planner" }, ze = { class: "planner__bar" }, Be = ["aria-label"], Ve = {
	class: "planner__title",
	"aria-live": "polite"
}, He = { class: "planner__layers" }, Ue = { class: "planner__legend" }, We = ["value"], Ge = {
	key: 0,
	class: "planner__hint"
}, Ke = {
	key: 1,
	class: "planner__loading",
	"aria-busy": "true"
}, qe = {
	key: 3,
	class: "planner__empty"
}, Je = /*#__PURE__*/ T(/* @__PURE__ */ k({
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
		]), z = d(null), B = S(() => P(N.value, I.value)), V = S(() => R(N.value, I.value)), H = o.calendar(V, L), U = S(() => {
			let e = H.data.value ?? [], t = z.value;
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
			let t = B.value[0], n = B.value[B.value.length - 1], r = (e) => e.toLocaleDateString("pl-PL", {
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
		function G(e) {
			I.value = F(N.value, I.value, e);
		}
		let K = (e) => x.value && e.itemType === "Task", q = j((e, t) => t.isDone ? e.reopenTask(t.id) : e.completeTask(t.id)), J = j(async (e, { id: t, day: n }) => {
			let r = U.value.find((e) => e.itemType === "Task" && e.id === t);
			if (!r?.date || r.date === n) return;
			let i = (await e.tasks({
				dueBy: r.date,
				status: "Open"
			})).find((e) => e.id === t);
			i && await e.updateTask(t, {
				...v(i),
				dueDate: n
			});
		}), Y = d(!1), X = d(null), Z = d(E()), Q = d(null), ue = S(() => x.value && (!X.value || X.value.createdBy === y.value || T.value));
		function de(e = E()) {
			X.value = null, Z.value = e, Y.value = !0;
		}
		async function fe(e) {
			if (e.itemType === "Task") K(e) && q.mutate(e);
			else if (e.itemType !== "External") try {
				let t = await u.value.event(e.id);
				X.value = e.isOccurrence ? {
					...t,
					occurrenceDate: e.occurrenceDate,
					title: e.title,
					description: e.description,
					location: e.location,
					startUtc: e.startUtc,
					endUtc: e.endUtc
				} : t, Y.value = !0;
			} catch (e) {
				i.notify("error", i.errorMessage(e));
			}
		}
		let pe = j((e, t) => e.createEvent(t)), me = j((e, { id: t, input: n }) => e.updateEvent(t, n)), he = j((e, t) => e.modifyInstance(t.seriesId, t.date, {
			scope: t.scope,
			title: t.input.title,
			description: t.input.description,
			location: t.input.location,
			startUtc: t.input.startUtc,
			endUtc: t.input.endUtc
		})), ge = j((e, t) => e.deleteEvent(t)), _e = j((e, t) => e.deleteInstance(t.seriesId, t.date, t.scope)), ve = S(() => pe.isPending.value || me.isPending.value || he.isPending.value), $ = { onSuccess: () => Y.value = !1 };
		function ye(e) {
			let t = X.value;
			t ? t.seriesId ? Q.value = {
				mode: "edit",
				input: e
			} : me.mutate({
				id: t.id,
				input: e
			}, $) : pe.mutate(e, $);
		}
		function be() {
			let e = X.value;
			e && (e.seriesId ? Q.value = { mode: "delete" } : window.confirm(i.t("calendar.deleteConfirm", { name: e.title })) && ge.mutate(e.id, $));
		}
		function xe(e) {
			let t = X.value, n = Q.value;
			if (Q.value = null, !t?.seriesId || !t.occurrenceDate || !n) return;
			let r = t.occurrenceDate;
			n.mode === "delete" ? _e.mutate({
				seriesId: t.seriesId,
				date: r,
				scope: e
			}, $) : he.mutate({
				seriesId: t.seriesId,
				date: r,
				scope: e,
				input: n.input
			}, $);
		}
		let Se = d(!1);
		return (t, o) => {
			let u = n("Button"), d = n("SelectButton"), p = n("Select"), h = n("Message"), v = n("Skeleton");
			return O(), f("div", Re, [
				D("header", ze, [
					D("div", {
						class: "planner__nav",
						role: "group",
						"aria-label": b(i).t("calendar.navigate")
					}, [
						m(u, {
							icon: "pi pi-chevron-left",
							text: "",
							"aria-label": b(i).t("calendar.previous"),
							class: "planner__touch",
							onClick: o[0] ||= (e) => G(-1)
						}, null, 8, ["aria-label"]),
						m(u, {
							label: b(i).t("calendar.today"),
							text: "",
							class: "planner__touch",
							onClick: o[1] ||= (e) => I.value = /* @__PURE__ */ new Date()
						}, null, 8, ["label"]),
						m(u, {
							icon: "pi pi-chevron-right",
							text: "",
							"aria-label": b(i).t("calendar.next"),
							class: "planner__touch",
							onClick: o[2] ||= (e) => G(1)
						}, null, 8, ["aria-label"])
					], 8, Be),
					D("h2", Ve, c(te.value), 1),
					m(d, {
						modelValue: N.value,
						"onUpdate:modelValue": o[3] ||= (e) => N.value = e,
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
					b(k).data.value?.length ? (O(), _(p, {
						key: 0,
						modelValue: z.value,
						"onUpdate:modelValue": o[4] ||= (e) => z.value = e,
						options: ae.value,
						"option-label": "displayName",
						"option-value": "userId",
						"aria-label": b(i).t("calendar.person")
					}, null, 8, [
						"modelValue",
						"options",
						"aria-label"
					])) : g("", !0),
					b(x) ? (O(), _(u, {
						key: 1,
						icon: "pi pi-plus",
						label: b(i).t("calendar.newEvent"),
						class: "planner__touch",
						onClick: o[5] ||= (e) => de()
					}, null, 8, ["label"])) : g("", !0),
					m(u, {
						icon: "pi pi-cog",
						text: "",
						"aria-label": b(i).t("calendar.settings.title"),
						class: "planner__touch",
						onClick: o[6] ||= (e) => Se.value = !0
					}, null, 8, ["aria-label"])
				]),
				D("fieldset", He, [D("legend", Ue, c(b(i).t("calendar.layersLabel")), 1), (O(!0), f(w, null, r(ie.value, (t) => (O(), f("label", {
					key: t.value,
					class: "planner__layer"
				}, [l(D("input", {
					"onUpdate:modelValue": o[7] ||= (e) => L.value = e,
					type: "checkbox",
					value: t.value
				}, null, 8, We), [[e, L.value]]), A(" " + c(t.label), 1)]))), 128))]),
				(O(!0), f(w, null, r(se.value, (e) => (O(), _(h, {
					key: e.id,
					severity: "warn",
					closable: !1,
					role: "alert"
				}, {
					default: s(() => [A(c(b(i).t("calendar.settings.stale", { when: ce(e.lastSyncedAt) })) + " (" + c(e.name) + ") ", 1)]),
					_: 2
				}, 1024))), 128)),
				b(x) ? g("", !0) : (O(), f("p", Ge, c(b(i).t("calendar.readOnly")), 1)),
				b(H).isPending.value ? (O(), f("div", Ke, [(O(!0), f(w, null, r(N.value === "agenda" ? 5 : 6, (e) => (O(), _(v, {
					key: e,
					height: N.value === "agenda" ? "3rem" : "6rem"
				}, null, 8, ["height"]))), 128))])) : b(H).isError.value ? (O(), _(h, {
					key: 2,
					severity: "error",
					role: "alert"
				}, {
					default: s(() => [A(c(b(i).errorMessage(b(H).error.value)) + " ", 1), m(u, {
						label: b(i).t("calendar.retry"),
						text: "",
						onClick: o[8] ||= (e) => b(H).refetch()
					}, null, 8, ["label"])]),
					_: 1
				})) : U.value.length === 0 && !z.value ? (O(), f("div", qe, [
					o[14] ||= D("i", {
						class: "pi pi-calendar",
						"aria-hidden": "true"
					}, null, -1),
					D("p", null, c(b(i).t("calendar.empty")), 1),
					b(x) ? (O(), _(u, {
						key: 0,
						label: b(i).t("calendar.addFirst"),
						onClick: o[9] ||= (e) => de()
					}, null, 8, ["label"])) : g("", !0)
				])) : N.value === "agenda" ? (O(), _(ee, {
					key: 4,
					days: B.value,
					items: U.value,
					"can-toggle": K,
					onOpen: fe,
					onToggle: b(q).mutate
				}, null, 8, [
					"days",
					"items",
					"onToggle"
				])) : (O(), _(le, {
					key: 5,
					days: B.value,
					items: U.value,
					month: N.value === "month" ? I.value.getMonth() : null,
					columns: ne.value,
					"can-write": b(x),
					"can-toggle": K,
					onOpen: fe,
					onToggle: b(q).mutate,
					onCreate: de,
					onDrop: o[10] ||= (e, t) => b(J).mutate({
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
				m(Fe, {
					visible: Y.value,
					"onUpdate:visible": o[11] ||= (e) => Y.value = e,
					event: X.value,
					day: Z.value,
					members: b(k).data.value ?? [],
					"can-edit": ue.value,
					"can-delete": ue.value,
					saving: ve.value,
					onSave: ye,
					onDelete: be
				}, null, 8, [
					"visible",
					"event",
					"day",
					"members",
					"can-edit",
					"can-delete",
					"saving"
				]),
				m(Le, {
					visible: Q.value !== null,
					mode: Q.value?.mode ?? "edit",
					"onUpdate:visible": o[12] ||= (e) => Q.value = null,
					onChoose: xe
				}, null, 8, ["visible", "mode"]),
				m(W, {
					visible: Se.value,
					"onUpdate:visible": o[13] ||= (e) => Se.value = e,
					"household-id": a.value
				}, null, 8, ["visible", "household-id"])
			]);
		};
	}
}), [["__scopeId", "data-v-2e8c6d21"]]);
export { Je as default };
