export interface AuthResponse {
  accessToken: string
  refreshToken: string
  expiration: string | Date
  userId: string
  fullName: string
  email: string
  role: string
}

export interface LoginRequest {
  email: string
  password?: string
}

export interface RegisterRequest {
  fullName: string
  email: string
  phoneNumber: string
  password?: string
  role: number
}
export interface User {
  id: number | string
  fullName: string
  email: string
  role: number // Đổi sang number
  status: number // Đổi sang number
}
