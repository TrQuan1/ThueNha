import apiClient from './api.client'
import type { LoginRequest, RegisterRequest, AuthResponse } from '../types/auth'

export const AuthService = {
  async login(data: LoginRequest): Promise<AuthResponse> {
    const response = await apiClient.post<AuthResponse>('/Auth/login', data)
    return response.data
  },

  async register(data: RegisterRequest): Promise<AuthResponse> {
    const response = await apiClient.post<AuthResponse>('/Auth/register', data)
    return response.data
  },
}
