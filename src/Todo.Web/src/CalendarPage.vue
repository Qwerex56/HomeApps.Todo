<script setup lang="ts">
import { computed, ref } from 'vue'
import { useQuery } from '@tanstack/vue-query'
import { useHost } from '@homeapps/plugin-sdk'
import {
  inputOf,
  type CalendarEventDetail,
  type CalendarItem,
  type CalendarLayer,
  type EventInput,
  type SeriesScope,
} from './api'
import { shift, visibleDays, windowOf, type CalendarView } from './calendarDates'
import { todayIso } from './dates'
import { useTodo } from './useTodo'
import AgendaList from './AgendaList.vue'
import CalendarSettingsDialog from './CalendarSettingsDialog.vue'
import DayGrid from './DayGrid.vue'
import EventEditorDrawer from './EventEditorDrawer.vue'
import SeriesScopeDialog from './SeriesScopeDialog.vue'

// "Planer": the household's calendar (events, tasks with a due date, imported calendars) in four views.
const host = useHost()
const householdId = computed(() => host.household.value!.id)
const todo = useTodo(householdId)
const { api, userId, canWrite, canManage, members, change } = todo

const views: CalendarView[] = ['month', 'week', 'day', 'agenda']
const view = ref<CalendarView>(window.matchMedia('(max-width: 640px)').matches ? 'agenda' : 'month')
const anchor = ref(new Date())
const layers = ref<CalendarLayer[]>(['events', 'private', 'tasks', 'external'])
const person = ref<string | null>(null)

const days = computed(() => visibleDays(view.value, anchor.value))
const range = computed(() => windowOf(view.value, anchor.value))
const calendar = todo.calendar(range, layers)
const items = computed(() => {
  const all = calendar.data.value ?? []
  const who = person.value
  return who ? all.filter((i) => i.createdBy === who || i.attendees.includes(who)) : all
})

const title = computed(() => {
  const a = anchor.value
  if (view.value === 'month') return a.toLocaleDateString('pl-PL', { month: 'long', year: 'numeric' })
  if (view.value === 'day') return a.toLocaleDateString('pl-PL', { weekday: 'long', day: 'numeric', month: 'long', year: 'numeric' })
  const first = days.value[0]!
  const last = days.value[days.value.length - 1]!
  const fmt = (d: Date) => d.toLocaleDateString('pl-PL', { day: 'numeric', month: 'short' })
  return `${fmt(first)} – ${fmt(last)}`
})
const columns = computed(() => (view.value === 'day' ? 1 : 7))
const viewOptions = computed(() => views.map((value) => ({ value, label: host.t(`calendar.views.${value}`) })))
const layerOptions = computed(() =>
  (['events', 'private', 'tasks', 'external'] as CalendarLayer[]).map((value) => ({ value, label: host.t(`calendar.layers.${value}`) })),
)
const personOptions = computed(() => [
  { userId: null, displayName: host.t('calendar.everyone') },
  ...(members.data.value ?? []).map((m) => ({ userId: m.userId, displayName: m.displayName })),
])

// Stale imported calendars: a warning with the age of the last good sync (3.10).
const externals = useQuery({
  queryKey: computed(() => ['plugins', 'todo', householdId.value, 'external'] as const),
  queryFn: () => api.value.externals(),
  enabled: computed(() => layers.value.includes('external')),
})
const stale = computed(() => (externals.data.value ?? []).filter((c) => c.isStale))
const staleAge = (iso: string | null) => (iso ? new Date(iso).toLocaleString('pl-PL') : host.t('calendar.settings.never'))

function go(direction: 1 | -1) {
  anchor.value = shift(view.value, anchor.value, direction)
}

// Tasks: tick off here, drop onto another day to move the due date.
const canToggle = (item: CalendarItem) => canWrite.value && item.itemType === 'Task'
const toggle = change((a, item: CalendarItem) => (item.isDone ? a.reopenTask(item.id) : a.completeTask(item.id)))
const dropTask = change(async (a, { id, day }: { id: string; day: string }) => {
  const item = items.value.find((i) => i.itemType === 'Task' && i.id === id)
  if (!item?.date || item.date === day) return
  const task = (await a.tasks({ dueBy: item.date, status: 'Open' })).find((t) => t.id === id)
  if (task) await a.updateTask(id, { ...inputOf(task), dueDate: day })
})

