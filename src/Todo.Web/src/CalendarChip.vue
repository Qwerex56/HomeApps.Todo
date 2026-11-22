<script setup lang="ts">
import { computed } from 'vue'
import { useHost } from '@homeapps/plugin-sdk'
import type { CalendarItem } from './api'
import { timeLabel } from './calendarDates'

// One entry in a day cell or the agenda: an event, a task with its checkbox, or an imported (read-only) event.
const props = defineProps<{ item: CalendarItem; canToggle: boolean; draggable?: boolean }>()
const emit = defineEmits<{ open: []; toggle: [] }>()

const host = useHost()
const time = computed(() => timeLabel(props.item))
const isTask = computed(() => props.item.itemType === 'Task')
const done = computed(() => props.item.isDone === true)
const label = computed(() => `${host.t(`calendar.type.${props.item.itemType}`)}: ${props.item.title}${time.value ? `, ${time.value}` : ''}`)
const style = computed(() => (props.item.color ? { '--chip-color': props.item.color } : undefined))

function onDragStart(event: DragEvent) {
  event.dataTransfer?.setData('text/plain', props.item.id)
  if (event.dataTransfer) event.dataTransfer.effectAllowed = 'move'
}
</script>

<template>
  <div
    class="chip"
    :class="[`chip--${item.itemType.toLowerCase()}`, { 'chip--done': done, 'chip--private': item.isPrivate }]"
    :style="style"
    :draggable="draggable"
    @dragstart="onDragStart"
  >
    <input
      v-if="isTask"
      type="checkbox"
      class="chip__check"
      :checked="done"
      :disabled="!canToggle"
      :aria-label="done ? host.t('reopen') : host.t('complete')"
      @click.stop
      @change="emit('toggle')"
    />
    <button type="button" class="chip__body" :aria-label="label" data-chip @click.stop="emit('open')" @keydown.stop>
      <span v-if="time" class="chip__time">{{ time }}</span>
      <i v-if="item.isPrivate" class="pi pi-lock" aria-hidden="true" />
      <span class="chip__title">{{ item.title }}</span>
    </button>
  </div>
</template>

<style scoped>
.chip {
  display: flex;
  align-items: center;
  gap: 0.25rem;
  min-height: 1.75rem;
  border-left: 4px solid var(--chip-color, var(--p-primary-color));
  background: color-mix(in srgb, var(--chip-color, var(--p-primary-color)) 14%, var(--p-content-background));
  border-radius: 4px;
  padding: 0 0.25rem;
}
.chip--task {
  --chip-color: var(--p-orange-600, #ea580c);
}
.chip--external {
  --chip-color: #475569;
}
.chip--done .chip__title {
  text-decoration: line-through;
  color: var(--p-text-muted-color);
}
.chip__check {
  width: 1.1rem;
  height: 1.1rem;
  flex: none;
}
.chip__body {
  all: unset;
  box-sizing: border-box;
  flex: 1;
  min-width: 0;
  min-height: 1.75rem;
  display: flex;
  align-items: center;
  gap: 0.25rem;
  cursor: pointer;
  font-size: 0.85rem;
}
.chip__body:focus-visible,
.chip__check:focus-visible {
  outline: 2px solid var(--p-primary-color);
  outline-offset: 1px;
}
.chip__time {
  font-variant-numeric: tabular-nums;
  color: var(--p-text-muted-color);
}
.chip__title {
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}
@media (pointer: coarse) {
  .chip,
  .chip__body {
    min-height: 44px;
  }
  .chip__check {
    width: 28px;
    height: 28px;
  }
}
</style>
