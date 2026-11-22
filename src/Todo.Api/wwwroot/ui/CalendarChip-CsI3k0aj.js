import { C as e, I as t, O as n, S as r, _ as i, c as a, f as o, g as s, k as c, l, m as u, t as d, v as f, w as p, x as m } from "./_plugin-vue_export-helper-DRFww2dX.js";
// src/calendarDates.ts
function h(e, t) {
	return new Date(e.getFullYear(), e.getMonth(), e.getDate() + t);
}
function g(e) {
	return h(e, -((e.getDay() + 6) % 7));
}
function _(e) {
	return new Date(e.getFullYear(), e.getMonth(), 1);
}
function v(e) {
	let t = g(_(e));
	return Array.from({ length: 42 }, (e, n) => h(t, n));
}
function y(e) {
	let t = g(e);
	return Array.from({ length: 7 }, (e, n) => h(t, n));
}
var b = 30;
function x(e, t) {
	return e === "month" ? v(t) : e === "week" ? y(t) : e === "day" ? [t] : Array.from({ length: b }, (e, n) => h(t, n));
}
function S(e, t) {
	let n = x(e, t), r = n[0], i = n[n.length - 1];
	return {
		from: r.toISOString(),
		to: h(i, 1).toISOString()
	};
}
function C(e, t, n) {
	return e === "month" ? new Date(t.getFullYear(), t.getMonth() + n, 1) : e === "week" ? h(t, 7 * n) : e === "agenda" ? h(t, b * n) : h(t, n);
}
function w(e) {
	return e.date ? e.date : l(new Date(e.startUtc));
}
function T(e) {
	if (e.date || !e.endUtc) return w(e);
	let t = new Date(e.endUtc), n = t > new Date(e.startUtc) ? /* @__PURE__ */ new Date(t.getTime() - 1) : t;
	return l(n);
}
function E(e, t) {
	return w(e) <= t && t <= T(e);
}
function D(e, t = "pl-PL") {
	return e.itemType === "Task" ? e.time ? e.time.slice(0, 5) : null : e.isAllDay || !e.startUtc ? null : new Date(e.startUtc).toLocaleTimeString(t, {
		hour: "2-digit",
		minute: "2-digit"
	});
}
function O(e) {
	return D(e) ?? "00:00";
}
function k(e, t) {
	return e.filter((e) => E(e, t)).sort((e, t) => O(e).localeCompare(O(t)) || e.title.localeCompare(t.title));
}
function A(e, t) {
	let [n, r] = (t || "00:00").split(":").map(Number), i = a(e);
	return i.setHours(n ?? 0, r ?? 0, 0, 0), i.toISOString();
}
function j(e) {
	let t = new Date(e);
	return `${String(t.getHours()).padStart(2, "0")}:${String(t.getMinutes()).padStart(2, "0")}`;
}
function M(e) {
	return l(new Date(e));
}
// src/CalendarChip.vue?vue&type=script&setup=true&lang.ts
var N = ["draggable"], P = [
	"checked",
	"disabled",
	"aria-label"
], F = ["aria-label"], I = {
	key: 0,
	class: "chip__time"
}, L = {
	key: 1,
	class: "pi pi-lock",
	"aria-hidden": "true"
}, R = { class: "chip__title" }, z = /*#__PURE__*/ d(/* @__PURE__ */ m({
	__name: "CalendarChip",
	props: {
		item: {},
		canToggle: { type: Boolean },
		draggable: { type: Boolean }
	},
	emits: ["open", "toggle"],
	setup(a, { emit: l }) {
		let d = a, m = l, h = o(), g = u(() => D(d.item)), _ = u(() => d.item.itemType === "Task"), v = u(() => d.item.isDone === !0), y = u(() => `${h.t(`calendar.type.${d.item.itemType}`)}: ${d.item.title}${g.value ? `, ${g.value}` : ""}`), b = u(() => d.item.color ? { "--chip-color": d.item.color } : void 0);
		function x(e) {
			e.dataTransfer?.setData("text/plain", d.item.id), e.dataTransfer && (e.dataTransfer.effectAllowed = "move");
		}
		return (o, l) => (p(), i("div", {
			class: r(["chip", [`chip--${a.item.itemType.toLowerCase()}`, {
				"chip--done": v.value,
				"chip--private": a.item.isPrivate
			}]]),
			style: e(b.value),
			draggable: a.draggable,
			onDragstart: x
		}, [_.value ? (p(), i("input", {
			key: 0,
			type: "checkbox",
			class: "chip__check",
			checked: v.value,
			disabled: !a.canToggle,
			"aria-label": v.value ? c(h).t("reopen") : c(h).t("complete"),
			onClick: l[0] ||= t(() => {}, ["stop"]),
			onChange: l[1] ||= (e) => m("toggle")
		}, null, 40, P)) : s("", !0), f("button", {
			type: "button",
			class: "chip__body",
			"aria-label": y.value,
			"data-chip": "",
			onClick: l[2] ||= t((e) => m("open"), ["stop"]),
			onKeydown: l[3] ||= t(() => {}, ["stop"])
		}, [
			g.value ? (p(), i("span", I, n(g.value), 1)) : s("", !0),
			a.item.isPrivate ? (p(), i("i", L)) : s("", !0),
			f("span", R, n(a.item.title), 1)
		], 40, F)], 46, N));
	}
}), [["__scopeId", "data-v-37cc63a0"]]);
export { j as a, A as c, M as i, x as l, h as n, C as o, k as r, w as s, z as t, S as u };
