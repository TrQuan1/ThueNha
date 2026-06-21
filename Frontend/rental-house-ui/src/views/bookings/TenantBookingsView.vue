<template>
  <div class="max-w-7xl mx-auto py-8 px-4 sm:px-6 lg:px-8">
    <h1 class="text-3xl font-extrabold text-gray-900 mb-8">Chuyến đi của tôi</h1>

    <div v-if="isLoading" class="text-center py-12 text-gray-500 font-medium">
      <p class="text-lg">Đang tải danh sách chuyến đi...</p>
    </div>

    <div v-else-if="error" class="text-center py-12 text-red-500 font-medium">
      <p class="text-lg">{{ error }}</p>
    </div>

    <div
      v-else-if="bookings.length === 0"
      class="text-center py-16 bg-white rounded-2xl shadow-sm border border-gray-200"
    >
      <span class="text-5xl block mb-4">🧳</span>
      <h3 class="text-xl font-bold text-gray-900 mb-2">Chưa có chuyến đi nào</h3>
      <p class="text-gray-500">
        Đã đến lúc phủ bụi vali và bắt đầu cuộc hành trình tiếp theo của bạn.
      </p>
    </div>

    <div v-else class="grid grid-cols-1 md:grid-cols-2 xl:grid-cols-3 gap-6">
      <div
        v-for="booking in bookings"
        :key="booking.id"
        class="bg-white rounded-2xl shadow-sm border border-gray-200 overflow-hidden flex flex-col transition hover:shadow-md"
      >
        <div class="p-6 grow space-y-4">
          <div class="flex justify-between items-start">
            <span class="text-xs font-bold text-gray-400 uppercase tracking-wider">
              Mã: #{{ String(booking.id).substring(0, 8) }}...
            </span>
            <span
              :class="[
                'px-3 py-1 text-xs font-bold rounded-full border',
                getStatusClass(booking.status),
              ]"
            >
              {{ getStatusText(booking.status) }}
            </span>
          </div>

          <router-link :to="`/properties/${booking.propertyId}`" class="block group">
            <h3
              class="text-xl font-bold text-gray-900 truncate group-hover:text-blue-600 transition cursor-pointer"
              title="Bấm để xem lại chi tiết căn nhà này"
            >
              {{ booking.propertyTitle || `Căn nhà #${booking.propertyId}` }}
            </h3>
          </router-link>

          <div class="grid grid-cols-2 gap-4 pt-2">
            <div class="bg-gray-50 p-3 rounded-xl border border-gray-100">
              <p class="text-xs uppercase text-gray-500 font-semibold mb-1">Nhận phòng</p>
              <p class="text-sm font-bold text-gray-900">{{ formatDate(booking.checkInDate) }}</p>
            </div>
            <div class="bg-gray-50 p-3 rounded-xl border border-gray-100">
              <p class="text-xs uppercase text-gray-500 font-semibold mb-1">Trả phòng</p>
              <p class="text-sm font-bold text-gray-900">{{ formatDate(booking.checkOutDate) }}</p>
            </div>
          </div>
        </div>

        <div
          class="bg-gray-50 p-6 border-t border-gray-100 flex justify-between items-center mt-auto"
        >
          <div>
            <p class="text-xs text-gray-500 font-bold uppercase mb-1">Tổng chi phí</p>
            <p class="text-lg font-extrabold text-blue-600">
              {{ booking.totalPrice.toLocaleString() }} VND
            </p>
          </div>

          <div class="flex items-center gap-2">
            <button
              v-if="booking.status === 5"
              @click="openReviewModal(booking.id)"
              class="flex items-center gap-1 bg-yellow-400 hover:bg-yellow-500 text-yellow-900 px-3 py-2 rounded-lg font-bold shadow-sm transition"
            >
              <span>⭐</span> Đánh giá
            </button>

            <button
              v-if="canCancel(booking.status)"
              @click="handleCancel(booking.id)"
              :disabled="isProcessing"
              class="bg-white text-red-600 border border-red-200 hover:bg-red-500 hover:text-white px-4 py-2.5 rounded-xl font-semibold transition cursor-pointer disabled:opacity-50 disabled:cursor-not-allowed shadow-sm"
            >
              Hủy
            </button>
          </div>
        </div>
      </div>
    </div>

    <ReviewModal
      v-if="isReviewModalOpen"
      :isOpen="isReviewModalOpen"
      :bookingId="selectedBookingId || 0"
      @close="isReviewModalOpen = false"
      @success="handleReviewSuccess"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { bookingService } from '@/services/booking.service'
