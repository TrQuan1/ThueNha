// src/types/auth.ts

export interface User {
  id: string
  fullName: string
  email: string
  role: string
}

export interface AuthResponse {
  token: string
  user: User
}

export interface LoginRequest {
  email: string
  password: string
}

export interface RegisterRequest {
  fullName: string
  email: string
  password: string
  phoneNumber: string
  role?: string
}
