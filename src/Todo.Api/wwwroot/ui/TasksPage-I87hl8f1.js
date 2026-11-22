import { D as e, E as t, I as n, M as r, N as i, O as a, S as o, T as s, _ as c, b as l, c as u, f as d, g as f, h as p, i as m, k as h, l as g, m as _, n as v, p as y, r as b, t as x, u as S, v as C, w, x as T, y as E } from "./_plugin-vue_export-helper-DX_-5sjr.js";
import { t as D } from "./TaskRow-BojeEe0Q.js";
// src/ListDialog.vue?vue&type=script&setup=true&lang.ts
var O = { class: "todo-list-form__actions" }, ee = /*#__PURE__*/ x(/* @__PURE__ */ T({
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
					C("div", O, [l(d, {
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
}), [["__scopeId", "data-v-25f3ed2f"]]), k = { class: "todo-editor__field" }, A = { class: "todo-editor__field" }, j = { class: "todo-editor__row" }, M = { class: "todo-editor__field" }, N = { class: "todo-editor__field todo-editor__time" }, P = { class: "todo-editor__field" }, F = {
	key: 0,
	class: "todo-editor__field"
}, I = { class: "todo-editor__field" }, L = { class: "todo-editor__actions" }, te = /*#__PURE__*/ x(/* @__PURE__ */ T({
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
		let o = t, v = i, y = d(), b = s(m(o.task)), x = s(o.task.listId);
		r(() => o.task, (e) => {
			b.value = m(e), x.value = e.listId;
		});
		let S = _(() => o.lists.find((e) => e.id === x.value)), T = _({
			get: () => b.value.dueDate ? u(b.value.dueDate) : null,
			set: (e) => {
				b.value.dueDate = e ? g(e) : null, e || (b.value.dueTime = null);
			}
		}), E = _({
			get: () => b.value.dueTime?.slice(0, 5) ?? "",
			set: (e) => b.value.dueTime = e ? `${e}:00` : null
		}), D = _(() => [
			"Low",
			"Normal",
			"High"
		].map((e) => ({
			value: e,
			label: y.t(`priority.${e}`)
		}))), O = _(() => {
			let e = S.value?.visibility === "Private" ? o.members.filter((e) => e.userId === o.userId) : o.members;
			return [{
				userId: null,
				displayName: y.t("nobody")
			}, ...e.map((e) => ({
				userId: e.userId,
				displayName: e.displayName
			}))];
		}), ee = _(() => o.lists.filter((e) => !e.archived || e.id === o.task.listId));
		return (r, i) => {
			let o = e("InputText"), s = e("Textarea"), u = e("DatePicker"), d = e("SelectButton"), m = e("Select"), g = e("Button");
			return w(), c("form", {
				class: "todo-editor",
				onSubmit: i[9] ||= n((e) => v("save", b.value, x.value), ["prevent"])
			}, [
				C("label", k, [C("span", null, a(h(y).t("fields.title")), 1), l(o, {
					modelValue: b.value.title,
					"onUpdate:modelValue": i[0] ||= (e) => b.value.title = e,
					disabled: !t.canEdit,
					maxlength: "200",
					required: "",
					autofocus: ""
				}, null, 8, ["modelValue", "disabled"])]),
				C("label", A, [C("span", null, a(h(y).t("fields.notes")), 1), l(s, {
					modelValue: b.value.notes,
					"onUpdate:modelValue": i[1] ||= (e) => b.value.notes = e,
					disabled: !t.canEdit,
					rows: "4",
					"auto-resize": "",
					maxlength: "2000"
				}, null, 8, ["modelValue", "disabled"])]),
				C("div", j, [C("label", M, [C("span", null, a(h(y).t("fields.due")), 1), l(u, {
					modelValue: T.value,
					"onUpdate:modelValue": i[2] ||= (e) => T.value = e,
					disabled: !t.canEdit,
					"show-button-bar": "",
					"date-format": "dd.mm.yy",
					"show-icon": ""
				}, null, 8, ["modelValue", "disabled"])]), C("label", N, [C("span", null, a(h(y).t("fields.time")), 1), l(o, {
					modelValue: E.value,
					"onUpdate:modelValue": i[3] ||= (e) => E.value = e,
					type: "time",
					disabled: !t.canEdit || !b.value.dueDate
				}, null, 8, ["modelValue", "disabled"])])]),
				C("div", P, [C("span", null, a(h(y).t("fields.priority")), 1), l(d, {
					modelValue: b.value.priority,
					"onUpdate:modelValue": i[4] ||= (e) => b.value.priority = e,
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
				t.members.length ? (w(), c("label", F, [C("span", null, a(h(y).t("fields.assignee")), 1), l(m, {
					modelValue: b.value.assigneeId,
					"onUpdate:modelValue": i[5] ||= (e) => b.value.assigneeId = e,
					placeholder: h(y).t("nobody"),
					options: O.value,
					"option-label": "displayName",
					"option-value": "userId",
					disabled: !t.canEdit
				}, null, 8, [
					"modelValue",
					"placeholder",
					"options",
					"disabled"
				])])) : f("", !0),
				C("label", I, [C("span", null, a(h(y).t("fields.list")), 1), l(m, {
					modelValue: x.value,
					"onUpdate:modelValue": i[6] ||= (e) => x.value = e,
					options: ee.value,
					"option-label": "name",
					"option-value": "id",
					disabled: !t.canEdit
				}, null, 8, [
					"modelValue",
					"options",
					"disabled"
				])]),
				C("div", L, [
					t.canDelete ? (w(), p(g, {
						key: 0,
						type: "button",
						severity: "danger",
						text: "",
						icon: "pi pi-trash",
						label: h(y).t("delete"),
						onClick: i[7] ||= (e) => v("delete")
					}, null, 8, ["label"])) : f("", !0),
					i[10] ||= C("span", { class: "todo-editor__spacer" }, null, -1),
					l(g, {
						type: "button",
						severity: "secondary",
						text: "",
						label: h(y).t("cancel"),
						onClick: i[8] ||= (e) => v("close")
					}, null, 8, ["label"]),
					t.canEdit ? (w(), p(g, {
						key: 1,
						type: "submit",
						icon: "pi pi-check",
						label: h(y).t("save"),
						loading: t.saving
					}, null, 8, ["label", "loading"])) : f("", !0)
				])
			], 32);
		};
	}
}), [["__scopeId", "data-v-efc1c6ff"]]), ne = { class: "todo-page" }, re = ["aria-label"], ie = ["onClick", "onDrop"], ae = { class: "todo-nav__name" }, oe = ["onClick"], se = { class: "todo-main" }, ce = { class: "todo-phone-picker" }, le = { class: "todo-header" }, ue = ["title"], de = {
	key: 2,
	class: "todo-loading"
}, fe = {
	key: 0,
	class: "todo-empty"
}, pe = {
	key: 0,
	class: "todo-tasks"
}, R = /*#__PURE__*/ x(/* @__PURE__ */ T({
	__name: "TasksPage",
	setup(u) {
		let m = d(), g = v(() => m.household.value.id), { lists: x, userId: T, canWrite: O, canManage: k, memberName: A, change: j } = g, M = s(null), N = s(!1), P = s(!1), F = _(() => x.data.value ?? []), I = _(() => [...F.value.filter((e) => !e.archived)].sort((e, t) => e.visibility === t.visibility ? 0 : e.visibility === "Shared" ? -1 : 1)), L = _(() => F.value.filter((e) => e.archived));
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
		}), B = g.tasks(z, () => M.value !== null), V = _(() => (B.data.value ?? []).filter((e) => e.status === "Open")), H = _(() => (B.data.value ?? []).filter((e) => e.status === "Done").sort((e, t) => (t.completedAt ?? "").localeCompare(e.completedAt ?? ""))), me = (e) => F.value.find((t) => t.id === e)?.name ?? null, U = _(() => M.value?.kind !== "list"), he = (e) => !!e && (e.visibility === "Shared" ? k.value : e.ownerId === T.value), W = (e) => O.value && !!e && !e.archived, ge = (e) => {
			let t = F.value.find((t) => t.id === e.listId);
			return W(t) ? t.visibility === "Private" || e.createdBy === T.value || k.value : !1;
		}, _e = _(() => [
			{
				key: "today",
				label: m.t("views.today")
			},
			...O.value ? [{
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
		]), G = _({
			get: () => M.value?.kind === "list" ? M.value.id : M.value?.kind ?? "today",
			set: (e) => K(e === "today" || e === "mine" ? { kind: e } : {
				kind: "list",
				id: e
			})
		});
		function K(e) {
			M.value = e, P.value = !1;
		}
		let q = (e) => G.value === e, J = s(""), ve = j((e, { listId: t, input: n }) => e.createTask(t, n));
		function ye() {
			let e = J.value.trim();
			e && R.value && (ve.mutate({
				listId: R.value.id,
				input: b(e)
			}), J.value = "");
		}
		let be = j((e, t) => t.status === "Done" ? e.reopenTask(t.id) : e.completeTask(t.id)), Y = s(null), xe = j(async (e, { task: t, input: n, listId: r }) => {
			await e.updateTask(t.id, n), r !== t.listId && await e.moveTask(t.id, r, null);
		}), Se = j((e, t) => e.deleteTask(t.id));
		function Ce(e, t) {
			Y.value && xe.mutate({
				task: Y.value,
				input: e,
				listId: t
			}, { onSuccess: () => Y.value = null });
		}
		function we() {
			Y.value && Se.mutate(Y.value, { onSuccess: () => Y.value = null });
		}
		let X = s(null), Te = j((e, { task: t, listId: n, before: r }) => e.moveTask(t.id, n, r)), Ee = _(() => W(R.value) && !U.value);
		function Z(e, t) {
			let n = X.value;
			if (X.value = null, !n || n.id === t) return;
			let r = F.value.find((t) => t.id === e);
			W(r) && Te.mutate({
				task: n,
				listId: e,
				before: t
			});
		}
		let Q = s(null), De = j((e, { name: t, visibility: n }) => e.createList(t, n)), Oe = j((e, { id: t, name: n }) => e.renameList(t, n));
		function ke(e, t) {
			Q.value?.mode === "rename" && R.value ? Oe.mutate({
				id: R.value.id,
				name: e
			}) : De.mutate({
				name: e,
				visibility: t
			}, { onSuccess: (e) => K({
				kind: "list",
				id: e
			}) }), Q.value = null;
		}
		let Ae = j((e, t) => t.archived ? e.restoreList(t.id) : e.archiveList(t.id)), je = j((e, t) => e.deleteList(t.id)), $ = s(!1);
		function Me() {
			let e = R.value;
			$.value = !1, e && je.mutate(e, { onSuccess: () => M.value = null });
		}
		let Ne = s(null), Pe = _(() => {
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
					command: () => Ae.mutate(e)
				},
				{
					label: m.t("deleteList"),
					icon: "pi pi-trash",
					command: () => $.value = !0
				}
			] : [];
		}), Fe = _(() => M.value?.kind === "today" ? m.t("views.today") : M.value?.kind === "mine" ? m.t("views.mine") : R.value?.name ?? "");
		return (r, s) => {
			let u = e("Divider"), d = e("Badge"), _ = e("Button"), v = e("Select"), b = e("Tag"), x = e("Menu"), S = e("InputText"), j = e("Message"), z = e("Skeleton"), ve = e("Drawer"), Se = e("Dialog");
			return w(), c("div", ne, [
				C("nav", {
					class: "todo-nav",
					"aria-label": h(m).t("lists")
				}, [
					C("button", {
						type: "button",
						class: o({ active: q("today") }),
						onClick: s[0] ||= (e) => K({ kind: "today" })
					}, [s[18] ||= C("i", {
						class: "pi pi-sun",
						"aria-hidden": "true"
					}, null, -1), E(a(h(m).t("views.today")), 1)], 2),
					h(O) ? (w(), c("button", {
						key: 0,
						type: "button",
						class: o({ active: q("mine") }),
						onClick: s[1] ||= (e) => K({ kind: "mine" })
					}, [s[19] ||= C("i", {
						class: "pi pi-user",
						"aria-hidden": "true"
					}, null, -1), E(a(h(m).t("views.mine")), 1)], 2)) : f("", !0),
					l(u),
					(w(!0), c(y, null, t(I.value, (e) => (w(), c("button", {
						key: e.id,
						type: "button",
						class: o({ active: q(e.id) }),
						onClick: (t) => K({
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
						C("span", ae, a(e.name), 1),
						e.openTasks ? (w(), p(d, {
							key: 0,
							value: e.openTasks,
							severity: "secondary"
						}, null, 8, ["value"])) : f("", !0)
					], 42, ie))), 128)),
					h(O) ? (w(), p(_, {
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
						class: o(["todo-nav__archived", { active: q(e.id) }]),
						onClick: (t) => K({
							kind: "list",
							id: e.id
						})
					}, [s[20] ||= C("i", {
						class: "pi pi-inbox",
						"aria-hidden": "true"
					}, null, -1), E(a(e.name), 1)], 10, oe))), 128)) : f("", !0)], 64)) : f("", !0)
				], 8, re),
				C("section", se, [
					C("div", ce, [l(v, {
						modelValue: G.value,
						"onUpdate:modelValue": s[5] ||= (e) => G.value = e,
						options: _e.value,
						"option-label": "label",
						"option-value": "key",
						class: "w-full"
					}, null, 8, ["modelValue", "options"]), h(O) ? (w(), p(_, {
						key: 0,
						icon: "pi pi-plus",
						text: "",
						"aria-label": h(m).t("newList"),
						onClick: s[6] ||= (e) => Q.value = { mode: "create" }
					}, null, 8, ["aria-label"])) : f("", !0)]),
					C("header", le, [
						C("h2", null, [R.value ? (w(), c("i", {
							key: 0,
							class: o(["pi", R.value.visibility === "Shared" ? "pi-users" : "pi-lock"]),
							title: h(m).t(`visibility.${R.value.visibility}`)
						}, null, 10, ue)) : f("", !0), E(" " + a(Fe.value), 1)]),
						R.value?.archived ? (w(), p(b, {
							key: 0,
							value: h(m).t("archived"),
							severity: "secondary"
						}, null, 8, ["value"])) : f("", !0),
						he(R.value) ? (w(), c(y, { key: 1 }, [l(_, {
							icon: "pi pi-ellipsis-v",
							text: "",
							rounded: "",
							"aria-label": h(m).t("listActions"),
							onClick: s[7] ||= (e) => Ne.value?.toggle(e)
						}, null, 8, ["aria-label"]), l(x, {
							ref_key: "listMenu",
							ref: Ne,
							model: Pe.value,
							popup: ""
						}, null, 8, ["model"])], 64)) : f("", !0)
					]),
					W(R.value) ? (w(), c("form", {
						key: 0,
						class: "todo-add",
						onSubmit: n(ye, ["prevent"])
					}, [l(S, {
						modelValue: J.value,
						"onUpdate:modelValue": s[8] ||= (e) => J.value = e,
						placeholder: h(m).t("addPlaceholder"),
						maxlength: "200",
						class: "todo-add__input"
					}, null, 8, ["modelValue", "placeholder"]), l(_, {
						type: "submit",
						icon: "pi pi-plus",
						"aria-label": h(m).t("add"),
						disabled: !J.value.trim()
					}, null, 8, ["aria-label", "disabled"])], 32)) : h(O) ? f("", !0) : (w(), p(j, {
						key: 1,
						severity: "secondary",
						size: "small"
					}, {
						default: i(() => [E(a(h(m).t("readOnly")), 1)]),
						_: 1
					})),
					h(B).isPending.value ? (w(), c("div", de, [(w(), c(y, null, t(3, (e) => l(z, {
						key: e,
						height: "2.5rem"
					})), 64))])) : (w(), c(y, { key: 3 }, [
						C("ul", {
							class: "todo-tasks",
							onDragover: s[10] ||= n(() => {}, ["prevent"]),
							onDrop: s[11] ||= n((e) => R.value && Z(R.value.id, null), ["prevent"])
						}, [(w(!0), c(y, null, t(V.value, (e) => (w(), p(D, {
							key: e.id,
							task: e,
							"list-name": U.value ? me(e.listId) : null,
							"assignee-name": h(A)(e.assigneeId),
							"can-toggle": W(F.value.find((t) => t.id === e.listId)),
							draggable: Ee.value,
							onToggle: (t) => h(be).mutate(e),
							onOpen: (t) => Y.value = e,
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
						V.value.length ? f("", !0) : (w(), c("p", fe, a(M.value?.kind === "today" ? h(m).t("empty.today") : M.value?.kind === "mine" ? h(m).t("empty.mine") : h(m).t("empty.list")), 1)),
						H.value.length ? (w(), c(y, { key: 1 }, [l(_, {
							text: "",
							size: "small",
							severity: "secondary",
							icon: P.value ? "pi pi-chevron-down" : "pi pi-chevron-right",
							label: h(m).t("doneCount", { count: H.value.length }),
							onClick: s[12] ||= (e) => P.value = !P.value
						}, null, 8, ["icon", "label"]), P.value ? (w(), c("ul", pe, [(w(!0), c(y, null, t(H.value, (e) => (w(), p(D, {
							key: e.id,
							task: e,
							"list-name": U.value ? me(e.listId) : null,
							"assignee-name": h(A)(e.assigneeId),
							"can-toggle": W(F.value.find((t) => t.id === e.listId)),
							onToggle: (t) => h(be).mutate(e),
							onOpen: (t) => Y.value = e
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
				l(ve, {
					visible: Y.value !== null,
					position: "right",
					header: h(m).t("editTask"),
					class: "todo-drawer",
					"onUpdate:visible": s[14] ||= (e) => !e && (Y.value = null)
				}, {
					default: i(() => [Y.value ? (w(), p(te, {
						key: 0,
						task: Y.value,
						lists: F.value,
						members: h(g).members.data.value ?? [],
						"user-id": h(T),
						"can-edit": W(F.value.find((e) => e.id === Y.value.listId)),
						"can-delete": ge(Y.value),
						saving: h(xe).isPending.value,
						onSave: Ce,
						onDelete: we,
						onClose: s[13] ||= (e) => Y.value = null
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
				l(ee, {
					visible: Q.value !== null,
					mode: Q.value?.mode ?? "create",
					name: Q.value?.mode === "rename" ? R.value?.name : "",
					"can-share": h(k),
					"onUpdate:visible": s[15] ||= (e) => !e && (Q.value = null),
					onSubmit: ke
				}, null, 8, [
					"visible",
					"mode",
					"name",
					"can-share"
				]),
				l(Se, {
					visible: $.value,
					"onUpdate:visible": s[17] ||= (e) => $.value = e,
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
						onClick: s[16] ||= (e) => $.value = !1
					}, null, 8, ["label"]), l(_, {
						severity: "danger",
						icon: "pi pi-trash",
						label: h(m).t("delete"),
						onClick: Me
					}, null, 8, ["label"])]),
					default: i(() => [C("p", null, a(h(m).t("deleteListConfirm", { name: R.value?.name ?? "" })), 1)]),
					_: 1
				}, 8, ["visible", "header"])
			]);
		};
	}
}), [["__scopeId", "data-v-defaf7ec"]]);
export { R as default };
