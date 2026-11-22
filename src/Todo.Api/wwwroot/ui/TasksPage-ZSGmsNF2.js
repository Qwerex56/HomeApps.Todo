import { D as e, E as t, I as n, M as r, N as i, O as a, S as o, T as s, _ as c, b as l, c as u, f as d, g as f, h as p, i as m, k as h, l as g, m as _, n as v, p as y, r as b, t as x, u as S, v as C, w, x as T, y as E } from "./_plugin-vue_export-helper-DRFww2dX.js";
import { t as D } from "./SeriesScopeDialog-B9qu5_34.js";
import { t as O } from "./TaskRow-BDB1gJUc.js";
// src/ListDialog.vue?vue&type=script&setup=true&lang.ts
var k = { class: "todo-list-form__actions" }, ee = /*#__PURE__*/ x(/* @__PURE__ */ T({
	__name: "ListDialog",
	props: {
		visible: { type: Boolean },
		mode: {},
		name: {},
		canShare: { type: Boolean }
	},
	emits: ["update:visible", "submit"],
	setup(t, { emit: o }) {
		let u = t, m = o, g = d(), _ = s(""), v = s("Private");
		r(() => u.visible, (e) => {
			e && (_.value = u.name ?? "", v.value = u.canShare ? "Shared" : "Private");
		});
		let b = [{
			value: "Shared",
			label: g.t("visibility.Shared")
		}, {
			value: "Private",
			label: g.t("visibility.Private")
		}];
		return (r, o) => {
			let s = e("InputText"), u = e("SelectButton"), d = e("Button"), x = e("Dialog");
			return w(), p(x, {
				visible: t.visible,
				modal: "",
				header: t.mode === "create" ? h(g).t("newList") : h(g).t("renameList"),
				style: {
					width: "24rem",
					maxWidth: "calc(100vw - 2rem)"
				},
				"onUpdate:visible": o[4] ||= (e) => m("update:visible", e)
			}, {
				default: i(() => [C("form", {
					class: "todo-list-form",
					onSubmit: o[3] ||= n((e) => m("submit", _.value, v.value), ["prevent"])
				}, [
					l(s, {
						modelValue: _.value,
						"onUpdate:modelValue": o[0] ||= (e) => _.value = e,
						placeholder: h(g).t("fields.listName"),
						maxlength: "40",
						required: "",
						autofocus: ""
					}, null, 8, ["modelValue", "placeholder"]),
					t.mode === "create" ? (w(), c(y, { key: 0 }, [t.canShare ? (w(), p(u, {
						key: 0,
						modelValue: v.value,
						"onUpdate:modelValue": o[1] ||= (e) => v.value = e,
						options: b,
						"option-label": "label",
						"option-value": "value",
						"allow-empty": !1
					}, null, 8, ["modelValue"])) : f("", !0), C("small", null, a(h(g).t(`visibilityHint.${v.value}`)), 1)], 64)) : f("", !0),
					C("div", k, [l(d, {
						type: "button",
						severity: "secondary",
						text: "",
						label: h(g).t("cancel"),
						onClick: o[2] ||= (e) => m("update:visible", !1)
					}, null, 8, ["label"]), l(d, {
						type: "submit",
						label: t.mode === "create" ? h(g).t("create") : h(g).t("save")
					}, null, 8, ["label"])])
				], 32)]),
				_: 1
			}, 8, ["visible", "header"]);
		};
	}
}), [["__scopeId", "data-v-25f3ed2f"]]), A = { class: "todo-editor__field" }, te = { class: "todo-editor__field" }, j = { class: "todo-editor__row" }, M = { class: "todo-editor__field" }, N = { class: "todo-editor__field todo-editor__time" }, P = { class: "todo-editor__field" }, F = { class: "todo-editor__field" }, I = {
	key: 0,
	class: "todo-editor__field"
}, L = {
	key: 1,
	class: "todo-editor__field"
}, R = { class: "todo-editor__field" }, z = { class: "todo-editor__actions" }, ne = /*#__PURE__*/ x(/* @__PURE__ */ T({
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
	setup(t, { emit: i }) {
		let o = [
			"FREQ=DAILY",
			"FREQ=WEEKLY",
			"FREQ=MONTHLY",
			"FREQ=YEARLY"
		], v = t, y = i, b = d(), x = s(m(v.task)), S = s(v.task.listId);
		r(() => v.task, (e) => {
			x.value = m(e), S.value = e.listId;
		});
		let T = _(() => v.lists.find((e) => e.id === S.value)), E = _({
			get: () => x.value.dueDate ? u(x.value.dueDate) : null,
			set: (e) => {
				x.value.dueDate = e ? g(e) : null, e || (x.value.dueTime = null);
			}
		}), D = _({
			get: () => x.value.dueTime?.slice(0, 5) ?? "",
			set: (e) => x.value.dueTime = e ? `${e}:00` : null
		}), O = _(() => [
			"Low",
			"Normal",
			"High"
		].map((e) => ({
			value: e,
			label: b.t(`priority.${e}`)
		}))), k = _(() => {
			let e = T.value?.visibility === "Private" ? v.members.filter((e) => e.userId === v.userId) : v.members;
			return [{
				userId: null,
				displayName: b.t("nobody")
			}, ...e.map((e) => ({
				userId: e.userId,
				displayName: e.displayName
			}))];
		}), ee = _(() => [
			{
				value: null,
				label: b.t("recurrence.none")
			},
			...o.map((e) => ({
				value: e,
				label: b.t(`recurrence.${e}`)
			})),
			...x.value.rrule && !o.includes(x.value.rrule) ? [{
				value: x.value.rrule,
				label: b.t("recurrence.custom")
			}] : []
		]), ne = _(() => ["FromDueDate", "FromCompletionDate"].map((e) => ({
			value: e,
			label: b.t(`recurrence.${e}`)
		})));
		r(() => x.value.rrule, (e) => x.value.anchor = e ? x.value.anchor ?? "FromDueDate" : null);
		let re = _(() => v.lists.filter((e) => !e.archived || e.id === v.task.listId));
		return (r, i) => {
			let o = e("InputText"), s = e("Textarea"), u = e("DatePicker"), d = e("SelectButton"), m = e("Select"), g = e("Button");
			return w(), c("form", {
				class: "todo-editor",
				onSubmit: i[11] ||= n((e) => y("save", x.value, S.value), ["prevent"])
			}, [
				C("label", A, [C("span", null, a(h(b).t("fields.title")), 1), l(o, {
					modelValue: x.value.title,
					"onUpdate:modelValue": i[0] ||= (e) => x.value.title = e,
					disabled: !t.canEdit,
					maxlength: "200",
					required: "",
					autofocus: ""
				}, null, 8, ["modelValue", "disabled"])]),
				C("label", te, [C("span", null, a(h(b).t("fields.notes")), 1), l(s, {
					modelValue: x.value.notes,
					"onUpdate:modelValue": i[1] ||= (e) => x.value.notes = e,
					disabled: !t.canEdit,
					rows: "4",
					"auto-resize": "",
					maxlength: "2000"
				}, null, 8, ["modelValue", "disabled"])]),
				C("div", j, [C("label", M, [C("span", null, a(h(b).t("fields.due")), 1), l(u, {
					modelValue: E.value,
					"onUpdate:modelValue": i[2] ||= (e) => E.value = e,
					disabled: !t.canEdit,
					"show-button-bar": "",
					"date-format": "dd.mm.yy",
					"show-icon": ""
				}, null, 8, ["modelValue", "disabled"])]), C("label", N, [C("span", null, a(h(b).t("fields.time")), 1), l(o, {
					modelValue: D.value,
					"onUpdate:modelValue": i[3] ||= (e) => D.value = e,
					type: "time",
					disabled: !t.canEdit || !x.value.dueDate
				}, null, 8, ["modelValue", "disabled"])])]),
				C("div", P, [C("span", null, a(h(b).t("fields.priority")), 1), l(d, {
					modelValue: x.value.priority,
					"onUpdate:modelValue": i[4] ||= (e) => x.value.priority = e,
					options: O.value,
					"option-label": "label",
					"option-value": "value",
					"allow-empty": !1,
					disabled: !t.canEdit
				}, null, 8, [
					"modelValue",
					"options",
					"disabled"
				])]),
				C("label", F, [C("span", null, a(h(b).t("recurrence.label")), 1), l(m, {
					modelValue: x.value.rrule,
					"onUpdate:modelValue": i[5] ||= (e) => x.value.rrule = e,
					options: ee.value,
					"option-label": "label",
					"option-value": "value",
					disabled: !t.canEdit
				}, null, 8, [
					"modelValue",
					"options",
					"disabled"
				])]),
				x.value.rrule ? (w(), c("label", I, [C("span", null, a(h(b).t("recurrence.anchorLabel")), 1), l(m, {
					modelValue: x.value.anchor,
					"onUpdate:modelValue": i[6] ||= (e) => x.value.anchor = e,
					options: ne.value,
					"option-label": "label",
					"option-value": "value",
					disabled: !t.canEdit
				}, null, 8, [
					"modelValue",
					"options",
					"disabled"
				])])) : f("", !0),
				t.members.length ? (w(), c("label", L, [C("span", null, a(h(b).t("fields.assignee")), 1), l(m, {
					modelValue: x.value.assigneeId,
					"onUpdate:modelValue": i[7] ||= (e) => x.value.assigneeId = e,
					placeholder: h(b).t("nobody"),
					options: k.value,
					"option-label": "displayName",
					"option-value": "userId",
					disabled: !t.canEdit
				}, null, 8, [
					"modelValue",
					"placeholder",
					"options",
					"disabled"
				])])) : f("", !0),
				C("label", R, [C("span", null, a(h(b).t("fields.list")), 1), l(m, {
					modelValue: S.value,
					"onUpdate:modelValue": i[8] ||= (e) => S.value = e,
					options: re.value,
					"option-label": "name",
					"option-value": "id",
					disabled: !t.canEdit
				}, null, 8, [
					"modelValue",
					"options",
					"disabled"
				])]),
				C("div", z, [
					t.canDelete ? (w(), p(g, {
						key: 0,
						type: "button",
						severity: "danger",
						text: "",
						icon: "pi pi-trash",
						label: h(b).t("delete"),
						onClick: i[9] ||= (e) => y("delete")
					}, null, 8, ["label"])) : f("", !0),
					i[12] ||= C("span", { class: "todo-editor__spacer" }, null, -1),
					l(g, {
						type: "button",
						severity: "secondary",
						text: "",
						label: h(b).t("cancel"),
						onClick: i[10] ||= (e) => y("close")
					}, null, 8, ["label"]),
					t.canEdit ? (w(), p(g, {
						key: 1,
						type: "submit",
						icon: "pi pi-check",
						label: h(b).t("save"),
						loading: t.saving
					}, null, 8, ["label", "loading"])) : f("", !0)
				])
			], 32);
		};
	}
}), [["__scopeId", "data-v-635beb8b"]]), re = { class: "todo-page" }, ie = ["aria-label"], ae = ["onClick", "onDrop"], oe = { class: "todo-nav__name" }, se = ["onClick"], ce = { class: "todo-main" }, le = { class: "todo-phone-picker" }, ue = { class: "todo-header" }, de = ["title"], fe = {
	key: 2,
	class: "todo-loading"
}, pe = {
	key: 0,
	class: "todo-empty"
}, me = {
	key: 0,
	class: "todo-tasks"
}, B = /*#__PURE__*/ x(/* @__PURE__ */ T({
	__name: "TasksPage",
	setup(u) {
		let m = d(), g = v(() => m.household.value.id), { lists: x, userId: T, canWrite: k, canManage: A, memberName: te, change: j } = g, M = s(null), N = s(!1), P = s(!1), F = _(() => x.data.value ?? []), I = _(() => [...F.value.filter((e) => !e.archived)].sort((e, t) => e.visibility === t.visibility ? 0 : e.visibility === "Shared" ? -1 : 1)), L = _(() => F.value.filter((e) => e.archived));
		r(() => x.data.value, (e) => {
			if (!e) return;
			let t = M.value;
			if (t?.kind === "list" && e.some((e) => e.id === t.id) || t && t.kind !== "list") return;
			let n = I.value[0];
			M.value = n ? {
				kind: "list",
				id: n.id
			} : { kind: "today" };
		}, { immediate: !0 });
		let R = _(() => {
			let e = M.value;
			return e?.kind === "list" ? F.value.find((t) => t.id === e.id) ?? null : null;
		}), z = _(() => {
			let e = M.value;
			return e?.kind === "list" ? { list: e.id } : e?.kind === "mine" ? { assignee: "me" } : { dueBy: S() };
		}), B = g.tasks(z, () => M.value !== null), he = _(() => (B.data.value ?? []).filter((e) => e.status === "Open")), ge = _(() => (B.data.value ?? []).filter((e) => e.status === "Done").sort((e, t) => (t.completedAt ?? "").localeCompare(e.completedAt ?? ""))), _e = (e) => F.value.find((t) => t.id === e)?.name ?? null, ve = _(() => M.value?.kind !== "list"), ye = (e) => !!e && (e.visibility === "Shared" ? A.value : e.ownerId === T.value), V = (e) => k.value && !!e && !e.archived, be = (e) => {
			let t = F.value.find((t) => t.id === e.listId);
			return V(t) ? t.visibility === "Private" || e.createdBy === T.value || A.value : !1;
		}, xe = _(() => [
			{
				key: "today",
				label: m.t("views.today")
			},
			...k.value ? [{
				key: "mine",
				label: m.t("views.mine")
			}] : [],
			...I.value.map((e) => ({
				key: e.id,
				label: e.name
			})),
			...L.value.map((e) => ({
				key: e.id,
				label: `${e.name} (${m.t("archived")})`
			}))
		]), H = _({
			get: () => M.value?.kind === "list" ? M.value.id : M.value?.kind ?? "today",
			set: (e) => U(e === "today" || e === "mine" ? { kind: e } : {
				kind: "list",
				id: e
			})
		});
		function U(e) {
			M.value = e, P.value = !1;
		}
		let W = (e) => H.value === e, G = s(""), Se = j((e, { listId: t, input: n }) => e.createTask(t, n));
		function Ce() {
			let e = G.value.trim();
			e && R.value && (Se.mutate({
				listId: R.value.id,
				input: b(e)
			}), G.value = "");
		}
		let we = j((e, t) => t.status === "Done" ? e.reopenTask(t.id) : e.completeTask(t.id)), K = s(null), q = j(async (e, { task: t, input: n, listId: r, scope: i }) => {
			await e.updateTask(t.id, n, i), r !== t.listId && await e.moveTask(t.id, r, null);
		}), J = j((e, { task: t, scope: n }) => e.deleteTask(t.id, n)), Y = s(null);
		function Te(e, t) {
			K.value && (K.value.rrule ? Y.value = {
				mode: "edit",
				input: e,
				listId: t
			} : q.mutate({
				task: K.value,
				input: e,
				listId: t
			}, { onSuccess: () => K.value = null }));
		}
		function Ee() {
			K.value && (K.value.rrule ? Y.value = { mode: "delete" } : J.mutate({ task: K.value }, { onSuccess: () => K.value = null }));
		}
		function De(e) {
			let t = K.value, n = Y.value;
			if (Y.value = null, !t || !n) return;
			let r = { onSuccess: () => K.value = null };
			n.mode === "edit" ? q.mutate({
				task: t,
				input: n.input,
				listId: n.listId,
				scope: e
			}, r) : J.mutate({
				task: t,
				scope: e
			}, r);
		}
		let X = s(null), Oe = j((e, { task: t, listId: n, before: r }) => e.moveTask(t.id, n, r)), ke = _(() => V(R.value) && !ve.value);
		function Z(e, t) {
			let n = X.value;
			if (X.value = null, !n || n.id === t) return;
			let r = F.value.find((t) => t.id === e);
			V(r) && Oe.mutate({
				task: n,
				listId: e,
				before: t
			});
		}
		let Q = s(null), Ae = j((e, { name: t, visibility: n }) => e.createList(t, n)), je = j((e, { id: t, name: n }) => e.renameList(t, n));
		function Me(e, t) {
			Q.value?.mode === "rename" && R.value ? je.mutate({
				id: R.value.id,
				name: e
			}) : Ae.mutate({
				name: e,
				visibility: t
			}, { onSuccess: (e) => U({
				kind: "list",
				id: e
			}) }), Q.value = null;
		}
		let Ne = j((e, t) => t.archived ? e.restoreList(t.id) : e.archiveList(t.id)), Pe = j((e, t) => e.deleteList(t.id)), $ = s(!1);
		function Fe() {
			let e = R.value;
			$.value = !1, e && Pe.mutate(e, { onSuccess: () => M.value = null });
		}
		let Ie = s(null), Le = _(() => {
			let e = R.value;
			return e ? [
				{
					label: m.t("renameList"),
					icon: "pi pi-pencil",
					disabled: e.archived,
					command: () => Q.value = { mode: "rename" }
				},
				{
					label: e.archived ? m.t("restoreList") : m.t("archiveList"),
					icon: e.archived ? "pi pi-replay" : "pi pi-inbox",
					command: () => Ne.mutate(e)
				},
				{
					label: m.t("deleteList"),
					icon: "pi pi-trash",
					command: () => $.value = !0
				}
			] : [];
		}), Re = _(() => M.value?.kind === "today" ? m.t("views.today") : M.value?.kind === "mine" ? m.t("views.mine") : R.value?.name ?? "");
		return (r, s) => {
			let u = e("Divider"), d = e("Badge"), _ = e("Button"), v = e("Select"), b = e("Tag"), x = e("Menu"), S = e("InputText"), j = e("Message"), z = e("Skeleton"), Se = e("Drawer"), J = e("Dialog");
			return w(), c("div", re, [
				C("nav", {
					class: "todo-nav",
					"aria-label": h(m).t("lists")
				}, [
					C("button", {
						type: "button",
						class: o({ active: W("today") }),
						onClick: s[0] ||= (e) => U({ kind: "today" })
					}, [s[19] ||= C("i", {
						class: "pi pi-sun",
						"aria-hidden": "true"
					}, null, -1), E(a(h(m).t("views.today")), 1)], 2),
					h(k) ? (w(), c("button", {
						key: 0,
						type: "button",
						class: o({ active: W("mine") }),
						onClick: s[1] ||= (e) => U({ kind: "mine" })
					}, [s[20] ||= C("i", {
						class: "pi pi-user",
						"aria-hidden": "true"
					}, null, -1), E(a(h(m).t("views.mine")), 1)], 2)) : f("", !0),
					l(u),
					(w(!0), c(y, null, t(I.value, (e) => (w(), c("button", {
						key: e.id,
						type: "button",
						class: o({ active: W(e.id) }),
						onClick: (t) => U({
							kind: "list",
							id: e.id
						}),
						onDragover: s[2] ||= n(() => {}, ["prevent"]),
						onDrop: n((t) => Z(e.id, null), ["prevent"])
					}, [
						C("i", {
							class: o(["pi", e.visibility === "Shared" ? "pi-users" : "pi-lock"]),
							"aria-hidden": "true"
						}, null, 2),
						C("span", oe, a(e.name), 1),
						e.openTasks ? (w(), p(d, {
							key: 0,
							value: e.openTasks,
							severity: "secondary"
						}, null, 8, ["value"])) : f("", !0)
					], 42, ae))), 128)),
					h(k) ? (w(), p(_, {
						key: 1,
						text: "",
						size: "small",
						icon: "pi pi-plus",
						label: h(m).t("newList"),
						onClick: s[3] ||= (e) => Q.value = { mode: "create" }
					}, null, 8, ["label"])) : f("", !0),
					L.value.length ? (w(), c(y, { key: 2 }, [l(_, {
						text: "",
						size: "small",
						severity: "secondary",
						icon: N.value ? "pi pi-chevron-down" : "pi pi-chevron-right",
						label: `${h(m).t("archive")} (${L.value.length})`,
						onClick: s[4] ||= (e) => N.value = !N.value
					}, null, 8, ["icon", "label"]), N.value ? (w(!0), c(y, { key: 0 }, t(L.value, (e) => (w(), c("button", {
						key: e.id,
						type: "button",
						class: o(["todo-nav__archived", { active: W(e.id) }]),
						onClick: (t) => U({
							kind: "list",
							id: e.id
						})
					}, [s[21] ||= C("i", {
						class: "pi pi-inbox",
						"aria-hidden": "true"
					}, null, -1), E(a(e.name), 1)], 10, se))), 128)) : f("", !0)], 64)) : f("", !0)
				], 8, ie),
				C("section", ce, [
					C("div", le, [l(v, {
						modelValue: H.value,
						"onUpdate:modelValue": s[5] ||= (e) => H.value = e,
						options: xe.value,
						"option-label": "label",
						"option-value": "key",
						class: "w-full"
					}, null, 8, ["modelValue", "options"]), h(k) ? (w(), p(_, {
						key: 0,
						icon: "pi pi-plus",
						text: "",
						"aria-label": h(m).t("newList"),
						onClick: s[6] ||= (e) => Q.value = { mode: "create" }
					}, null, 8, ["aria-label"])) : f("", !0)]),
					C("header", ue, [
						C("h2", null, [R.value ? (w(), c("i", {
							key: 0,
							class: o(["pi", R.value.visibility === "Shared" ? "pi-users" : "pi-lock"]),
							title: h(m).t(`visibility.${R.value.visibility}`)
						}, null, 10, de)) : f("", !0), E(" " + a(Re.value), 1)]),
						R.value?.archived ? (w(), p(b, {
							key: 0,
							value: h(m).t("archived"),
							severity: "secondary"
						}, null, 8, ["value"])) : f("", !0),
						ye(R.value) ? (w(), c(y, { key: 1 }, [l(_, {
							icon: "pi pi-ellipsis-v",
							text: "",
							rounded: "",
							"aria-label": h(m).t("listActions"),
							onClick: s[7] ||= (e) => Ie.value?.toggle(e)
						}, null, 8, ["aria-label"]), l(x, {
							ref_key: "listMenu",
							ref: Ie,
							model: Le.value,
							popup: ""
						}, null, 8, ["model"])], 64)) : f("", !0)
					]),
					V(R.value) ? (w(), c("form", {
						key: 0,
						class: "todo-add",
						onSubmit: n(Ce, ["prevent"])
					}, [l(S, {
						modelValue: G.value,
						"onUpdate:modelValue": s[8] ||= (e) => G.value = e,
						placeholder: h(m).t("addPlaceholder"),
						maxlength: "200",
						class: "todo-add__input"
					}, null, 8, ["modelValue", "placeholder"]), l(_, {
						type: "submit",
						icon: "pi pi-plus",
						"aria-label": h(m).t("add"),
						disabled: !G.value.trim()
					}, null, 8, ["aria-label", "disabled"])], 32)) : h(k) ? f("", !0) : (w(), p(j, {
						key: 1,
						severity: "secondary",
						size: "small"
					}, {
						default: i(() => [E(a(h(m).t("readOnly")), 1)]),
						_: 1
					})),
					h(B).isPending.value ? (w(), c("div", fe, [(w(), c(y, null, t(3, (e) => l(z, {
						key: e,
						height: "2.5rem"
					})), 64))])) : (w(), c(y, { key: 3 }, [
						C("ul", {
							class: "todo-tasks",
							onDragover: s[10] ||= n(() => {}, ["prevent"]),
							onDrop: s[11] ||= n((e) => R.value && Z(R.value.id, null), ["prevent"])
						}, [(w(!0), c(y, null, t(he.value, (e) => (w(), p(O, {
							key: e.id,
							task: e,
							"list-name": ve.value ? _e(e.listId) : null,
							"assignee-name": h(te)(e.assigneeId),
							"can-toggle": V(F.value.find((t) => t.id === e.listId)),
							draggable: ke.value,
							onToggle: (t) => h(we).mutate(e),
							onOpen: (t) => K.value = e,
							onDragstart: (t) => X.value = e,
							onDragend: s[9] ||= (e) => X.value = null,
							onDrop: n((t) => Z(e.listId, e.id), ["stop", "prevent"])
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
						he.value.length ? f("", !0) : (w(), c("p", pe, a(M.value?.kind === "today" ? h(m).t("empty.today") : M.value?.kind === "mine" ? h(m).t("empty.mine") : h(m).t("empty.list")), 1)),
						ge.value.length ? (w(), c(y, { key: 1 }, [l(_, {
							text: "",
							size: "small",
							severity: "secondary",
							icon: P.value ? "pi pi-chevron-down" : "pi pi-chevron-right",
							label: h(m).t("doneCount", { count: ge.value.length }),
							onClick: s[12] ||= (e) => P.value = !P.value
						}, null, 8, ["icon", "label"]), P.value ? (w(), c("ul", me, [(w(!0), c(y, null, t(ge.value, (e) => (w(), p(O, {
							key: e.id,
							task: e,
							"list-name": ve.value ? _e(e.listId) : null,
							"assignee-name": h(te)(e.assigneeId),
							"can-toggle": V(F.value.find((t) => t.id === e.listId)),
							onToggle: (t) => h(we).mutate(e),
							onOpen: (t) => K.value = e
						}, null, 8, [
							"task",
							"list-name",
							"assignee-name",
							"can-toggle",
							"onToggle",
							"onOpen"
						]))), 128))])) : f("", !0)], 64)) : f("", !0)
					], 64))
				]),
				l(Se, {
					visible: K.value !== null,
					position: "right",
					header: h(m).t("editTask"),
					class: "todo-drawer",
					"onUpdate:visible": s[14] ||= (e) => !e && (K.value = null)
				}, {
					default: i(() => [K.value ? (w(), p(ne, {
						key: 0,
						task: K.value,
						lists: F.value,
						members: h(g).members.data.value ?? [],
						"user-id": h(T),
						"can-edit": V(F.value.find((e) => e.id === K.value.listId)),
						"can-delete": be(K.value),
						saving: h(q).isPending.value,
						onSave: Te,
						onDelete: Ee,
						onClose: s[13] ||= (e) => K.value = null
					}, null, 8, [
						"task",
						"lists",
						"members",
						"user-id",
						"can-edit",
						"can-delete",
						"saving"
					])) : f("", !0)]),
					_: 1
				}, 8, ["visible", "header"]),
				l(D, {
					visible: Y.value !== null,
					mode: Y.value?.mode ?? "edit",
					"onUpdate:visible": s[15] ||= (e) => !e && (Y.value = null),
					onChoose: De
				}, null, 8, ["visible", "mode"]),
				l(ee, {
					visible: Q.value !== null,
					mode: Q.value?.mode ?? "create",
					name: Q.value?.mode === "rename" ? R.value?.name : "",
					"can-share": h(A),
					"onUpdate:visible": s[16] ||= (e) => !e && (Q.value = null),
					onSubmit: Me
				}, null, 8, [
					"visible",
					"mode",
					"name",
					"can-share"
				]),
				l(J, {
					visible: $.value,
					"onUpdate:visible": s[18] ||= (e) => $.value = e,
					modal: "",
					header: h(m).t("deleteList"),
					style: {
						width: "24rem",
						maxWidth: "calc(100vw - 2rem)"
					}
				}, {
					footer: i(() => [l(_, {
						severity: "secondary",
						text: "",
						label: h(m).t("cancel"),
						onClick: s[17] ||= (e) => $.value = !1
					}, null, 8, ["label"]), l(_, {
						severity: "danger",
						icon: "pi pi-trash",
						label: h(m).t("delete"),
						onClick: Fe
					}, null, 8, ["label"])]),
					default: i(() => [C("p", null, a(h(m).t("deleteListConfirm", { name: R.value?.name ?? "" })), 1)]),
					_: 1
				}, 8, ["visible", "header"])
			]);
		};
	}
}), [["__scopeId", "data-v-08d13021"]]);
export { B as default };
