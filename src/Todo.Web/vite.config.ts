import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import { homeAppsPlugin } from '@homeapps/plugin-sdk/vite'

// The plugin's UI as one ES module the HomeApps web app imports (docs/plugins.md in HomeApps, "UI"). Vue, the
// router, i18n, Pinia and vue-query come from the app; the build lands where Todo.Api serves it.
export default defineConfig({
  plugins: [vue(), homeAppsPlugin({ outDir: '../Todo.Api/wwwroot/ui' })],
})
