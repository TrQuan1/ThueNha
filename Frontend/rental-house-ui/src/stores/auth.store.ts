import { defineStore } from 'pinia'
import { AuthService } from '../services/auth.service'
import type { LoginRequest } from '../types/auth'

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

        this.token = response.token
        this.user = {
          userId: response.userId,
          name: response.name,
          email: response.email,
          role: response.role,
        }
        this.isAuthenticated = true

        localStorage.setItem('token', response.token)
        localStorage.setItem('user', JSON.stringify(this.user))
      } catch (error: unknown) {
        if (error instanceof Error) {
          this.error = error.message
        } else {
          this.error = 'Đã xảy ra lỗi hệ thống'
        }
      } finally {
        this.isLoading = false
      }
    },

    logout() {
      this.user = null
      this.token = null
      this.isAuthenticated = false
      this.error = null

      localStorage.removeItem('token')
      localStorage.removeItem('user')
    },
  },
})
