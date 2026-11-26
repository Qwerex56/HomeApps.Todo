// \0homeapps-shared:vue
var e = globalThis.__HOMEAPPS_SHARED__?.vue;
if (!e) throw Error("HomeApps does not share vue; is this plugin loaded by the HomeApps web app?");
e.default;
var { BaseTransition: t, BaseTransitionPropsValidators: n, Comment: r, DeprecationTypes: i, EffectScope: a, ErrorCodes: o, ErrorTypeStrings: s, Fragment: c, KeepAlive: l, ReactiveEffect: u, Static: d, Suspense: f, Teleport: ee, Text: p, TrackOpTypes: te, Transition: ne, TransitionGroup: re, TriggerOpTypes: ie, VueElement: ae, assertNumber: oe, callWithAsyncErrorHandling: se, callWithErrorHandling: ce, camelize: le, capitalize: ue, cloneVNode: de, compatUtils: fe, compile: pe, computed: m, createApp: me, createBlock: h, createCommentVNode: g, createElementBlock: _, createElementVNode: v, createHydrationRenderer: he, createPropsRestProxy: ge, createRenderer: _e, createSSRApp: ve, createSlots: ye, createStaticVNode: be, createTextVNode: y, createVNode: xe, customRef: Se, defineAsyncComponent: Ce, defineComponent: we, defineCustomElement: Te, defineEmits: Ee, defineExpose: De, defineModel: Oe, defineOptions: ke, defineProps: Ae, defineSSRCustomElement: je, defineSlots: Me, devtools: Ne, effect: Pe, effectScope: Fe, getCurrentInstance: Ie, getCurrentScope: Le, getCurrentWatcher: Re, getTransitionRawChildren: ze, guardReactiveProps: Be, h: Ve, handleError: He, hasInjectionContext: Ue, hydrate: We, hydrateOnIdle: Ge, hydrateOnInteraction: Ke, hydrateOnMediaQuery: qe, hydrateOnVisible: Je, initCustomFormatter: Ye, initDirectivesForSSR: Xe, inject: b, isMemoSame: Ze, isProxy: Qe, isReactive: $e, isReadonly: et, isRef: tt, isRuntimeOnly: nt, isShallow: rt, isVNode: it, markRaw: at, mergeDefaults: ot, mergeModels: st, mergeProps: ct, nextTick: lt, nodeOps: ut, normalizeClass: x, normalizeProps: dt, normalizeStyle: S, onActivated: ft, onBeforeMount: pt, onBeforeUnmount: C, onBeforeUpdate: mt, onDeactivated: ht, onErrorCaptured: gt, onMounted: _t, onRenderTracked: vt, onRenderTriggered: yt, onScopeDispose: bt, onServerPrefetch: xt, onUnmounted: St, onUpdated: Ct, onWatcherCleanup: wt, openBlock: w, patchProp: Tt, popScopeId: Et, provide: Dt, proxyRefs: Ot, pushScopeId: kt, queuePostFlushCb: At, reactive: jt, readonly: Mt, ref: T, registerRuntimeCompiler: Nt, render: Pt, renderList: E, renderSlot: Ft, resolveComponent: D, resolveDirective: It, resolveDynamicComponent: Lt, resolveFilter: Rt, resolveTransitionHooks: zt, setBlockTracking: Bt, setDevtoolsHook: Vt, setTransitionHooks: Ht, shallowReactive: Ut, shallowReadonly: Wt, shallowRef: Gt, ssrContextKey: Kt, ssrUtils: qt, stop: Jt, toDisplayString: O, toHandlerKey: Yt, toHandlers: Xt, toRaw: Zt, toRef: Qt, toRefs: $t, toValue: k, transformVNodeArgs: en, triggerRef: tn, unref: A, useAttrs: nn, useCssModule: rn, useCssVars: an, useHost: on, useId: sn, useModel: cn, useSSRContext: ln, useShadowRoot: un, useSlots: dn, useTemplateRef: fn, useTransitionState: pn, vModelCheckbox: j, vModelDynamic: mn, vModelRadio: hn, vModelSelect: gn, vModelText: M, vShow: _n, version: vn, warn: yn, watch: N, watchEffect: bn, watchPostEffect: xn, watchSyncEffect: Sn, withAsyncContext: Cn, withCtx: P, withDefaults: wn, withDirectives: F, withKeys: I, withMemo: Tn, withModifiers: L, withScopeId: En } = e;
// node_modules/@home-apps/plugin-sdk/dist/index.js
function R(e) {
	return e;
}
var z = "homeapps:plugin-host";
function B() {
	let e = b(z);
	if (!e) throw Error("useHost() works only inside a component the HomeApps web app renders for a plugin.");
	return e;
}
// src/dates.ts
function V(e = /* @__PURE__ */ new Date()) {
	return H(e);
}
function H(e) {
	let t = (e) => String(e).padStart(2, "0");
	return `${e.getFullYear()}-${t(e.getMonth() + 1)}-${t(e.getDate())}`;
}
function U(e) {
	let [t, n, r] = e.split("-").map(Number);
	return new Date(t ?? 1970, (n ?? 1) - 1, r ?? 1);
}
function W(e, t = V()) {
	return e < t ? "overdue" : e === t ? "today" : (U(e).getTime() - U(t).getTime()) / 864e5 <= 7 ? "soon" : "later";
}
function G(e, t, n, r = "pl-PL", i = V()) {
	let a = Math.round((U(e).getTime() - U(i).getTime()) / 864e5), o = a === 0 ? n.today : a === 1 ? n.tomorrow : a === -1 ? n.yesterday : U(e).toLocaleDateString(r, {
		weekday: "short",
		day: "numeric",
		month: "numeric"
	});
	return t ? `${o} ${t.slice(0, 5)}` : o;
}
// \0homeapps-shared:@tanstack/vue-query
var K = globalThis.__HOMEAPPS_SHARED__?.["@tanstack/vue-query"];
if (!K) throw Error("HomeApps does not share @tanstack/vue-query; is this plugin loaded by the HomeApps web app?");
K.default;
var { CancelledError: Dn, InfiniteQueryObserver: On, Mutation: kn, MutationCache: An, MutationObserver: jn, QueriesObserver: Mn, Query: Nn, QueryCache: Pn, QueryClient: Fn, QueryObserver: In, VUE_QUERY_CLIENT: Ln, VueQueryPlugin: Rn, dataTagErrorSymbol: zn, dataTagSymbol: Bn, defaultScheduler: Vn, defaultShouldDehydrateMutation: Hn, defaultShouldDehydrateQuery: Un, dehydrate: Wn, dehydrateQuery: Gn, environmentManager: Kn, experimental_streamedQuery: qn, focusManager: Jn, hashKey: Yn, hydrate: Xn, infiniteQueryOptions: Zn, isCancelledError: Qn, isServer: $n, keepPreviousData: er, matchMutation: tr, matchQuery: nr, mutationOptions: rr, noop: ir, notifyManager: ar, onlineManager: or, partialMatchKey: sr, queryOptions: cr, replaceEqualDeep: lr, shouldThrowError: ur, skipToken: dr, timeoutManager: fr, unsetMarker: pr, useInfiniteQuery: mr, useIsFetching: hr, useIsMutating: gr, useMutation: q, useMutationState: _r, usePrefetchInfiniteQuery: vr, usePrefetchQuery: yr, useQueries: br, useQuery: J, useQueryClient: Y } = K, X = [
	"Sunday",
	"Monday",
	"Tuesday",
	"Wednesday",
	"Thursday",
	"Friday",
	"Saturday"
];
function Z(e) {
	let t = typeof e.firstDayOfWeek == "number" ? e.firstDayOfWeek : X.indexOf(e.firstDayOfWeek);
	return {
		...e,
		firstDayOfWeek: t < 0 ? 1 : t
	};
}
function xr(e, t, n) {
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
		updateTask: (t, n, i) => e.put(`${r}/tasks/${t}`, n, { params: i ? { scope: i } : void 0 }),
		completeTask: (t) => e.post(`${r}/tasks/${t}/complete`),
		reopenTask: (t) => e.post(`${r}/tasks/${t}/reopen`),
		moveTask: (t, n, i) => e.post(`${r}/tasks/${t}/move`, {
			listId: n,
			beforeTaskId: i
		}),
		deleteTask: (t, n) => e.delete(`${r}/tasks/${t}`, { params: n ? { scope: n } : void 0 }),
		calendarItems: async (t, n, i) => (await e.get(`${r}/calendar/events`, { params: {
			from: t,
			to: n,
			layers: i.join(",")
		} })).data,
		event: async (t) => (await e.get(`${r}/calendar/events/${t}`)).data,
		createEvent: async (t) => (await e.post(`${r}/calendar/events`, t)).data,
		updateEvent: async (t, n) => (await e.put(`${r}/calendar/events/${t}`, n)).data,
		deleteEvent: (t) => e.delete(`${r}/calendar/events/${t}`),
		modifyInstance: async (t, n, i) => (await e.put(`${r}/calendar/events/${t}/instances/${n}`, i)).data,
		deleteInstance: (t, n, i) => e.delete(`${r}/calendar/events/${t}/instances/${n}`, { params: { scope: i } }),
		subscriptions: async () => (await e.get(`${r}/calendar/subscriptions`)).data,
		createSubscription: async (t) => (await e.post(`${r}/calendar/subscriptions`, t)).data,
		revokeSubscription: (t) => e.delete(`${r}/calendar/subscriptions/${t}`),
		externals: async () => (await e.get(`${r}/calendar/external`)).data,
		addExternal: async (t) => (await e.post(`${r}/calendar/external`, t)).data,
		syncExternal: async (t) => (await e.post(`${r}/calendar/external/${t}/sync`)).data,
		removeExternal: (t) => e.delete(`${r}/calendar/external/${t}`),
		plannerSettings: async () => Z((await e.get(`${r}/calendar/settings`)).data),
		savePlannerSettings: async (t) => Z((await e.put(`${r}/calendar/settings`, {
			...t,
			firstDayOfWeek: X[t.firstDayOfWeek]
		})).data),
		members: async () => (await e.get(`/households/v1/${n}/members`)).data
	};
}
function Sr(e = "") {
	return {
		title: e,
		notes: null,
		dueDate: null,
		dueTime: null,
		priority: "Normal",
		assigneeId: null,
		rrule: null,
		anchor: null
	};
}
function Cr(e) {
	return {
		title: e.title,
		notes: e.notes,
		dueDate: e.dueDate,
		dueTime: e.dueTime,
		priority: e.priority,
		assigneeId: e.assigneeId,
		rrule: e.rrule,
		anchor: e.anchor
	};
}
// src/useTodo.ts
var Q = 3e4, $ = {
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
function wr(e) {
	let t = B(), n = Y(), r = m(() => k(e)), i = m(() => xr(t.http, t.apiBase, r.value)), a = m(() => t.user.value?.id ?? null), o = J({
		queryKey: m(() => $.lists(r.value)),
		queryFn: () => i.value.lists(),
		refetchInterval: Q
	}), s = m(() => (t.household.value?.myRole ?? "Guest") !== "Guest"), c = J({
		queryKey: m(() => $.members(r.value)),
		queryFn: () => i.value.members(),
		enabled: s,
		staleTime: 6e4
	}), l = (e) => e ? c.data.value?.find((t) => t.userId === e)?.displayName ?? null : null, u = J({
		queryKey: m(() => [
			"plugins",
			"todo",
			r.value,
			"settings"
		]),
		queryFn: () => i.value.plannerSettings(),
		staleTime: 6e4
	}), d = m(() => u.data.value?.timeZone ?? Intl.DateTimeFormat().resolvedOptions().timeZone);
	function f(e, t = !0) {
		return J({
			queryKey: m(() => $.tasks(r.value, k(e))),
			queryFn: () => i.value.tasks(k(e)),
			enabled: m(() => k(t)),
			refetchInterval: Q
		});
	}
	function ee(e, t) {
		return J({
			queryKey: m(() => [
				"plugins",
				"todo",
				r.value,
				"calendar",
				k(e),
				k(t)
			]),
			queryFn: () => i.value.calendarItems(k(e).from, k(e).to, k(t)),
			refetchInterval: Q
		});
	}
	function p(e) {
		return q({
			mutationFn: (t) => e(i.value, t),
			onError: (e) => t.notify("error", t.errorMessage(e)),
			onSettled: () => n.invalidateQueries({ queryKey: $.all(r.value) })
		});
	}
	return {
		host: t,
		api: i,
		userId: a,
		lists: o,
		members: c,
		memberName: l,
		settings: u,
		timeZone: d,
		tasks: f,
		calendar: ee,
		change: p,
		canWrite: m(() => t.can("tasks.write")),
		canManage: m(() => t.can("tasks.manage"))
	};
}
// \0plugin-vue:export-helper
var Tr = (e, t) => {
	let n = e.__vccOpts || e;
	for (let [e, r] of t) n[e] = r;
	return n;
};
export { A, S as C, E as D, T as E, F, I, L, M, N, D as O, P, x as S, w as T, _, J as a, xe as b, U as c, R as d, B as f, g, h, Cr as i, j, O as k, H as l, m, wr as n, G as o, c as p, Sr as r, W as s, Tr as t, V as u, v, C as w, we as x, y };
