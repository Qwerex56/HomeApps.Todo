import { D as e, E as t, N as n, _ as r, b as i, f as a, h as o, k as s, p as c, t as l, v as u, w as d, x as f } from "./_plugin-vue_export-helper-DRFww2dX.js";
// src/SeriesScopeDialog.vue?vue&type=script&setup=true&lang.ts
var p = ["aria-label"], m = /*#__PURE__*/ l(/* @__PURE__ */ f({
	__name: "SeriesScopeDialog",
	props: {
		visible: { type: Boolean },
		mode: {}
	},
	emits: ["update:visible", "choose"],
	setup(l, { emit: f }) {
		let m = f, h = a(), g = [
			"ThisInstance",
			"ThisAndFuture",
			"All"
		];
		return (a, f) => {
			let _ = e("Button"), v = e("Dialog");
			return d(), o(v, {
				visible: l.visible,
				modal: "",
				header: s(h).t(l.mode === "edit" ? "calendar.scope.editTitle" : "calendar.scope.deleteTitle"),
				style: {
					width: "24rem",
					maxWidth: "calc(100vw - 2rem)"
				},
				"onUpdate:visible": f[1] ||= (e) => m("update:visible", e)
			}, {
				default: n(() => [u("div", {
					class: "scope",
					role: "group",
					"aria-label": s(h).t("calendar.scope.group")
				}, [(d(), r(c, null, t(g, (e) => i(_, {
					key: e,
					label: s(h).t(`calendar.scope.${e}`),
					severity: l.mode === "delete" ? "danger" : "primary",
					outlined: e !== "ThisInstance",
					class: "scope__button",
					onClick: (t) => m("choose", e)
				}, null, 8, [
					"label",
					"severity",
					"outlined",
					"onClick"
				])), 64)), i(_, {
					label: s(h).t("cancel"),
					severity: "secondary",
					text: "",
					class: "scope__button",
					onClick: f[0] ||= (e) => m("update:visible", !1)
				}, null, 8, ["label"])], 8, p)]),
				_: 1
			}, 8, ["visible", "header"]);
		};
	}
}), [["__scopeId", "data-v-35daf7fc"]]);
export { m as t };
