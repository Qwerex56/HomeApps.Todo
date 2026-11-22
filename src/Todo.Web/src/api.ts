import type { AxiosInstance } from 'axios'

// The plugin's API (Todo.Api, README "API"). Enums travel as their names.

export type ListVisibility = 'Shared' | 'Private'
export type TaskPriority = 'Low' | 'Normal' | 'High'
export type TodoStatus = 'Open' | 'Done'

export interface TodoListView {
  id: string
  name: string
  visibility: ListVisibility
  ownerId: string
  archived: boolean
  openTasks: number
  createdAt: string
}

export interface TaskView {
  id: string
  listId: string
  title: string
  notes: string | null
  dueDate: string | null
  dueTime: string | null
  assigneeId: string | null
  priority: TaskPriority
  status: TodoStatus
  position: number
  createdBy: string
  createdAt: string
  completedBy: string | null
  completedAt: string | null
}

export interface TaskInput {
  title: string
  notes: string | null
  dueDate: string | null
  dueTime: string | null
  priority: TaskPriority
  assigneeId: string | null
}

export interface TaskQuery {
  list?: string
  assignee?: 'me'
  dueBy?: string
  status?: TodoStatus
}

/** A member as Households lists them (GET /households/{id}/members); only Members and up may read it. */
export interface HouseholdMember {
  userId: string
  displayName: string
  avatarUrl: string | null
  role: string
}

export type CalendarItemType = 'Event' | 'Task' | 'External'
export type SeriesScope = 'ThisInstance' | 'ThisAndFuture' | 'All'
export type SubscriptionTasksMode = 'None' | 'Assigned' | 'All'
export type ExternalVisibility = 'OnlyMe' | 'Busy' | 'Details'
export type CalendarLayer = 'events' | 'private' | 'tasks' | 'external'

/** One entry of a calendar window (GET /calendar/events). Events carry UTC instants, tasks their local due date. */
export interface CalendarItem {
  itemType: CalendarItemType
  id: string
  seriesId: string | null
  isOccurrence: boolean
  occurrenceDate: string | null
  title: string
  description: string | null
  location: string | null
  timeZoneId: string | null
  startUtc: string | null
  endUtc: string | null
  date: string | null
  time: string | null
  isAllDay: boolean
  isPrivate: boolean
  attendees: string[]
  createdBy: string
  isDone: boolean | null
  color: string | null
}

export interface CalendarEventDetail {
  id: string
  seriesId: string | null
  occurrenceDate: string | null
  title: string
  description: string | null
  location: string | null
  timeZoneId: string
  startUtc: string
  endUtc: string
  isAllDay: boolean
  isPrivate: boolean
  attendees: string[]
  reminderMinutes: number[]
  rrule: string | null
  createdBy: string
}

export interface EventInput {
  title: string
  description: string | null
  location: string | null
  timeZoneId: string
  startUtc: string
  endUtc: string
  isAllDay: boolean
  isPrivate: boolean
  attendees: string[]
  reminderMinutes: number[]
  rrule?: string | null
}

export interface InstanceInput {
  scope: SeriesScope
  title?: string | null
  description?: string | null
  location?: string | null
  startUtc?: string | null
  endUtc?: string | null
  reminderMinutes?: number[] | null
  rrule?: string | null
}

export interface SubscriptionView {
  id: string
  label: string
  includeHouseholdEvents: boolean
  includePrivateEvents: boolean
  tasksMode: SubscriptionTasksMode
  lastAccessedAt: string | null
  createdAt: string
}

export interface CreatedSubscription {
  subscription: SubscriptionView
  token: string
  feedPath: string
}

export interface ExternalCalendarView {
  id: string
  name: string
  color: string
  visibility: ExternalVisibility
  lastSyncedAt: string | null
  lastSyncFailedAt: string | null
  lastErrorMessage: string | null
  isStale: boolean
}

