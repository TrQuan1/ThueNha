export interface Property {
  id: number // Đổi thành number thay vì string
  hostId: number
  title: string
  description: string
  address: string
  pricePerNight: number // Ở FE lúc nãy gọi là price, giờ phải đổi thành pricePerNight
  maxGuests: number // Số khách tối đa (thay cho area)

  // Các trường này Backend C# của bạn chưa có, tạm thời để tùy chọn (?)
  status?: string
  imageUrl?: string
}