// Events: the drawer, and the scope dialog for series.
const editorOpen = ref(false)
const editing = ref<CalendarEventDetail | null>(null)
const draftDay = ref(todayIso())
const pending = ref<{ mode: 'edit' | 'delete'; input?: EventInput } | null>(null)
const canEditEvent = computed(() => canWrite.value && (!editing.value || editing.value.createdBy === userId.value || canManage.value))

function newEvent(day = todayIso()) {
  editing.value = null
  draftDay.value = day
  editorOpen.value = true
}
async function open(item: CalendarItem) {
  if (item.itemType === 'Task') {
    if (canToggle(item)) toggle.mutate(item)
    return
  }
  if (item.itemType === 'External') return
  try {
    const detail = await api.value.event(item.id)
    // An occurrence shows its own date and details, the series supplies the rule and reminders.
    editing.value = item.isOccurrence
      ? { ...detail, occurrenceDate: item.occurrenceDate, title: item.title, description: item.description, location: item.location, startUtc: item.startUtc!, endUtc: item.endUtc! }
      : detail
    editorOpen.value = true
  } catch (error) {
    host.notify('error', host.errorMessage(error))
  }
}

const create = change((a, input: EventInput) => a.createEvent(input))
const update = change((a, { id, input }: { id: string; input: EventInput }) => a.updateEvent(id, input))
const modify = change((a, args: { seriesId: string; date: string; scope: SeriesScope; input: EventInput }) =>
  a.modifyInstance(args.seriesId, args.date, {
    scope: args.scope,
    title: args.input.title,
    description: args.input.description,
    location: args.input.location,
    startUtc: args.input.startUtc,
    endUtc: args.input.endUtc,
  }),
)
const removeEvent = change((a, id: string) => a.deleteEvent(id))
const removeInstance = change((a, args: { seriesId: string; date: string; scope: SeriesScope }) =>
  a.deleteInstance(args.seriesId, args.date, args.scope),
)
const saving = computed(() => create.isPending.value || update.isPending.value || modify.isPending.value)
const close = { onSuccess: () => (editorOpen.value = false) }

function onSave(input: EventInput) {
  const event = editing.value
  if (!event) create.mutate(input, close)
  else if (event.seriesId) pending.value = { mode: 'edit', input }
  else update.mutate({ id: event.id, input }, close)
}
function onDelete() {
  const event = editing.value
  if (!event) return
  if (event.seriesId) pending.value = { mode: 'delete' }
  else if (window.confirm(host.t('calendar.deleteConfirm', { name: event.title }))) removeEvent.mutate(event.id, close)
}
function onScope(scope: SeriesScope) {
  const event = editing.value
  const action = pending.value
  pending.value = null
  if (!event?.seriesId || !event.occurrenceDate || !action) return
  const date = event.occurrenceDate
  if (action.mode === 'delete') removeInstance.mutate({ seriesId: event.seriesId, date, scope }, close)
  else modify.mutate({ seriesId: event.seriesId, date, scope, input: action.input! }, close)
}

const settingsOpen = ref(false)
</script>

