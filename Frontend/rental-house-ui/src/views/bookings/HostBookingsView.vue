<template>
  <div class="max-w-7xl mx-auto py-8 px-4 sm:px-6 lg:px-8">
    <h1 class="text-3xl font-extrabold text-gray-900 mb-8">Quản lý yêu cầu đặt phòng</h1>

    <div v-if="isLoading" class="text-center py-10 text-gray-500 font-medium">
      Đang tải dữ liệu...
    </div>

    <div v-else-if="error" class="text-center py-10 text-red-500 font-medium">
      {{ error }}
    </div>

    <div v-else-if="bookings.length === 0" class="text-center py-10 text-gray-500 italic">
      Hiện tại chưa có yêu cầu đặt phòng nào.
    </div>

    <div v-else class="overflow-x-auto bg-white rounded-2xl shadow-sm border border-gray-200">
      <table class="min-w-full divide-y divide-gray-200">
        <thead class="bg-gray-50">
          <tr>
            <th
              scope="col"
              class="px-6 py-4 text-left text-xs font-bold text-gray-500 uppercase tracking-wider"
            >
              Mã Nhà
            </th>
            <th
              scope="col"
              class="px-6 py-4 text-left text-xs font-bold text-gray-500 uppercase tracking-wider"
            >
              Nhận phòng
            </th>
            <th
              scope="col"
              class="px-6 py-4 text-left text-xs font-bold text-gray-500 uppercase tracking-wider"
            >
              Trả phòng
            </th>
            <th
              scope="col"
              class="px-6 py-4 text-left text-xs font-bold text-gray-500 uppercase tracking-wider"
            >
              Tổng tiền
            </th>
            <th
              scope="col"
              class="px-6 py-4 text-left text-xs font-bold text-gray-500 uppercase tracking-wider"
            >
              Trạng thái
            </th>
            <th
              scope="col"
              class="px-6 py-4 text-center text-xs font-bold text-gray-500 uppercase tracking-wider"
            >
              Hành động
            </th>
          </tr>
        </thead>
        <tbody class="bg-white divide-y divide-gray-200">
          <tr v-for="booking in bookings" :key="booking.id" class="hover:bg-gray-50 transition">
            <td class="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">
              {{ booking.propertyId }}
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-600">
              {{ formatDate(booking.checkInDate) }}
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-600">
              {{ formatDate(booking.checkOutDate) }}
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm font-bold text-blue-600">
              {{ booking.totalPrice.toLocaleString() }} VND
            </td>
            <td class="px-6 py-4 whitespace-nowrap">
              <span :class="getStatusClass(booking.status)">
                {{ getStatusText(booking.status) }}
              </span>
            </td>
            <td
              class="px-6 py-4 whitespace-nowrap text-center text-sm font-medium flex justify-center gap-2"
            >
              <template v-if="booking.status === 1">
                <button
                  @click="handleApprove(booking.id)"
                  :disabled="isProcessing"
                  class="bg-green-500 hover:bg-green-600 text-white px-4 py-2 rounded-lg font-semibold transition disabled:opacity-50 disabled:cursor-not-allowed cursor-pointer shadow-sm"
                >
                  Duyệt
                </button>
                <button
                  @click="handleReject(booking.id)"
                  :disabled="isProcessing"
                  class="bg-red-500 hover:bg-red-600 text-white px-4 py-2 rounded-lg font-semibold transition disabled:opacity-50 disabled:cursor-not-allowed cursor-pointer shadow-sm"
                >
                  Từ chối
                </button>
              </template>

              <template v-else-if="booking.status === 2">
                <button
                  @click="handleCancelByHost(booking.id)"
                  :disabled="isProcessing"
                  class="bg-gray-500 hover:bg-gray-600 text-white px-4 py-2 rounded-lg font-semibold transition disabled:opacity-50 disabled:cursor-not-allowed cursor-pointer shadow-sm"
                >
                  Hủy
                </button>
              </template>
              <template v-else>
                <span class="text-gray-400 italic">Không khả dụng</span>
              </template>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { bookingService } from '@/services/booking.service'
