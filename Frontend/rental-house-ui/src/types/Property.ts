export interface Facility {
  id: number | string
  name: string
  icon?: string
}

export interface Property {
  id: string | number
  title: string
  description: string
  address: string
  pricePerNight: number
  maxGuests: number
  imageUrl?: string
  facilities?: Facility[] // Bổ sung dòng này
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
  facilityIds?: (number | string)[] // Bổ sung dòng này
}
