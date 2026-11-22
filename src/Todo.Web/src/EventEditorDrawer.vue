<script setup lang="ts">
import { computed, ref, watch } from 'vue'
import { useHost } from '@homeapps/plugin-sdk'
import type { CalendarEventDetail, EventInput, HouseholdMember } from './api'
import { localDay, localTime, toInstant } from './calendarDates'

// Create or edit one event in a side drawer (full screen on a phone). A recurring series is edited per occurrence;
// its rule can only be set when the event is created (the API takes attendees and reminders on single events only).
const props = defineProps<{
  visible: boolean
  /** null = a new event on `day`. */
  event: CalendarEventDetail | null
  day: string
  /** The user's IANA zone (planner settings) for a new event. */
  timeZone: string
  members: HouseholdMember[]
  canEdit: boolean
  canDelete: boolean
  saving: boolean
}>()
const emit = defineEmits<{ 'update:visible': [boolean]; save: [input: EventInput]; delete: [] }>()

const host = useHost()
const REMINDERS = [0, 10, 30, 60, 1440]
const RULES = ['', 'FREQ=DAILY', 'FREQ=WEEKLY', 'FREQ=MONTHLY', 'FREQ=YEARLY']

interface Form {
  title: string
  description: string
  location: string
  startDate: string
  startTime: string
  endDate: string
  endTime: string
  isAllDay: boolean
  isPrivate: boolean
  attendees: string[]
  reminderMinutes: number[]
  rrule: string
}

function blank(day: string): Form {
  return { title: '', description: '', location: '', startDate: day, startTime: '09:00', endDate: day, endTime: '10:00', isAllDay: false, isPrivate: false, attendees: [], reminderMinutes: [], rrule: '' }
}
function formOf(e: CalendarEventDetail): Form {
  return {
    title: e.title,
    description: e.description ?? '',
    location: e.location ?? '',
    startDate: localDay(e.startUtc),
    startTime: localTime(e.startUtc),
    endDate: localDay(e.endUtc),
    endTime: localTime(e.endUtc),
    isAllDay: e.isAllDay,
    isPrivate: e.isPrivate,
    attendees: [...e.attendees],
    reminderMinutes: [...e.reminderMinutes],
    rrule: e.rrule ?? '',
  }
}

const form = ref<Form>(blank(props.day))
const submitted = ref(false)
watch(
  () => [props.visible, props.event, props.day] as const,
  ([open]) => {
    if (!open) return
    form.value = props.event ? formOf(props.event) : blank(props.day)
    submitted.value = false
  },
  { immediate: true },
)

const isSeries = computed(() => !!props.event?.seriesId)
const recurring = computed(() => isSeries.value || form.value.rrule !== '')
const readOnly = computed(() => !props.canEdit)
const startsIso = computed(() => toInstant(form.value.startDate, form.value.isAllDay ? '00:00' : form.value.startTime))
const endsIso = computed(() => toInstant(form.value.endDate, form.value.isAllDay ? '23:59' : form.value.endTime))
const titleError = computed(() => (submitted.value && !form.value.title.trim() ? host.t('calendar.errors.title') : null))
const rangeError = computed(() => (submitted.value && endsIso.value < startsIso.value ? host.t('calendar.errors.range') : null))
const ruleOptions = RULES.map((value) => ({ value, label: host.t(`calendar.repeat.${value || 'none'}`) }))

function submit() {
  submitted.value = true
  if (!form.value.title.trim() || endsIso.value < startsIso.value) return
  const f = form.value
  emit('save', {
    title: f.title.trim(),
    description: f.description.trim() || null,
    location: f.location.trim() || null,
    timeZoneId: props.event?.timeZoneId ?? props.timeZone,
    startUtc: startsIso.value,
    endUtc: endsIso.value,
    isAllDay: f.isAllDay,
    isPrivate: f.isPrivate,
    attendees: recurring.value ? [] : f.attendees,
    reminderMinutes: recurring.value ? [] : f.reminderMinutes,
    rrule: isSeries.value ? null : f.rrule || null,
  })
}

function reminderLabel(minutes: number) {
  return minutes === 0 ? host.t('calendar.reminder.atStart') : host.t('calendar.reminder.before', { minutes })
}
</script>

