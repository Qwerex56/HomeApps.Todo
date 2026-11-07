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
