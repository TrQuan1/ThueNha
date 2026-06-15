// src/services/booking.service.ts
import apiClient from './api.client'
import type { CreateBookingRequest, BookingResponse } from '../types/booking'

export const bookingService = {
  async createBooking(payload: CreateBookingRequest): Promise<BookingResponse> {
    const response = await apiClient.post<BookingResponse>('/Booking', payload)
    return response.data
  },

  async getHostBookings(): Promise<BookingResponse[]> {
    const response = await apiClient.get<BookingResponse[] | { items: BookingResponse[] }>(
      '/Booking/host-requests',
    )
    const data = response.data
    return 'items' in data ? data.items : data
  },

  async approveBooking(id: number): Promise<{ message: string }> {
    const response = await apiClient.put<{ message: string }>(`/Booking/${id}/approve`)
    return response.data
  },

  async rejectBooking(id: number): Promise<{ message: string }> {
    const response = await apiClient.put<{ message: string }>(`/Booking/${id}/reject`)
    return response.data
  },
}
