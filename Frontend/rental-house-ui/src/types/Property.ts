export interface Property {
  id: string | number
  title: string
  description: string
  address: string
  pricePerNight: number
  maxGuests: number
  imageUrl?: string
}

export interface PropertyFilterParams {
  searchTerm?: string
  minPrice?: number
  maxPrice?: number
}
