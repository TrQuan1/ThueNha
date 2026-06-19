import apiClient from './api.client'
import type { Property, PropertyFilterParams, CreatePropertyRequest } from '@/types/property'

export const propertyService = {
  async getProperties(params?: PropertyFilterParams): Promise<Property[]> {
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

  async updateProperty(id: string | number, data: CreatePropertyRequest): Promise<Property> {
    const response = await apiClient.put<Property>(`/properties/${id}`, data)
    return response.data
  },

  async deleteProperty(id: string | number): Promise<void> {
    await apiClient.delete(`/properties/${id}`)
  },

  async getMyProperties(): Promise<Property[]> {
    const response = await apiClient.get<Property[]>('/properties/my-properties')
    return response.data
  },

  // --- ADMIN API ---
  async getPendingProperties(): Promise<Property[]> {
    const response = await apiClient.get<Property[]>('/admin/properties/pending')
    return response.data
  },

  async approveProperty(id: string | number): Promise<void> {
    await apiClient.put(`/admin/properties/${id}/approve`)
  },

  async rejectProperty(id: string | number, reason: string): Promise<void> {
    await apiClient.put(`/admin/properties/${id}/reject`, { reason })
  },

  // ---------------------------------

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

  async getBookedDates(id: number | string): Promise<string[]> {
    const response = await apiClient.get<string[]>(`/properties/${id}/booked-dates`)
    return response.data
  },
}

export default propertyService
