<script setup lang="ts">
import { computed, ref, watch } from 'vue'
import { useHost } from '@homeapps/plugin-sdk'
import { emptyInput, type ListVisibility, type TaskInput, type TaskQuery, type TaskView, type TodoListView } from './api'
import { todayIso } from './dates'
import { useTodo } from './useTodo'
import ListDialog from './ListDialog.vue'
import TaskEditor from './TaskEditor.vue'
import TaskRow from './TaskRow.vue'

// "Zadania": lists on the left (a dropdown on a phone), the chosen list or view on the right, editing in a side panel.
type View = { kind: 'list'; id: string } | { kind: 'today' } | { kind: 'mine' }

const host = useHost()
const todo = useTodo(() => host.household.value!.id)
const { lists, userId, canWrite, canManage, memberName, change } = todo

const view = ref<View | null>(null)
const showArchived = ref(false)
const showDone = ref(false)

const allLists = computed(() => lists.data.value ?? [])
const activeLists = computed(() =>
  [...allLists.value.filter((l) => !l.archived)].sort((a, b) =>
    a.visibility === b.visibility ? 0 : a.visibility === 'Shared' ? -1 : 1,
  ),
)
const archivedLists = computed(() => allLists.value.filter((l) => l.archived))

// Start on the first shared list ("Dom") once lists are there; fall back when the chosen list disappears.
watch(
  () => lists.data.value,
  (all) => {
    if (!all) return
    const current = view.value
    if (current?.kind === 'list' && all.some((l) => l.id === current.id)) return
    if (current && current.kind !== 'list') return
    const first = activeLists.value[0]
    view.value = first ? { kind: 'list', id: first.id } : { kind: 'today' }
  },
  { immediate: true },
)

const currentList = computed(() => {
  const current = view.value
  return current?.kind === 'list' ? (allLists.value.find((l) => l.id === current.id) ?? null) : null
})
const query = computed<TaskQuery>(() => {
  const current = view.value
  if (current?.kind === 'list') return { list: current.id }
  if (current?.kind === 'mine') return { assignee: 'me' }
  return { dueBy: todayIso() }
})
const tasks = todo.tasks(query, () => view.value !== null)
const openTasks = computed(() => (tasks.data.value ?? []).filter((t) => t.status === 'Open'))
const doneTasks = computed(() =>
  (tasks.data.value ?? [])
    .filter((t) => t.status === 'Done')
    .sort((a, b) => (b.completedAt ?? '').localeCompare(a.completedAt ?? '')),
)
const listName = (id: string) => allLists.value.find((l) => l.id === id)?.name ?? null
const mixed = computed(() => view.value?.kind !== 'list')

// What the caller may do, as the API decides it (manifest permissions plus the domain's owner rules).
const ownsOrManages = (list: TodoListView | null | undefined) =>
  !!list && (list.visibility === 'Shared' ? canManage.value : list.ownerId === userId.value)
const canChangeTasksOn = (list: TodoListView | null | undefined) => canWrite.value && !!list && !list.archived
const canDeleteTask = (task: TaskView) => {
  const list = allLists.value.find((l) => l.id === task.listId)
  if (!canChangeTasksOn(list)) return false
  return list!.visibility === 'Private' || task.createdBy === userId.value || canManage.value
}

// Phone: one dropdown with the views and lists.
const choices = computed(() => [
  { key: 'today', label: host.t('views.today') },
  ...(canWrite.value ? [{ key: 'mine', label: host.t('views.mine') }] : []),
  ...activeLists.value.map((l) => ({ key: l.id, label: l.name })),
  ...archivedLists.value.map((l) => ({ key: l.id, label: `${l.name} (${host.t('archived')})` })),
])
const choice = computed({
  get: () => (view.value?.kind === 'list' ? view.value.id : (view.value?.kind ?? 'today')),
  set: (key: string) => select(key === 'today' || key === 'mine' ? { kind: key } : { kind: 'list', id: key }),
})

function select(next: View) {
  view.value = next
  showDone.value = false
}
const isSelected = (key: string) => choice.value === key

// Quick add: a title and Enter, at the end of the current list.
const newTitle = ref('')
const create = change((api, { listId, input }: { listId: string; input: TaskInput }) => api.createTask(listId, input))
function quickAdd() {
  const title = newTitle.value.trim()
  if (!title || !currentList.value) return
  create.mutate({ listId: currentList.value.id, input: emptyInput(title) })
  newTitle.value = ''
}

