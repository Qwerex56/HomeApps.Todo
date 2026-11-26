<script setup lang="ts">
import { computed, ref, watch } from 'vue'
import { useQuery } from '@tanstack/vue-query'
import { useHost } from '@home-apps/plugin-sdk'
import type { CreatedSubscription, PlannerSettings, ExternalVisibility, SubscriptionTasksMode } from './api'
import { useTodo } from './useTodo'

// Calendar settings: the caller's own preferences (default reminders, first day of the week, time zone), ICS
// subscriptions (feed addresses to give other apps) and imports of external calendars.
const props = defineProps<{ visible: boolean; householdId: string }>()
const emit = defineEmits<{ 'update:visible': [boolean] }>()

const host = useHost()
const todo = useTodo(() => props.householdId)
const { api, change } = todo
const open = computed(() => props.visible)

const subscriptions = useQuery({
  queryKey: computed(() => ['plugins', 'todo', props.householdId, 'subscriptions'] as const),
  queryFn: () => api.value.subscriptions(),
  enabled: open,
})
const externals = useQuery({
  queryKey: computed(() => ['plugins', 'todo', props.householdId, 'external'] as const),
  queryFn: () => api.value.externals(),
  enabled: computed(() => open.value && todo.canWrite.value),
})

// Own preferences. The server keeps instants in UTC; the zone only decides how the UI shows them.
const REMINDERS = [0, 10, 30, 60, 1440]
const prefs = ref<PlannerSettings>({ eventReminders: [], taskReminders: [], firstDayOfWeek: 1, timeZone: null })
watch(
  () => todo.settings.data.value,
  (value) => value && (prefs.value = { ...value, timeZone: value.timeZone ?? todo.timeZone.value }),
  { immediate: true },
)
const savePrefs = change((a, input: PlannerSettings) => a.savePlannerSettings(input))
const reminderOptions = REMINDERS.map((value) => ({ value, label: value === 0 ? host.t('calendar.reminder.atStart') : host.t('calendar.reminder.before', { minutes: value }) }))
const zones = computed(() => {
  const all = (Intl as { supportedValuesOf?: (key: string) => string[] }).supportedValuesOf?.('timeZone') ?? []
  return prefs.value.timeZone && !all.includes(prefs.value.timeZone) ? [prefs.value.timeZone, ...all] : all
})
const weekDays = [1, 0, 6].map((value) => ({ value, label: host.t(`calendar.settings.weekDay.${value}`) }))

// Subscriptions.
const label = ref('')
const tasksMode = ref<SubscriptionTasksMode>('None')
const includePrivate = ref(false)
const created = ref<CreatedSubscription | null>(null)
const createSubscription = change((a, input: { label: string; includeHouseholdEvents: boolean; includePrivateEvents: boolean; tasksMode: SubscriptionTasksMode }) => a.createSubscription(input))
const revokeSubscription = change((a, id: string) => a.revokeSubscription(id))
const modeOptions = (['None', 'Assigned', 'All'] as const).map((value) => ({ value, label: host.t(`calendar.settings.tasksMode.${value}`) }))
const feedUrl = computed(() => (created.value ? `${window.location.origin}${created.value.feedPath}` : ''))

function addSubscription() {
  if (!label.value.trim()) return
  createSubscription.mutate(
    { label: label.value.trim(), includeHouseholdEvents: true, includePrivateEvents: includePrivate.value, tasksMode: tasksMode.value },
    {
      onSuccess: (result) => {
        created.value = result as CreatedSubscription
        label.value = ''
      },
    },
  )
}
async function copy() {
  await navigator.clipboard?.writeText(feedUrl.value)
  host.notify('success', host.t('calendar.settings.copied'))
}

// Revoking and removing ask first.
const asking = ref<{ kind: 'subscription' | 'external'; id: string; name: string } | null>(null)
const askOpen = computed({ get: () => asking.value !== null, set: (v) => !v && (asking.value = null) })
function confirmAsking() {
  const a = asking.value
  asking.value = null
  if (!a) return
  if (a.kind === 'subscription') revokeSubscription.mutate(a.id)
  else removeExternal.mutate(a.id)
}

