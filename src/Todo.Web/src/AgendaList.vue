<script setup lang="ts">
import { computed } from 'vue'
import { useHost } from '@home-apps/plugin-sdk'
import type { CalendarItem } from './api'
import { itemsOn } from './calendarDates'
import { isoDate } from './dates'
import CalendarChip from './CalendarChip.vue'

// Agenda: a vertical list of the days that have something in them; the default view on a phone.
const props = defineProps<{ days: Date[]; items: CalendarItem[]; canToggle: (item: CalendarItem) => boolean }>()
const emit = defineEmits<{ open: [item: CalendarItem]; toggle: [item: CalendarItem] }>()

const host = useHost()
const groups = computed(() =>
  props.days
    .map((date) => ({
      iso: isoDate(date),
      label: date.toLocaleDateString('pl-PL', { weekday: 'long', day: 'numeric', month: 'long' }),
      items: itemsOn(props.items, isoDate(date)),
    }))
    .filter((g) => g.items.length > 0),
)
</script>

<template>
  <div class="agenda">
    <p v-if="groups.length === 0" class="agenda__empty">{{ host.t('calendar.agendaEmpty') }}</p>
    <section v-for="group in groups" :key="group.iso" class="agenda__day">
      <h3 class="agenda__heading">{{ group.label }}</h3>
      <CalendarChip
        v-for="item in group.items"
        :key="`${item.itemType}-${item.id}-${item.occurrenceDate ?? ''}`"
        :item="item"
        :can-toggle="canToggle(item)"
        @open="emit('open', item)"
        @toggle="emit('toggle', item)"
      />
    </section>
  </div>
</template>

<style scoped>
.agenda {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}
.agenda__day {
  display: flex;
  flex-direction: column;
  gap: 4px;
}
.agenda__heading {
  margin: 0;
  font-size: 1rem;
  text-transform: capitalize;
}
.agenda__empty {
  color: var(--p-text-muted-color);
}
</style>
