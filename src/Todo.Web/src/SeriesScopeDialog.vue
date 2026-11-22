<script setup lang="ts">
import { useHost } from '@homeapps/plugin-sdk'
import type { SeriesScope } from './api'

// Which occurrences an edit or a delete of a recurring event applies to.
defineProps<{ visible: boolean; mode: 'edit' | 'delete' }>()
const emit = defineEmits<{ 'update:visible': [boolean]; choose: [scope: SeriesScope] }>()

const host = useHost()
const scopes: SeriesScope[] = ['ThisInstance', 'ThisAndFuture', 'All']
</script>

<template>
  <Dialog
    :visible="visible"
    modal
    :header="host.t(mode === 'edit' ? 'calendar.scope.editTitle' : 'calendar.scope.deleteTitle')"
    :style="{ width: '24rem', maxWidth: 'calc(100vw - 2rem)' }"
    @update:visible="emit('update:visible', $event)"
  >
    <div class="scope" role="group" :aria-label="host.t('calendar.scope.group')">
      <Button
        v-for="scope in scopes"
        :key="scope"
        :label="host.t(`calendar.scope.${scope}`)"
        :severity="mode === 'delete' ? 'danger' : 'primary'"
        :outlined="scope !== 'ThisInstance'"
        class="scope__button"
        @click="emit('choose', scope)"
      />
      <Button :label="host.t('cancel')" severity="secondary" text class="scope__button" @click="emit('update:visible', false)" />
    </div>
  </Dialog>
</template>

<style scoped>
.scope {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}
.scope__button {
  min-height: 44px;
}
</style>
