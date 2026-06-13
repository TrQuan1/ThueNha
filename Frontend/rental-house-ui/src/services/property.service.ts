import apiClient from './api.client'
import type { Property } from '@/types/property'

const propertyService = {
  // Hàm lấy danh sách nhà từ API
  async getAllProperties() {
    // Đảm bảo gọi đúng endpoint '/properties' có chữ 's'
    return await apiClient.get<Property[]>('/properties')
  },
  // BỔ SUNG HÀM NÀY: Lấy chi tiết 1 căn nhà theo ID
  async getPropertyById(id: string | number) {
    return await apiClient.get<Property>(`/properties/${id}`)
  },
}

export default propertyService
