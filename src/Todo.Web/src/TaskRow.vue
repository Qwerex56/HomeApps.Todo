<script setup lang="ts">
import { computed } from 'vue'
import { useHost } from '@homeapps/plugin-sdk'
import type { TaskView } from './api'
import { dueLabel, dueState } from './dates'

const props = defineProps<{
  task: TaskView
  /** Shown under the title in views that mix lists ("Moje", "Na dziś"). */
  listName?: string | null
  assigneeName?: string | null
  canToggle: boolean
  draggable?: boolean
  large?: boolean
}>()
const emit = defineEmits<{ toggle: []; open: [] }>()

const host = useHost()
const done = computed(() => props.task.status === 'Done')
const due = computed(() => {
  const { dueDate, dueTime } = props.task
  if (!dueDate) return null
  const words = { today: host.t('due.today'), tomorrow: host.t('due.tomorrow'), yesterday: host.t('due.yesterday') }
  return { label: dueLabel(dueDate, dueTime, words), state: done.value ? 'later' : dueState(dueDate) }
})
const severity = { overdue: 'danger', today: 'warn', soon: 'info', later: 'secondary' } as const
</script>

<template>
  <li
    class="todo-row"
    :class="{ 'todo-row--done': done, 'todo-row--large': large }"
    :draggable="draggable"
    :data-task-id="task.id"
  >
    <i v-if="draggable" class="pi pi-bars todo-row__handle" aria-hidden="true" />
    <Checkbox
      :model-value="done"
      binary
      :disabled="!canToggle"
      :input-id="`todo-${task.id}`"
      :aria-label="done ? host.t('reopen') : host.t('complete')"
      @update:model-value="emit('toggle')"
    />
    <button type="button" class="todo-row__body" @click="emit('open')">
      <span class="todo-row__title">
        <i v-if="task.priority === 'High'" class="pi pi-flag-fill todo-row__high" :title="host.t('priority.High')" />
        {{ task.title }}
        <i v-if="task.notes" class="pi pi-align-left todo-row__notes" :title="host.t('notes')" />
      </span>
      <span v-if="due || assigneeName || listName" class="todo-row__meta">
        <Tag v-if="due" :value="due.label" :severity="severity[due.state]" icon="pi pi-calendar" />
        <span v-if="assigneeName"><i class="pi pi-user" aria-hidden="true" /> {{ assigneeName }}</span>
        <span v-if="listName"><i class="pi pi-list" aria-hidden="true" /> {{ listName }}</span>
      </span>
    </button>
  </li>
</template>

<style scoped>
.todo-row {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  padding: 0.5rem 0.25rem;
  border-bottom: 1px solid var(--p-content-border-color);
}
.todo-row[draggable='true'] {
  cursor: grab;
}
.todo-row__handle {
  color: var(--p-text-muted-color);
  font-size: 0.75rem;
}
.todo-row__body {
  flex: 1;
  min-width: 0;
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
  text-align: left;
  background: none;
  border: 0;
  padding: 0;
  color: inherit;
  font: inherit;
  cursor: pointer;
}
.todo-row__title {
  overflow-wrap: anywhere;
}
.todo-row__high {
  color: var(--p-red-500);
  font-size: 0.8rem;
}
.todo-row__notes {
  color: var(--p-text-muted-color);
  font-size: 0.75rem;
  margin-left: 0.25rem;
}
.todo-row__meta {
  display: flex;
  flex-wrap: wrap;
  align-items: center;
  gap: 0.75rem;
  font-size: 0.8rem;
  color: var(--p-text-muted-color);
}
.todo-row--done .todo-row__title {
  text-decoration: line-through;
  color: var(--p-text-muted-color);
}
.todo-row--large {
  gap: 1rem;
  padding: 0.75rem 0;
  font-size: 1.5rem;
}
.todo-row--large .todo-row__meta {
  font-size: 1rem;
}
</style>
