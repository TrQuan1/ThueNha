<template>
  <div class="flex flex-col min-h-screen bg-gray-50">
    <header
      class="bg-white shadow-sm px-6 py-4 flex justify-between items-center sticky top-0 z-40"
    >
      <div class="text-2xl font-extrabold text-gray-900 cursor-pointer tracking-tight">
        <router-link to="/">RentalHouse</router-link>
      </div>

      <div class="flex items-center gap-4 sm:gap-6">
        <template v-if="authStore.isAuthenticated">
          <template v-if="authStore.user?.role === 'Host'">
            <router-link
              :to="{ name: 'host-properties' }"
              class="text-sm font-semibold text-gray-600 hover:text-blue-600 transition-colors flex items-center gap-1"
            >
              <span>🏠</span> Nhà của tôi
            </router-link>

            <router-link
              :to="{ name: 'host-bookings' }"
              class="text-sm font-semibold text-gray-600 hover:text-blue-600 transition-colors"
            >
              Quản lý yêu cầu
            </router-link>

            <router-link
              :to="{ name: 'property-create' }"
              class="flex items-center gap-2 bg-blue-600 hover:bg-blue-700 text-white px-4 py-2.5 rounded-xl font-semibold transition-colors shadow-sm cursor-pointer"
            >
              <span class="text-lg">🏠</span>
              <span class="hidden sm:inline">Đăng tin mới</span>
            </router-link>
          </template>

          <template v-else-if="authStore.user?.role === 'Admin'">
            <router-link
              :to="{ name: 'admin-properties' }"
              class="text-sm font-semibold text-gray-600 hover:text-blue-600 transition-colors flex items-center gap-1"
            >
              <span>🛡️</span> Duyệt tin đăng
            </router-link>

            <router-link
              to="/admin/users"
              class="text-sm font-semibold text-gray-600 hover:text-blue-600 transition-colors flex items-center gap-1"
            >
              <span>👥</span> Quản lý người dùng
            </router-link>
          </template>

          <template v-else-if="authStore.user?.role === 'Tenant'">
            <router-link
              :to="{ name: 'wishlist' }"
              class="text-sm font-semibold text-gray-600 hover:text-red-500 transition-colors flex items-center gap-1"
            >
              <span>❤️</span> Yêu thích
            </router-link>

            <router-link
              :to="{ name: 'my-bookings' }"
              class="text-sm font-semibold text-gray-600 hover:text-blue-600 transition-colors"
            >
              Chuyến đi của tôi
            </router-link>
          </template>
        </template>

        <template v-if="!authStore.isAuthenticated">
          <button
            class="bg-blue-600 hover:bg-blue-700 text-white px-5 py-2.5 rounded-xl font-semibold transition-colors cursor-pointer shadow-sm"
            @click="isAuthModalOpen = true"
          >
            Đăng nhập / Đăng ký
          </button>
        </template>

        <template v-else>
          <router-link
            to="/account/profile"
            class="font-medium text-gray-700 hover:text-blue-600 transition-colors hidden sm:flex items-center cursor-pointer group"
          >
            <span class="mr-2 text-xl group-hover:scale-110 transition-transform"></span>
            Xin chào,
            <span class="font-bold text-gray-900 group-hover:text-blue-600 ml-1">{{
              authStore.user?.name
            }}</span>
          </router-link>

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
import { RouterView, RouterLink, useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth.store'
import AuthModal from '@/components/common/AuthModal.vue'

const router = useRouter()
const authStore = useAuthStore()
const isAuthModalOpen = ref(false)

const handleLogout = async () => {
  authStore.logout()
  await router.push('/')
}
</script>
