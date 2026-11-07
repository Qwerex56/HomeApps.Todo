<script setup lang="ts">
import { computed } from 'vue'
import type { WidgetProps } from '@homeapps/plugin-sdk'
import type { TaskView } from './api'
import { todayIso } from './dates'
import { useTodo } from './useTodo'
import TaskRow from './TaskRow.vue'

// "Na dziś": open tasks due today or earlier from every list the viewer sees; tick off with a touch (wall tablet).
const props = defineProps<WidgetProps>()
const todo = useTodo(() => props.householdId)
const { host, lists, canWrite, memberName, change } = todo

const MAX = 8
const tasks = todo.tasks(() => ({ dueBy: todayIso(), status: 'Open' as const }))
const open = computed(() => (tasks.data.value ?? []).filter((t) => t.status === 'Open'))
const shown = computed(() => open.value.slice(0, MAX))
const listOf = (id: string) => lists.data.value?.find((l) => l.id === id)
const large = computed(() => props.place === 'Wall')

const toggle = change((api, task: TaskView) => api.completeTask(task.id))
const canToggle = (task: TaskView) => {
  const list = listOf(task.listId)
  return canWrite.value && !!list && !list.archived
}
</script>

<template>
  <div class="todo-widget" :class="{ 'todo-widget--large': large }">
    <div v-if="tasks.isPending.value" class="todo-widget__loading">
      <Skeleton v-for="n in 3" :key="n" height="2rem" />
    </div>
    <template v-else>
      <ul v-if="shown.length" class="todo-widget__tasks">
        <TaskRow
          v-for="task in shown"
          :key="task.id"
          :task="task"
          :list-name="listOf(task.listId)?.name ?? null"
          :assignee-name="memberName(task.assigneeId)"
          :can-toggle="canToggle(task)"
          :large="large"
          @toggle="toggle.mutate(task)"
        />
      </ul>
      <p v-else class="todo-widget__empty"><i class="pi pi-check-circle" aria-hidden="true" /> {{ host.t('empty.today') }}</p>
      <p v-if="open.length > MAX" class="todo-widget__more">{{ host.t('more', { count: open.length - MAX }) }}</p>
    </template>
  </div>
</template>

<style scoped>
.todo-widget__tasks {
  list-style: none;
  margin: 0;
  padding: 0;
}
.todo-widget__empty,
.todo-widget__more {
  margin: 0.5rem 0 0;
  color: var(--p-text-muted-color);
}
.todo-widget--large .todo-widget__empty {
  font-size: 1.25rem;
}
.todo-widget__loading {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}
</style>