<template>
  <div class="planner">
    <header class="planner__bar">
      <div class="planner__nav" role="group" :aria-label="host.t('calendar.navigate')">
        <Button icon="pi pi-chevron-left" text :aria-label="host.t('calendar.previous')" class="planner__touch" @click="go(-1)" />
        <Button :label="host.t('calendar.today')" text class="planner__touch" @click="anchor = new Date()" />
        <Button icon="pi pi-chevron-right" text :aria-label="host.t('calendar.next')" class="planner__touch" @click="go(1)" />
      </div>
      <h2 class="planner__title" aria-live="polite">{{ title }}</h2>
      <SelectButton v-model="view" :options="viewOptions" option-label="label" option-value="value" :allow-empty="false" :aria-label="host.t('calendar.viewSwitch')" />
      <Select v-if="members.data.value?.length" v-model="person" :options="personOptions" option-label="displayName" option-value="userId" :aria-label="host.t('calendar.person')" />
      <Button v-if="canWrite" icon="pi pi-plus" :label="host.t('calendar.newEvent')" class="planner__touch" @click="newEvent()" />
      <Button icon="pi pi-cog" text :aria-label="host.t('calendar.settings.title')" class="planner__touch" @click="settingsOpen = true" />
    </header>

    <fieldset class="planner__layers">
      <legend class="planner__legend">{{ host.t('calendar.layersLabel') }}</legend>
      <label v-for="layer in layerOptions" :key="layer.value" class="planner__layer">
        <input v-model="layers" type="checkbox" :value="layer.value" />
        {{ layer.label }}
      </label>
    </fieldset>

    <Message v-for="c in stale" :key="c.id" severity="warn" :closable="false" role="alert">
      {{ host.t('calendar.settings.stale', { when: staleAge(c.lastSyncedAt) }) }} ({{ c.name }})
    </Message>
    <p v-if="!canWrite" class="planner__hint">{{ host.t('calendar.readOnly') }}</p>

    <div v-if="calendar.isPending.value" class="planner__loading" aria-busy="true">
      <Skeleton v-for="n in view === 'agenda' ? 5 : 6" :key="n" :height="view === 'agenda' ? '3rem' : '6rem'" />
    </div>
    <Message v-else-if="calendar.isError.value" severity="error" role="alert">
      {{ host.errorMessage(calendar.error.value) }}
      <Button :label="host.t('calendar.retry')" text @click="calendar.refetch()" />
    </Message>
    <div v-else-if="items.length === 0 && !person" class="planner__empty">
      <i class="pi pi-calendar" aria-hidden="true" />
      <p>{{ host.t('calendar.empty') }}</p>
      <Button v-if="canWrite" :label="host.t('calendar.addFirst')" @click="newEvent()" />
    </div>
    <AgendaList v-else-if="view === 'agenda'" :days="days" :items="items" :can-toggle="canToggle" @open="open" @toggle="toggle.mutate" />
    <DayGrid
      v-else
      :days="days"
      :items="items"
      :month="view === 'month' ? anchor.getMonth() : null"
      :columns="columns"
      :can-write="canWrite"
      :can-toggle="canToggle"
      @open="open"
      @toggle="toggle.mutate"
      @create="newEvent"
      @drop="(id, day) => dropTask.mutate({ id, day })"
    />

    <EventEditorDrawer
      v-model:visible="editorOpen"
      :event="editing"
      :day="draftDay"
      :members="members.data.value ?? []"
      :can-edit="canEditEvent"
      :can-delete="canEditEvent"
      :saving="saving"
      @save="onSave"
      @delete="onDelete"
    />
    <SeriesScopeDialog :visible="pending !== null" :mode="pending?.mode ?? 'edit'" @update:visible="pending = null" @choose="onScope" />
    <CalendarSettingsDialog v-model:visible="settingsOpen" :household-id="householdId" />
  </div>
</template>

<style scoped>
.planner {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
}
.planner__bar {
  display: flex;
  flex-wrap: wrap;
  align-items: center;
  gap: 0.5rem;
}
.planner__nav {
  display: flex;
}
.planner__title {
  flex: 1;
  margin: 0;
  font-size: 1.25rem;
  text-transform: capitalize;
}
.planner__touch {
  min-height: 44px;
  min-width: 44px;
}
.planner__layers {
  display: flex;
  flex-wrap: wrap;
  gap: 0.25rem 1rem;
  border: 0;
  margin: 0;
  padding: 0;
}
.planner__legend {
  position: absolute;
  width: 1px;
  height: 1px;
  overflow: hidden;
  clip: rect(0 0 0 0);
}
.planner__layer {
  display: flex;
  align-items: center;
  gap: 0.35rem;
  min-height: 44px;
}
.planner__hint {
  margin: 0;
  color: var(--p-text-muted-color);
}
.planner__loading {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}
.planner__empty {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 0.5rem;
  padding: 2rem 0;
  color: var(--p-text-muted-color);
}
.planner__empty .pi {
  font-size: 3rem;
}
</style>
