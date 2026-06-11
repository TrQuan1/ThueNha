<script setup lang="ts">
import { computed } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth.store'

const router = useRouter()
const authStore = useAuthStore()

const userName = computed(() => authStore.user?.fullName ?? 'Tài khoản')

const handleLogout = () => {
  authStore.logout()
  router.push('/')
}
</script>

<template>
  <div class="flex min-h-screen flex-col bg-slate-50 text-slate-900">
    <header class="sticky top-0 z-50 border-b border-slate-200 bg-white/95 backdrop-blur">
      <nav class="mx-auto flex h-16 max-w-7xl items-center justify-between px-4 sm:px-6 lg:px-8">
        <RouterLink to="/" class="flex items-center gap-3 font-bold text-slate-950">
          <span class="flex h-10 w-10 items-center justify-center rounded-xl bg-blue-600 text-white">
            RH
          </span>
          <span class="text-xl">RentalHouse</span>
        </RouterLink>

        <div class="flex items-center gap-3">
          <template v-if="authStore.isAuthenticated">
            <span class="hidden text-sm font-medium text-slate-600 sm:inline">
              {{ userName }}
            </span>
            <button
              type="button"
              class="rounded-xl border border-slate-300 px-4 py-2 text-sm font-semibold text-slate-700 transition-colors hover:border-red-200 hover:bg-red-50 hover:text-red-600"
              @click="handleLogout"
            >
              Đăng xuất
            </button>
          </template>

          <template v-else>
            <RouterLink
              to="/login"
              class="rounded-xl px-4 py-2 text-sm font-semibold text-slate-700 transition-colors hover:bg-slate-100"
            >
              Đăng nhập
            </RouterLink>
            <RouterLink
              to="/register"
              class="rounded-xl bg-blue-600 px-4 py-2 text-sm font-semibold text-white shadow-sm transition-colors hover:bg-blue-700"
            >
              Đăng ký
            </RouterLink>
          </template>
        </div>
      </nav>
    </header>

    <main class="flex-1">
      <RouterView />
    </main>

    <footer class="border-t border-slate-200 bg-white">
      <div
        class="mx-auto flex max-w-7xl flex-col gap-2 px-4 py-6 text-sm text-slate-500 sm:flex-row sm:items-center sm:justify-between sm:px-6 lg:px-8"
      >
        <p>© 2026 RentalHouse. All rights reserved.</p>
        <p>Nền tảng thuê nhà tiện lợi và an toàn.</p>
      </div>
    </footer>
  </div>
</template>
