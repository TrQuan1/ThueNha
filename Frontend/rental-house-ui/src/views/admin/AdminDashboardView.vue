<template>
  <div class="max-w-7xl mx-auto py-8 px-4 sm:px-6 lg:px-8 bg-gray-50 min-h-screen">
    <div class="flex flex-col sm:flex-row justify-between items-start sm:items-center mb-8 gap-4">
      <h1 class="text-3xl font-extrabold text-gray-900">Thống kê & Giao dịch toàn sàn</h1>

      <div class="flex items-center gap-3 bg-white p-2 rounded-xl shadow-sm border border-gray-200">
        <span class="text-sm font-medium text-gray-500 ml-2">🕒 Kỳ soát:</span>
        <select
          v-model="selectedMonth"
          @change="fetchDashboardData"
          class="bg-gray-50 border border-gray-300 text-gray-900 text-sm rounded-lg focus:ring-blue-500 focus:border-blue-500 block px-3 py-2 outline-none cursor-pointer"
        >
          <option v-for="m in 12" :key="m" :value="m">Tháng {{ m }}</option>
        </select>

        <select
          v-model="selectedYear"
          @change="fetchDashboardData"
          class="bg-gray-50 border border-gray-300 text-gray-900 text-sm rounded-lg focus:ring-blue-500 focus:border-blue-500 block px-3 py-2 outline-none cursor-pointer"
        >
          <option :value="2025">Năm 2025</option>
          <option :value="2026">Năm 2026</option>
        </select>
      </div>
    </div>

    <div v-if="isLoading" class="flex justify-center items-center h-64">
      <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600"></div>
    </div>

    <div v-else>
      <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
        <div
          class="bg-white rounded-2xl shadow-sm p-6 border border-gray-100 flex items-center gap-5 transition hover:shadow-md"
        >
          <div
            class="flex items-center justify-center w-14 h-14 bg-indigo-50 text-indigo-600 rounded-xl text-3xl"
          >
            🌍
          </div>
          <div>
            <p class="text-sm font-bold text-gray-500 uppercase tracking-wide">Người dùng</p>
            <p class="text-3xl font-extrabold text-gray-900 mt-1">
              {{ dashboardData.totalUsers }}
            </p>
          </div>
        </div>

        <div
          class="bg-white rounded-2xl shadow-sm p-6 border border-gray-100 flex items-center gap-5 transition hover:shadow-md"
        >
          <div
            class="flex items-center justify-center w-14 h-14 bg-blue-50 text-blue-600 rounded-xl text-3xl"
          >
            🏠
          </div>
          <div>
            <p class="text-sm font-bold text-gray-500 uppercase tracking-wide">Tổng số nhà</p>
            <p class="text-3xl font-extrabold text-gray-900 mt-1">
              {{ dashboardData.totalProperties }}
            </p>
          </div>
        </div>

        <div
          class="bg-white rounded-2xl shadow-sm p-6 border border-gray-100 flex items-center gap-5 transition hover:shadow-md"
        >
          <div
            class="flex items-center justify-center w-14 h-14 bg-green-50 text-green-600 rounded-xl text-3xl"
          >
            📦
          </div>
          <div>
            <p class="text-sm font-bold text-gray-500 uppercase tracking-wide">Đơn đã duyệt</p>
            <p class="text-3xl font-extrabold text-gray-900 mt-1">
              {{ dashboardData.totalBookings }}
            </p>
          </div>
        </div>

        <div
          class="bg-white rounded-2xl shadow-sm p-6 border border-gray-100 flex items-center gap-5 transition hover:shadow-md"
        >
          <div
            class="flex items-center justify-center w-14 h-14 bg-yellow-50 text-yellow-600 rounded-xl text-3xl"
          >
            💎
          </div>
          <div>
            <p class="text-sm font-bold text-gray-500 uppercase tracking-wide">GMV Toàn sàn</p>
            <p class="text-2xl font-extrabold text-blue-600 mt-1">
              {{ formatCurrency(dashboardData.totalGMV) }}
            </p>
          </div>
        </div>
      </div>

      <div class="bg-white rounded-2xl shadow-sm p-6 border border-gray-100">
        <h2 class="text-xl font-bold text-gray-900 mb-6">
          Biểu đồ Tổng GMV - Tháng {{ selectedMonth }}/{{ selectedYear }}
        </h2>

        <div v-if="dashboardData.revenueChart && dashboardData.revenueChart.length > 0">
          <VueApexCharts
            type="bar"
            height="380"
            :options="chartOptions"
            :series="series"
          ></VueApexCharts>
        </div>

        <div
          v-else
          class="flex flex-col items-center justify-center py-16 text-gray-500 bg-gray-50 rounded-xl border border-dashed border-gray-200"
        >
          <span class="text-5xl mb-4 opacity-50">📊</span>
          <p class="text-lg font-medium text-gray-600">Không có dữ liệu giao dịch</p>
          <p class="text-sm">
            Chưa có lượt đặt phòng nào được duyệt trên toàn hệ thống trong Tháng
            {{ selectedMonth }}/{{ selectedYear }}.
          </p>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import VueApexCharts from 'vue3-apexcharts'
