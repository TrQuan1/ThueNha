import apiClient from './api.client'
import type { Property } from '@/types/property'

export const wishlistService = {
  // Hàm thêm vào danh sách yêu thích
  async addWishlist(propertyId: number | string) {
    // SỬ DỤNG apiClient ĐỂ TỰ ĐỘNG ĐÍNH KÈM TOKEN
    const response = await apiClient.post(`/Wishlist/${propertyId}`)
    return response.data
  },

  // Hàm xóa khỏi danh sách yêu thích
  async removeWishlist(propertyId: number | string) {
    const response = await apiClient.delete(`/Wishlist/${propertyId}`)
    return response.data
  },

  // Hàm lấy danh sách nhà đã thả tim
  async getMyWishlist(): Promise<Property[]> {
    const response = await apiClient.get('/Wishlist')
    return response.data
  },
}
