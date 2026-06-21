<template>
  <div v-if="modelValue" class="modal-backdrop" @click.self="closeModal">
    <div class="modal-box">
      <button class="close-btn" @click="closeModal">&times;</button>

      <div class="modal-header">
        <div class="logo-placeholder">🏠</div>
        <h3>
          <template v-if="currentView === 'login'">Chào mừng đến với RentalHouse</template>
          <template v-else-if="currentView === 'register'">Tạo tài khoản mới</template>
          <template v-else-if="currentView === 'forgot'">Khôi phục mật khẩu</template>
          <template v-else-if="currentView === 'reset'">Đặt lại mật khẩu</template>
        </h3>
      </div>

      <div class="form-container">
        <div v-if="localError || authStore.error" class="error-msg">
          {{ localError || authStore.error }}
        </div>
        <div
          v-if="localSuccess"
          class="success-msg"
          style="text-align: center; margin-bottom: 1rem"
        >
          {{ localSuccess }}
        </div>

        <form v-if="currentView === 'login'" @submit.prevent="handleLogin">
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
            <a href="#" class="link-forgot" @click.prevent="switchView('forgot')">Quên mật khẩu?</a>
            <span class="link-switch" @click="switchView('register')">Đăng ký</span>
          </div>

          <button type="submit" class="submit-btn" :disabled="authStore.isLoading">
            {{ authStore.isLoading ? 'Đang xử lý...' : 'Đăng nhập' }}
          </button>
        </form>

        <form v-else-if="currentView === 'register'" @submit.prevent="handleRegister">
          <div class="form-group">
            <input v-model="registerForm.fullName" type="text" placeholder="Họ và Tên" required />
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

          <div class="form-group role-group">
            <label>
              <input type="radio" v-model.number="registerForm.role" :value="3" /> Người thuê nhà
            </label>
            <label>
              <input type="radio" v-model.number="registerForm.role" :value="2" /> Chủ nhà
            </label>
          </div>

          <div class="form-footer justify-end">
            <span class="link-switch" @click="switchView('login')">Đã có tài khoản? Đăng nhập</span>
          </div>

          <button type="submit" class="submit-btn" :disabled="authStore.isLoading">
            {{ authStore.isLoading ? 'Đang xử lý...' : 'Đăng ký' }}
          </button>
        </form>

        <form v-else-if="currentView === 'forgot'" @submit.prevent="handleForgotPassword">
          <p style="text-align: center; margin-bottom: 1rem; color: #666">
            Nhập email để nhận mã xác thực.
          </p>
          <div class="form-group">
            <input
              v-model="forgotForm.email"
              type="email"
              placeholder="Nhập địa chỉ Email"
              required
            />
          </div>
          <div class="form-footer" style="justify-content: center">
            <a href="#" class="link-forgot" @click.prevent="switchView('login')"
              >⬅ Quay lại đăng nhập</a
            >
          </div>
          <button type="submit" class="submit-btn" :disabled="isProcessing">
            {{ isProcessing ? 'Đang gửi...' : 'Gửi mã OTP' }}
          </button>
        </form>

        <form v-else-if="currentView === 'reset'" @submit.prevent="handleResetPassword">
          <p style="text-align: center; margin-bottom: 1rem; color: #666">
            Nhập mã OTP và mật khẩu mới.
          </p>
          <div class="form-group">
            <input v-model="forgotForm.otp" type="text" placeholder="Nhập mã OTP (6 số)" required />
          </div>
          <div class="form-group password-group">
            <input
              v-model="forgotForm.newPassword"
              :type="showPassword ? 'text' : 'password'"
              placeholder="Mật khẩu mới (ít nhất 6 ký tự)"
              required
              minlength="6"
            />
            <span class="eye-icon" @click="showPassword = !showPassword">
              {{ showPassword ? '👁️' : '🔒' }}
            </span>
          </div>
          <button type="submit" class="submit-btn" :disabled="isProcessing">
            {{ isProcessing ? 'Đang xử lý...' : 'Xác nhận đổi mật khẩu' }}
          </button>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive } from 'vue'
import { useAuthStore } from '@/stores/auth.store'
import { AuthService } from '@/services/auth.service'

defineProps<{ modelValue: boolean }>()
const emit = defineEmits(['update:modelValue', 'login-success'])

const authStore = useAuthStore()

const currentView = ref<'login' | 'register' | 'forgot' | 'reset'>('login')
const showPassword = ref(false)
const isProcessing = ref(false)
const localError = ref<string | null>(null)
const localSuccess = ref<string | null>(null)

const loginForm = reactive({ email: '', password: '' })
const registerForm = reactive({ fullName: '', email: '', phoneNumber: '', password: '', role: 3 })
const forgotForm = reactive({ email: '', otp: '', newPassword: '' })

const switchView = (view: 'login' | 'register' | 'forgot' | 'reset') => {
  currentView.value = view
  localError.value = null
  localSuccess.value = null
  authStore.error = null
}

const closeModal = () => {
  authStore.error = null
  emit('update:modelValue', false)
  setTimeout(() => switchView('login'), 300)
}

const handleLogin = async () => {
  localError.value = null
  try {
    await authStore.login(loginForm)
    if (authStore.isAuthenticated) {
      emit('login-success')
      closeModal()
    }
  } catch {
    // Lỗi đã được store bắt
  }
}

const handleRegister = async () => {
  await authStore.register(registerForm)
  if (!authStore.error) {
    alert('Đăng ký thành công!')
    switchView('login')
  }
}

const handleForgotPassword = async () => {
  localError.value = null
  isProcessing.value = true
  try {
    await AuthService.forgotPassword(forgotForm.email.trim())
    localSuccess.value = 'Mã OTP đã gửi! Kiểm tra Email của bạn.'
    setTimeout(() => {
      currentView.value = 'reset'
      localSuccess.value = null
    }, 2000)
  } catch (error: unknown) {
    // FIX: Đổi 'any' thành 'unknown'
    const err = error as { response?: { data?: { message?: string } } }
    localError.value = err.response?.data?.message || 'Lỗi gửi mã OTP.'
  } finally {
    isProcessing.value = false
  }
}

const handleResetPassword = async () => {
  localError.value = null
  isProcessing.value = true
  try {
    await AuthService.resetPassword({
      email: forgotForm.email.trim(),
      otp: forgotForm.otp.trim(),
      newPassword: forgotForm.newPassword,
    })
    localSuccess.value = 'Đổi mật khẩu thành công!'
    setTimeout(() => switchView('login'), 2000)
  } catch (error: unknown) {
    // FIX: Đổi 'any' thành 'unknown'
    const err = error as { response?: { data?: { message?: string } } }
    localError.value = err.response?.data?.message || 'Mã OTP không đúng hoặc hết hạn.'
  } finally {
    isProcessing.value = false
  }
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
input[type='text'],
input[type='email'],
input[type='tel'],
input[type='password'] {
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
.link-forgot,
.link-switch {
  color: #f97316;
  text-decoration: none;
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
.success-msg {
  color: #16a34a;
  margin-top: 1rem;
  font-size: 0.85rem;
}
</style>