const toggle = change((api, task: TaskView) => (task.status === 'Done' ? api.reopenTask(task.id) : api.completeTask(task.id)))

// Editing in the side panel.
const editing = ref<TaskView | null>(null)
const save = change(async (api, { task, input, listId }: { task: TaskView; input: TaskInput; listId: string }) => {
  await api.updateTask(task.id, input)
  if (listId !== task.listId) await api.moveTask(task.id, listId, null)
})
const remove = change((api, task: TaskView) => api.deleteTask(task.id))
function onSave(input: TaskInput, listId: string) {
  if (!editing.value) return
  save.mutate({ task: editing.value, input, listId }, { onSuccess: () => (editing.value = null) })
}
function onDelete() {
  if (!editing.value) return
  remove.mutate(editing.value, { onSuccess: () => (editing.value = null) })
}

// Drag and drop (desktop): onto a task = before it, onto the list's end or another list in the sidebar = at its end.
const dragging = ref<TaskView | null>(null)
const move = change((api, { task, listId, before }: { task: TaskView; listId: string; before: string | null }) =>
  api.moveTask(task.id, listId, before),
)
const canDrag = computed(() => canChangeTasksOn(currentList.value) && !mixed.value)
function dropOn(listId: string, before: string | null) {
  const task = dragging.value
  dragging.value = null
  if (!task || task.id === before) return
  const target = allLists.value.find((l) => l.id === listId)
  if (!canChangeTasksOn(target)) return
  move.mutate({ task, listId, before })
}

// Lists: new, rename, archive, restore, delete.
const dialog = ref<{ mode: 'create' | 'rename' } | null>(null)
const createList = change((api, { name, visibility }: { name: string; visibility: ListVisibility }) =>
  api.createList(name, visibility),
)
const renameList = change((api, { id, name }: { id: string; name: string }) => api.renameList(id, name))
function onListSubmit(name: string, visibility: ListVisibility) {
  if (dialog.value?.mode === 'rename' && currentList.value) {
    renameList.mutate({ id: currentList.value.id, name })
  } else {
    createList.mutate({ name, visibility }, { onSuccess: (id) => select({ kind: 'list', id: id as string }) })
  }
  dialog.value = null
}
const archive = change((api, list: TodoListView) => (list.archived ? api.restoreList(list.id) : api.archiveList(list.id)))
const deleteList = change((api, list: TodoListView) => api.deleteList(list.id))
const confirmDelete = ref(false)
function onDeleteList() {
  const list = currentList.value
  confirmDelete.value = false
  if (list) deleteList.mutate(list, { onSuccess: () => (view.value = null) })
}

const listMenu = ref<{ toggle: (event: Event) => void } | null>(null)
const listMenuItems = computed(() => {
  const list = currentList.value
  if (!list) return []
  return [
    { label: host.t('renameList'), icon: 'pi pi-pencil', disabled: list.archived, command: () => (dialog.value = { mode: 'rename' }) },
    {
      label: list.archived ? host.t('restoreList') : host.t('archiveList'),
      icon: list.archived ? 'pi pi-replay' : 'pi pi-inbox',
      command: () => archive.mutate(list),
    },
    { label: host.t('deleteList'), icon: 'pi pi-trash', command: () => (confirmDelete.value = true) },
  ]
})
const title = computed(() =>
  view.value?.kind === 'today' ? host.t('views.today') : view.value?.kind === 'mine' ? host.t('views.mine') : (currentList.value?.name ?? ''),
)
</script>

