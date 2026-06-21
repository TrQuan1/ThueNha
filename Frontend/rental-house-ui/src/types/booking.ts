export interface CreateBookingRequest {
  propertyId: number | string // Có thể là number hoặc string tùy vào cấu trúc ID thực tế của Backend
  checkInDate: string // Định dạng YYYY-MM-DD
  checkOutDate: string
  numberOfGuests: number
  totalPrice: number
}

export interface BookingResponse {
  id: number
  propertyId: number
  tenantId: number
  checkInDate: string
  checkOutDate: string
  totalPrice: number
  status: number
  propertyTitle?: string
}
