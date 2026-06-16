import apiClient from '@/services/api.client'
import type { User } from '@/types/auth'
import type { ProfileDto, UpdateProfilePayload } from '@/types/auth'

export const userService = {
  async getUsers(search?: string): Promise<User[]> {
    const response = await apiClient.get<User[]>('/admin/users', { params: { search } })
    return response.data
  },
  async banUser(id: number | string): Promise<void> {
    await apiClient.put(`/admin/users/${id}/ban`)
  },
  async unbanUser(id: number | string): Promise<void> {
    await apiClient.put(`/admin/users/${id}/unban`)
  },
  async getProfile(): Promise<ProfileDto> {
    const response = await apiClient.get<ProfileDto>('/account/profile')
    return response.data
  },

  async updateProfile(data: UpdateProfilePayload): Promise<void> {
    await apiClient.put('/account/profile', data)
  },
}