<template>
  <div class="todo-page">
    <nav class="todo-nav" :aria-label="host.t('lists')">
      <button type="button" :class="{ active: isSelected('today') }" @click="select({ kind: 'today' })">
        <i class="pi pi-sun" aria-hidden="true" />{{ host.t('views.today') }}
      </button>
      <button v-if="canWrite" type="button" :class="{ active: isSelected('mine') }" @click="select({ kind: 'mine' })">
        <i class="pi pi-user" aria-hidden="true" />{{ host.t('views.mine') }}
      </button>
      <Divider />
      <button
        v-for="list in activeLists"
        :key="list.id"
        type="button"
        :class="{ active: isSelected(list.id) }"
        @click="select({ kind: 'list', id: list.id })"
        @dragover.prevent
        @drop.prevent="dropOn(list.id, null)"
      >
        <i :class="['pi', list.visibility === 'Shared' ? 'pi-users' : 'pi-lock']" aria-hidden="true" />
        <span class="todo-nav__name">{{ list.name }}</span>
        <Badge v-if="list.openTasks" :value="list.openTasks" severity="secondary" />
      </button>
      <Button
        v-if="canWrite"
        text
        size="small"
        icon="pi pi-plus"
        :label="host.t('newList')"
        @click="dialog = { mode: 'create' }"
      />
      <template v-if="archivedLists.length">
        <Button
          text
          size="small"
          severity="secondary"
          :icon="showArchived ? 'pi pi-chevron-down' : 'pi pi-chevron-right'"
          :label="`${host.t('archive')} (${archivedLists.length})`"
          @click="showArchived = !showArchived"
        />
        <template v-if="showArchived">
          <button
            v-for="list in archivedLists"
            :key="list.id"
            type="button"
            class="todo-nav__archived"
            :class="{ active: isSelected(list.id) }"
            @click="select({ kind: 'list', id: list.id })"
          >
            <i class="pi pi-inbox" aria-hidden="true" />{{ list.name }}
          </button>
        </template>
      </template>
    </nav>

    <section class="todo-main">
      <div class="todo-phone-picker">
        <Select v-model="choice" :options="choices" option-label="label" option-value="key" class="w-full" />
        <Button v-if="canWrite" icon="pi pi-plus" text :aria-label="host.t('newList')" @click="dialog = { mode: 'create' }" />
      </div>

      <header class="todo-header">
        <h2>
          <i
            v-if="currentList"
            :class="['pi', currentList.visibility === 'Shared' ? 'pi-users' : 'pi-lock']"
            :title="host.t(`visibility.${currentList.visibility}`)"
          />
          {{ title }}
        </h2>
        <Tag v-if="currentList?.archived" :value="host.t('archived')" severity="secondary" />
        <template v-if="ownsOrManages(currentList)">
          <Button icon="pi pi-ellipsis-v" text rounded :aria-label="host.t('listActions')" @click="listMenu?.toggle($event)" />
          <Menu ref="listMenu" :model="listMenuItems" popup />
        </template>
      </header>

      <form v-if="canChangeTasksOn(currentList)" class="todo-add" @submit.prevent="quickAdd">
        <InputText v-model="newTitle" :placeholder="host.t('addPlaceholder')" maxlength="200" class="todo-add__input" />
        <Button type="submit" icon="pi pi-plus" :aria-label="host.t('add')" :disabled="!newTitle.trim()" />
      </form>
      <Message v-else-if="!canWrite" severity="secondary" size="small">{{ host.t('readOnly') }}</Message>

      <div v-if="tasks.isPending.value" class="todo-loading">
        <Skeleton v-for="n in 3" :key="n" height="2.5rem" />
      </div>
      <template v-else>
        <ul class="todo-tasks" @dragover.prevent @drop.prevent="currentList && dropOn(currentList.id, null)">
          <TaskRow
            v-for="task in openTasks"
            :key="task.id"
            :task="task"
            :list-name="mixed ? listName(task.listId) : null"
            :assignee-name="memberName(task.assigneeId)"
            :can-toggle="canChangeTasksOn(allLists.find((l) => l.id === task.listId))"
            :draggable="canDrag"
            @toggle="toggle.mutate(task)"
            @open="editing = task"
            @dragstart="dragging = task"
            @dragend="dragging = null"
            @drop.stop.prevent="dropOn(task.listId, task.id)"
          />
        </ul>
        <p v-if="!openTasks.length" class="todo-empty">
          {{ view?.kind === 'today' ? host.t('empty.today') : view?.kind === 'mine' ? host.t('empty.mine') : host.t('empty.list') }}
        </p>

        <template v-if="doneTasks.length">
          <Button
            text
            size="small"
            severity="secondary"
            :icon="showDone ? 'pi pi-chevron-down' : 'pi pi-chevron-right'"
            :label="host.t('doneCount', { count: doneTasks.length })"
            @click="showDone = !showDone"
          />
          <ul v-if="showDone" class="todo-tasks">
            <TaskRow
              v-for="task in doneTasks"
              :key="task.id"
              :task="task"
              :list-name="mixed ? listName(task.listId) : null"
              :assignee-name="memberName(task.assigneeId)"
              :can-toggle="canChangeTasksOn(allLists.find((l) => l.id === task.listId))"
              @toggle="toggle.mutate(task)"
              @open="editing = task"
            />
          </ul>
        </template>
      </template>
    </section>

    <Drawer
      :visible="editing !== null"
      position="right"
      :header="host.t('editTask')"
      class="todo-drawer"
      @update:visible="!$event && (editing = null)"
    >
      <TaskEditor
        v-if="editing"
        :task="editing"
        :lists="allLists"
        :members="todo.members.data.value ?? []"
        :user-id="userId"
        :can-edit="canChangeTasksOn(allLists.find((l) => l.id === editing!.listId))"
        :can-delete="canDeleteTask(editing)"
        :saving="save.isPending.value"
        @save="onSave"
        @delete="onDelete"
        @close="editing = null"
      />
    </Drawer>

    <ListDialog
      :visible="dialog !== null"
      :mode="dialog?.mode ?? 'create'"
      :name="dialog?.mode === 'rename' ? currentList?.name : ''"
      :can-share="canManage"
      @update:visible="!$event && (dialog = null)"
      @submit="onListSubmit"
    />

    <Dialog v-model:visible="confirmDelete" modal :header="host.t('deleteList')" :style="{ width: '24rem', maxWidth: 'calc(100vw - 2rem)' }">
      <p>{{ host.t('deleteListConfirm', { name: currentList?.name ?? '' }) }}</p>
      <template #footer>
        <Button severity="secondary" text :label="host.t('cancel')" @click="confirmDelete = false" />
        <Button severity="danger" icon="pi pi-trash" :label="host.t('delete')" @click="onDeleteList" />
      </template>
    </Dialog>
  </div>
