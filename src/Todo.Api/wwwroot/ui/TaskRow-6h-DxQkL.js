// \0homeapps-shared:vue
var e = globalThis.__HOMEAPPS_SHARED__?.vue;
if (!e) throw Error("HomeApps does not share vue; is this plugin loaded by the HomeApps web app?");
e.default;
var { BaseTransition: t, BaseTransitionPropsValidators: n, Comment: r, DeprecationTypes: i, EffectScope: a, ErrorCodes: o, ErrorTypeStrings: s, Fragment: c, KeepAlive: l, ReactiveEffect: u, Static: d, Suspense: ee, Teleport: te, Text: ne, TrackOpTypes: re, Transition: ie, TransitionGroup: ae, TriggerOpTypes: oe, VueElement: se, assertNumber: ce, callWithAsyncErrorHandling: le, callWithErrorHandling: ue, camelize: de, capitalize: fe, cloneVNode: pe, compatUtils: me, compile: he, computed: f, createApp: ge, createBlock: p, createCommentVNode: m, createElementBlock: h, createElementVNode: g, createHydrationRenderer: _e, createPropsRestProxy: ve, createRenderer: ye, createSSRApp: be, createSlots: xe, createStaticVNode: Se, createTextVNode: _, createVNode: v, customRef: Ce, defineAsyncComponent: we, defineComponent: y, defineCustomElement: Te, defineEmits: Ee, defineExpose: De, defineModel: Oe, defineOptions: ke, defineProps: Ae, defineSSRCustomElement: je, defineSlots: Me, devtools: Ne, effect: Pe, effectScope: Fe, getCurrentInstance: Ie, getCurrentScope: Le, getCurrentWatcher: Re, getTransitionRawChildren: ze, guardReactiveProps: Be, h: Ve, handleError: He, hasInjectionContext: Ue, hydrate: We, hydrateOnIdle: Ge, hydrateOnInteraction: Ke, hydrateOnMediaQuery: qe, hydrateOnVisible: Je, initCustomFormatter: Ye, initDirectivesForSSR: Xe, inject: Ze, isMemoSame: Qe, isProxy: $e, isReactive: et, isReadonly: tt, isRef: nt, isRuntimeOnly: rt, isShallow: it, isVNode: at, markRaw: ot, mergeDefaults: st, mergeModels: ct, mergeProps: lt, nextTick: ut, nodeOps: dt, normalizeClass: b, normalizeProps: ft, normalizeStyle: pt, onActivated: mt, onBeforeMount: ht, onBeforeUnmount: gt, onBeforeUpdate: _t, onDeactivated: vt, onErrorCaptured: yt, onMounted: bt, onRenderTracked: xt, onRenderTriggered: St, onScopeDispose: Ct, onServerPrefetch: wt, onUnmounted: Tt, onUpdated: Et, onWatcherCleanup: Dt, openBlock: x, patchProp: Ot, popScopeId: kt, provide: At, proxyRefs: jt, pushScopeId: Mt, queuePostFlushCb: Nt, reactive: Pt, readonly: Ft, ref: S, registerRuntimeCompiler: It, render: Lt, renderList: C, renderSlot: Rt, resolveComponent: w, resolveDirective: zt, resolveDynamicComponent: Bt, resolveFilter: Vt, resolveTransitionHooks: Ht, setBlockTracking: Ut, setDevtoolsHook: Wt, setTransitionHooks: Gt, shallowReactive: Kt, shallowReadonly: qt, shallowRef: Jt, ssrContextKey: Yt, ssrUtils: Xt, stop: Zt, toDisplayString: T, toHandlerKey: Qt, toHandlers: $t, toRaw: en, toRef: tn, toRefs: nn, toValue: E, transformVNodeArgs: rn, triggerRef: an, unref: D, useAttrs: on, useCssModule: sn, useCssVars: cn, useHost: ln, useId: un, useModel: dn, useSSRContext: fn, useShadowRoot: pn, useSlots: mn, useTemplateRef: hn, useTransitionState: gn, vModelCheckbox: _n, vModelDynamic: vn, vModelRadio: yn, vModelSelect: bn, vModelText: xn, vShow: Sn, version: Cn, warn: wn, watch: O, watchEffect: Tn, watchPostEffect: En, watchSyncEffect: Dn, withAsyncContext: On, withCtx: k, withDefaults: kn, withDirectives: An, withKeys: jn, withMemo: Mn, withModifiers: A, withScopeId: Nn } = e;
// node_modules/@homeapps/plugin-sdk/dist/index.js
function j(e) {
	return e;
}
var M = "homeapps:plugin-host";
function N() {
	let e = Ze(M);
	if (!e) throw Error("useHost() works only inside a component the HomeApps web app renders for a plugin.");
	return e;
}
// src/dates.ts
function P(e = /* @__PURE__ */ new Date()) {
	return F(e);
}
function F(e) {
	let t = (e) => String(e).padStart(2, "0");
	return `${e.getFullYear()}-${t(e.getMonth() + 1)}-${t(e.getDate())}`;
}
function I(e) {
	let [t, n, r] = e.split("-").map(Number);
	return new Date(t ?? 1970, (n ?? 1) - 1, r ?? 1);
}
function L(e, t = P()) {
	return e < t ? "overdue" : e === t ? "today" : (I(e).getTime() - I(t).getTime()) / 864e5 <= 7 ? "soon" : "later";
}
function R(e, t, n, r = "pl-PL", i = P()) {
	let a = Math.round((I(e).getTime() - I(i).getTime()) / 864e5), o = a === 0 ? n.today : a === 1 ? n.tomorrow : a === -1 ? n.yesterday : I(e).toLocaleDateString(r, {
		weekday: "short",
		day: "numeric",
		month: "numeric"
	});
	return t ? `${o} ${t.slice(0, 5)}` : o;
}
// \0homeapps-shared:@tanstack/vue-query
var z = globalThis.__HOMEAPPS_SHARED__?.["@tanstack/vue-query"];
if (!z) throw Error("HomeApps does not share @tanstack/vue-query; is this plugin loaded by the HomeApps web app?");
z.default;
var { CancelledError: Pn, InfiniteQueryObserver: Fn, Mutation: In, MutationCache: Ln, MutationObserver: Rn, QueriesObserver: zn, Query: Bn, QueryCache: Vn, QueryClient: Hn, QueryObserver: Un, VUE_QUERY_CLIENT: Wn, VueQueryPlugin: Gn, dataTagErrorSymbol: Kn, dataTagSymbol: qn, defaultScheduler: Jn, defaultShouldDehydrateMutation: Yn, defaultShouldDehydrateQuery: Xn, dehydrate: Zn, dehydrateQuery: Qn, environmentManager: $n, experimental_streamedQuery: er, focusManager: tr, hashKey: nr, hydrate: rr, infiniteQueryOptions: ir, isCancelledError: ar, isServer: or, keepPreviousData: sr, matchMutation: cr, matchQuery: lr, mutationOptions: ur, noop: dr, notifyManager: fr, onlineManager: pr, partialMatchKey: mr, queryOptions: hr, replaceEqualDeep: gr, shouldThrowError: _r, skipToken: vr, timeoutManager: yr, unsetMarker: br, useInfiniteQuery: xr, useIsFetching: Sr, useIsMutating: Cr, useMutation: B, useMutationState: wr, usePrefetchInfiniteQuery: Tr, usePrefetchQuery: Er, useQueries: Dr, useQuery: V, useQueryClient: H } = z;
// src/api.ts
function U(e, t, n) {
	let r = `${t}/households/${n}`;
	return {
		lists: async () => (await e.get(`${r}/lists`)).data,
		createList: async (t, n) => (await e.post(`${r}/lists`, {
			name: t,
			visibility: n
		})).data.id,
		renameList: (t, n) => e.put(`${r}/lists/${t}`, { name: n }),
		archiveList: (t) => e.post(`${r}/lists/${t}/archive`),
		restoreList: (t) => e.post(`${r}/lists/${t}/restore`),
		deleteList: (t) => e.delete(`${r}/lists/${t}`),
		tasks: async (t) => (await e.get(`${r}/tasks`, { params: t })).data,
		createTask: async (t, n) => (await e.post(`${r}/lists/${t}/tasks`, n)).data.id,
		updateTask: (t, n) => e.put(`${r}/tasks/${t}`, n),
		completeTask: (t) => e.post(`${r}/tasks/${t}/complete`),
		reopenTask: (t) => e.post(`${r}/tasks/${t}/reopen`),
		moveTask: (t, n, i) => e.post(`${r}/tasks/${t}/move`, {
			listId: n,
			beforeTaskId: i
		}),
		deleteTask: (t) => e.delete(`${r}/tasks/${t}`),
		members: async () => (await e.get(`/households/${n}/members`)).data
	};
}
function W(e = "") {
	return {
		title: e,
		notes: null,
		dueDate: null,
		dueTime: null,
		priority: "Normal",
		assigneeId: null
	};
}
function G(e) {
	return {
		title: e.title,
		notes: e.notes,
		dueDate: e.dueDate,
		dueTime: e.dueTime,
		priority: e.priority,
		assigneeId: e.assigneeId
	};
}
// src/useTodo.ts
var K = 3e4, q = {
	all: (e) => [
		"plugins",
		"todo",
		e
	],
	lists: (e) => [
		"plugins",
		"todo",
		e,
		"lists"
	],
	tasks: (e, t) => [
		"plugins",
		"todo",
		e,
		"tasks",
		t
	],
	members: (e) => [
		"households",
		e,
		"members"
	]
};
function J(e) {
	let t = N(), n = H(), r = f(() => E(e)), i = f(() => U(t.http, t.apiBase, r.value)), a = f(() => t.user.value?.id ?? null), o = V({
		queryKey: f(() => q.lists(r.value)),
		queryFn: () => i.value.lists(),
		refetchInterval: K
	}), s = f(() => (t.household.value?.myRole ?? "Guest") !== "Guest"), c = V({
		queryKey: f(() => q.members(r.value)),
		queryFn: () => i.value.members(),
		enabled: s,
		staleTime: 6e4
	}), l = (e) => e ? c.data.value?.find((t) => t.userId === e)?.displayName ?? null : null;
	function u(e, t = !0) {
		return V({
			queryKey: f(() => q.tasks(r.value, E(e))),
			queryFn: () => i.value.tasks(E(e)),
			enabled: f(() => E(t)),
			refetchInterval: K
		});
	}
	function d(e) {
		return B({
			mutationFn: (t) => e(i.value, t),
			onError: (e) => t.notify("error", t.errorMessage(e)),
			onSettled: () => n.invalidateQueries({ queryKey: q.all(r.value) })
		});
	}
	return {
		host: t,
		api: i,
		userId: a,
		lists: o,
		members: c,
		memberName: l,
		tasks: u,
		change: d,
		canWrite: f(() => t.can("tasks.write")),
		canManage: f(() => t.can("tasks.manage"))
	};
}
// src/TaskRow.vue?vue&type=script&setup=true&lang.ts
var Y = ["draggable", "data-task-id"], X = {
	key: 0,
	class: "pi pi-bars todo-row__handle",
	"aria-hidden": "true"
}, Z = { class: "todo-row__title" }, Q = ["title"], Or = ["title"], kr = {
	key: 0,
	class: "todo-row__meta"
}, Ar = { key: 1 }, jr = { key: 2 }, Mr = /*@__PURE__*/ y({
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
	setup(e, { emit: t }) {
		let n = e, r = t, i = N(), a = f(() => n.task.status === "Done"), o = f(() => {
			let { dueDate: e, dueTime: t } = n.task;
			return e ? {
				label: R(e, t, {
					today: i.t("due.today"),
					tomorrow: i.t("due.tomorrow"),
					yesterday: i.t("due.yesterday")
				}),
				state: a.value ? "later" : L(e)
			} : null;
		}), s = {
			overdue: "danger",
			today: "warn",
			soon: "info",
			later: "secondary"
		};
		return (t, n) => {
			let c = w("Checkbox"), l = w("Tag");
			return x(), h("li", {
				class: b(["todo-row", {
					"todo-row--done": a.value,
					"todo-row--large": e.large
				}]),
				draggable: e.draggable,
				"data-task-id": e.task.id
			}, [
				e.draggable ? (x(), h("i", X)) : m("", !0),
				v(c, {
					"model-value": a.value,
					binary: "",
					disabled: !e.canToggle,
					"input-id": `todo-${e.task.id}`,
					"aria-label": a.value ? D(i).t("reopen") : D(i).t("complete"),
					"onUpdate:modelValue": n[0] ||= (e) => r("toggle")
				}, null, 8, [
					"model-value",
					"disabled",
					"input-id",
					"aria-label"
				]),
				g("button", {
					type: "button",
					class: "todo-row__body",
					onClick: n[1] ||= (e) => r("open")
				}, [g("span", Z, [
					e.task.priority === "High" ? (x(), h("i", {
						key: 0,
						class: "pi pi-flag-fill todo-row__high",
						title: D(i).t("priority.High")
					}, null, 8, Q)) : m("", !0),
					_(" " + T(e.task.title) + " ", 1),
					e.task.notes ? (x(), h("i", {
						key: 1,
						class: "pi pi-align-left todo-row__notes",
						title: D(i).t("notes")
					}, null, 8, Or)) : m("", !0)
				]), o.value || e.assigneeName || e.listName ? (x(), h("span", kr, [
					o.value ? (x(), p(l, {
						key: 0,
						value: o.value.label,
						severity: s[o.value.state],
						icon: "pi pi-calendar"
					}, null, 8, ["value", "severity"])) : m("", !0),
					e.assigneeName ? (x(), h("span", Ar, [n[2] ||= g("i", {
						class: "pi pi-user",
						"aria-hidden": "true"
					}, null, -1), _(" " + T(e.assigneeName), 1)])) : m("", !0),
					e.listName ? (x(), h("span", jr, [n[3] ||= g("i", {
						class: "pi pi-list",
						"aria-hidden": "true"
					}, null, -1), _(" " + T(e.listName), 1)])) : m("", !0)
				])) : m("", !0)])
			], 10, Y);
		};
	}
}), $ = (e, t) => {
	let n = e.__vccOpts || e;
	for (let [e, r] of t) n[e] = r;
	return n;
}, Nr = /*#__PURE__*/ $(Mr, [["__scopeId", "data-v-e063312c"]]);
export { C, O as D, D as E, k as O, S, T, _, G as a, b, P as c, c as d, f, g, h, W as i, A as k, j as l, m, $ as n, I as o, p, J as r, F as s, Nr as t, N as u, v, w, x, y };
