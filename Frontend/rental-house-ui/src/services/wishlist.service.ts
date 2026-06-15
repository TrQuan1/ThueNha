import apiClient from './api.client'
import type { Property } from '@/types/property'

export const wishlistService = {
  async addWishlist(propertyId: number | string): Promise<unknown> {
    const response = await apiClient.post(`/Wishlist/${propertyId}`)
    return response.data
  },

  async removeWishlist(propertyId: number | string): Promise<unknown> {
    const response = await apiClient.delete(`/Wishlist/${propertyId}`)
    return response.data
  },

  async getMyWishlist(): Promise<Property[]> {
    const response = await apiClient.get<Property[]>('/Wishlist')
    return response.data
  },
}

export default wishlistService
