// \0homeapps-shared:vue
var e = globalThis.__HOMEAPPS_SHARED__?.vue;
if (!e) throw Error("HomeApps does not share vue; is this plugin loaded by the HomeApps web app?");
e.default;
var { BaseTransition: t, BaseTransitionPropsValidators: n, Comment: r, DeprecationTypes: i, EffectScope: a, ErrorCodes: o, ErrorTypeStrings: s, Fragment: c, KeepAlive: l, ReactiveEffect: u, Static: d, Suspense: f, Teleport: p, Text: m, TrackOpTypes: ee, Transition: te, TransitionGroup: ne, TriggerOpTypes: re, VueElement: ie, assertNumber: ae, callWithAsyncErrorHandling: oe, callWithErrorHandling: se, camelize: ce, capitalize: le, cloneVNode: ue, compatUtils: de, compile: fe, computed: h, createApp: pe, createBlock: g, createCommentVNode: _, createElementBlock: v, createElementVNode: me, createHydrationRenderer: he, createPropsRestProxy: ge, createRenderer: _e, createSSRApp: ve, createSlots: ye, createStaticVNode: be, createTextVNode: xe, createVNode: y, customRef: Se, defineAsyncComponent: Ce, defineComponent: b, defineCustomElement: we, defineEmits: Te, defineExpose: Ee, defineModel: De, defineOptions: Oe, defineProps: ke, defineSSRCustomElement: Ae, defineSlots: je, devtools: Me, effect: Ne, effectScope: Pe, getCurrentInstance: Fe, getCurrentScope: Ie, getCurrentWatcher: Le, getTransitionRawChildren: Re, guardReactiveProps: ze, h: Be, handleError: Ve, hasInjectionContext: He, hydrate: Ue, hydrateOnIdle: We, hydrateOnInteraction: Ge, hydrateOnMediaQuery: Ke, hydrateOnVisible: qe, initCustomFormatter: Je, initDirectivesForSSR: Ye, inject: x, isMemoSame: Xe, isProxy: Ze, isReactive: Qe, isReadonly: $e, isRef: et, isRuntimeOnly: tt, isShallow: nt, isVNode: rt, markRaw: it, mergeDefaults: at, mergeModels: ot, mergeProps: st, nextTick: ct, nodeOps: lt, normalizeClass: S, normalizeProps: ut, normalizeStyle: C, onActivated: dt, onBeforeMount: ft, onBeforeUnmount: pt, onBeforeUpdate: mt, onDeactivated: ht, onErrorCaptured: gt, onMounted: _t, onRenderTracked: vt, onRenderTriggered: yt, onScopeDispose: bt, onServerPrefetch: xt, onUnmounted: St, onUpdated: Ct, onWatcherCleanup: wt, openBlock: w, patchProp: Tt, popScopeId: Et, provide: Dt, proxyRefs: Ot, pushScopeId: kt, queuePostFlushCb: At, reactive: jt, readonly: Mt, ref: T, registerRuntimeCompiler: Nt, render: Pt, renderList: E, renderSlot: Ft, resolveComponent: D, resolveDirective: It, resolveDynamicComponent: Lt, resolveFilter: Rt, resolveTransitionHooks: zt, setBlockTracking: Bt, setDevtoolsHook: Vt, setTransitionHooks: Ht, shallowReactive: Ut, shallowReadonly: Wt, shallowRef: Gt, ssrContextKey: Kt, ssrUtils: qt, stop: Jt, toDisplayString: O, toHandlerKey: Yt, toHandlers: Xt, toRaw: Zt, toRef: Qt, toRefs: $t, toValue: k, transformVNodeArgs: en, triggerRef: tn, unref: A, useAttrs: nn, useCssModule: rn, useCssVars: an, useHost: on, useId: sn, useModel: cn, useSSRContext: ln, useShadowRoot: un, useSlots: dn, useTemplateRef: fn, useTransitionState: pn, vModelCheckbox: j, vModelDynamic: mn, vModelRadio: hn, vModelSelect: gn, vModelText: M, vShow: _n, version: vn, warn: yn, watch: N, watchEffect: bn, watchPostEffect: xn, watchSyncEffect: Sn, withAsyncContext: Cn, withCtx: P, withDefaults: wn, withDirectives: Tn, withKeys: F, withMemo: En, withModifiers: I, withScopeId: Dn } = e;
// node_modules/@homeapps/plugin-sdk/dist/index.js
function L(e) {
	return e;
}
var R = "homeapps:plugin-host";
function z() {
	let e = x(R);
	if (!e) throw Error("useHost() works only inside a component the HomeApps web app renders for a plugin.");
	return e;
}
// src/dates.ts
function B(e = /* @__PURE__ */ new Date()) {
	return V(e);
}
function V(e) {
	let t = (e) => String(e).padStart(2, "0");
	return `${e.getFullYear()}-${t(e.getMonth() + 1)}-${t(e.getDate())}`;
}
function H(e) {
	let [t, n, r] = e.split("-").map(Number);
	return new Date(t ?? 1970, (n ?? 1) - 1, r ?? 1);
}
function U(e, t = B()) {
	return e < t ? "overdue" : e === t ? "today" : (H(e).getTime() - H(t).getTime()) / 864e5 <= 7 ? "soon" : "later";
}
function W(e, t, n, r = "pl-PL", i = B()) {
	let a = Math.round((H(e).getTime() - H(i).getTime()) / 864e5), o = a === 0 ? n.today : a === 1 ? n.tomorrow : a === -1 ? n.yesterday : H(e).toLocaleDateString(r, {
		weekday: "short",
		day: "numeric",
		month: "numeric"
	});
	return t ? `${o} ${t.slice(0, 5)}` : o;
}
// \0homeapps-shared:@tanstack/vue-query
var G = globalThis.__HOMEAPPS_SHARED__?.["@tanstack/vue-query"];
if (!G) throw Error("HomeApps does not share @tanstack/vue-query; is this plugin loaded by the HomeApps web app?");
G.default;
var { CancelledError: On, InfiniteQueryObserver: kn, Mutation: An, MutationCache: jn, MutationObserver: Mn, QueriesObserver: Nn, Query: Pn, QueryCache: Fn, QueryClient: In, QueryObserver: Ln, VUE_QUERY_CLIENT: Rn, VueQueryPlugin: zn, dataTagErrorSymbol: Bn, dataTagSymbol: Vn, defaultScheduler: Hn, defaultShouldDehydrateMutation: Un, defaultShouldDehydrateQuery: Wn, dehydrate: Gn, dehydrateQuery: Kn, environmentManager: qn, experimental_streamedQuery: Jn, focusManager: Yn, hashKey: Xn, hydrate: Zn, infiniteQueryOptions: Qn, isCancelledError: $n, isServer: er, keepPreviousData: tr, matchMutation: nr, matchQuery: rr, mutationOptions: ir, noop: ar, notifyManager: or, onlineManager: sr, partialMatchKey: cr, queryOptions: lr, replaceEqualDeep: ur, shouldThrowError: dr, skipToken: fr, timeoutManager: pr, unsetMarker: mr, useInfiniteQuery: hr, useIsFetching: gr, useIsMutating: _r, useMutation: K, useMutationState: vr, usePrefetchInfiniteQuery: yr, usePrefetchQuery: br, useQueries: xr, useQuery: q, useQueryClient: J } = G;
// src/api.ts
function Y(e, t, n) {
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
		plannerSettings: async () => (await e.get(`${r}/calendar/settings`)).data,
		savePlannerSettings: async (t) => (await e.put(`${r}/calendar/settings`, t)).data,
		members: async () => (await e.get(`/households/${n}/members`)).data
	};
}
function X(e = "") {
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
function Z(e) {
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
function Sr(e) {
	let t = z(), n = J(), r = h(() => k(e)), i = h(() => Y(t.http, t.apiBase, r.value)), a = h(() => t.user.value?.id ?? null), o = q({
		queryKey: h(() => $.lists(r.value)),
		queryFn: () => i.value.lists(),
		refetchInterval: Q
	}), s = h(() => (t.household.value?.myRole ?? "Guest") !== "Guest"), c = q({
		queryKey: h(() => $.members(r.value)),
		queryFn: () => i.value.members(),
		enabled: s,
		staleTime: 6e4
	}), l = (e) => e ? c.data.value?.find((t) => t.userId === e)?.displayName ?? null : null, u = q({
		queryKey: h(() => [
			"plugins",
			"todo",
			r.value,
			"settings"
		]),
		queryFn: () => i.value.plannerSettings(),
		staleTime: 6e4
	}), d = h(() => u.data.value?.timeZone ?? Intl.DateTimeFormat().resolvedOptions().timeZone);
	function f(e, t = !0) {
		return q({
			queryKey: h(() => $.tasks(r.value, k(e))),
			queryFn: () => i.value.tasks(k(e)),
			enabled: h(() => k(t)),
			refetchInterval: Q
		});
	}
	function p(e, t) {
		return q({
			queryKey: h(() => [
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
	function m(e) {
		return K({
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
		calendar: p,
		change: m,
		canWrite: h(() => t.can("tasks.write")),
		canManage: h(() => t.can("tasks.manage"))
	};
}
// \0plugin-vue:export-helper
var Cr = (e, t) => {
	let n = e.__vccOpts || e;
	for (let [e, r] of t) n[e] = r;
	return n;
};
export { j as A, C, D, E, F, I, N as M, P as N, O, Tn as P, S, T, v as _, q as a, y as b, H as c, L as d, z as f, _ as g, g as h, Z as i, M as j, A as k, V as l, h as m, Sr as n, W as o, c as p, X as r, U as s, Cr as t, B as u, me as v, w, b as x, xe as y };
