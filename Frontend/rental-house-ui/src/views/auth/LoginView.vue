<script setup lang="ts">
import { ref, reactive } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '../../stores/auth.store'
import type { LoginRequest } from '../../types/auth'

const router = useRouter()
const authStore = useAuthStore()

// Trạng thái Form nhập liệu
const form = reactive<LoginRequest>({
  email: '',
  password: '',
})

// Trạng thái bắt lỗi của từng trường dữ liệu (Validation)
const errors = reactive({
  email: '',
  password: '',
})

const loading = ref(false)
const globalError = ref('')

/**
 * Kiểm tra dữ liệu form trước khi gửi lên Server
 */
const validateForm = (): boolean => {
  let isValid = true
  globalError.value = ''

  // Kiểm tra Email
  if (!form.email.trim()) {
    errors.email = 'Email không được để trống'
    isValid = false
  } else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(form.email)) {
    errors.email = 'Email không đúng định dạng'
    isValid = false
  } else {
    errors.email = ''
  }

  // Kiểm tra Mật khẩu
  if (!form.password) {
    errors.password = 'Mật khẩu không được để trống'
    isValid = false
  } else if (form.password.length < 6) {
    errors.password = 'Mật khẩu phải có ít nhất 6 ký tự'
    isValid = false
  } else {
    errors.password = ''
  }

  return isValid
}

/**
 * Xử lý sự kiện Submit đăng nhập
 */
const onSubmit = async () => {
  if (!validateForm()) return

  loading.value = true
  globalError.value = ''

  try {
    // Gọi action handleLogin từ Pinia Store đã thiết lập
    await authStore.handleLogin({
      email: form.email,
      password: form.password,
    })

    // Điều hướng về trang chủ khi đăng nhập thành công
    router.push('/')
  } catch (error) {
    const err = error as { response?: { data?: { message?: string } }; message?: string }
    globalError.value =
      err?.response?.data?.message || err?.message || 'Sai tài khoản hoặc mật khẩu'
  } finally {
    loading.value = false
  }
}
</script>

<template>
  <div class="min-h-screen flex items-center justify-center bg-gray-50 px-4 sm:px-6 lg:px-8">
    <div
      class="max-w-md w-full space-y-8 bg-white p-8 rounded-2xl shadow-xl border border-gray-100"
    >
      <div>
        <h2 class="mt-2 text-center text-3xl font-extrabold text-gray-900">Đăng nhập hệ thống</h2>
        <p class="mt-2 text-center text-sm text-gray-600">
          Hoặc
          <router-link
            to="/register"
            class="font-medium text-blue-600 hover:text-blue-500 transition-colors"
          >
            đăng ký tài khoản mới
          </router-link>
        </p>
      </div>

      <div v-if="globalError" class="bg-red-50 border-l-4 border-red-500 p-4 rounded-md">
        <div class="flex">
          <div class="flex-shrink-0">
            <svg class="h-5 w-5 text-red-500" viewBox="0 0 20 20" fill="currentColor">
              <path
                fill-rule="evenodd"
                d="M10 18a8 8 0 100-16 8 8 0 000 16zM8.707 7.293a1 1 0 00-1.414 1.414L8.586 10l-1.293 1.293a1 1 0 101.414 1.414L10 11.414l1.293 1.293a1 1 0 001.414-1.414L11.414 10l1.293-1.293a1 1 0 00-1.414-1.414L10 8.586 8.707 7.293z"
                clip-rule="evenodd"
              />
            </svg>
          </div>
          <div class="ml-3">
            <p class="text-sm text-red-700 font-medium">{{ globalError }}</p>
          </div>
        </div>
      </div>

      <form class="mt-8 space-y-6" @submit.prevent="onSubmit">
        <div class="space-y-4">
          <div>
            <label for="email" class="block text-sm font-medium text-gray-700 mb-1"
              >Địa chỉ Email</label
            >
            <input
              id="email"
              v-model="form.email"
              type="text"
              autocomplete="email"
              class="appearance-none block w-full px-4 py-3 border rounded-xl shadow-sm placeholder-gray-400 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-all text-sm"
              :class="
                errors.email
                  ? 'border-red-300 focus:ring-red-500 focus:border-red-500'
                  : 'border-gray-300'
              "
              placeholder="example@gmail.com"
            />
            <p v-if="errors.email" class="mt-1 text-xs text-red-600 font-medium">
              {{ errors.email }}
            </p>
          </div>

          <div>
            <label for="password" class="block text-sm font-medium text-gray-700 mb-1"
              >Mật khẩu</label
            >
            <input
              id="password"
              v-model="form.password"
              type="password"
              autocomplete="current-password"
              class="appearance-none block w-full px-4 py-3 border rounded-xl shadow-sm placeholder-gray-400 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-all text-sm"
              :class="
                errors.password
                  ? 'border-red-300 focus:ring-red-500 focus:border-red-500'
                  : 'border-gray-300'
              "
              placeholder="••••••••"
            />
            <p v-if="errors.password" class="mt-1 text-xs text-red-600 font-medium">
              {{ errors.password }}
            </p>
          </div>
        </div>

        <div>
          <button
            type="submit"
            :disabled="loading"
            class="group relative w-full flex justify-center py-3 px-4 border border-transparent text-sm font-semibold rounded-xl text-white bg-blue-600 hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500 disabled:opacity-60 disabled:cursor-not-allowed shadow-md hover:shadow-lg transition-all"
          >
            <svg
              v-if="loading"
              class="animate-spin -ml-1 mr-3 h-5 w-5 text-white"
              xmlns="http://www.w3.org/2000/svg"
              fill="none"
              viewBox="0 0 24 24"
            >
              <circle
                class="opacity-25"
                cx="12"
                cy="12"
                r="10"
                stroke="currentColor"
                stroke-width="4"
              ></circle>
              <path
                class="opacity-75"
                fill="currentColor"
                d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"
              ></path>
            </svg>
            {{ loading ? 'Đang xử lý...' : 'Đăng nhập' }}
          </button>
        </div>
      </form>
    </div>
  </div>
</template>
