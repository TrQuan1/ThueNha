import apiClient from './api.client'

export interface NotificationDto {
  id: number
  title: string
  content: string
  redirectUrl: string | null
  isRead: boolean
  createdAt: string
}

export const notificationService = {
  // Lấy danh sách thông báo
  async getMyNotifications(): Promise<NotificationDto[]> {
    const response = await apiClient.get('/notifications')
    return response.data
  },

  // Đánh dấu đã đọc
  async markAsRead(id: number): Promise<void> {
    await apiClient.post(`/notifications/${id}/read`)
  },
}
