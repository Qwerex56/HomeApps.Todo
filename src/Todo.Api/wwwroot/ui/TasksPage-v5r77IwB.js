import { C as e, D as t, E as n, O as r, S as i, T as a, _ as o, a as s, b as c, c as l, d as u, f as d, g as f, h as p, i as m, k as h, m as g, n as _, o as v, p as y, r as b, s as x, t as S, u as C, v as w, w as T, x as E, y as D } from "./TaskRow-6h-DxQkL.js";
// src/ListDialog.vue?vue&type=script&setup=true&lang.ts
var O = { class: "todo-list-form__actions" }, k = /*#__PURE__*/ _(/* @__PURE__ */ D({
	__name: "ListDialog",
	props: {
		visible: { type: Boolean },
		mode: {},
		name: {},
		canShare: { type: Boolean }
	},
	emits: ["update:visible", "submit"],
	setup(e, { emit: o }) {
		let s = e, c = o, l = C(), d = i(""), m = i("Private");
		t(() => s.visible, (e) => {
			e && (d.value = s.name ?? "", m.value = s.canShare ? "Shared" : "Private");
		});
		let _ = [{
			value: "Shared",
			label: l.t("visibility.Shared")
		}, {
			value: "Private",
			label: l.t("visibility.Private")
		}];
		return (t, i) => {
			let o = T("InputText"), s = T("SelectButton"), v = T("Button"), b = T("Dialog");
			return E(), y(b, {
				visible: e.visible,
				modal: "",
				header: e.mode === "create" ? n(l).t("newList") : n(l).t("renameList"),
				style: {
					width: "24rem",
					maxWidth: "calc(100vw - 2rem)"
				},
				"onUpdate:visible": i[4] ||= (e) => c("update:visible", e)
			}, {
				default: r(() => [f("form", {
					class: "todo-list-form",
					onSubmit: i[3] ||= h((e) => c("submit", d.value, m.value), ["prevent"])
				}, [
					w(o, {
						modelValue: d.value,
						"onUpdate:modelValue": i[0] ||= (e) => d.value = e,
						placeholder: n(l).t("fields.listName"),
						maxlength: "40",
						required: "",
						autofocus: ""
					}, null, 8, ["modelValue", "placeholder"]),
					e.mode === "create" ? (E(), p(u, { key: 0 }, [e.canShare ? (E(), y(s, {
						key: 0,
						modelValue: m.value,
						"onUpdate:modelValue": i[1] ||= (e) => m.value = e,
						options: _,
						"option-label": "label",
						"option-value": "value",
						"allow-empty": !1
					}, null, 8, ["modelValue"])) : g("", !0), f("small", null, a(n(l).t(`visibilityHint.${m.value}`)), 1)], 64)) : g("", !0),
					f("div", O, [w(v, {
						type: "button",
						severity: "secondary",
						text: "",
						label: n(l).t("cancel"),
						onClick: i[2] ||= (e) => c("update:visible", !1)
					}, null, 8, ["label"]), w(v, {
						type: "submit",
						label: e.mode === "create" ? n(l).t("create") : n(l).t("save")
					}, null, 8, ["label"])])
				], 32)]),
				_: 1
			}, 8, ["visible", "header"]);
		};
	}
}), [["__scopeId", "data-v-25f3ed2f"]]), A = { class: "todo-editor__field" }, j = { class: "todo-editor__field" }, M = { class: "todo-editor__row" }, N = { class: "todo-editor__field" }, P = { class: "todo-editor__field todo-editor__time" }, F = { class: "todo-editor__field" }, I = {
	key: 0,
	class: "todo-editor__field"
}, L = { class: "todo-editor__field" }, R = { class: "todo-editor__actions" }, ee = /*#__PURE__*/ _(/* @__PURE__ */ D({
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
	setup(e, { emit: r }) {
		let o = e, c = r, l = C(), u = i(s(o.task)), m = i(o.task.listId);
		t(() => o.task, (e) => {
			u.value = s(e), m.value = e.listId;
		});
		let _ = d(() => o.lists.find((e) => e.id === m.value)), b = d({
			get: () => u.value.dueDate ? v(u.value.dueDate) : null,
			set: (e) => {
				u.value.dueDate = e ? x(e) : null, e || (u.value.dueTime = null);
			}
		}), S = d({
			get: () => u.value.dueTime?.slice(0, 5) ?? "",
			set: (e) => u.value.dueTime = e ? `${e}:00` : null
		}), D = d(() => [
			"Low",
			"Normal",
			"High"
		].map((e) => ({
			value: e,
			label: l.t(`priority.${e}`)
		}))), O = d(() => {
			let e = _.value?.visibility === "Private" ? o.members.filter((e) => e.userId === o.userId) : o.members;
			return [{
				userId: null,
				displayName: l.t("nobody")
			}, ...e.map((e) => ({
				userId: e.userId,
				displayName: e.displayName
			}))];
		}), k = d(() => o.lists.filter((e) => !e.archived || e.id === o.task.listId));
		return (t, r) => {
			let i = T("InputText"), o = T("Textarea"), s = T("DatePicker"), d = T("SelectButton"), _ = T("Select"), v = T("Button");
			return E(), p("form", {
				class: "todo-editor",
				onSubmit: r[9] ||= h((e) => c("save", u.value, m.value), ["prevent"])
			}, [
				f("label", A, [f("span", null, a(n(l).t("fields.title")), 1), w(i, {
					modelValue: u.value.title,
					"onUpdate:modelValue": r[0] ||= (e) => u.value.title = e,
					disabled: !e.canEdit,
					maxlength: "200",
					required: "",
					autofocus: ""
				}, null, 8, ["modelValue", "disabled"])]),
				f("label", j, [f("span", null, a(n(l).t("fields.notes")), 1), w(o, {
					modelValue: u.value.notes,
					"onUpdate:modelValue": r[1] ||= (e) => u.value.notes = e,
					disabled: !e.canEdit,
					rows: "4",
					"auto-resize": "",
					maxlength: "2000"
				}, null, 8, ["modelValue", "disabled"])]),
				f("div", M, [f("label", N, [f("span", null, a(n(l).t("fields.due")), 1), w(s, {
					modelValue: b.value,
					"onUpdate:modelValue": r[2] ||= (e) => b.value = e,
					disabled: !e.canEdit,
					"show-button-bar": "",
					"date-format": "dd.mm.yy",
					"show-icon": ""
				}, null, 8, ["modelValue", "disabled"])]), f("label", P, [f("span", null, a(n(l).t("fields.time")), 1), w(i, {
					modelValue: S.value,
					"onUpdate:modelValue": r[3] ||= (e) => S.value = e,
					type: "time",
					disabled: !e.canEdit || !u.value.dueDate
				}, null, 8, ["modelValue", "disabled"])])]),
				f("div", F, [f("span", null, a(n(l).t("fields.priority")), 1), w(d, {
					modelValue: u.value.priority,
					"onUpdate:modelValue": r[4] ||= (e) => u.value.priority = e,
					options: D.value,
					"option-label": "label",
					"option-value": "value",
					"allow-empty": !1,
					disabled: !e.canEdit
				}, null, 8, [
					"modelValue",
					"options",
					"disabled"
				])]),
				e.members.length ? (E(), p("label", I, [f("span", null, a(n(l).t("fields.assignee")), 1), w(_, {
					modelValue: u.value.assigneeId,
					"onUpdate:modelValue": r[5] ||= (e) => u.value.assigneeId = e,
					placeholder: n(l).t("nobody"),
					options: O.value,
					"option-label": "displayName",
					"option-value": "userId",
					disabled: !e.canEdit
				}, null, 8, [
					"modelValue",
					"placeholder",
					"options",
					"disabled"
				])])) : g("", !0),
				f("label", L, [f("span", null, a(n(l).t("fields.list")), 1), w(_, {
					modelValue: m.value,
					"onUpdate:modelValue": r[6] ||= (e) => m.value = e,
					options: k.value,
					"option-label": "name",
					"option-value": "id",
					disabled: !e.canEdit
				}, null, 8, [
					"modelValue",
					"options",
					"disabled"
				])]),
				f("div", R, [
					e.canDelete ? (E(), y(v, {
						key: 0,
						type: "button",
						severity: "danger",
						text: "",
						icon: "pi pi-trash",
						label: n(l).t("delete"),
						onClick: r[7] ||= (e) => c("delete")
					}, null, 8, ["label"])) : g("", !0),
					r[10] ||= f("span", { class: "todo-editor__spacer" }, null, -1),
					w(v, {
						type: "button",
						severity: "secondary",
						text: "",
						label: n(l).t("cancel"),
						onClick: r[8] ||= (e) => c("close")
					}, null, 8, ["label"]),
					e.canEdit ? (E(), y(v, {
						key: 1,
						type: "submit",
						icon: "pi pi-check",
						label: n(l).t("save"),
						loading: e.saving
					}, null, 8, ["label", "loading"])) : g("", !0)
				])
			], 32);
		};
	}
}), [["__scopeId", "data-v-efc1c6ff"]]), te = { class: "todo-page" }, ne = ["aria-label"], re = ["onClick", "onDrop"], ie = { class: "todo-nav__name" }, ae = ["onClick"], oe = { class: "todo-main" }, se = { class: "todo-phone-picker" }, ce = { class: "todo-header" }, le = ["title"], ue = {
	key: 2,
	class: "todo-loading"
}, de = {
	key: 0,
	class: "todo-empty"
}, fe = {
	key: 0,
	class: "todo-tasks"
}, z = /*#__PURE__*/ _(/* @__PURE__ */ D({
	__name: "TasksPage",
	setup(s) {
		let _ = C(), v = b(() => _.household.value.id), { lists: x, userId: D, canWrite: O, canManage: A, memberName: j, change: M } = v, N = i(null), P = i(!1), F = i(!1), I = d(() => x.data.value ?? []), L = d(() => [...I.value.filter((e) => !e.archived)].sort((e, t) => e.visibility === t.visibility ? 0 : e.visibility === "Shared" ? -1 : 1)), R = d(() => I.value.filter((e) => e.archived));
		t(() => x.data.value, (e) => {
			if (!e) return;
			let t = N.value;
			if (t?.kind === "list" && e.some((e) => e.id === t.id) || t && t.kind !== "list") return;
			let n = L.value[0];
			N.value = n ? {
				kind: "list",
				id: n.id
			} : { kind: "today" };
		}, { immediate: !0 });
		let z = d(() => {
			let e = N.value;
			return e?.kind === "list" ? I.value.find((t) => t.id === e.id) ?? null : null;
		}), B = d(() => {
			let e = N.value;
			return e?.kind === "list" ? { list: e.id } : e?.kind === "mine" ? { assignee: "me" } : { dueBy: l() };
		}), V = v.tasks(B, () => N.value !== null), pe = d(() => (V.data.value ?? []).filter((e) => e.status === "Open")), H = d(() => (V.data.value ?? []).filter((e) => e.status === "Done").sort((e, t) => (t.completedAt ?? "").localeCompare(e.completedAt ?? ""))), me = (e) => I.value.find((t) => t.id === e)?.name ?? null, U = d(() => N.value?.kind !== "list"), he = (e) => !!e && (e.visibility === "Shared" ? A.value : e.ownerId === D.value), W = (e) => O.value && !!e && !e.archived, ge = (e) => {
			let t = I.value.find((t) => t.id === e.listId);
			return W(t) ? t.visibility === "Private" || e.createdBy === D.value || A.value : !1;
		}, _e = d(() => [
			{
				key: "today",
				label: _.t("views.today")
			},
			...O.value ? [{
				key: "mine",
				label: _.t("views.mine")
			}] : [],
			...L.value.map((e) => ({
				key: e.id,
				label: e.name
			})),
			...R.value.map((e) => ({
				key: e.id,
				label: `${e.name} (${_.t("archived")})`
			}))
		]), G = d({
			get: () => N.value?.kind === "list" ? N.value.id : N.value?.kind ?? "today",
			set: (e) => K(e === "today" || e === "mine" ? { kind: e } : {
				kind: "list",
				id: e
			})
		});
		function K(e) {
			N.value = e, F.value = !1;
		}
		let q = (e) => G.value === e, J = i(""), ve = M((e, { listId: t, input: n }) => e.createTask(t, n));
		function ye() {
			let e = J.value.trim();
			e && z.value && (ve.mutate({
				listId: z.value.id,
				input: m(e)
			}), J.value = "");
		}
		let be = M((e, t) => t.status === "Done" ? e.reopenTask(t.id) : e.completeTask(t.id)), Y = i(null), xe = M(async (e, { task: t, input: n, listId: r }) => {
			await e.updateTask(t.id, n), r !== t.listId && await e.moveTask(t.id, r, null);
		}), Se = M((e, t) => e.deleteTask(t.id));
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
		let X = i(null), Te = M((e, { task: t, listId: n, before: r }) => e.moveTask(t.id, n, r)), Ee = d(() => W(z.value) && !U.value);
		function Z(e, t) {
			let n = X.value;
			if (X.value = null, !n || n.id === t) return;
			let r = I.value.find((t) => t.id === e);
			W(r) && Te.mutate({
				task: n,
				listId: e,
				before: t
			});
		}
		let Q = i(null), De = M((e, { name: t, visibility: n }) => e.createList(t, n)), Oe = M((e, { id: t, name: n }) => e.renameList(t, n));
		function ke(e, t) {
			Q.value?.mode === "rename" && z.value ? Oe.mutate({
				id: z.value.id,
				name: e
			}) : De.mutate({
				name: e,
				visibility: t
			}, { onSuccess: (e) => K({
				kind: "list",
				id: e
			}) }), Q.value = null;
		}
		let Ae = M((e, t) => t.archived ? e.restoreList(t.id) : e.archiveList(t.id)), je = M((e, t) => e.deleteList(t.id)), $ = i(!1);
		function Me() {
			let e = z.value;
			$.value = !1, e && je.mutate(e, { onSuccess: () => N.value = null });
		}
		let Ne = i(null), Pe = d(() => {
			let e = z.value;
			return e ? [
				{
					label: _.t("renameList"),
					icon: "pi pi-pencil",
					disabled: e.archived,
					command: () => Q.value = { mode: "rename" }
				},
				{
					label: e.archived ? _.t("restoreList") : _.t("archiveList"),
					icon: e.archived ? "pi pi-replay" : "pi pi-inbox",
					command: () => Ae.mutate(e)
				},
				{
					label: _.t("deleteList"),
					icon: "pi pi-trash",
					command: () => $.value = !0
				}
			] : [];
		}), Fe = d(() => N.value?.kind === "today" ? _.t("views.today") : N.value?.kind === "mine" ? _.t("views.mine") : z.value?.name ?? "");
		return (t, i) => {
			let s = T("Divider"), l = T("Badge"), d = T("Button"), m = T("Select"), b = T("Tag"), x = T("Menu"), C = T("InputText"), M = T("Message"), B = T("Skeleton"), ve = T("Drawer"), Se = T("Dialog");
			return E(), p("div", te, [
				f("nav", {
					class: "todo-nav",
					"aria-label": n(_).t("lists")
				}, [
					f("button", {
						type: "button",
						class: c({ active: q("today") }),
						onClick: i[0] ||= (e) => K({ kind: "today" })
					}, [i[18] ||= f("i", {
						class: "pi pi-sun",
						"aria-hidden": "true"
					}, null, -1), o(a(n(_).t("views.today")), 1)], 2),
					n(O) ? (E(), p("button", {
						key: 0,
						type: "button",
						class: c({ active: q("mine") }),
						onClick: i[1] ||= (e) => K({ kind: "mine" })
					}, [i[19] ||= f("i", {
						class: "pi pi-user",
						"aria-hidden": "true"
					}, null, -1), o(a(n(_).t("views.mine")), 1)], 2)) : g("", !0),
					w(s),
					(E(!0), p(u, null, e(L.value, (e) => (E(), p("button", {
						key: e.id,
						type: "button",
						class: c({ active: q(e.id) }),
						onClick: (t) => K({
							kind: "list",
							id: e.id
						}),
						onDragover: i[2] ||= h(() => {}, ["prevent"]),
						onDrop: h((t) => Z(e.id, null), ["prevent"])
					}, [
						f("i", {
							class: c(["pi", e.visibility === "Shared" ? "pi-users" : "pi-lock"]),
							"aria-hidden": "true"
						}, null, 2),
						f("span", ie, a(e.name), 1),
						e.openTasks ? (E(), y(l, {
							key: 0,
							value: e.openTasks,
							severity: "secondary"
						}, null, 8, ["value"])) : g("", !0)
					], 42, re))), 128)),
					n(O) ? (E(), y(d, {
						key: 1,
						text: "",
						size: "small",
						icon: "pi pi-plus",
						label: n(_).t("newList"),
						onClick: i[3] ||= (e) => Q.value = { mode: "create" }
					}, null, 8, ["label"])) : g("", !0),
					R.value.length ? (E(), p(u, { key: 2 }, [w(d, {
						text: "",
						size: "small",
						severity: "secondary",
						icon: P.value ? "pi pi-chevron-down" : "pi pi-chevron-right",
						label: `${n(_).t("archive")} (${R.value.length})`,
						onClick: i[4] ||= (e) => P.value = !P.value
					}, null, 8, ["icon", "label"]), P.value ? (E(!0), p(u, { key: 0 }, e(R.value, (e) => (E(), p("button", {
						key: e.id,
						type: "button",
						class: c(["todo-nav__archived", { active: q(e.id) }]),
						onClick: (t) => K({
							kind: "list",
							id: e.id
						})
					}, [i[20] ||= f("i", {
						class: "pi pi-inbox",
						"aria-hidden": "true"
					}, null, -1), o(a(e.name), 1)], 10, ae))), 128)) : g("", !0)], 64)) : g("", !0)
				], 8, ne),
				f("section", oe, [
					f("div", se, [w(m, {
						modelValue: G.value,
						"onUpdate:modelValue": i[5] ||= (e) => G.value = e,
						options: _e.value,
						"option-label": "label",
						"option-value": "key",
						class: "w-full"
					}, null, 8, ["modelValue", "options"]), n(O) ? (E(), y(d, {
						key: 0,
						icon: "pi pi-plus",
						text: "",
						"aria-label": n(_).t("newList"),
						onClick: i[6] ||= (e) => Q.value = { mode: "create" }
					}, null, 8, ["aria-label"])) : g("", !0)]),
					f("header", ce, [
						f("h2", null, [z.value ? (E(), p("i", {
							key: 0,
							class: c(["pi", z.value.visibility === "Shared" ? "pi-users" : "pi-lock"]),
							title: n(_).t(`visibility.${z.value.visibility}`)
						}, null, 10, le)) : g("", !0), o(" " + a(Fe.value), 1)]),
						z.value?.archived ? (E(), y(b, {
							key: 0,
							value: n(_).t("archived"),
							severity: "secondary"
						}, null, 8, ["value"])) : g("", !0),
						he(z.value) ? (E(), p(u, { key: 1 }, [w(d, {
							icon: "pi pi-ellipsis-v",
							text: "",
							rounded: "",
							"aria-label": n(_).t("listActions"),
							onClick: i[7] ||= (e) => Ne.value?.toggle(e)
						}, null, 8, ["aria-label"]), w(x, {
							ref_key: "listMenu",
							ref: Ne,
							model: Pe.value,
							popup: ""
						}, null, 8, ["model"])], 64)) : g("", !0)
					]),
					W(z.value) ? (E(), p("form", {
						key: 0,
						class: "todo-add",
						onSubmit: h(ye, ["prevent"])
					}, [w(C, {
						modelValue: J.value,
						"onUpdate:modelValue": i[8] ||= (e) => J.value = e,
						placeholder: n(_).t("addPlaceholder"),
						maxlength: "200",
						class: "todo-add__input"
					}, null, 8, ["modelValue", "placeholder"]), w(d, {
						type: "submit",
						icon: "pi pi-plus",
						"aria-label": n(_).t("add"),
						disabled: !J.value.trim()
					}, null, 8, ["aria-label", "disabled"])], 32)) : n(O) ? g("", !0) : (E(), y(M, {
						key: 1,
						severity: "secondary",
						size: "small"
					}, {
						default: r(() => [o(a(n(_).t("readOnly")), 1)]),
						_: 1
					})),
					n(V).isPending.value ? (E(), p("div", ue, [(E(), p(u, null, e(3, (e) => w(B, {
						key: e,
						height: "2.5rem"
					})), 64))])) : (E(), p(u, { key: 3 }, [
						f("ul", {
							class: "todo-tasks",
							onDragover: i[10] ||= h(() => {}, ["prevent"]),
							onDrop: i[11] ||= h((e) => z.value && Z(z.value.id, null), ["prevent"])
						}, [(E(!0), p(u, null, e(pe.value, (e) => (E(), y(S, {
							key: e.id,
							task: e,
							"list-name": U.value ? me(e.listId) : null,
							"assignee-name": n(j)(e.assigneeId),
							"can-toggle": W(I.value.find((t) => t.id === e.listId)),
							draggable: Ee.value,
							onToggle: (t) => n(be).mutate(e),
							onOpen: (t) => Y.value = e,
							onDragstart: (t) => X.value = e,
							onDragend: i[9] ||= (e) => X.value = null,
							onDrop: h((t) => Z(e.listId, e.id), ["stop", "prevent"])
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
						pe.value.length ? g("", !0) : (E(), p("p", de, a(N.value?.kind === "today" ? n(_).t("empty.today") : N.value?.kind === "mine" ? n(_).t("empty.mine") : n(_).t("empty.list")), 1)),
						H.value.length ? (E(), p(u, { key: 1 }, [w(d, {
							text: "",
							size: "small",
							severity: "secondary",
							icon: F.value ? "pi pi-chevron-down" : "pi pi-chevron-right",
							label: n(_).t("doneCount", { count: H.value.length }),
							onClick: i[12] ||= (e) => F.value = !F.value
						}, null, 8, ["icon", "label"]), F.value ? (E(), p("ul", fe, [(E(!0), p(u, null, e(H.value, (e) => (E(), y(S, {
							key: e.id,
							task: e,
							"list-name": U.value ? me(e.listId) : null,
							"assignee-name": n(j)(e.assigneeId),
							"can-toggle": W(I.value.find((t) => t.id === e.listId)),
							onToggle: (t) => n(be).mutate(e),
							onOpen: (t) => Y.value = e
						}, null, 8, [
							"task",
							"list-name",
							"assignee-name",
							"can-toggle",
							"onToggle",
							"onOpen"
						]))), 128))])) : g("", !0)], 64)) : g("", !0)
					], 64))
				]),
				w(ve, {
					visible: Y.value !== null,
					position: "right",
					header: n(_).t("editTask"),
					class: "todo-drawer",
					"onUpdate:visible": i[14] ||= (e) => !e && (Y.value = null)
				}, {
					default: r(() => [Y.value ? (E(), y(ee, {
						key: 0,
						task: Y.value,
						lists: I.value,
						members: n(v).members.data.value ?? [],
						"user-id": n(D),
						"can-edit": W(I.value.find((e) => e.id === Y.value.listId)),
						"can-delete": ge(Y.value),
						saving: n(xe).isPending.value,
						onSave: Ce,
						onDelete: we,
						onClose: i[13] ||= (e) => Y.value = null
					}, null, 8, [
						"task",
						"lists",
						"members",
						"user-id",
						"can-edit",
						"can-delete",
						"saving"
					])) : g("", !0)]),
					_: 1
				}, 8, ["visible", "header"]),
				w(k, {
					visible: Q.value !== null,
					mode: Q.value?.mode ?? "create",
					name: Q.value?.mode === "rename" ? z.value?.name : "",
					"can-share": n(A),
					"onUpdate:visible": i[15] ||= (e) => !e && (Q.value = null),
					onSubmit: ke
				}, null, 8, [
					"visible",
					"mode",
					"name",
					"can-share"
				]),
				w(Se, {
					visible: $.value,
					"onUpdate:visible": i[17] ||= (e) => $.value = e,
					modal: "",
					header: n(_).t("deleteList"),
					style: {
						width: "24rem",
						maxWidth: "calc(100vw - 2rem)"
					}
				}, {
					footer: r(() => [w(d, {
						severity: "secondary",
						text: "",
						label: n(_).t("cancel"),
						onClick: i[16] ||= (e) => $.value = !1
					}, null, 8, ["label"]), w(d, {
						severity: "danger",
						icon: "pi pi-trash",
						label: n(_).t("delete"),
						onClick: Me
					}, null, 8, ["label"])]),
					default: r(() => [f("p", null, a(n(_).t("deleteListConfirm", { name: z.value?.name ?? "" })), 1)]),
					_: 1
				}, 8, ["visible", "header"])
			]);
		};
	}
}), [["__scopeId", "data-v-defaf7ec"]]);
export { z as default };
