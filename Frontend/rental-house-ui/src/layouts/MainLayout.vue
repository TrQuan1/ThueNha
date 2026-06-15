<template>
  <div class="flex flex-col min-h-screen bg-gray-50">
    <header
      class="bg-white shadow-sm px-6 py-4 flex justify-between items-center sticky top-0 z-40"
    >
      <div class="text-2xl font-extrabold text-gray-900 cursor-pointer tracking-tight">
        <router-link to="/">RentalHouse</router-link>
      </div>

      <div class="flex items-center gap-3 sm:gap-5">
        <router-link
          v-if="authStore.isAuthenticated && authStore.user?.role === 'Host'"
          :to="{ name: 'property-create' }"
          class="flex items-center gap-2 bg-orange-500 hover:bg-orange-600 text-white px-4 py-2.5 rounded-xl font-semibold transition-colors shadow-sm cursor-pointer"
        >
          <span class="text-lg">🏠</span>
          <span class="hidden sm:inline">Đăng tin mới</span>
        </router-link>

        <template v-if="!authStore.isAuthenticated">
          <button
            class="bg-blue-600 hover:bg-blue-700 text-white px-5 py-2.5 rounded-xl font-semibold transition-colors cursor-pointer shadow-sm"
            @click="isAuthModalOpen = true"
          >
            Đăng nhập / Đăng ký
          </button>
        </template>

        <template v-else>
          <span class="font-medium text-gray-700 hidden sm:block">
            Xin chào, <span class="font-bold text-gray-900">{{ authStore.user?.name }}</span>
          </span>
          <button
            class="text-red-500 border border-red-500 hover:bg-red-50 px-4 py-2.5 rounded-xl font-semibold transition-colors cursor-pointer"
            @click="handleLogout"
          >
            Đăng xuất
          </button>
        </template>
      </div>
    </header>

    <main class="grow w-full max-w-7xl mx-auto p-4 sm:p-6 lg:p-8">
      <RouterView />
    </main>

    <footer class="bg-gray-800 text-gray-400 text-center py-6 mt-auto">
      <p>&copy; 2026 RentalHouse. All rights reserved.</p>
    </footer>

    <AuthModal v-model="isAuthModalOpen" />
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { RouterView, RouterLink } from 'vue-router'
import { useAuthStore } from '../stores/auth.store'
import AuthModal from '../components/common/AuthModal.vue'

const authStore = useAuthStore()
const isAuthModalOpen = ref(false)

const handleLogout = () => {
  authStore.logout()
}
</script>
