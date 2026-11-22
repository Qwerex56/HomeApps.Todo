<script setup lang="ts">
import { computed } from 'vue'
import { useHost } from '@homeapps/plugin-sdk'
import type { CalendarItem } from './api'
import { itemsOn } from './calendarDates'
import { isoDate, todayIso } from './dates'
import CalendarChip from './CalendarChip.vue'

// Month, Week and Day views: one grid of day cells (7 columns for month and week, one for day). Arrow keys move
// between cells, Enter on a cell adds an event on that day, Enter or Space on an entry opens it.
const props = defineProps<{
  days: Date[]
  items: CalendarItem[]
  /** Month shown, to dim the days of the neighbouring months; null for week and day. */
  month: number | null
  canWrite: boolean
  canToggle: (item: CalendarItem) => boolean
  columns: number
}>()
const emit = defineEmits<{ open: [item: CalendarItem]; toggle: [item: CalendarItem]; create: [day: string]; drop: [id: string, day: string] }>()

const host = useHost()
const today = todayIso()
const MAX = 3
const cells = computed(() =>
  props.days.map((date) => {
    const iso = isoDate(date)
    const all = itemsOn(props.items, iso)
    const limit = props.columns === 1 || props.month === null ? all.length : MAX
    return {
      iso,
      date,
      shown: all.slice(0, limit),
      rest: all.length - limit,
      dim: props.month !== null && date.getMonth() !== props.month,
      label: date.toLocaleDateString('pl-PL', { weekday: 'long', day: 'numeric', month: 'long' }),
    }
  }),
)
const weekdays = computed(() => props.days.slice(0, Math.min(7, props.days.length)).map((d) => d.toLocaleDateString('pl-PL', { weekday: 'short' })))

function move(event: KeyboardEvent, index: number) {
  const step = { ArrowLeft: -1, ArrowRight: 1, ArrowUp: -props.columns, ArrowDown: props.columns }[event.key]
  if (step === undefined) return
  event.preventDefault()
  const next = (event.currentTarget as HTMLElement).parentElement?.children[index + step] as HTMLElement | undefined
  next?.focus()
}

function onDrop(event: DragEvent, day: string) {
  event.preventDefault()
  const id = event.dataTransfer?.getData('text/plain')
  if (id && props.canWrite) emit('drop', id, day)
}
</script>

<template>
  <div class="grid" role="grid" :aria-label="host.t('calendar.grid')">
    <div v-if="columns > 1" class="grid__head" role="row">
      <span v-for="w in weekdays" :key="w" role="columnheader">{{ w }}</span>
    </div>
    <div class="grid__body" :style="{ gridTemplateColumns: `repeat(${columns}, minmax(0, 1fr))` }">
      <div
        v-for="(cell, index) in cells"
        :key="cell.iso"
        class="cell"
        :class="{ 'cell--dim': cell.dim, 'cell--today': cell.iso === today }"
        role="gridcell"
        :tabindex="index === 0 ? 0 : -1"
        :aria-label="cell.label"
        :aria-current="cell.iso === today ? 'date' : undefined"
        @keydown="move($event, index)"
        @keydown.enter.self="canWrite && emit('create', cell.iso)"
        @dragover.prevent
        @drop="onDrop($event, cell.iso)"
      >
        <span class="cell__day">{{ cell.date.getDate() }}</span>
        <CalendarChip
          v-for="item in cell.shown"
          :key="`${item.itemType}-${item.id}-${item.occurrenceDate ?? ''}`"
          :item="item"
          :can-toggle="canToggle(item)"
          :draggable="canWrite && item.itemType === 'Task'"
          @open="emit('open', item)"
          @toggle="emit('toggle', item)"
        />
        <span v-if="cell.rest > 0" class="cell__more">{{ host.t('more', { count: cell.rest }) }}</span>
      </div>
    </div>
  </div>
</template>

<style scoped>
.grid__head {
  display: grid;
  grid-template-columns: repeat(7, minmax(0, 1fr));
  text-align: center;
  color: var(--p-text-muted-color);
  font-size: 0.85rem;
  padding-bottom: 0.25rem;
}
.grid__body {
  display: grid;
  gap: 1px;
  background: var(--p-content-border-color);
  border: 1px solid var(--p-content-border-color);
}
.cell {
  display: flex;
  flex-direction: column;
  gap: 2px;
  min-height: 6rem;
  padding: 0.25rem;
  background: var(--p-content-background);
}
.cell:focus-visible {
  outline: 2px solid var(--p-primary-color);
  outline-offset: -2px;
}
.cell--dim {
  background: var(--p-content-hover-background);
  color: var(--p-text-muted-color);
}
.cell--today .cell__day {
  background: var(--p-primary-color);
  color: var(--p-primary-contrast-color);
  border-radius: 999px;
}
.cell__day {
  align-self: flex-start;
  min-width: 1.5rem;
  text-align: center;
  font-size: 0.85rem;
}
.cell__more {
  font-size: 0.8rem;
  color: var(--p-text-muted-color);
}
</style>
