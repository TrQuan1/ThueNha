// src/services/auth.service.ts
import apiClient from './api.client'
import type { LoginRequest, RegisterRequest, AuthResponse } from '../types/auth'

export const authService = {
  /**
   * Gửi yêu cầu đăng nhập
   * @param payload Thông tin đăng nhập (email, password)
   * @returns AuthResponse chứa token và thông tin user
   */
  async login(payload: LoginRequest): Promise<AuthResponse> {
    // Truyền AuthResponse cho cả T và R để loại bỏ hoàn toàn any
    return apiClient.post<AuthResponse, AuthResponse>('/api/auth/login', payload)
  },

  /**
   * Gửi yêu cầu đăng ký tài khoản mới
   * @param payload Thông tin đăng ký
   * @returns AuthResponse chứa thông tin tài khoản mới tạo
   */
  async register(payload: RegisterRequest): Promise<AuthResponse> {
    return apiClient.post<AuthResponse, AuthResponse>('/api/auth/register', payload)
  },
}
