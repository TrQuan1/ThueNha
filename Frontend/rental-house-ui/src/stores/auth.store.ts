import { defineStore } from 'pinia'
import { AuthService } from '../services/auth.service'
import type { LoginRequest, RegisterRequest, AuthResponse } from '../types/auth'

interface UserInfo {
  userId: string
  name: string
  email: string
  role: string
}

interface AuthState {
  user: UserInfo | null
  token: string | null
  isAuthenticated: boolean
  isLoading: boolean
  error: string | null
}

export const useAuthStore = defineStore('auth', {
  state: (): AuthState => ({
    user: localStorage.getItem('user') ? JSON.parse(localStorage.getItem('user') as string) : null,
    token: localStorage.getItem('token'),
    isAuthenticated: !!localStorage.getItem('token'),
    isLoading: false,
    error: null,
  }),

  actions: {
    async login(credentials: LoginRequest) {
      this.isLoading = true
      this.error = null
      try {
        const response = await AuthService.login(credentials)
        this.setAuthData(response)
      } catch (error: unknown) {
        if (error instanceof Error) {
          this.error = error.message
        } else {
          this.error = 'Đăng nhập thất bại. Vui lòng kiểm tra thông tin.'
        }
        throw error
      } finally {
        this.isLoading = false
      }
    },

    async register(credentials: RegisterRequest) {
      this.isLoading = true
      this.error = null
      try {
        // Chỉ gọi API, không thiết lập dữ liệu đăng nhập (Token) ở bước này
        await AuthService.register(credentials)
      } catch (error: unknown) {
        if (error instanceof Error) {
          this.error = error.message
        } else {
          this.error = 'Đăng ký thất bại. Vui lòng kiểm tra lại.'
        }
        throw error
      } finally {
        this.isLoading = false
      }
    },

    setAuthData(response: AuthResponse) {
      this.token = response.accessToken
      this.user = {
        userId: response.userId,
        name: response.fullName,
        email: response.email,
        role: response.role,
      }
      this.isAuthenticated = true
      localStorage.setItem('token', response.accessToken)
      localStorage.setItem('user', JSON.stringify(this.user))
    },

    logout() {
      this.user = null
      this.token = null
      this.isAuthenticated = false
      this.error = null

      localStorage.removeItem('token')
      localStorage.removeItem('user')
      window.location.reload()
    },
  },
})