<template>
  <Drawer
    :visible="visible"
    position="right"
    :header="host.t(event ? 'calendar.editEvent' : 'calendar.newEvent')"
    class="editor-drawer"
    @update:visible="emit('update:visible', $event)"
  >
    <form class="editor" novalidate @submit.prevent="submit">
      <label class="editor__field">
        <span>{{ host.t('fields.title') }}</span>
        <InputText v-model="form.title" maxlength="200" required :disabled="readOnly" :invalid="!!titleError" :aria-describedby="titleError ? 'ev-title-err' : undefined" />
        <small v-if="titleError" id="ev-title-err" class="editor__error" role="alert">{{ titleError }}</small>
      </label>
      <label class="editor__check">
        <input v-model="form.isAllDay" type="checkbox" :disabled="readOnly" />
        <span>{{ host.t('calendar.allDay') }}</span>
      </label>
      <div class="editor__row">
        <label class="editor__field">
          <span>{{ host.t('calendar.start') }}</span>
          <input v-model="form.startDate" type="date" class="editor__input" required :disabled="readOnly" />
        </label>
        <label v-if="!form.isAllDay" class="editor__field">
          <span class="sr-only">{{ host.t('fields.time') }}</span>
          <input v-model="form.startTime" type="time" class="editor__input" required :disabled="readOnly" />
        </label>
      </div>
      <div class="editor__row">
        <label class="editor__field">
          <span>{{ host.t('calendar.end') }}</span>
          <input v-model="form.endDate" type="date" class="editor__input" required :disabled="readOnly" />
        </label>
        <label v-if="!form.isAllDay" class="editor__field">
          <span class="sr-only">{{ host.t('fields.time') }}</span>
          <input v-model="form.endTime" type="time" class="editor__input" required :disabled="readOnly" />
        </label>
      </div>
      <small v-if="rangeError" class="editor__error" role="alert">{{ rangeError }}</small>
      <label class="editor__field">
        <span>{{ host.t('calendar.location') }}</span>
        <InputText v-model="form.location" maxlength="500" :disabled="readOnly" />
      </label>
      <label class="editor__field">
        <span>{{ host.t('fields.notes') }}</span>
        <Textarea v-model="form.description" rows="3" auto-resize maxlength="4000" :disabled="readOnly" />
      </label>
      <label class="editor__check">
        <input v-model="form.isPrivate" type="checkbox" :disabled="readOnly" />
        <span>{{ host.t('calendar.private') }}</span>
      </label>

      <label v-if="!isSeries" class="editor__field">
        <span>{{ host.t('calendar.repeat.label') }}</span>
        <Select v-model="form.rrule" :options="ruleOptions" option-label="label" option-value="value" :disabled="readOnly || !!event" />
      </label>
      <p v-else class="editor__hint">{{ host.t('calendar.repeat.series') }}</p>

      <fieldset v-if="!recurring && members.length" class="editor__group">
        <legend>{{ host.t('calendar.attendees') }}</legend>
        <label v-for="m in members" :key="m.userId" class="editor__check">
          <input v-model="form.attendees" type="checkbox" :value="m.userId" :disabled="readOnly" />
          <span>{{ m.displayName }}</span>
        </label>
      </fieldset>
      <fieldset v-if="!recurring" class="editor__group">
        <legend>{{ host.t('calendar.reminder.label') }}</legend>
        <label v-for="r in REMINDERS" :key="r" class="editor__check">
          <input v-model="form.reminderMinutes" type="checkbox" :value="r" :disabled="readOnly" />
          <span>{{ reminderLabel(r) }}</span>
        </label>
      </fieldset>

      <div class="editor__actions">
        <Button v-if="event && canDelete" type="button" :label="host.t('delete')" severity="danger" text class="editor__button" @click="emit('delete')" />
        <span class="editor__spacer" />
        <Button type="button" :label="host.t('cancel')" severity="secondary" text class="editor__button" @click="emit('update:visible', false)" />
        <Button v-if="canEdit" type="submit" :label="host.t(event ? 'save' : 'create')" :loading="saving" class="editor__button" />
      </div>
    </form>
  </Drawer>
</template>

<style scoped>
.editor {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
}
.editor__field {
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
  flex: 1;
}
.editor__row {
  display: flex;
  gap: 0.5rem;
  align-items: flex-end;
}
.editor__input {
  min-height: 44px;
  padding: 0 0.5rem;
  border: 1px solid var(--p-content-border-color);
  border-radius: 6px;
  background: var(--p-content-background);
  color: inherit;
}
.editor__check {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  min-height: 44px;
}
.editor__group {
  border: 1px solid var(--p-content-border-color);
  border-radius: 6px;
}
.editor__error {
  color: var(--p-red-600, #dc2626);
}
.editor__hint {
  margin: 0;
  color: var(--p-text-muted-color);
}
.editor__actions {
  display: flex;
  gap: 0.5rem;
  align-items: center;
}
.editor__spacer {
  flex: 1;
}
.editor__button {
  min-height: 44px;
}
.sr-only {
  position: absolute;
  width: 1px;
  height: 1px;
  overflow: hidden;
  clip: rect(0 0 0 0);
}
</style>

<style>
.editor-drawer {
  width: 26rem;
  max-width: 100vw;
}
@media (max-width: 640px) {
  .editor-drawer {
    width: 100vw;
    height: 100dvh;
  }
}
</style>
