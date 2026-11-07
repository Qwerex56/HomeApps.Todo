<script setup lang="ts">
import { ref, watch } from 'vue'
import { useHost } from '@homeapps/plugin-sdk'
import type { ListVisibility } from './api'

// New list (name + who sees it) or renaming one (name only).
const props = defineProps<{ visible: boolean; mode: 'create' | 'rename'; name?: string; canShare: boolean }>()
const emit = defineEmits<{ 'update:visible': [boolean]; submit: [name: string, visibility: ListVisibility] }>()

const host = useHost()
const name = ref('')
const visibility = ref<ListVisibility>('Private')
watch(
  () => props.visible,
  (open) => {
    if (open) {
      name.value = props.name ?? ''
      visibility.value = props.canShare ? 'Shared' : 'Private'
    }
  },
)
const options = [
  { value: 'Shared', label: host.t('visibility.Shared') },
  { value: 'Private', label: host.t('visibility.Private') },
]
</script>

<template>
  <Dialog
    :visible="visible"
    modal
    :header="mode === 'create' ? host.t('newList') : host.t('renameList')"
    :style="{ width: '24rem', maxWidth: 'calc(100vw - 2rem)' }"
    @update:visible="emit('update:visible', $event)"
  >
    <form class="todo-list-form" @submit.prevent="emit('submit', name, visibility)">
      <InputText v-model="name" :placeholder="host.t('fields.listName')" maxlength="40" required autofocus />
      <template v-if="mode === 'create'">
        <SelectButton
          v-if="canShare"
          v-model="visibility"
          :options="options"
          option-label="label"
          option-value="value"
          :allow-empty="false"
        />
        <small>{{ host.t(`visibilityHint.${visibility}`) }}</small>
      </template>
      <div class="todo-list-form__actions">
        <Button type="button" severity="secondary" text :label="host.t('cancel')" @click="emit('update:visible', false)" />
        <Button type="submit" :label="mode === 'create' ? host.t('create') : host.t('save')" />
      </div>
    </form>
  </Dialog>
</template>

<style scoped>
.todo-list-form {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
}
.todo-list-form small {
  color: var(--p-text-muted-color);
}
.todo-list-form__actions {
  display: flex;
  justify-content: flex-end;
  gap: 0.5rem;
}
</style>
