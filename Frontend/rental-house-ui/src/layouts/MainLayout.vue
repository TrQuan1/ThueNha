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
              :to="{ name: 'HostDashboard' }"
              class="text-sm font-semibold text-gray-600 hover:text-blue-600 transition-colors flex items-center gap-1"
            >
              <span>📊</span> Thống kê
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
            <router-link
              to="/admin/dashboard"
              class="text-sm font-semibold text-gray-600 hover:text-blue-600 transition-colors flex items-center gap-1"
            >
              <span>📊</span> Thống kê & Giao dịch
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
            to="/chat"
            class="text-gray-600 hover:text-blue-600 font-medium px-3 py-2 rounded-md"
          >
            💬 Tin nhắn
          </router-link>
          <div class="relative flex items-center ml-1 sm:ml-2">
            <button
              @click="toggleNotifications"
              class="relative p-2 text-gray-600 hover:text-blue-600 transition-colors cursor-pointer outline-none"
            >
              <span class="text-2xl">🔔</span>
              <span
                v-if="unreadCount > 0"
                class="absolute top-0 right-0 flex items-center justify-center min-w-5 h-5 text-[11px] font-bold text-white bg-red-500 rounded-full px-1 border-2 border-white shadow-sm"
              >
                {{ unreadCount > 99 ? '99+' : unreadCount }}
              </span>
            </button>

            <div
              v-if="isNotificationOpen"
              @click="isNotificationOpen = false"
              class="fixed inset-0 z-40"
            ></div>

            <div
              v-if="isNotificationOpen"
              class="absolute right-0 top-14 mt-2 w-80 sm:w-96 bg-white rounded-2xl shadow-xl border border-gray-100 overflow-hidden z-50"
            >
              <div
                class="p-4 border-b border-gray-100 bg-gray-50/80 flex justify-between items-center"
              >
                <h3 class="font-bold text-gray-900">Thông báo</h3>
                <span
                  v-if="unreadCount > 0"
                  class="text-xs font-semibold bg-blue-100 text-blue-700 px-2.5 py-1 rounded-full"
                  >{{ unreadCount }} chưa đọc</span
                >
              </div>

              <div class="max-h-100 overflow-y-auto">
                <div v-if="notifications.length === 0" class="p-8 text-center text-gray-500">
                  <span class="text-4xl block mb-3 opacity-40">🔕</span>
                  <p class="text-sm font-medium">Bạn chưa có thông báo nào</p>
                </div>

                <div
                  v-for="notif in notifications"
                  :key="notif.id"
                  @click="handleNotificationClick(notif)"
                  class="p-4 border-b border-gray-50 hover:bg-gray-50 cursor-pointer transition flex gap-3"
                  :class="{ 'bg-blue-50/40': !notif.isRead }"
                >
                  <div class="mt-1 shrink-0">
                    <span
                      v-if="!notif.isRead"
                      class="w-2.5 h-2.5 bg-blue-600 rounded-full inline-block shadow-sm"
                    ></span>
                    <span v-else class="w-2.5 h-2.5 bg-gray-300 rounded-full inline-block"></span>
                  </div>
                  <div class="flex-1 min-w-0">
                    <p
                      class="text-sm font-bold text-gray-900 truncate"
                      :class="{ 'text-blue-700': !notif.isRead }"
                    >
                      {{ notif.title }}
                    </p>
                    <p class="text-xs text-gray-600 mt-1 line-clamp-2 leading-relaxed">
                      {{ notif.content }}
                    </p>
                    <p class="text-[10px] text-gray-400 mt-2 font-medium">
                      {{ formatDate(notif.createdAt) }}
                    </p>
                  </div>
                </div>
              </div>
            </div>
          </div>
          <router-link
            to="/account/profile"
            class="font-medium text-gray-700 hover:text-blue-600 transition-colors hidden sm:flex items-center cursor-pointer group ml-2"
          >
            Xin chào,
            <span class="font-bold text-gray-900 group-hover:text-blue-600 ml-1">{{
              authStore.user?.name
            }}</span>
          </router-link>

          <button
            class="text-red-500 border border-red-500 hover:bg-red-50 px-4 py-2.5 rounded-xl font-semibold transition-colors cursor-pointer ml-4"
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
import { ref, onMounted, onUnmounted, computed } from 'vue'
import { RouterView, RouterLink, useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth.store'
import AuthModal from '@/components/common/AuthModal.vue'
import { notificationService, type NotificationDto } from '@/services/notification.service'

const router = useRouter()
const authStore = useAuthStore()
const isAuthModalOpen = ref(false)

// --- LOGIC CHUÔNG THÔNG BÁO ---
const isNotificationOpen = ref(false)
const notifications = ref<NotificationDto[]>([])

const unreadCount = computed(() => {
  return notifications.value.filter((n) => !n.isRead).length
})

const fetchNotifications = async () => {
  if (authStore.isAuthenticated) {
    try {
      notifications.value = await notificationService.getMyNotifications()
    } catch (error) {
      console.error('Lỗi tải thông báo', error)
    }
  }
}

const toggleNotifications = () => {
  isNotificationOpen.value = !isNotificationOpen.value
  // Mỗi lần mở chuông lên sẽ tải lại dữ liệu cho tươi mới
  if (isNotificationOpen.value) {
    fetchNotifications()
  }
}

const handleNotificationClick = async (notif: NotificationDto) => {
  // Gửi API đánh dấu đã đọc nếu nó đang sáng màu
  if (!notif.isRead) {
    try {
      await notificationService.markAsRead(notif.id)
      notif.isRead = true // Tắt đèn chấm xanh trên giao diện ngay lập tức
    } catch (error) {
      console.error('Lỗi khi đánh dấu đã đọc', error)
    }
  }

  isNotificationOpen.value = false // Đóng menu

  // Điều hướng (Ví dụ: nhảy đến trang lịch sử đặt phòng)
  if (notif.redirectUrl) {
    router.push(notif.redirectUrl)
  }
}

const formatDate = (dateString: string) => {
  const utcDateString = dateString.endsWith('Z') ? dateString : dateString + 'Z'
  const d = new Date(utcDateString)
  return d.toLocaleString('vi-VN', {
    hour: '2-digit',
    minute: '2-digit',
    day: '2-digit',
    month: '2-digit',
    year: 'numeric',
  })
}

// Gọi API lần đầu khi load toàn bộ khung trang
// Biến lưu trữ bộ đếm giờ
let pollingInterval: ReturnType<typeof setInterval> | null = null

// Gọi API lần đầu khi load toàn bộ khung trang
onMounted(() => {
  fetchNotifications()

  // BÍ THUẬT SHORT POLLING: Tự động gọi lại ngầm mỗi 10 giây (10000ms)
  pollingInterval = setInterval(() => {
    if (authStore.isAuthenticated) {
      fetchNotifications()
    }
  }, 5000)
})

// Dọn dẹp bộ đếm khi đóng trình duyệt hoặc tắt component (Chuẩn Clean Code)
onUnmounted(() => {
  if (pollingInterval) {
    clearInterval(pollingInterval)
  }
})
// -----------------------------

const handleLogout = async () => {
  authStore.logout()
  await router.push('/')
}
</script>
