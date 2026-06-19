import apiClient from './api.client'

export interface DailyRevenue {
  date: string
  revenue: number
  bookingCount: number
}

export interface HostDashboard {
  totalProperties: number
  totalGuests: number
  totalRevenue: number
  revenueChart: DailyRevenue[]
}

export const dashboardService = {
  async getHostDashboard(): Promise<HostDashboard> {
    const response = await apiClient.get<HostDashboard>('/dashboard/host')
    return response.data
  },
}
