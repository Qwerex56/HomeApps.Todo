import { computed, toValue, type MaybeRefOrGetter } from 'vue'
import { useMutation, useQuery, useQueryClient } from '@tanstack/vue-query'
import { useHost } from '@homeapps/plugin-sdk'
import { todoApi, type HouseholdMember, type TaskQuery, type TodoApi } from './api'

/** How often open views ask again (plan 4.6: no realtime in 1.0, 30 s plus right after one's own changes). */
export const REFRESH_MS = 30_000

const keys = {
  all: (householdId: string) => ['plugins', 'todo', householdId] as const,
  lists: (householdId: string) => ['plugins', 'todo', householdId, 'lists'] as const,
  tasks: (householdId: string, query: TaskQuery) => ['plugins', 'todo', householdId, 'tasks', query] as const,
  // The web app's own key for the member list, so both share one request.
  members: (householdId: string) => ['households', householdId, 'members'] as const,
}

/** The plugin's API, queries and the permission checks for one household. */
export function useTodo(householdId: MaybeRefOrGetter<string>) {
  const host = useHost()
  const queryClient = useQueryClient()
  const id = computed(() => toValue(householdId))
  const api = computed(() => todoApi(host.http, host.apiBase, id.value))
  const userId = computed(() => host.user.value?.id ?? null)

  const lists = useQuery({
    queryKey: computed(() => keys.lists(id.value)),
    queryFn: () => api.value.lists(),
    refetchInterval: REFRESH_MS,
  })

  // Names for assignees: Households shows the member list to Members and up only (D2a), so guests see none.
  const canSeeMembers = computed(() => (host.household.value?.myRole ?? 'Guest') !== 'Guest')
  const members = useQuery({
    queryKey: computed(() => keys.members(id.value)),
    queryFn: () => api.value.members(),
    enabled: canSeeMembers,
    staleTime: 60_000,
  })
  const memberName = (userId: string | null) =>
    userId ? (members.data.value?.find((m: HouseholdMember) => m.userId === userId)?.displayName ?? null) : null

  function tasks(query: MaybeRefOrGetter<TaskQuery>, enabled: MaybeRefOrGetter<boolean> = true) {
    return useQuery({
      queryKey: computed(() => keys.tasks(id.value, toValue(query))),
      queryFn: () => api.value.tasks(toValue(query)),
      enabled: computed(() => toValue(enabled)),
      refetchInterval: REFRESH_MS,
    })
  }

  /** A change through the API, then everything of this household's plugin data asked again. Errors become a toast. */
  function change<TArgs>(run: (api: TodoApi, args: TArgs) => Promise<unknown>) {
    return useMutation({
      mutationFn: (args: TArgs) => run(api.value, args),
      onError: (error) => host.notify('error', host.errorMessage(error)),
      onSettled: () => queryClient.invalidateQueries({ queryKey: keys.all(id.value) }),
    })
  }

  return {
    host,
    api,
    userId,
    lists,
    members,
    memberName,
    tasks,
    change,
    canWrite: computed(() => host.can('tasks.write')),
    canManage: computed(() => host.can('tasks.manage')),
  }
}
