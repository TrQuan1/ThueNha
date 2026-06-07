import apiClient from './api.client'
import type { Property } from '@/types/Property'

const fallbackPropertyImage =
  'https://images.unsplash.com/photo-1564013799919-ab600027ffc6?auto=format&fit=crop&w=1200&q=80'

type PropertyImageApiResponse = {
  imageUrl?: string
  ImageUrl?: string
  url?: string
  Url?: string
}

type PropertyApiResponse = {
  id?: number
  Id?: number
  title?: string
  Title?: string
  address?: string
  Address?: string
  price?: number
  Price?: number
  pricePerNight?: number
  PricePerNight?: number
  imageUrl?: string
  ImageUrl?: string
  images?: PropertyImageApiResponse[]
  Images?: PropertyImageApiResponse[]
}

type PropertiesPayload =
  | PropertyApiResponse[]
  | {
      data?: PropertyApiResponse[]
      items?: PropertyApiResponse[]
    }

const normalizeImageUrl = (imageUrl?: string): string => {
  if (!imageUrl) return fallbackPropertyImage
  if (/^(https?:|data:)/i.test(imageUrl)) return imageUrl

  const baseUrl = apiClient.defaults.baseURL?.replace(/\/$/, '') ?? ''
  return imageUrl.startsWith('/') ? `${baseUrl}${imageUrl}` : imageUrl
}

const getFirstImageUrl = (property: PropertyApiResponse): string | undefined => {
  const images = property.images ?? property.Images ?? []
  const firstImage = images[0]

  return (
    property.imageUrl ??
    property.ImageUrl ??
    firstImage?.imageUrl ??
    firstImage?.ImageUrl ??
    firstImage?.url ??
    firstImage?.Url
  )
}

const mapToProperty = (property: PropertyApiResponse): Property => ({
  id: property.id ?? property.Id ?? 0,
  title: property.title ?? property.Title ?? 'Nhà cho thuê',
  price: property.price ?? property.Price ?? property.pricePerNight ?? property.PricePerNight ?? 0,
  address: property.address ?? property.Address ?? 'Đang cập nhật địa chỉ',
  imageUrl: normalizeImageUrl(getFirstImageUrl(property)),
})

export const getProperties = async (): Promise<Property[]> => {
  const response = await apiClient.get<PropertiesPayload, PropertiesPayload>('/api/properties')
  const properties = Array.isArray(response) ? response : response.data ?? response.items ?? []

  return properties.map(mapToProperty)
}