import type { BookingResponse } from '@/types/booking'

const bookings = ref<BookingResponse[]>([])
const isLoading = ref(true)
const error = ref<string | null>(null)
const isProcessing = ref(false)

const fetchBookings = async () => {
  isLoading.value = true
  error.value = null
  try {
    bookings.value = await bookingService.getHostBookings()
  } catch (err: unknown) {
    const errorObj = err as { response?: { data?: { message?: string } } }
    error.value = errorObj.response?.data?.message || 'Không thể tải danh sách yêu cầu đặt phòng.'
  } finally {
    isLoading.value = false
  }
}

onMounted(() => {
  fetchBookings()
})

const formatDate = (dateString: string) => {
  if (!dateString) return ''
  return new Date(dateString).toLocaleDateString('vi-VN')
}

// Bỏ kiểm tra string, nhận chuẩn number
const getStatusText = (status: number) => {
  if (status === 1) return 'Chờ duyệt'
  if (status === 2) return 'Đã duyệt'
  if (status === 3) return 'Đã từ chối'
  if (status === 4) return 'Đã hủy'
  if (status === 5) return 'Hoàn thành'
  return 'Không xác định'
}

// Bỏ kiểm tra string, nhận chuẩn number
const getStatusClass = (status: number) => {
  const baseClass = 'px-3 py-1 inline-flex text-xs leading-5 font-semibold rounded-full '
  if (status === 1) return baseClass + 'bg-yellow-100 text-yellow-800'
  if (status === 2) return baseClass + 'bg-green-100 text-green-800'
  if (status === 3) return baseClass + 'bg-red-100 text-red-800'
  if (status === 4) return baseClass + 'bg-gray-100 text-gray-800'
  if (status === 5) return baseClass + 'bg-blue-100 text-blue-800'
  return baseClass + 'bg-gray-100 text-gray-800'
}

// id là number
const handleApprove = async (id: number) => {
  if (!confirm('Bạn có chắc chắn muốn DUYỆT yêu cầu này?')) return
  isProcessing.value = true
  try {
    await bookingService.approveBooking(id)
    alert('Đã duyệt yêu cầu đặt phòng thành công!')
    await fetchBookings()
  } catch (err: unknown) {
    const errorObj = err as { response?: { data?: { message?: string } } }
    alert(errorObj.response?.data?.message || 'Có lỗi xảy ra khi duyệt.')
  } finally {
    isProcessing.value = false
  }
}

// id là number
const handleReject = async (id: number) => {
  if (!confirm('Bạn có chắc chắn muốn TỪ CHỐI yêu cầu này?')) return
  isProcessing.value = true
  try {
    await bookingService.rejectBooking(id)
    alert('Đã từ chối yêu cầu đặt phòng!')
    await fetchBookings()
  } catch (err: unknown) {
    const errorObj = err as { response?: { data?: { message?: string } } }
    alert(errorObj.response?.data?.message || 'Có lỗi xảy ra khi từ chối.')
  } finally {
    isProcessing.value = false
  }
}

const handleCancelByHost = async (id: number) => {
  if (
    !confirm(
      'Khách không đến nhận phòng (No-show)? Bạn có chắc chắn muốn HỦY đơn này? Doanh thu dự kiến sẽ bị trừ đi.',
    )
  )
    return
  isProcessing.value = true
  try {
    await bookingService.cancelBookingByHost(id)
    alert('Đã hủy đơn đặt phòng thành công!')
    await fetchBookings() // Load lại danh sách
  } catch (err: unknown) {
    const errorObj = err as { response?: { data?: { message?: string } } }
    alert(errorObj.response?.data?.message || 'Có lỗi xảy ra khi hủy.')
  } finally {
    isProcessing.value = false
  }
}
</script>