</template>

<style scoped>
.todo-page {
  display: grid;
  grid-template-columns: 15rem 1fr;
  gap: 1.5rem;
  align-items: start;
}
.todo-nav {
  display: flex;
  flex-direction: column;
  gap: 0.15rem;
  position: sticky;
  top: 1rem;
}
.todo-nav button:not(.p-button) {
  display: flex;
  align-items: center;
  gap: 0.6rem;
  width: 100%;
  padding: 0.5rem 0.75rem;
  border: 0;
  border-radius: var(--p-border-radius-md, 6px);
  background: none;
  color: inherit;
  font: inherit;
  text-align: left;
  cursor: pointer;
}
.todo-nav button:not(.p-button):hover {
  background: var(--p-content-hover-background);
}
.todo-nav button.active {
  background: var(--p-highlight-background);
  color: var(--p-highlight-color);
}
.todo-nav > .p-button {
  align-self: flex-start;
}
.todo-nav__name {
  flex: 1;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}
.todo-nav__archived {
  color: var(--p-text-muted-color) !important;
}
.todo-main {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
  min-width: 0;
}
.todo-main > .p-button {
  align-self: flex-start;
}
.todo-phone-picker {
  display: none;
  gap: 0.5rem;
}
.todo-header {
  display: flex;
  align-items: center;
  gap: 0.75rem;
}
.todo-header h2 {
  margin: 0;
  font-size: 1.25rem;
  font-weight: 600;
  display: flex;
  align-items: center;
  gap: 0.5rem;
}
.todo-header :deep(.p-button) {
  margin-left: auto;
}
.todo-add {
  display: flex;
  gap: 0.5rem;
}
.todo-add__input {
  flex: 1;
}
.todo-tasks {
  list-style: none;
  margin: 0;
  padding: 0;
  min-height: 1rem;
}
.todo-empty {
  color: var(--p-text-muted-color);
  margin: 0.5rem 0;
}
.todo-loading {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}
@media (max-width: 767px) {
  .todo-page {
    grid-template-columns: 1fr;
  }
  .todo-nav {
    display: none;
  }
  .todo-phone-picker {
    display: flex;
  }
}
</style>
<style>
.todo-drawer.p-drawer {
  width: min(28rem, 100vw);
}
</style>
