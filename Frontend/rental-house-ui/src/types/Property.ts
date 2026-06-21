export interface Facility {
  id: number | string
  name: string
  icon?: string
}

export interface Property {
  id: string | number
  hostId?: string | number
  title: string
  description: string
  address: string
  pricePerNight: number
  maxGuests: number
  imageUrl?: string
  status?: number // 0: Pending, 1: Approved, 2: Rejected
  facilities?: Facility[] // Bổ sung dòng này
  averageRating?: number // Khai báo thêm Điểm trung bình
  reviewCount?: number // Khai báo thêm Số lượt đánh giá
}

export interface PropertyFilterParams {
  searchTerm?: string
  minPrice?: number
  maxPrice?: number
}

export interface CreatePropertyRequest {
  title: string
  description: string
  address: string
  pricePerNight: number
  maxGuests: number
  facilityIds: number[]
}
