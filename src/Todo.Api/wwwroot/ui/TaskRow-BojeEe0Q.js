import { D as e, O as t, S as n, _ as r, b as i, f as a, g as o, h as s, k as c, m as l, o as u, s as d, t as f, v as p, w as m, x as h, y as g } from "./_plugin-vue_export-helper-DX_-5sjr.js";
// src/TaskRow.vue?vue&type=script&setup=true&lang.ts
var _ = ["draggable", "data-task-id"], v = {
	key: 0,
	class: "pi pi-bars todo-row__handle",
	"aria-hidden": "true"
}, y = { class: "todo-row__title" }, b = ["title"], x = ["title"], S = {
	key: 0,
	class: "todo-row__meta"
}, C = { key: 1 }, w = { key: 2 }, T = /*#__PURE__*/ f(/* @__PURE__ */ h({
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
	setup(f, { emit: h }) {
		let T = f, E = h, D = a(), O = l(() => T.task.status === "Done"), k = l(() => {
			let { dueDate: e, dueTime: t } = T.task;
			if (!e) return null;
			let n = {
				today: D.t("due.today"),
				tomorrow: D.t("due.tomorrow"),
				yesterday: D.t("due.yesterday")
			};
			return {
				label: u(e, t, n),
				state: O.value ? "later" : d(e)
			};
		}), A = {
			overdue: "danger",
			today: "warn",
			soon: "info",
			later: "secondary"
		};
		return (a, l) => {
			let u = e("Checkbox"), d = e("Tag");
			return m(), r("li", {
				class: n(["todo-row", {
					"todo-row--done": O.value,
					"todo-row--large": f.large
				}]),
				draggable: f.draggable,
				"data-task-id": f.task.id
			}, [
				f.draggable ? (m(), r("i", v)) : o("", !0),
				i(u, {
					"model-value": O.value,
					binary: "",
					disabled: !f.canToggle,
					"input-id": `todo-${f.task.id}`,
					"aria-label": O.value ? c(D).t("reopen") : c(D).t("complete"),
					"onUpdate:modelValue": l[0] ||= (e) => E("toggle")
				}, null, 8, [
					"model-value",
					"disabled",
					"input-id",
					"aria-label"
				]),
				p("button", {
					type: "button",
					class: "todo-row__body",
					onClick: l[1] ||= (e) => E("open")
				}, [p("span", y, [
					f.task.priority === "High" ? (m(), r("i", {
						key: 0,
						class: "pi pi-flag-fill todo-row__high",
						title: c(D).t("priority.High")
					}, null, 8, b)) : o("", !0),
					g(" " + t(f.task.title) + " ", 1),
					f.task.notes ? (m(), r("i", {
						key: 1,
						class: "pi pi-align-left todo-row__notes",
						title: c(D).t("notes")
					}, null, 8, x)) : o("", !0)
				]), k.value || f.assigneeName || f.listName ? (m(), r("span", S, [
					k.value ? (m(), s(d, {
						key: 0,
						value: k.value.label,
						severity: A[k.value.state],
						icon: "pi pi-calendar"
					}, null, 8, ["value", "severity"])) : o("", !0),
					f.assigneeName ? (m(), r("span", C, [l[2] ||= p("i", {
						class: "pi pi-user",
						"aria-hidden": "true"
					}, null, -1), g(" " + t(f.assigneeName), 1)])) : o("", !0),
					f.listName ? (m(), r("span", w, [l[3] ||= p("i", {
						class: "pi pi-list",
						"aria-hidden": "true"
					}, null, -1), g(" " + t(f.listName), 1)])) : o("", !0)
				])) : o("", !0)])
			], 10, _);
		};
	}
}), [["__scopeId", "data-v-e063312c"]]);
export { T as t };
