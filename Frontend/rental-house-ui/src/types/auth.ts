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