// External calendars.
const url = ref('')
const name = ref('')
const color = ref('#3b82f6')
const visibility = ref<ExternalVisibility>('Busy')
const addExternal = change((a, input: { url: string; name: string; color: string; visibility: ExternalVisibility }) => a.addExternal(input))
const syncExternal = change((a, id: string) => a.syncExternal(id))
const removeExternal = change((a, id: string) => a.removeExternal(id))
const visibilityOptions = (['OnlyMe', 'Busy', 'Details'] as const).map((value) => ({ value, label: host.t(`calendar.settings.visibility.${value}`) }))

function submitExternal() {
  if (!url.value.trim() || !name.value.trim()) return
  addExternal.mutate(
    { url: url.value.trim(), name: name.value.trim(), color: color.value, visibility: visibility.value },
    { onSuccess: () => ((url.value = ''), (name.value = '')) },
  )
}
const ago = (iso: string | null) => (iso ? new Date(iso).toLocaleString('pl-PL') : host.t('calendar.settings.never'))
</script>

<template>
  <Dialog
    :visible="visible"
    modal
    :header="host.t('calendar.settings.title')"
    :style="{ width: '40rem', maxWidth: 'calc(100vw - 2rem)' }"
    @update:visible="emit('update:visible', $event)"
  >
    <section class="settings" :aria-label="host.t('calendar.settings.preferences')">
      <h3>{{ host.t('calendar.settings.preferences') }}</h3>
      <form class="settings__form" @submit.prevent="savePrefs.mutate(prefs)">
        <label class="settings__field">
          <span>{{ host.t('calendar.settings.eventReminders') }}</span>
          <MultiSelect v-model="prefs.eventReminders" :options="reminderOptions" option-label="label" option-value="value" display="chip" />
        </label>
        <label class="settings__field">
          <span>{{ host.t('calendar.settings.taskReminders') }}</span>
          <MultiSelect v-model="prefs.taskReminders" :options="reminderOptions" option-label="label" option-value="value" display="chip" />
        </label>
        <label class="settings__field">
          <span>{{ host.t('calendar.settings.timeZone') }}</span>
          <Select v-model="prefs.timeZone" :options="zones" filter />
        </label>
        <label class="settings__field">
          <span>{{ host.t('calendar.settings.firstDay') }}</span>
          <Select v-model="prefs.firstDayOfWeek" :options="weekDays" option-label="label" option-value="value" />
        </label>
        <Button type="submit" :label="host.t('save')" :loading="savePrefs.isPending.value" />
      </form>
    </section>

    <section class="settings" :aria-label="host.t('calendar.settings.subscriptions')">
      <h3>{{ host.t('calendar.settings.subscriptions') }}</h3>
      <p class="settings__hint">{{ host.t('calendar.settings.subscriptionsHint') }}</p>
      <Skeleton v-if="subscriptions.isPending.value" height="2rem" />
      <Message v-else-if="subscriptions.isError.value" severity="error">{{ host.errorMessage(subscriptions.error.value) }}</Message>
      <ul v-else class="settings__list">
        <li v-for="s in subscriptions.data.value" :key="s.id">
          <span>{{ s.label }} <small>({{ host.t('calendar.settings.lastUsed') }}: {{ ago(s.lastAccessedAt) }})</small></span>
          <Button icon="pi pi-trash" text severity="danger" :aria-label="host.t('calendar.settings.revoke', { name: s.label })" @click="asking = { kind: 'subscription', id: s.id, name: s.label }" />
        </li>
        <li v-if="!subscriptions.data.value?.length" class="settings__hint">{{ host.t('calendar.settings.noSubscriptions') }}</li>
      </ul>
      <Message v-if="created" severity="success" :closable="false">
        <div role="status">
          <p>{{ host.t('calendar.settings.feedOnce') }}</p>
          <InputText :model-value="feedUrl" readonly class="settings__wide" :aria-label="host.t('calendar.settings.feedUrl')" />
          <Button :label="host.t('calendar.settings.copy')" icon="pi pi-copy" text @click="copy" />
        </div>
      </Message>
      <form class="settings__form" @submit.prevent="addSubscription">
        <InputText v-model="label" maxlength="100" required :placeholder="host.t('calendar.settings.label')" :aria-label="host.t('calendar.settings.label')" />
        <Select v-model="tasksMode" :options="modeOptions" option-label="label" option-value="value" :aria-label="host.t('calendar.settings.tasksModeLabel')" />
        <label class="settings__check"><input v-model="includePrivate" type="checkbox" /> {{ host.t('calendar.settings.includePrivate') }}</label>
        <Button type="submit" :label="host.t('calendar.settings.addSubscription')" :loading="createSubscription.isPending.value" />
      </form>
    </section>

    <section v-if="todo.canWrite.value" class="settings" :aria-label="host.t('calendar.settings.external')">
      <h3>{{ host.t('calendar.settings.external') }}</h3>
      <Skeleton v-if="externals.isPending.value" height="2rem" />
      <Message v-else-if="externals.isError.value" severity="error">{{ host.errorMessage(externals.error.value) }}</Message>
      <ul v-else class="settings__list">
        <li v-for="c in externals.data.value" :key="c.id">
          <span>
            <i class="settings__dot" :style="{ background: c.color }" aria-hidden="true" /> {{ c.name }}
            <small>{{ host.t(`calendar.settings.visibility.${c.visibility}`) }}</small>
            <Message v-if="c.isStale" severity="warn" :closable="false" role="alert">
              {{ host.t('calendar.settings.stale', { when: ago(c.lastSyncedAt) }) }}
              <template v-if="c.lastErrorMessage">: {{ c.lastErrorMessage }}</template>
            </Message>
          </span>
          <span>
            <Button icon="pi pi-refresh" text :loading="syncExternal.isPending.value" :aria-label="host.t('calendar.settings.sync', { name: c.name })" @click="syncExternal.mutate(c.id)" />
            <Button icon="pi pi-trash" text severity="danger" :aria-label="host.t('calendar.settings.remove', { name: c.name })" @click="asking = { kind: 'external', id: c.id, name: c.name }" />
          </span>
        </li>
        <li v-if="!externals.data.value?.length" class="settings__hint">{{ host.t('calendar.settings.noExternal') }}</li>
      </ul>
      <form class="settings__form" @submit.prevent="submitExternal">
        <InputText v-model="url" type="url" required :placeholder="host.t('calendar.settings.url')" :aria-label="host.t('calendar.settings.url')" />
        <InputText v-model="name" maxlength="100" required :placeholder="host.t('calendar.settings.name')" :aria-label="host.t('calendar.settings.name')" />
        <input v-model="color" type="color" :aria-label="host.t('calendar.settings.color')" class="settings__color" />
        <Select v-model="visibility" :options="visibilityOptions" option-label="label" option-value="value" :aria-label="host.t('calendar.settings.visibilityLabel')" />
        <Button type="submit" :label="host.t('calendar.settings.addExternal')" :loading="addExternal.isPending.value" />
      </form>
    </section>
    <Dialog v-model:visible="askOpen" modal :header="host.t('calendar.settings.confirmTitle')" :style="{ width: '24rem', maxWidth: 'calc(100vw - 2rem)' }">
      <p>{{ host.t('calendar.settings.confirmRemove', { name: asking?.name ?? '' }) }}</p>
      <template #footer>
        <Button severity="secondary" text :label="host.t('cancel')" @click="asking = null" />
        <Button severity="danger" :label="host.t('delete')" @click="confirmAsking" />
      </template>
    </Dialog>
  </Dialog>
</template>

<style scoped>
.settings h3 {
  margin: 0.5rem 0;
}
.settings__hint {
  color: var(--p-text-muted-color);
  margin: 0 0 0.5rem;
}
.settings__list {
  list-style: none;
  padding: 0;
  margin: 0 0 0.75rem;
}
.settings__list li {
  display: flex;
  justify-content: space-between;
  align-items: center;
  gap: 0.5rem;
  min-height: 44px;
}
.settings__form {
  display: flex;
  flex-wrap: wrap;
  gap: 0.5rem;
  align-items: center;
  margin-bottom: 1rem;
}
.settings__field {
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
  flex: 1 1 14rem;
}
.settings__field > span {
  font-size: 0.85rem;
  color: var(--p-text-muted-color);
}
.settings__wide {
  width: 100%;
}
.settings__check {
  display: flex;
  gap: 0.25rem;
  align-items: center;
  min-height: 44px;
}
.settings__color {
  width: 44px;
  height: 44px;
}
.settings__dot {
  display: inline-block;
  width: 0.75rem;
  height: 0.75rem;
  border-radius: 999px;
}
</style>
