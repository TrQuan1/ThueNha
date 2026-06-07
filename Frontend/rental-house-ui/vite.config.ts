import { defineConfig } from 'vite'

// https://vite.dev/config/
export default defineConfig({
  server: {
    proxy: {
      '/api': {
        target: 'https://localhost:7023',
        changeOrigin: true,
        secure: false,
      },
    },
  },
})
