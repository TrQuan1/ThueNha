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
  name: string
  email: string
  phoneNumber: string
  password?: string
}
