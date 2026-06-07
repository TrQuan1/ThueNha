// src/stores/auth.store.ts
import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import { authService } from '../services/auth.service'
import type { User, LoginRequest } from '../types/auth'

export const useAuthStore = defineStore('auth', () => {
  // --- STATE ---
  // Khởi tạo token từ localStorage để giữ trạng thái đăng nhập khi reload trang
  const token = ref<string | null>(localStorage.getItem('accessToken'))
  const user = ref<User | null>(null)

  // --- GETTERS (Computed properties) ---
  const isAuthenticated = computed<boolean>(() => !!token.value)

  // --- ACTIONS ---
  /**
   * Xử lý luồng đăng nhập hệ thống
   * @param payload Bản tin chứa email và password tuân thủ LoginRequest interface
   */
  async function handleLogin(payload: LoginRequest): Promise<void> {
    try {
      const response = await authService.login(payload)

      // Cập nhật dữ liệu vào State bộ nhớ tạm
      token.value = response.token
      user.value = response.user

      // Đồng bộ mã thông báo Token vào localStorage để duy trì phiên
      localStorage.setItem('accessToken', response.token)
    } catch (error) {
      // Chuyển tiếp lỗi ra ngoài để các Component (.vue) có thể tiếp nhận và hiển thị thông báo lỗi lên UI
      return Promise.reject(error)
    }
  }

  /**
   * Đăng xuất, xóa toàn bộ trạng thái trong State và bộ lưu trữ vật lý của trình duyệt
   */
  function logout(): void {
    token.value = null
    user.value = null
    localStorage.removeItem('accessToken')
  }

  return {
    token,
    user,
    isAuthenticated,
    handleLogin,
    logout,
  }
})
