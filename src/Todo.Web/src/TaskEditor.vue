<script setup lang="ts">
import { computed, ref, watch } from 'vue'
import { useHost } from '@homeapps/plugin-sdk'
import { inputOf, type HouseholdMember, type TaskInput, type TaskPriority, type TaskView, type TodoListView } from './api'
import { fromIso, isoDate } from './dates'

// The side panel for one task: the whole form (PUT), moving to another list, deleting.
const props = defineProps<{
  task: TaskView
  lists: TodoListView[]
  members: HouseholdMember[]
  userId: string | null
  canEdit: boolean
  canDelete: boolean
  saving: boolean
}>()
const emit = defineEmits<{ save: [input: TaskInput, listId: string]; delete: []; close: [] }>()

const host = useHost()
const form = ref<TaskInput>(inputOf(props.task))
const listId = ref(props.task.listId)
watch(
  () => props.task,
  (task) => {
    form.value = inputOf(task)
    listId.value = task.listId
  },
)

const list = computed(() => props.lists.find((l) => l.id === listId.value))
const date = computed({
  get: () => (form.value.dueDate ? fromIso(form.value.dueDate) : null),
  set: (value: Date | null) => {
    form.value.dueDate = value ? isoDate(value) : null
    if (!value) form.value.dueTime = null
  },
})
const time = computed({
  get: () => form.value.dueTime?.slice(0, 5) ?? '',
  set: (value: string) => (form.value.dueTime = value ? `${value}:00` : null),
})
const priorities = computed(() =>
  (['Low', 'Normal', 'High'] as TaskPriority[]).map((value) => ({ value, label: host.t(`priority.${value}`) })),
)
// A private list's tasks can be assigned to its owner only (the API says the same).
const assignees = computed(() => {
  const people =
    list.value?.visibility === 'Private' ? props.members.filter((m) => m.userId === props.userId) : props.members
  return [{ userId: null, displayName: host.t('nobody') }, ...people.map((m) => ({ userId: m.userId, displayName: m.displayName }))]
})
const targetLists = computed(() => props.lists.filter((l) => !l.archived || l.id === props.task.listId))
</script>

<template>
  <form class="todo-editor" @submit.prevent="emit('save', form, listId)">
    <label class="todo-editor__field">
      <span>{{ host.t('fields.title') }}</span>
      <InputText v-model="form.title" :disabled="!canEdit" maxlength="200" required autofocus />
    </label>
    <label class="todo-editor__field">
      <span>{{ host.t('fields.notes') }}</span>
      <Textarea v-model="form.notes" :disabled="!canEdit" rows="4" auto-resize maxlength="2000" />
    </label>
    <div class="todo-editor__row">
      <label class="todo-editor__field">
        <span>{{ host.t('fields.due') }}</span>
        <DatePicker v-model="date" :disabled="!canEdit" show-button-bar date-format="dd.mm.yy" show-icon />
      </label>
      <label class="todo-editor__field todo-editor__time">
        <span>{{ host.t('fields.time') }}</span>
        <InputText v-model="time" type="time" :disabled="!canEdit || !form.dueDate" />
      </label>
    </div>
    <div class="todo-editor__field">
      <span>{{ host.t('fields.priority') }}</span>
      <SelectButton
        v-model="form.priority"
        :options="priorities"
        option-label="label"
        option-value="value"
        :allow-empty="false"
        :disabled="!canEdit"
      />
    </div>
    <label v-if="members.length" class="todo-editor__field">
      <span>{{ host.t('fields.assignee') }}</span>
      <Select
        v-model="form.assigneeId"
        :placeholder="host.t('nobody')"
        :options="assignees"
        option-label="displayName"
        option-value="userId"
        :disabled="!canEdit"
      />
    </label>
    <label class="todo-editor__field">
      <span>{{ host.t('fields.list') }}</span>
      <Select v-model="listId" :options="targetLists" option-label="name" option-value="id" :disabled="!canEdit" />
    </label>
    <div class="todo-editor__actions">
      <Button
        v-if="canDelete"
        type="button"
        severity="danger"
        text
        icon="pi pi-trash"
        :label="host.t('delete')"
        @click="emit('delete')"
      />
      <span class="todo-editor__spacer" />
      <Button type="button" severity="secondary" text :label="host.t('cancel')" @click="emit('close')" />
      <Button v-if="canEdit" type="submit" icon="pi pi-check" :label="host.t('save')" :loading="saving" />
    </div>
  </form>
</template>

<style scoped>
.todo-editor {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}
.todo-editor__field {
  display: flex;
  flex-direction: column;
  gap: 0.35rem;
  flex: 1;
}
.todo-editor__field > span {
  font-size: 0.85rem;
  color: var(--p-text-muted-color);
}
.todo-editor__row {
  display: flex;
  gap: 0.75rem;
  flex-wrap: wrap;
}
.todo-editor__time {
  flex: 0 0 8rem;
}
.todo-editor__actions {
  display: flex;
  align-items: center;
  gap: 0.5rem;
}
.todo-editor__spacer {
  flex: 1;
}
</style>
