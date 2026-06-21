import apiClient from './api.client'

export interface ReviewDto {
  id: number
  bookingId: number
  tenantId: number
  tenantName: string
  rating: number
  comment: string
  createdAt: string
}

export interface CreateReviewPayload {
  bookingId: number
  rating: number
  comment: string
}

export interface UpdateReviewPayload {
  rating: number
  comment: string
}

export const reviewService = {
  // Lấy danh sách đánh giá của một căn nhà
  async getReviewsByProperty(propertyId: number | string): Promise<ReviewDto[]> {
    const response = await apiClient.get<ReviewDto[]>(`/properties/${propertyId}/reviews`)
    return response.data
  },

  // Tạo mới đánh giá cho một chuyến đi (Booking)
  async createReview(data: CreateReviewPayload): Promise<ReviewDto> {
    const response = await apiClient.post<ReviewDto>('/reviews', data)
    return response.data
  },

  // Cập nhật đánh giá
  async updateReview(id: number | string, data: UpdateReviewPayload): Promise<ReviewDto> {
    const response = await apiClient.put<ReviewDto>(`/reviews/${id}`, data)
    return response.data
  },

  // Xóa đánh giá
  async deleteReview(id: number | string): Promise<void> {
    await apiClient.delete(`/reviews/${id}`)
  },
}
