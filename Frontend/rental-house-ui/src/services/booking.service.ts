import apiClient from './api.client'
import type { CreateBookingRequest, BookingResponse } from '../types/booking'

export const bookingService = {
  async createBooking(payload: CreateBookingRequest): Promise<BookingResponse> {
    const response = await apiClient.post<BookingResponse>('/Booking', payload)
    return response.data
  },

  async getHostBookings(): Promise<BookingResponse[]> {
    const response = await apiClient.get<unknown>('/Booking/host-requests')
    const data = response.data as { items?: BookingResponse[] } | BookingResponse[]
    if (data && typeof data === 'object' && 'items' in data && Array.isArray(data.items)) {
      return data.items
    }
    if (Array.isArray(data)) {
      return data
    }
    return []
  },

  async approveBooking(id: number | string): Promise<unknown> {
    const response = await apiClient.put(`/Booking/${id}/approve`)
    return response.data
  },

  async rejectBooking(id: number | string): Promise<unknown> {
    const response = await apiClient.put(`/Booking/${id}/reject`)
    return response.data
  },

  // THÊM MỚI: API dành cho Tenant (Người thuê)
  async getTenantBookings(): Promise<BookingResponse[]> {
    const response = await apiClient.get<unknown>('/Booking/my-bookings')
    // Xử lý an toàn cho cả 2 trường hợp: Backend trả về PagedResult (có .items) hoặc mảng trực tiếp
    const data = response.data as { items?: BookingResponse[] } | BookingResponse[]
    if (data && typeof data === 'object' && 'items' in data && Array.isArray(data.items)) {
      return data.items
    }
    if (Array.isArray(data)) {
      return data
    }
    return []
  },

  async cancelBooking(id: number | string): Promise<{ message: string }> {
    const response = await apiClient.put<{ message: string }>(`/Booking/${id}/cancel`)
    return response.data
  },
}
