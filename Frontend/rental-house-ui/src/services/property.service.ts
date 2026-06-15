import apiClient from './api.client'
import type { Property } from '@/types/property'

export interface CreatePropertyRequest {
  title: string
  description: string
  address: string
  pricePerNight: number
  maxGuests: number
}

export const propertyService = {
  async getAllProperties(): Promise<Property[]> {
    const response = await apiClient.get<Property[]>('/properties')
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

    // Lặp qua mảng files và append từng file với key là 'files'
    files.forEach((file) => {
      formData.append('files', file)
    })

    // Gửi request POST kèm theo config header multipart/form-data
    const response = await apiClient.post<unknown>(`/properties/${propertyId}/images`, formData, {
      headers: {
        'Content-Type': 'multipart/form-data',
      },
    })

    return response.data
  },
}

export default propertyService
