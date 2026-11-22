import { A as e, O as t, S as n, T as r, _ as i, b as a, f as o, g as s, h as c, k as l, m as u, o as d, s as f, t as p, v as m, x as h, y as g } from "./_plugin-vue_export-helper-Ntn7wuVI.js";
// src/TaskRow.vue?vue&type=script&setup=true&lang.ts
var _ = ["draggable", "data-task-id"], v = {
	key: 0,
	class: "pi pi-bars todo-row__handle",
	"aria-hidden": "true"
}, y = { class: "todo-row__title" }, b = ["title"], x = ["title"], S = {
	key: 0,
	class: "todo-row__meta"
}, C = { key: 1 }, w = { key: 2 }, T = /*#__PURE__*/ p(/* @__PURE__ */ h({
	__name: "TaskRow",
	props: {
		task: {},
		listName: {},
		assigneeName: {},
		canToggle: { type: Boolean },
		draggable: { type: Boolean },
		large: { type: Boolean }
	},
	emits: ["toggle", "open"],
	setup(p, { emit: h }) {
		let T = p, E = h, D = o(), O = u(() => T.task.status === "Done"), k = u(() => {
			let { dueDate: e, dueTime: t } = T.task;
			if (!e) return null;
			let n = {
				today: D.t("due.today"),
				tomorrow: D.t("due.tomorrow"),
				yesterday: D.t("due.yesterday")
			};
			return {
				label: d(e, t, n),
				state: O.value ? "later" : f(e)
			};
		}), A = {
			overdue: "danger",
			today: "warn",
			soon: "info",
			later: "secondary"
		};
		return (o, u) => {
			let d = t("Checkbox"), f = t("Tag");
			return r(), i("li", {
				class: n(["todo-row", {
					"todo-row--done": O.value,
					"todo-row--large": p.large
				}]),
				draggable: p.draggable,
				"data-task-id": p.task.id
			}, [
				p.draggable ? (r(), i("i", v)) : s("", !0),
				a(d, {
					"model-value": O.value,
					binary: "",
					disabled: !p.canToggle,
					"input-id": `todo-${p.task.id}`,
					"aria-label": O.value ? e(D).t("reopen") : e(D).t("complete"),
					"onUpdate:modelValue": u[0] ||= (e) => E("toggle")
				}, null, 8, [
					"model-value",
					"disabled",
					"input-id",
					"aria-label"
				]),
				m("button", {
					type: "button",
					class: "todo-row__body",
					onClick: u[1] ||= (e) => E("open")
				}, [m("span", y, [
					p.task.priority === "High" ? (r(), i("i", {
						key: 0,
						class: "pi pi-flag-fill todo-row__high",
						title: e(D).t("priority.High")
					}, null, 8, b)) : s("", !0),
					g(" " + l(p.task.title) + " ", 1),
					p.task.notes ? (r(), i("i", {
						key: 1,
						class: "pi pi-align-left todo-row__notes",
						title: e(D).t("notes")
					}, null, 8, x)) : s("", !0)
				]), k.value || p.assigneeName || p.listName ? (r(), i("span", S, [
					k.value ? (r(), c(f, {
						key: 0,
						value: k.value.label,
						severity: A[k.value.state],
						icon: "pi pi-calendar"
					}, null, 8, ["value", "severity"])) : s("", !0),
					p.assigneeName ? (r(), i("span", C, [u[2] ||= m("i", {
						class: "pi pi-user",
						"aria-hidden": "true"
					}, null, -1), g(" " + l(p.assigneeName), 1)])) : s("", !0),
					p.listName ? (r(), i("span", w, [u[3] ||= m("i", {
						class: "pi pi-list",
						"aria-hidden": "true"
					}, null, -1), g(" " + l(p.listName), 1)])) : s("", !0)
				])) : s("", !0)])
			], 10, _);
		};
	}
}), [["__scopeId", "data-v-e063312c"]]);
export { T as t };
