import apiClient from './api.client'
import type { Facility } from '@/types/property'

export const facilityService = {
  async getAllFacilities(): Promise<Facility[]> {
    const response = await apiClient.get<Facility[]>('/facilities')
    return response.data
  },
}

export default facilityService