import type { ApexOptions } from 'apexcharts'
import apiClient from '@/services/api.client'

interface DailyRevenue {
  date: string
  revenue: number
  bookingCount: number
}

// Data model cho Admin (khác Host ở chỗ có Users, Bookings và GMV)
interface AdminDashboardData {
  totalUsers: number
  totalProperties: number
  totalBookings: number
  totalGMV: number
  revenueChart: DailyRevenue[]
}

// State
const isLoading = ref(true)
const selectedMonth = ref(new Date().getMonth() + 1)
const selectedYear = ref(new Date().getFullYear())

const dashboardData = ref<AdminDashboardData>({
  totalUsers: 0,
  totalProperties: 0,
  totalBookings: 0,
  totalGMV: 0,
  revenueChart: [],
})

const series = ref([{ name: 'GMV (VNĐ)', data: [] as number[] }])

const chartOptions = ref<ApexOptions>({
  chart: {
    type: 'bar',
    fontFamily: 'Inter, ui-sans-serif, system-ui, sans-serif',
    toolbar: { show: false },
  },
  // Đổi sang màu Xanh Ngọc/Xanh Lục bảo để phân biệt với màn Host
  colors: ['#0ea5e9'],
  plotOptions: { bar: { borderRadius: 6, columnWidth: '40%' } },
  dataLabels: { enabled: false },
  xaxis: { categories: [] as string[], labels: { style: { colors: '#6B7280', fontWeight: 500 } } },
  yaxis: {
    labels: {
      style: { colors: '#6B7280', fontWeight: 500 },
      formatter: (value) => new Intl.NumberFormat('vi-VN').format(value) + ' đ',
    },
  },
  tooltip: {
    theme: 'light',
    y: { formatter: (value) => new Intl.NumberFormat('vi-VN').format(value) + ' VNĐ' },
  },
  grid: { borderColor: '#F3F4F6', strokeDashArray: 4 },
})

const formatCurrency = (value: number) => {
  return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(value)
}

// Gọi API của ADMIN
const fetchDashboardData = async () => {
  isLoading.value = true
  try {
    const response = await apiClient.get('/dashboard/admin', {
      params: { month: selectedMonth.value, year: selectedYear.value },
    })
    const data = response.data
    dashboardData.value = data

    if (data.revenueChart && data.revenueChart.length > 0) {
      series.value = [
        {
          name: 'GMV (VNĐ)',
          data: data.revenueChart.map((item: DailyRevenue) => item.revenue),
        },
      ]

      chartOptions.value = {
        ...chartOptions.value,
        xaxis: {
          categories: data.revenueChart.map((item: DailyRevenue) => {
            const d = new Date(item.date)
            return `${String(d.getDate()).padStart(2, '0')}/${String(d.getMonth() + 1).padStart(2, '0')}`
          }),
        },
      }
    } else {
      series.value = [{ name: 'GMV (VNĐ)', data: [] }]
      chartOptions.value = { ...chartOptions.value, xaxis: { categories: [] } }
    }
  } catch (error) {
    console.error('Lỗi tải Admin Dashboard:', error)
  } finally {
    isLoading.value = false
  }
}

onMounted(() => {
  fetchDashboardData()
})
</script>
