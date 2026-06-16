import apiClient from './api.client'
import type { Property, PropertyFilterParams, CreatePropertyRequest } from '@/types/property'

export const propertyService = {
  // Thay thế getAllProperties bằng getProperties có hỗ trợ filter
  async getProperties(params?: PropertyFilterParams): Promise<Property[]> {
    // Endpoint phụ thuộc vào cấu hình route của Controller C#, thường là /Properties hoặc /Property
    const response = await apiClient.get<Property[]>('/properties', { params })
    return response.data
  },

  async getPropertyById(id: string | number): Promise<Property> {
    const response = await apiClient.get<Property>(`/properties/${id}`)
    return response.data
  },

  async createProperty(data: CreatePropertyRequest): Promise<Property> {
    const response = await apiClient.post<Property>('/properties', data)
    return response.data
  },

  async uploadImages(propertyId: number | string, files: File[]): Promise<unknown> {
    const formData = new FormData()
    files.forEach((file) => {
      formData.append('files', file)
    })

    const response = await apiClient.post<unknown>(`/properties/${propertyId}/images`, formData, {
      headers: {
        'Content-Type': 'multipart/form-data',
      },
    })
    return response.data
  },
}

export default propertyService
