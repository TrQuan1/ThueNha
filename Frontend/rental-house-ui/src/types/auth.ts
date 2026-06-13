export interface AuthResponse {
  token: string
  refreshToken: string
  expiration: string | Date
  userId: string
  name: string
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