import type { BookingResponse } from '@/types/booking'
// 👉 [THÊM MỚI] IMPORT MODAL
import ReviewModal from '@/components/reviews/ReviewModal.vue'

const bookings = ref<BookingResponse[]>([])
const isLoading = ref(true)
const error = ref<string | null>(null)
const isProcessing = ref(false)

// 👉 [THÊM MỚI] STATE CHO MODAL
const isReviewModalOpen = ref(false)
const selectedBookingId = ref<number | null>(null)

// 👉 [THÊM MỚI] HÀM MỞ MODAL
const openReviewModal = (bookingId: number) => {
  selectedBookingId.value = bookingId
  isReviewModalOpen.value = true
}

// 👉 [THÊM MỚI] HÀM XỬ LÝ KHI ĐÁNH GIÁ THÀNH CÔNG
const handleReviewSuccess = () => {
  isReviewModalOpen.value = false
  // Có thể gọi lại fetchBookings() nếu muốn cập nhật thông tin gì đó trên thẻ Booking (VD: đánh dấu là đã review)
  // fetchBookings()
}

const fetchBookings = async () => {
  isLoading.value = true
  error.value = null
  try {
    bookings.value = await bookingService.getTenantBookings()
  } catch (err: unknown) {
    const errorObj = err as { response?: { data?: { message?: string } } }
    error.value = errorObj.response?.data?.message || 'Không thể tải danh sách chuyến đi của bạn.'
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

// 👉 BỔ SUNG TRẠNG THÁI COMPLETED VÀO HÀM (GIẢ SỬ LÀ SỐ 5)
const getStatusText = (status: string | number) => {
  if (status === 1 || status === '1' || status === 'Pending') return 'Đang chờ duyệt'
  if (status === 2 || status === '2' || status === 'Approved') return 'Đã xác nhận'
  if (status === 3 || status === '3' || status === 'Rejected') return 'Bị từ chối'
  if (status === 4 || status === '4' || status === 'Cancelled') return 'Đã hủy'
  if (status === 5 || status === '5' || status === 'Completed') return 'Đã hoàn thành'
  return 'Không xác định'
}

const getStatusClass = (status: string | number) => {
  if (status === 1 || status === '1' || status === 'Pending')
    return 'bg-orange-50 text-orange-700 border-orange-200'
  if (status === 2 || status === '2' || status === 'Approved')
    return 'bg-green-50 text-green-700 border-green-200'
  if (status === 3 || status === '3' || status === 'Rejected')
    return 'bg-red-50 text-red-700 border-red-200'
  if (status === 4 || status === '4' || status === 'Cancelled')
    return 'bg-gray-100 text-gray-600 border-gray-200'
  if (status === 5 || status === '5' || status === 'Completed')
    return 'bg-blue-50 text-blue-700 border-blue-200'
  return 'bg-gray-100 text-gray-600 border-gray-200'
}

const canCancel = (status: string | number) => {
  return (
    status === 1 ||
    status === '1' ||
    status === 'Pending' ||
    status === 2 ||
    status === '2' ||
    status === 'Approved'
  )
}

const handleCancel = async (id: string | number) => {
  if (!confirm('Bạn có chắc chắn muốn HỦY chuyến đi này? Thao tác này không thể hoàn tác.')) return

  isProcessing.value = true
  try {
    await bookingService.cancelBooking(id)
    alert('Đã hủy chuyến đi thành công!')
    await fetchBookings()
  } catch (err: unknown) {
    const errorObj = err as { response?: { data?: { message?: string } } }
    alert(
      errorObj.response?.data?.message || 'Có lỗi xảy ra khi hủy chuyến đi. Vui lòng thử lại sau.',
    )
  } finally {
    isProcessing.value = false
  }
}
</script>
