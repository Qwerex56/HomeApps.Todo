<script setup lang="ts">
import { computed } from 'vue'
import type { WidgetProps } from '@homeapps/plugin-sdk'
import type { CalendarItem } from './api'
import { addDays, itemsOn, startDay } from './calendarDates'
import { isoDate } from './dates'
import { useTodo } from './useTodo'
import CalendarChip from './CalendarChip.vue'

// Wall and household tile: what is on today and tomorrow (events, tasks due, imported calendars). Touch ticks tasks.
const props = defineProps<WidgetProps>()
const todo = useTodo(() => props.householdId)
const { host, canWrite, change } = todo

const start = new Date()
const range = computed(() => ({
  from: new Date(start.getFullYear(), start.getMonth(), start.getDate()).toISOString(),
  to: addDays(new Date(start.getFullYear(), start.getMonth(), start.getDate()), 2).toISOString(),
}))
const calendar = todo.calendar(range, ['events', 'tasks', 'external'])
const large = computed(() => props.place === 'Wall')
const MAX = 8

const days = computed(() =>
  [0, 1].map((offset) => {
    const iso = isoDate(addDays(start, offset))
    return { iso, label: host.t(offset === 0 ? 'due.today' : 'due.tomorrow'), items: itemsOn(calendar.data.value ?? [], iso).slice(0, MAX) }
  }),
)
const empty = computed(() => days.value.every((d) => d.items.length === 0))
const canToggle = (item: CalendarItem) => canWrite.value && item.itemType === 'Task'
const toggle = change((a, item: CalendarItem) => (item.isDone ? a.reopenTask(item.id) : a.completeTask(item.id)))
const key = (item: CalendarItem) => `${item.itemType}-${item.id}-${item.occurrenceDate ?? startDay(item)}`
</script>

<template>
  <div class="agenda-widget" :class="{ 'agenda-widget--large': large }">
    <div v-if="calendar.isPending.value" class="agenda-widget__list" aria-busy="true">
      <Skeleton v-for="n in 3" :key="n" height="2rem" />
    </div>
    <p v-else-if="calendar.isError.value" class="agenda-widget__muted" role="alert">{{ host.errorMessage(calendar.error.value) }}</p>
    <p v-else-if="empty" class="agenda-widget__muted">{{ host.t('calendar.widgetEmpty') }}</p>
    <template v-else>
      <section v-for="day in days" :key="day.iso" :aria-label="day.label">
        <h4 v-if="day.items.length" class="agenda-widget__day">{{ day.label }}</h4>
        <div class="agenda-widget__list">
          <CalendarChip v-for="item in day.items" :key="key(item)" :item="item" :can-toggle="canToggle(item)" @open="canToggle(item) && toggle.mutate(item)" @toggle="toggle.mutate(item)" />
        </div>
      </section>
    </template>
  </div>
</template>

<style scoped>
.agenda-widget__list {
  display: flex;
  flex-direction: column;
  gap: 4px;
}
.agenda-widget__day {
  margin: 0.5rem 0 0.25rem;
  text-transform: capitalize;
}
.agenda-widget__muted {
  color: var(--p-text-muted-color);
}
.agenda-widget--large {
  font-size: 1.25rem;
}
</style>
