import apiClient from './api.client'
import type { CreateBookingRequest, BookingResponse } from '../types/booking'

export const bookingService = {
  async createBooking(payload: CreateBookingRequest): Promise<BookingResponse> {
    const response = await apiClient.post<BookingResponse>('/Booking', payload)
    return response.data
  },
}