export function todoApi(http: AxiosInstance, apiBase: string, householdId: string) {
  const base = `${apiBase}/households/${householdId}`
  return {
    lists: async () => (await http.get<TodoListView[]>(`${base}/lists`)).data,
    createList: async (name: string, visibility: ListVisibility) =>
      (await http.post<{ id: string }>(`${base}/lists`, { name, visibility })).data.id,
    renameList: (id: string, name: string) => http.put(`${base}/lists/${id}`, { name }),
    archiveList: (id: string) => http.post(`${base}/lists/${id}/archive`),
    restoreList: (id: string) => http.post(`${base}/lists/${id}/restore`),
    deleteList: (id: string) => http.delete(`${base}/lists/${id}`),

    tasks: async (query: TaskQuery) => (await http.get<TaskView[]>(`${base}/tasks`, { params: query })).data,
    createTask: async (listId: string, input: TaskInput) =>
      (await http.post<{ id: string }>(`${base}/lists/${listId}/tasks`, input)).data.id,
    updateTask: (id: string, input: TaskInput) => http.put(`${base}/tasks/${id}`, input),
    completeTask: (id: string) => http.post(`${base}/tasks/${id}/complete`),
    reopenTask: (id: string) => http.post(`${base}/tasks/${id}/reopen`),
    moveTask: (id: string, listId: string, beforeTaskId: string | null) =>
      http.post(`${base}/tasks/${id}/move`, { listId, beforeTaskId }),
    deleteTask: (id: string) => http.delete(`${base}/tasks/${id}`),

    calendarItems: async (from: string, to: string, layers: CalendarLayer[]) =>
      (await http.get<CalendarItem[]>(`${base}/calendar/events`, { params: { from, to, layers: layers.join(',') } })).data,
    event: async (id: string) => (await http.get<CalendarEventDetail>(`${base}/calendar/events/${id}`)).data,
    createEvent: async (input: EventInput) => (await http.post<CalendarEventDetail>(`${base}/calendar/events`, input)).data,
    updateEvent: async (id: string, input: EventInput) =>
      (await http.put<CalendarEventDetail>(`${base}/calendar/events/${id}`, input)).data,
    deleteEvent: (id: string) => http.delete(`${base}/calendar/events/${id}`),
    modifyInstance: async (seriesId: string, date: string, input: InstanceInput) =>
      (await http.put<CalendarEventDetail>(`${base}/calendar/events/${seriesId}/instances/${date}`, input)).data,
    deleteInstance: (seriesId: string, date: string, scope: SeriesScope) =>
      http.delete(`${base}/calendar/events/${seriesId}/instances/${date}`, { params: { scope } }),

    subscriptions: async () => (await http.get<SubscriptionView[]>(`${base}/calendar/subscriptions`)).data,
    createSubscription: async (input: { label: string; includeHouseholdEvents: boolean; includePrivateEvents: boolean; tasksMode: SubscriptionTasksMode }) =>
      (await http.post<CreatedSubscription>(`${base}/calendar/subscriptions`, input)).data,
    revokeSubscription: (id: string) => http.delete(`${base}/calendar/subscriptions/${id}`),

    externals: async () => (await http.get<ExternalCalendarView[]>(`${base}/calendar/external`)).data,
    addExternal: async (input: { url: string; name: string; color: string; visibility: ExternalVisibility }) =>
      (await http.post<ExternalCalendarView>(`${base}/calendar/external`, input)).data,
    syncExternal: async (id: string) => (await http.post<ExternalCalendarView>(`${base}/calendar/external/${id}/sync`)).data,
    removeExternal: (id: string) => http.delete(`${base}/calendar/external/${id}`),

    members: async () => (await http.get<HouseholdMember[]>(`/households/${householdId}/members`)).data,
  }
}

export type TodoApi = ReturnType<typeof todoApi>

export function emptyInput(title = ''): TaskInput {
  return { title, notes: null, dueDate: null, dueTime: null, priority: 'Normal', assigneeId: null }
}

export function inputOf(task: TaskView): TaskInput {
  return {
    title: task.title,
    notes: task.notes,
    dueDate: task.dueDate,
    dueTime: task.dueTime,
    priority: task.priority,
    assigneeId: task.assigneeId,
  }
}
