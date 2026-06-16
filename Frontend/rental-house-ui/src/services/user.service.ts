import apiClient from '@/services/api.client'
import type { User } from '@/types/auth' // Giả định đã có interface User

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
}
