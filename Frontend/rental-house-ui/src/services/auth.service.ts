import apiClient from '@/services/api.client'
import type { LoginRequest, AuthResponse } from '../types/auth'

export const AuthService = {
  async login(data: LoginRequest): Promise<AuthResponse> {
    const response = await apiClient.post<AuthResponse>('/Auth/login', data)
    return response.data
  },
}
