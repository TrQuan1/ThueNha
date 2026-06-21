<template>
  <div class="login-wrapper">
    <div class="login-card">
      <h2 class="title">Đăng Nhập</h2>
      <form @submit.prevent="handleLogin">
        <div class="form-group">
          <label for="email">Email</label>
          <input
            id="email"
            v-model="loginForm.email"
            type="email"
            placeholder="Nhập email"
            required
          />
        </div>

        <div class="form-group">
          <label for="password">Mật khẩu</label>
          <input
            id="password"
            v-model="loginForm.password"
            type="password"
            placeholder="Nhập mật khẩu"
            required
          />
        </div>

        <div class="forgot-password-link">
          <router-link to="/forgot-password">Quên mật khẩu?</router-link>
        </div>

        <div v-if="authStore.error" class="error-message">
          {{ authStore.error }}
        </div>

        <button type="submit" class="submit-btn" :disabled="authStore.isLoading">
          {{ authStore.isLoading ? 'Đang xử lý...' : 'Đăng nhập' }}
        </button>
      </form>
    </div>
  </div>
</template>

<script setup lang="ts">
import { reactive } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '../../stores/auth.store'
import type { LoginRequest } from '../../types/auth'

const router = useRouter()
const authStore = useAuthStore()

const loginForm = reactive<LoginRequest>({
  email: '',
  password: '',
})

const handleLogin = async () => {
  try {
    await authStore.login(loginForm)
    if (authStore.isAuthenticated) {
      router.push('/')
    }
  } catch {
    // Lỗi đã được bắt và lưu vào biến authStore.error ở trong file auth.store.ts rồi
  }
}
</script>

<style scoped>
.login-wrapper {
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: 100vh;
  background-color: #f3f4f6;
  font-family: Arial, sans-serif;
}

.login-card {
  background: #ffffff;
  padding: 2.5rem 2rem;
  border-radius: 8px;
  box-shadow: 0 4px 10px rgba(0, 0, 0, 0.1);
  width: 100%;
  max-width: 400px;
}

.title {
  text-align: center;
  margin-bottom: 1.5rem;
  color: #111827;
}

.form-group {
  margin-bottom: 1.25rem;
}

label {
  display: block;
  margin-bottom: 0.5rem;
  font-size: 0.875rem;
  font-weight: 600;
  color: #374151;
}

input {
  width: 100%;
  padding: 0.75rem;
  border: 1px solid #d1d5db;
  border-radius: 6px;
  font-size: 1rem;
  transition: border-color 0.2s;
  box-sizing: border-box;
}

input:focus {
  outline: none;
  border-color: #3b82f6;
  box-shadow: 0 0 0 2px rgba(59, 130, 246, 0.2);
}

.error-message {
  color: #ef4444;
  font-size: 0.875rem;
  margin-bottom: 1rem;
  text-align: center;
}

.submit-btn {
  width: 100%;
  padding: 0.75rem;
  background-color: #3b82f6;
  color: #ffffff;
  border: none;
  border-radius: 6px;
  font-size: 1rem;
  font-weight: 600;
  cursor: pointer;
  transition: background-color 0.2s;
}

.submit-btn:hover:not(:disabled) {
  background-color: #2563eb;
}

.submit-btn:disabled {
  background-color: #9ca3af;
  cursor: not-allowed;
}

/* 👉 CSS CHO LINK QUÊN MẬT KHẨU */
/* .forgot-password-link {
  text-align: right;
  margin-bottom: 1.25rem;
}

.forgot-password-link a {
  font-size: 0.875rem;
  color: #3b82f6;
  text-decoration: none;
  font-weight: 500;
  transition: color 0.2s;
}

.forgot-password-link a:hover {
  text-decoration: underline;
  color: #2563eb;
} */
</style>
