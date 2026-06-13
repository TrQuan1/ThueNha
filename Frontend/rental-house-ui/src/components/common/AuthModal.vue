<template>
  <div v-if="modelValue" class="modal-backdrop" @click.self="closeModal">
    <div class="modal-box">
      <button class="close-btn" @click="closeModal">&times;</button>

      <div class="modal-header">
        <div class="logo-placeholder">🏠</div>
        <h3>Chào mừng bạn đến với RentalHouse</h3>
      </div>

      <div class="form-container">
        <form v-if="isLogin" @submit.prevent="handleLogin">
          <div class="form-group">
            <input
              v-model="loginForm.email"
              type="text"
              placeholder="Email hoặc Số điện thoại"
              required
            />
          </div>
          <div class="form-group password-group">
            <input
              v-model="loginForm.password"
              :type="showPassword ? 'text' : 'password'"
              placeholder="Mật khẩu"
              required
            />
            <span class="eye-icon" @click="showPassword = !showPassword">
              {{ showPassword ? '👁️' : '🔒' }}
            </span>
          </div>

          <div class="form-footer">
            <a href="#" class="link-forgot">Quên mật khẩu?</a>
            <span class="link-switch" @click="isLogin = false">Đăng ký</span>
          </div>

          <button type="submit" class="submit-btn" :disabled="authStore.isLoading">
            {{ authStore.isLoading ? 'Đang xử lý...' : 'Đăng nhập' }}
          </button>
        </form>

        <form v-else @submit.prevent="handleRegister">
          <div class="form-group">
            <input v-model="registerForm.name" type="text" placeholder="Họ và Tên" required />
          </div>
          <div class="form-group">
            <input v-model="registerForm.email" type="email" placeholder="Email" required />
          </div>
          <div class="form-group">
            <input
              v-model="registerForm.phoneNumber"
              type="tel"
              placeholder="Số điện thoại"
              required
            />
          </div>
          <div class="form-group password-group">
            <input
              v-model="registerForm.password"
              :type="showPassword ? 'text' : 'password'"
              placeholder="Mật khẩu"
              required
            />
            <span class="eye-icon" @click="showPassword = !showPassword">
              {{ showPassword ? '👁️' : '🔒' }}
            </span>
          </div>

          <div class="form-footer justify-end">
            <span class="link-switch" @click="isLogin = true">Đã có tài khoản? Đăng nhập</span>
          </div>

          <button type="submit" class="submit-btn" :disabled="authStore.isLoading">
            {{ authStore.isLoading ? 'Đang xử lý...' : 'Đăng ký' }}
          </button>
        </form>

        <div v-if="authStore.error" class="error-msg">{{ authStore.error }}</div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive } from 'vue'
import { useAuthStore } from '../../stores/auth.store'
import type { LoginRequest, RegisterRequest } from '../../types/auth'

defineProps<{ modelValue: boolean }>()
const emit = defineEmits<{ (e: 'update:modelValue', value: boolean): void }>()

const authStore = useAuthStore()
const isLogin = ref(true)
const showPassword = ref(false)

const loginForm = reactive<LoginRequest>({ email: '', password: '' })
const registerForm = reactive<RegisterRequest>({
  name: '',
  email: '',
  phoneNumber: '',
  password: '',
})

const closeModal = () => {
  authStore.error = null
  emit('update:modelValue', false)
}

const handleLogin = async () => {
  await authStore.login(loginForm)
  if (authStore.isAuthenticated) closeModal()
}

const handleRegister = async () => {
  await authStore.register(registerForm)
  if (authStore.isAuthenticated) closeModal()
}
</script>

<style scoped>
.modal-backdrop {
  position: fixed;
  inset: 0;
  background: rgba(0, 0, 0, 0.6);
  display: flex;
  justify-content: center;
  align-items: center;
  z-index: 100;
}

.modal-box {
  background: #fff;
  width: 100%;
  max-width: 420px;
  padding: 2rem;
  border-radius: 16px;
  position: relative;
  box-shadow: 0 10px 25px rgba(0, 0, 0, 0.2);
}

.close-btn {
  position: absolute;
  top: 1rem;
  right: 1.5rem;
  border: none;
  background: none;
  font-size: 1.5rem;
  cursor: pointer;
}

.modal-header {
  text-align: center;
  margin-bottom: 2rem;
}

.logo-placeholder {
  font-size: 3rem;
  margin-bottom: 0.5rem;
}

.modal-header h3 {
  margin: 0;
  font-size: 1.25rem;
  color: #333;
}

.form-group {
  margin-bottom: 1rem;
  position: relative;
}

input {
  width: 100%;
  padding: 0.9rem;
  border: 1px solid #ddd;
  border-radius: 8px;
  box-sizing: border-box;
}

.password-group {
  display: flex;
  align-items: center;
}

.eye-icon {
  position: absolute;
  right: 1rem;
  cursor: pointer;
  user-select: none;
}

.form-footer {
  display: flex;
  justify-content: space-between;
  font-size: 0.9rem;
  margin-bottom: 1.5rem;
}

.justify-end {
  justify-content: flex-end;
}

.link-forgot {
  color: #666;
  text-decoration: none;
}
.link-switch {
  color: #f97316;
  cursor: pointer;
  font-weight: 600;
}

.submit-btn {
  width: 100%;
  padding: 1rem;
  background: linear-gradient(90deg, #f97316, #ef4444);
  color: white;
  border: none;
  border-radius: 50px;
  font-weight: bold;
  cursor: pointer;
}

.error-msg {
  color: #ef4444;
  text-align: center;
  margin-top: 1rem;
  font-size: 0.85rem;
}
</style>
