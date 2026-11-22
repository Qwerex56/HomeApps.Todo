import { A as e, D as t, E as n, L as r, N as i, O as a, P as o, S as s, T as c, _ as l, b as u, c as d, f, g as p, h as m, i as h, k as g, l as _, m as v, n as y, p as b, r as x, t as S, u as C, v as w, x as T, y as E } from "./_plugin-vue_export-helper-BiWouYOk.js";
import { t as ee } from "./SeriesScopeDialog-D1dDKhuj.js";
import { t as D } from "./TaskRow-BkXZN4_D.js";
// src/ListDialog.vue?vue&type=script&setup=true&lang.ts
var O = { class: "todo-list-form__actions" }, te = /*#__PURE__*/ S(/* @__PURE__ */ T({
	__name: "ListDialog",
	props: {
		visible: { type: Boolean },
		mode: {},
		name: {},
		canShare: { type: Boolean }
	},
	emits: ["update:visible", "submit"],
	setup(t, { emit: s }) {
		let d = t, h = s, _ = f(), v = n(""), y = n("Private");
		i(() => d.visible, (e) => {
			e && (v.value = d.name ?? "", y.value = d.canShare ? "Shared" : "Private");
		});
		let x = [{
			value: "Shared",
			label: _.t("visibility.Shared")
		}, {
			value: "Private",
			label: _.t("visibility.Private")
		}];
		return (n, i) => {
			let s = a("InputText"), d = a("SelectButton"), f = a("Button"), S = a("Dialog");
			return c(), m(S, {
				visible: t.visible,
				modal: "",
				header: t.mode === "create" ? e(_).t("newList") : e(_).t("renameList"),
				style: {
					width: "24rem",
					maxWidth: "calc(100vw - 2rem)"
				},
				"onUpdate:visible": i[4] ||= (e) => h("update:visible", e)
			}, {
				default: o(() => [w("form", {
					class: "todo-list-form",
					onSubmit: i[3] ||= r((e) => h("submit", v.value, y.value), ["prevent"])
				}, [
					u(s, {
						modelValue: v.value,
						"onUpdate:modelValue": i[0] ||= (e) => v.value = e,
						placeholder: e(_).t("fields.listName"),
						maxlength: "40",
						required: "",
						autofocus: ""
					}, null, 8, ["modelValue", "placeholder"]),
					t.mode === "create" ? (c(), l(b, { key: 0 }, [t.canShare ? (c(), m(d, {
						key: 0,
						modelValue: y.value,
						"onUpdate:modelValue": i[1] ||= (e) => y.value = e,
						options: x,
						"option-label": "label",
						"option-value": "value",
						"allow-empty": !1
					}, null, 8, ["modelValue"])) : p("", !0), w("small", null, g(e(_).t(`visibilityHint.${y.value}`)), 1)], 64)) : p("", !0),
					w("div", O, [u(f, {
						type: "button",
						severity: "secondary",
						text: "",
						label: e(_).t("cancel"),
						onClick: i[2] ||= (e) => h("update:visible", !1)
					}, null, 8, ["label"]), u(f, {
						type: "submit",
						label: t.mode === "create" ? e(_).t("create") : e(_).t("save")
					}, null, 8, ["label"])])
				], 32)]),
				_: 1
			}, 8, ["visible", "header"]);
		};
	}
}), [["__scopeId", "data-v-25f3ed2f"]]), k = { class: "todo-editor__field" }, A = { class: "todo-editor__field" }, j = { class: "todo-editor__row" }, M = { class: "todo-editor__field" }, N = { class: "todo-editor__field todo-editor__time" }, P = { class: "todo-editor__field" }, F = { class: "todo-editor__field" }, I = {
	key: 0,
	class: "todo-editor__field"
}, L = {
	key: 1,
	class: "todo-editor__field"
}, R = { class: "todo-editor__field" }, ne = { class: "todo-editor__actions" }, re = /*#__PURE__*/ S(/* @__PURE__ */ T({
	__name: "TaskEditor",
	props: {
		task: {},
		lists: {},
		members: {},
		userId: {},
		canEdit: { type: Boolean },
		canDelete: { type: Boolean },
		saving: { type: Boolean }
	},
	emits: [
		"save",
		"delete",
		"close"
	],
	setup(t, { emit: o }) {
		let s = [
			"FREQ=DAILY",
			"FREQ=WEEKLY",
			"FREQ=MONTHLY",
			"FREQ=YEARLY"
		], y = t, b = o, x = f(), S = n(h(y.task)), C = n(y.task.listId);
		i(() => y.task, (e) => {
			S.value = h(e), C.value = e.listId;
		});
		let T = v(() => y.lists.find((e) => e.id === C.value)), E = v({
			get: () => S.value.dueDate ? d(S.value.dueDate) : null,
			set: (e) => {
				S.value.dueDate = e ? _(e) : null, e || (S.value.dueTime = null);
			}
		}), ee = v({
			get: () => S.value.dueTime?.slice(0, 5) ?? "",
			set: (e) => S.value.dueTime = e ? `${e}:00` : null
		}), D = v(() => [
			"Low",
			"Normal",
			"High"
		].map((e) => ({
			value: e,
			label: x.t(`priority.${e}`)
		}))), O = v(() => {
			let e = T.value?.visibility === "Private" ? y.members.filter((e) => e.userId === y.userId) : y.members;
			return [{
				userId: null,
				displayName: x.t("nobody")
			}, ...e.map((e) => ({
				userId: e.userId,
				displayName: e.displayName
			}))];
		}), te = v(() => [
			{
				value: null,
				label: x.t("recurrence.none")
			},
			...s.map((e) => ({
				value: e,
				label: x.t(`recurrence.${e}`)
			})),
			...S.value.rrule && !s.includes(S.value.rrule) ? [{
				value: S.value.rrule,
				label: x.t("recurrence.custom")
			}] : []
		]), re = v(() => ["FromDueDate", "FromCompletionDate"].map((e) => ({
			value: e,
			label: x.t(`recurrence.${e}`)
		})));
		i(() => S.value.rrule, (e) => S.value.anchor = e ? S.value.anchor ?? "FromDueDate" : null);
		let ie = v(() => y.lists.filter((e) => !e.archived || e.id === y.task.listId));
		return (n, i) => {
			let o = a("InputText"), s = a("Textarea"), d = a("DatePicker"), f = a("SelectButton"), h = a("Select"), _ = a("Button");
			return c(), l("form", {
				class: "todo-editor",
				onSubmit: i[11] ||= r((e) => b("save", S.value, C.value), ["prevent"])
			}, [
				w("label", k, [w("span", null, g(e(x).t("fields.title")), 1), u(o, {
					modelValue: S.value.title,
					"onUpdate:modelValue": i[0] ||= (e) => S.value.title = e,
					disabled: !t.canEdit,
					maxlength: "200",
					required: "",
					autofocus: ""
				}, null, 8, ["modelValue", "disabled"])]),
				w("label", A, [w("span", null, g(e(x).t("fields.notes")), 1), u(s, {
					modelValue: S.value.notes,
					"onUpdate:modelValue": i[1] ||= (e) => S.value.notes = e,
					disabled: !t.canEdit,
					rows: "4",
					"auto-resize": "",
					maxlength: "2000"
				}, null, 8, ["modelValue", "disabled"])]),
				w("div", j, [w("label", M, [w("span", null, g(e(x).t("fields.due")), 1), u(d, {
					modelValue: E.value,
					"onUpdate:modelValue": i[2] ||= (e) => E.value = e,
					disabled: !t.canEdit,
					"show-button-bar": "",
					"date-format": "dd.mm.yy",
					"show-icon": ""
				}, null, 8, ["modelValue", "disabled"])]), w("label", N, [w("span", null, g(e(x).t("fields.time")), 1), u(o, {
					modelValue: ee.value,
					"onUpdate:modelValue": i[3] ||= (e) => ee.value = e,
					type: "time",
					disabled: !t.canEdit || !S.value.dueDate
				}, null, 8, ["modelValue", "disabled"])])]),
				w("div", P, [w("span", null, g(e(x).t("fields.priority")), 1), u(f, {
					modelValue: S.value.priority,
					"onUpdate:modelValue": i[4] ||= (e) => S.value.priority = e,
					options: D.value,
					"option-label": "label",
					"option-value": "value",
					"allow-empty": !1,
					disabled: !t.canEdit
				}, null, 8, [
					"modelValue",
					"options",
					"disabled"
				])]),
				w("label", F, [w("span", null, g(e(x).t("recurrence.label")), 1), u(h, {
					modelValue: S.value.rrule,
					"onUpdate:modelValue": i[5] ||= (e) => S.value.rrule = e,
					options: te.value,
					"option-label": "label",
					"option-value": "value",
					disabled: !t.canEdit
				}, null, 8, [
					"modelValue",
					"options",
					"disabled"
				])]),
				S.value.rrule ? (c(), l("label", I, [w("span", null, g(e(x).t("recurrence.anchorLabel")), 1), u(h, {
					modelValue: S.value.anchor,
					"onUpdate:modelValue": i[6] ||= (e) => S.value.anchor = e,
					options: re.value,
					"option-label": "label",
					"option-value": "value",
					disabled: !t.canEdit
				}, null, 8, [
					"modelValue",
					"options",
					"disabled"
				])])) : p("", !0),
				t.members.length ? (c(), l("label", L, [w("span", null, g(e(x).t("fields.assignee")), 1), u(h, {
					modelValue: S.value.assigneeId,
					"onUpdate:modelValue": i[7] ||= (e) => S.value.assigneeId = e,
					placeholder: e(x).t("nobody"),
					options: O.value,
					"option-label": "displayName",
					"option-value": "userId",
					disabled: !t.canEdit
				}, null, 8, [
					"modelValue",
					"placeholder",
					"options",
					"disabled"
				])])) : p("", !0),
				w("label", R, [w("span", null, g(e(x).t("fields.list")), 1), u(h, {
					modelValue: C.value,
					"onUpdate:modelValue": i[8] ||= (e) => C.value = e,
					options: ie.value,
					"option-label": "name",
					"option-value": "id",
					disabled: !t.canEdit
				}, null, 8, [
					"modelValue",
					"options",
					"disabled"
				])]),
				w("div", ne, [
					t.canDelete ? (c(), m(_, {
						key: 0,
						type: "button",
						severity: "danger",
						text: "",
						icon: "pi pi-trash",
						label: e(x).t("delete"),
						onClick: i[9] ||= (e) => b("delete")
					}, null, 8, ["label"])) : p("", !0),
					i[12] ||= w("span", { class: "todo-editor__spacer" }, null, -1),
					u(_, {
						type: "button",
						severity: "secondary",
						text: "",
						label: e(x).t("cancel"),
						onClick: i[10] ||= (e) => b("close")
					}, null, 8, ["label"]),
					t.canEdit ? (c(), m(_, {
						key: 1,
						type: "submit",
						icon: "pi pi-check",
						label: e(x).t("save"),
						loading: t.saving
					}, null, 8, ["label", "loading"])) : p("", !0)
				])
			], 32);
		};
	}
}), [["__scopeId", "data-v-635beb8b"]]), ie = { class: "todo-page" }, ae = ["aria-label"], oe = ["onClick", "onDrop"], se = { class: "todo-nav__name" }, ce = ["onClick"], le = { class: "todo-main" }, ue = { class: "todo-phone-picker" }, de = { class: "todo-header" }, fe = ["title"], pe = {
	key: 2,
	class: "todo-loading"
}, me = {
	key: 0,
	class: "todo-empty"
}, he = {
	key: 0,
	class: "todo-tasks"
}, z = /*#__PURE__*/ S(/* @__PURE__ */ T({
	__name: "TasksPage",
	setup(d) {
		let h = f(), _ = y(() => h.household.value.id), { lists: S, userId: T, canWrite: O, canManage: k, memberName: A, change: j } = _, M = n(null), N = n(!1), P = n(!1), F = v(() => S.data.value ?? []), I = v(() => [...F.value.filter((e) => !e.archived)].sort((e, t) => e.visibility === t.visibility ? 0 : e.visibility === "Shared" ? -1 : 1)), L = v(() => F.value.filter((e) => e.archived));
		i(() => S.data.value, (e) => {
			if (!e) return;
			let t = M.value;
			if (t?.kind === "list" && e.some((e) => e.id === t.id) || t && t.kind !== "list") return;
			let n = I.value[0];
			M.value = n ? {
				kind: "list",
				id: n.id
			} : { kind: "today" };
		}, { immediate: !0 });
		let R = v(() => {
			let e = M.value;
			return e?.kind === "list" ? F.value.find((t) => t.id === e.id) ?? null : null;
		}), ne = v(() => {
			let e = M.value;
			return e?.kind === "list" ? { list: e.id } : e?.kind === "mine" ? { assignee: "me" } : { dueBy: C() };
		}), z = _.tasks(ne, () => M.value !== null), ge = v(() => (z.data.value ?? []).filter((e) => e.status === "Open")), B = v(() => (z.data.value ?? []).filter((e) => e.status === "Done").sort((e, t) => (t.completedAt ?? "").localeCompare(e.completedAt ?? ""))), _e = (e) => F.value.find((t) => t.id === e)?.name ?? null, V = v(() => M.value?.kind !== "list"), ve = (e) => !!e && (e.visibility === "Shared" ? k.value : e.ownerId === T.value), H = (e) => O.value && !!e && !e.archived, ye = (e) => {
			let t = F.value.find((t) => t.id === e.listId);
			return H(t) ? t.visibility === "Private" || e.createdBy === T.value || k.value : !1;
		}, be = v(() => [
			{
				key: "today",
				label: h.t("views.today")
			},
			...O.value ? [{
				key: "mine",
				label: h.t("views.mine")
			}] : [],
			...I.value.map((e) => ({
				key: e.id,
				label: e.name
			})),
			...L.value.map((e) => ({
				key: e.id,
				label: `${e.name} (${h.t("archived")})`
			}))
		]), U = v({
			get: () => M.value?.kind === "list" ? M.value.id : M.value?.kind ?? "today",
			set: (e) => W(e === "today" || e === "mine" ? { kind: e } : {
				kind: "list",
				id: e
			})
		});
		function W(e) {
			M.value = e, P.value = !1;
		}
		let G = (e) => U.value === e, K = n(""), xe = j((e, { listId: t, input: n }) => e.createTask(t, n));
		function Se() {
			let e = K.value.trim();
			e && R.value && (xe.mutate({
				listId: R.value.id,
				input: x(e)
			}), K.value = "");
		}
		let Ce = j((e, t) => t.status === "Done" ? e.reopenTask(t.id) : e.completeTask(t.id)), q = n(null), J = j(async (e, { task: t, input: n, listId: r, scope: i }) => {
			await e.updateTask(t.id, n, i), r !== t.listId && await e.moveTask(t.id, r, null);
		}), Y = j((e, { task: t, scope: n }) => e.deleteTask(t.id, n)), X = n(null);
		function we(e, t) {
			q.value && (q.value.rrule ? X.value = {
				mode: "edit",
				input: e,
				listId: t
			} : J.mutate({
				task: q.value,
				input: e,
				listId: t
			}, { onSuccess: () => q.value = null }));
		}
		function Te() {
			q.value && (q.value.rrule ? X.value = { mode: "delete" } : Y.mutate({ task: q.value }, { onSuccess: () => q.value = null }));
		}
		function Ee(e) {
			let t = q.value, n = X.value;
			if (X.value = null, !t || !n) return;
			let r = { onSuccess: () => q.value = null };
			n.mode === "edit" ? J.mutate({
				task: t,
				input: n.input,
				listId: n.listId,
				scope: e
			}, r) : Y.mutate({
				task: t,
				scope: e
			}, r);
		}
		let Z = n(null), De = j((e, { task: t, listId: n, before: r }) => e.moveTask(t.id, n, r)), Oe = v(() => H(R.value) && !V.value);
		function ke(e, t) {
			let n = Z.value;
			if (Z.value = null, !n || n.id === t) return;
			let r = F.value.find((t) => t.id === e);
			H(r) && De.mutate({
				task: n,
				listId: e,
				before: t
			});
		}
		let Q = n(null), Ae = j((e, { name: t, visibility: n }) => e.createList(t, n)), je = j((e, { id: t, name: n }) => e.renameList(t, n));
		function Me(e, t) {
			Q.value?.mode === "rename" && R.value ? je.mutate({
				id: R.value.id,
				name: e
			}) : Ae.mutate({
				name: e,
				visibility: t
			}, { onSuccess: (e) => W({
				kind: "list",
				id: e
			}) }), Q.value = null;
		}
		let Ne = j((e, t) => t.archived ? e.restoreList(t.id) : e.archiveList(t.id)), Pe = j((e, t) => e.deleteList(t.id)), $ = n(!1);
		function Fe() {
			let e = R.value;
			$.value = !1, e && Pe.mutate(e, { onSuccess: () => M.value = null });
		}
		let Ie = n(null), Le = v(() => {
			let e = R.value;
			return e ? [
				{
					label: h.t("renameList"),
					icon: "pi pi-pencil",
					disabled: e.archived,
					command: () => Q.value = { mode: "rename" }
				},
				{
					label: e.archived ? h.t("restoreList") : h.t("archiveList"),
					icon: e.archived ? "pi pi-replay" : "pi pi-inbox",
					command: () => Ne.mutate(e)
				},
				{
					label: h.t("deleteList"),
					icon: "pi pi-trash",
					command: () => $.value = !0
				}
			] : [];
		}), Re = v(() => M.value?.kind === "today" ? h.t("views.today") : M.value?.kind === "mine" ? h.t("views.mine") : R.value?.name ?? "");
		return (n, i) => {
			let d = a("Divider"), f = a("Badge"), v = a("Button"), y = a("Select"), x = a("Tag"), S = a("Menu"), C = a("InputText"), j = a("Message"), ne = a("Skeleton"), xe = a("Drawer"), Y = a("Dialog");
			return c(), l("div", ie, [
				w("nav", {
					class: "todo-nav",
					"aria-label": e(h).t("lists")
				}, [
					w("button", {
						type: "button",
						class: s({ active: G("today") }),
						onClick: i[0] ||= (e) => W({ kind: "today" })
					}, [i[19] ||= w("i", {
						class: "pi pi-sun",
						"aria-hidden": "true"
					}, null, -1), E(g(e(h).t("views.today")), 1)], 2),
					e(O) ? (c(), l("button", {
						key: 0,
						type: "button",
						class: s({ active: G("mine") }),
						onClick: i[1] ||= (e) => W({ kind: "mine" })
					}, [i[20] ||= w("i", {
						class: "pi pi-user",
						"aria-hidden": "true"
					}, null, -1), E(g(e(h).t("views.mine")), 1)], 2)) : p("", !0),
					u(d),
					(c(!0), l(b, null, t(I.value, (e) => (c(), l("button", {
						key: e.id,
						type: "button",
						class: s({ active: G(e.id) }),
						onClick: (t) => W({
							kind: "list",
							id: e.id
						}),
						onDragover: i[2] ||= r(() => {}, ["prevent"]),
						onDrop: r((t) => ke(e.id, null), ["prevent"])
					}, [
						w("i", {
							class: s(["pi", e.visibility === "Shared" ? "pi-users" : "pi-lock"]),
							"aria-hidden": "true"
						}, null, 2),
						w("span", se, g(e.name), 1),
						e.openTasks ? (c(), m(f, {
							key: 0,
							value: e.openTasks,
							severity: "secondary"
						}, null, 8, ["value"])) : p("", !0)
					], 42, oe))), 128)),
					e(O) ? (c(), m(v, {
						key: 1,
						text: "",
						size: "small",
						icon: "pi pi-plus",
						label: e(h).t("newList"),
						onClick: i[3] ||= (e) => Q.value = { mode: "create" }
					}, null, 8, ["label"])) : p("", !0),
					L.value.length ? (c(), l(b, { key: 2 }, [u(v, {
						text: "",
						size: "small",
						severity: "secondary",
						icon: N.value ? "pi pi-chevron-down" : "pi pi-chevron-right",
						label: `${e(h).t("archive")} (${L.value.length})`,
						onClick: i[4] ||= (e) => N.value = !N.value
					}, null, 8, ["icon", "label"]), N.value ? (c(!0), l(b, { key: 0 }, t(L.value, (e) => (c(), l("button", {
						key: e.id,
						type: "button",
						class: s(["todo-nav__archived", { active: G(e.id) }]),
						onClick: (t) => W({
							kind: "list",
							id: e.id
						})
					}, [i[21] ||= w("i", {
						class: "pi pi-inbox",
						"aria-hidden": "true"
					}, null, -1), E(g(e.name), 1)], 10, ce))), 128)) : p("", !0)], 64)) : p("", !0)
				], 8, ae),
				w("section", le, [
					w("div", ue, [u(y, {
						modelValue: U.value,
						"onUpdate:modelValue": i[5] ||= (e) => U.value = e,
						options: be.value,
						"option-label": "label",
						"option-value": "key",
						class: "w-full"
					}, null, 8, ["modelValue", "options"]), e(O) ? (c(), m(v, {
						key: 0,
						icon: "pi pi-plus",
						text: "",
						"aria-label": e(h).t("newList"),
						onClick: i[6] ||= (e) => Q.value = { mode: "create" }
					}, null, 8, ["aria-label"])) : p("", !0)]),
					w("header", de, [
						w("h2", null, [R.value ? (c(), l("i", {
							key: 0,
							class: s(["pi", R.value.visibility === "Shared" ? "pi-users" : "pi-lock"]),
							title: e(h).t(`visibility.${R.value.visibility}`)
						}, null, 10, fe)) : p("", !0), E(" " + g(Re.value), 1)]),
						R.value?.archived ? (c(), m(x, {
							key: 0,
							value: e(h).t("archived"),
							severity: "secondary"
						}, null, 8, ["value"])) : p("", !0),
						ve(R.value) ? (c(), l(b, { key: 1 }, [u(v, {
							icon: "pi pi-ellipsis-v",
							text: "",
							rounded: "",
							"aria-label": e(h).t("listActions"),
							onClick: i[7] ||= (e) => Ie.value?.toggle(e)
						}, null, 8, ["aria-label"]), u(S, {
							ref_key: "listMenu",
							ref: Ie,
							model: Le.value,
							popup: ""
						}, null, 8, ["model"])], 64)) : p("", !0)
					]),
					H(R.value) ? (c(), l("form", {
						key: 0,
						class: "todo-add",
						onSubmit: r(Se, ["prevent"])
					}, [u(C, {
						modelValue: K.value,
						"onUpdate:modelValue": i[8] ||= (e) => K.value = e,
						placeholder: e(h).t("addPlaceholder"),
						maxlength: "200",
						class: "todo-add__input"
					}, null, 8, ["modelValue", "placeholder"]), u(v, {
						type: "submit",
						icon: "pi pi-plus",
						"aria-label": e(h).t("add"),
						disabled: !K.value.trim()
					}, null, 8, ["aria-label", "disabled"])], 32)) : e(O) ? p("", !0) : (c(), m(j, {
						key: 1,
						severity: "secondary",
						size: "small"
					}, {
						default: o(() => [E(g(e(h).t("readOnly")), 1)]),
						_: 1
					})),
					e(z).isPending.value ? (c(), l("div", pe, [(c(), l(b, null, t(3, (e) => u(ne, {
						key: e,
						height: "2.5rem"
					})), 64))])) : (c(), l(b, { key: 3 }, [
						w("ul", {
							class: "todo-tasks",
							onDragover: i[10] ||= r(() => {}, ["prevent"]),
							onDrop: i[11] ||= r((e) => R.value && ke(R.value.id, null), ["prevent"])
						}, [(c(!0), l(b, null, t(ge.value, (t) => (c(), m(D, {
							key: t.id,
							task: t,
							"list-name": V.value ? _e(t.listId) : null,
							"assignee-name": e(A)(t.assigneeId),
							"can-toggle": H(F.value.find((e) => e.id === t.listId)),
							draggable: Oe.value,
							onToggle: (n) => e(Ce).mutate(t),
							onOpen: (e) => q.value = t,
							onDragstart: (e) => Z.value = t,
							onDragend: i[9] ||= (e) => Z.value = null,
							onDrop: r((e) => ke(t.listId, t.id), ["stop", "prevent"])
						}, null, 8, [
							"task",
							"list-name",
							"assignee-name",
							"can-toggle",
							"draggable",
							"onToggle",
							"onOpen",
							"onDragstart",
							"onDrop"
						]))), 128))], 32),
						ge.value.length ? p("", !0) : (c(), l("p", me, g(M.value?.kind === "today" ? e(h).t("empty.today") : M.value?.kind === "mine" ? e(h).t("empty.mine") : e(h).t("empty.list")), 1)),
						B.value.length ? (c(), l(b, { key: 1 }, [u(v, {
							text: "",
							size: "small",
							severity: "secondary",
							icon: P.value ? "pi pi-chevron-down" : "pi pi-chevron-right",
							label: e(h).t("doneCount", { count: B.value.length }),
							onClick: i[12] ||= (e) => P.value = !P.value
						}, null, 8, ["icon", "label"]), P.value ? (c(), l("ul", he, [(c(!0), l(b, null, t(B.value, (t) => (c(), m(D, {
							key: t.id,
							task: t,
							"list-name": V.value ? _e(t.listId) : null,
							"assignee-name": e(A)(t.assigneeId),
							"can-toggle": H(F.value.find((e) => e.id === t.listId)),
							onToggle: (n) => e(Ce).mutate(t),
							onOpen: (e) => q.value = t
						}, null, 8, [
							"task",
							"list-name",
							"assignee-name",
							"can-toggle",
							"onToggle",
							"onOpen"
						]))), 128))])) : p("", !0)], 64)) : p("", !0)
					], 64))
				]),
				u(xe, {
					visible: q.value !== null,
					position: "right",
					header: e(h).t("editTask"),
					class: "todo-drawer",
					"onUpdate:visible": i[14] ||= (e) => !e && (q.value = null)
				}, {
					default: o(() => [q.value ? (c(), m(re, {
						key: 0,
						task: q.value,
						lists: F.value,
						members: e(_).members.data.value ?? [],
						"user-id": e(T),
						"can-edit": H(F.value.find((e) => e.id === q.value.listId)),
						"can-delete": ye(q.value),
						saving: e(J).isPending.value,
						onSave: we,
						onDelete: Te,
						onClose: i[13] ||= (e) => q.value = null
					}, null, 8, [
						"task",
						"lists",
						"members",
						"user-id",
						"can-edit",
						"can-delete",
						"saving"
					])) : p("", !0)]),
					_: 1
				}, 8, ["visible", "header"]),
				u(ee, {
					visible: X.value !== null,
					mode: X.value?.mode ?? "edit",
					"onUpdate:visible": i[15] ||= (e) => !e && (X.value = null),
					onChoose: Ee
				}, null, 8, ["visible", "mode"]),
				u(te, {
					visible: Q.value !== null,
					mode: Q.value?.mode ?? "create",
					name: Q.value?.mode === "rename" ? R.value?.name : "",
					"can-share": e(k),
					"onUpdate:visible": i[16] ||= (e) => !e && (Q.value = null),
					onSubmit: Me
				}, null, 8, [
					"visible",
					"mode",
					"name",
					"can-share"
				]),
				u(Y, {
					visible: $.value,
					"onUpdate:visible": i[18] ||= (e) => $.value = e,
					modal: "",
					header: e(h).t("deleteList"),
					style: {
						width: "24rem",
						maxWidth: "calc(100vw - 2rem)"
					}
				}, {
					footer: o(() => [u(v, {
						severity: "secondary",
						text: "",
						label: e(h).t("cancel"),
						onClick: i[17] ||= (e) => $.value = !1
					}, null, 8, ["label"]), u(v, {
						severity: "danger",
						icon: "pi pi-trash",
						label: e(h).t("delete"),
						onClick: Fe
					}, null, 8, ["label"])]),
					default: o(() => [w("p", null, g(e(h).t("deleteListConfirm", { name: R.value?.name ?? "" })), 1)]),
					_: 1
				}, 8, ["visible", "header"])
			]);
		};
	}
}), [["__scopeId", "data-v-08d13021"]]);
export { z as default };
