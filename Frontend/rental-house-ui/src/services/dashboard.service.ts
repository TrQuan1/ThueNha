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

export interface AdminDashboard {
  totalUsers: number
  totalProperties: number
  totalBookings: number
  totalGMV: number
  revenueChart: DailyRevenue[]
}

export const dashboardService = {
  async getHostDashboard(): Promise<HostDashboard> {
    const response = await apiClient.get<HostDashboard>('/dashboard/host')
    return response.data
  },

  async getAdminDashboard(month?: number, year?: number): Promise<AdminDashboard> {
    const params = new URLSearchParams()
    if (month) params.append('month', month.toString())
    if (year) params.append('year', year.toString())

    const response = await apiClient.get<AdminDashboard>(`/dashboard/admin?${params.toString()}`)
    return response.data
  },
}
