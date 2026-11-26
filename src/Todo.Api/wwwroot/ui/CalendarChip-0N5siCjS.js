import { A as e, C as t, L as n, S as r, T as i, _ as a, f as o, g as s, k as c, l, m as u, t as d, v as f, x as p } from "./_plugin-vue_export-helper-B-kgh5nT.js";
// src/calendarDates.ts
function m(e, t) {
	return new Date(e.getFullYear(), e.getMonth(), e.getDate() + t);
}
function h(e, t = 1) {
	return m(e, -((e.getDay() - t + 7) % 7));
}
function g(e) {
	return new Date(e.getFullYear(), e.getMonth(), 1);
}
function _(e, t = 1) {
	let n = h(g(e), t);
	return Array.from({ length: 42 }, (e, t) => m(n, t));
}
function v(e, t = 1) {
	let n = h(e, t);
	return Array.from({ length: 7 }, (e, t) => m(n, t));
}
var y = 30;
function b(e, t, n = 1) {
	return e === "month" ? _(t, n) : e === "week" ? v(t, n) : e === "day" ? [t] : Array.from({ length: y }, (e, n) => m(t, n));
}
function x(e, t, n = 1) {
	let r = b(e, t, n), i = r[0], a = r[r.length - 1];
	return {
		from: i.toISOString(),
		to: m(a, 1).toISOString()
	};
}
function S(e, t, n) {
	return e === "month" ? new Date(t.getFullYear(), t.getMonth() + n, 1) : e === "week" ? m(t, 7 * n) : e === "agenda" ? m(t, y * n) : m(t, n);
}
function C(e) {
	return e.date ? e.date : l(new Date(e.startUtc));
}
function w(e) {
	if (e.date || !e.endUtc) return C(e);
	let t = new Date(e.endUtc), n = t > new Date(e.startUtc) ? /* @__PURE__ */ new Date(t.getTime() - 1) : t;
	return l(n);
}
function T(e, t) {
	return C(e) <= t && t <= w(e);
}
function E(e, t = "pl-PL") {
	return e.itemType === "Task" ? e.time ? e.time.slice(0, 5) : null : e.isAllDay || !e.startUtc ? null : new Date(e.startUtc).toLocaleTimeString(t, {
		hour: "2-digit",
		minute: "2-digit"
	});
}
function D(e) {
	return E(e) ?? "00:00";
}
function O(e, t) {
	return e.filter((e) => T(e, t)).sort((e, t) => D(e).localeCompare(D(t)) || e.title.localeCompare(t.title));
}
var k = (e, t) => {
	let n = Object.fromEntries(new Intl.DateTimeFormat("en-CA", {
		timeZone: t,
		hourCycle: "h23",
		year: "numeric",
		month: "2-digit",
		day: "2-digit",
		hour: "2-digit",
		minute: "2-digit"
	}).formatToParts(new Date(e)).map((e) => [e.type, e.value]));
	return {
		day: `${n.year}-${n.month}-${n.day}`,
		time: `${n.hour}:${n.minute}`,
		asUtc: Date.UTC(+n.year, n.month - 1, +n.day, +n.hour, +n.minute)
	};
};
function A(e, t, n) {
	let [r, i] = (t || "00:00").split(":").map(Number), [a, o, s] = e.split("-").map(Number), c = Date.UTC(a, o - 1, s, r ?? 0, i ?? 0), l = c;
	for (let e = 0; e < 2; e++) l = c - (k(l, n).asUtc - l);
	return new Date(l).toISOString();
}
function j(e, t) {
	return k(Date.parse(e), t).time;
}
function M(e, t) {
	return k(Date.parse(e), t).day;
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
}, R = { class: "chip__title" }, z = /*#__PURE__*/ d(/* @__PURE__ */ p({
	__name: "CalendarChip",
	props: {
		item: {},
		canToggle: { type: Boolean },
		draggable: { type: Boolean }
	},
	emits: ["open", "toggle"],
	setup(l, { emit: d }) {
		let p = l, m = d, h = o(), g = u(() => E(p.item)), _ = u(() => p.item.itemType === "Task"), v = u(() => p.item.isDone === !0), y = u(() => `${h.t(`calendar.type.${p.item.itemType}`)}: ${p.item.title}${g.value ? `, ${g.value}` : ""}`), b = u(() => p.item.color ? { "--chip-color": p.item.color } : void 0);
		function x(e) {
			e.dataTransfer?.setData("text/plain", p.item.id), e.dataTransfer && (e.dataTransfer.effectAllowed = "move");
		}
		return (o, u) => (i(), a("div", {
			class: r(["chip", [`chip--${l.item.itemType.toLowerCase()}`, {
				"chip--done": v.value,
				"chip--private": l.item.isPrivate
			}]]),
			style: t(b.value),
			draggable: l.draggable,
			onDragstart: x
		}, [_.value ? (i(), a("input", {
			key: 0,
			type: "checkbox",
			class: "chip__check",
			checked: v.value,
			disabled: !l.canToggle,
			"aria-label": v.value ? e(h).t("reopen") : e(h).t("complete"),
			onClick: u[0] ||= n(() => {}, ["stop"]),
			onChange: u[1] ||= (e) => m("toggle")
		}, null, 40, P)) : s("", !0), f("button", {
			type: "button",
			class: "chip__body",
			"aria-label": y.value,
			"data-chip": "",
			onClick: u[2] ||= n((e) => m("open"), ["stop"]),
			onKeydown: u[3] ||= n(() => {}, ["stop"])
		}, [
			g.value ? (i(), a("span", I, c(g.value), 1)) : s("", !0),
			l.item.isPrivate ? (i(), a("i", L)) : s("", !0),
			f("span", R, c(l.item.title), 1)
		], 40, F)], 46, N));
	}
}), [["__scopeId", "data-v-a8dd170b"]]);
export { j as a, A as c, M as i, b as l, m as n, S as o, O as r, C as s, z as t, x as u };
