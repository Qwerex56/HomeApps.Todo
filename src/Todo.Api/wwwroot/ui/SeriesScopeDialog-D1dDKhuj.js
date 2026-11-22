import { A as e, D as t, O as n, P as r, T as i, _ as a, b as o, f as s, h as c, p as l, t as u, v as d, x as f } from "./_plugin-vue_export-helper-BiWouYOk.js";
// src/SeriesScopeDialog.vue?vue&type=script&setup=true&lang.ts
var p = ["aria-label"], m = /*#__PURE__*/ u(/* @__PURE__ */ f({
	__name: "SeriesScopeDialog",
	props: {
		visible: { type: Boolean },
		mode: {}
	},
	emits: ["update:visible", "choose"],
	setup(u, { emit: f }) {
		let m = f, h = s(), g = [
			"ThisInstance",
			"ThisAndFuture",
			"All"
		];
		return (s, f) => {
			let _ = n("Button"), v = n("Dialog");
			return i(), c(v, {
				visible: u.visible,
				modal: "",
				header: e(h).t(u.mode === "edit" ? "calendar.scope.editTitle" : "calendar.scope.deleteTitle"),
				style: {
					width: "24rem",
					maxWidth: "calc(100vw - 2rem)"
				},
				"onUpdate:visible": f[1] ||= (e) => m("update:visible", e)
			}, {
				default: r(() => [d("div", {
					class: "scope",
					role: "group",
					"aria-label": e(h).t("calendar.scope.group")
				}, [(i(), a(l, null, t(g, (t) => o(_, {
					key: t,
					label: e(h).t(`calendar.scope.${t}`),
					severity: u.mode === "delete" ? "danger" : "primary",
					outlined: t !== "ThisInstance",
					class: "scope__button",
					onClick: (e) => m("choose", t)
				}, null, 8, [
					"label",
					"severity",
					"outlined",
					"onClick"
				])), 64)), o(_, {
					label: e(h).t("cancel"),
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
