<template>
  <div class="max-w-7xl mx-auto py-8">
    <div v-if="isLoading" class="text-center py-20 text-gray-500">
      Đang tải thông tin chi tiết...
    </div>

    <div v-else-if="error" class="text-center py-20 text-red-500 font-medium">
      {{ error }}
    </div>

    <div v-else-if="property" class="grid grid-cols-1 lg:grid-cols-3 gap-10">
      <div class="lg:col-span-2 space-y-8">
        <div>
          <h1 class="text-3xl font-extrabold text-gray-900">{{ property.title }}</h1>
          <p class="text-gray-600 mt-2">📍 {{ property.address }}</p>
        </div>

        <div class="w-full h-100 overflow-hidden rounded-2xl bg-gray-200">
          <img
            :src="getFullImageUrl(property.imageUrl)"
            :alt="property.title"
            class="w-full h-full object-cover"
            @error="
              (e) =>
                ((e.target as HTMLImageElement).src = 'https://placehold.co/800x400?text=No+Image')
            "
          />
        </div>

        <div class="prose max-w-none text-gray-700">
          <h3 class="text-xl font-bold text-gray-900 mb-3">Mô tả</h3>
          <p class="whitespace-pre-line">{{ property.description }}</p>
        </div>
      </div>

      <div
        v-if="property.facilities && property.facilities.length > 0"
        class="mt-10 pt-8 border-t border-gray-200"
      >
        <h3 class="text-2xl font-bold text-gray-900 mb-6">Nơi này có những gì cho bạn</h3>
        <div class="grid grid-cols-1 sm:grid-cols-2 gap-y-5 gap-x-8">
          <div
            v-for="facility in property.facilities"
            :key="facility.id"
            class="flex items-center gap-4 text-gray-700"
          >
            <div class="w-8 h-8 flex items-center justify-center text-gray-600">
              <span
                v-if="facility.icon"
                v-html="facility.icon"
                class="w-full h-full flex items-center justify-center"
              ></span>
              <span v-else class="text-xl">✨</span>
            </div>
            <span class="text-lg font-medium">{{ facility.name }}</span>
          </div>
        </div>
      </div>

      <div class="lg:col-span-1">
        <div class="sticky top-24 bg-white p-6 border border-gray-200 rounded-2xl shadow-xl">
          <h3 class="text-2xl font-bold text-gray-900 mb-4">
            {{ property.pricePerNight.toLocaleString() }} VND
            <span class="text-base font-normal text-gray-500">/ đêm</span>
          </h3>

          <div class="space-y-4 mb-6">
            <div class="flex flex-col">
              <label class="text-xs font-bold text-gray-700 uppercase mb-1">Nhận phòng</label>
              <input
                type="date"
                v-model="checkInDate"
                :min="today"
                class="w-full px-4 py-2.5 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 outline-hidden transition"
              />
            </div>

            <div class="flex flex-col">
              <label class="text-xs font-bold text-gray-700 uppercase mb-1">Trả phòng</label>
              <input
                type="date"
                v-model="checkOutDate"
                :min="checkInDate || today"
                class="w-full px-4 py-2.5 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 outline-hidden transition"
              />
            </div>

            <div class="flex flex-col">
              <label class="text-xs font-bold text-gray-700 uppercase mb-1"
                >Số khách (Tối đa: {{ property.maxGuests }})</label
              >
              <input
                type="number"
                v-model.number="guests"
                min="1"
                :max="property.maxGuests"
                class="w-full px-4 py-2.5 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 outline-hidden transition"
              />
            </div>
          </div>

          <div v-if="totalNights > 0" class="border-t border-gray-200 pt-4 mb-4 space-y-3">
            <div class="flex justify-between text-gray-600">
              <span>{{ property.pricePerNight.toLocaleString() }} VND x {{ totalNights }} đêm</span>
              <span>{{ totalPrice.toLocaleString() }} VND</span>
            </div>
            <div
              class="flex justify-between font-bold text-lg text-gray-900 border-t border-gray-200 pt-3 mt-3"
            >
              <span>Tổng cộng</span>
              <span>{{ totalPrice.toLocaleString() }} VND</span>
            </div>
          </div>

          <div
            v-if="bookingError"
            class="mb-4 p-3 bg-red-50 text-red-600 text-sm rounded-lg font-medium text-center"
          >
            {{ bookingError }}
          </div>

          <button
            @click="handleBooking"
            :disabled="isBooking || totalNights <= 0"
            class="w-full bg-blue-600 hover:bg-blue-700 text-white font-bold py-3.5 rounded-xl transition cursor-pointer shadow-md disabled:bg-gray-400 disabled:shadow-none disabled:cursor-not-allowed"
          >
            {{ isBooking ? 'Đang xử lý...' : 'Đặt phòng ngay' }}
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRoute } from 'vue-router'
import { propertyService } from '@/services/property.service'
import { bookingService } from '@/services/booking.service'
import { useAuthStore } from '@/stores/auth.store'
import type { Property } from '@/types/property'
import type { CreateBookingRequest } from '@/types/booking'

const route = useRoute()
const authStore = useAuthStore()

// Cấu hình URL Ảnh
const BACKEND_URL = import.meta.env.VITE_API_URL || 'https://localhost:7023/'
const getFullImageUrl = (relativeUrl: string | undefined) => {
  if (!relativeUrl) return ''
  return `${BACKEND_URL.replace('/api', '')}${relativeUrl}`
}

// State dữ liệu
const property = ref<Property | null>(null)
const isLoading = ref(true)
const error = ref<string | null>(null)

// Form Đặt phòng
const checkInDate = ref<string>('')
const checkOutDate = ref<string>('')
const guests = ref<number>(1)
const isBooking = ref(false)
const bookingError = ref<string | null>(null)

// Lấy ngày hôm nay theo chuẩn YYYY-MM-DD để chặn quá khứ
const today = new Date().toISOString().split('T')[0]

// Computed: Tính số đêm
const totalNights = computed(() => {
  if (!checkInDate.value || !checkOutDate.value) return 0
  const start = new Date(checkInDate.value)
  const end = new Date(checkOutDate.value)
  const diffTime = end.getTime() - start.getTime()
  const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24))
  return diffDays > 0 ? diffDays : 0
})

// Computed: Tính tổng tiền
const totalPrice = computed(() => {
  if (!property.value) return 0
  return totalNights.value * property.value.pricePerNight
})

// Lấy dữ liệu khi load trang
onMounted(async () => {
  const id = route.params.id as string
  try {
    property.value = await propertyService.getPropertyById(id)
  } catch {
    error.value = 'Không thể tải thông tin chi tiết căn nhà.'
  } finally {
    isLoading.value = false
  }
})

// Xử lý gửi Form Đặt phòng
const handleBooking = async () => {
  bookingError.value = null

  // 1. Kiểm tra đăng nhập
  if (!authStore.isAuthenticated) {
    bookingError.value = 'Vui lòng Đăng nhập để tiến hành đặt phòng.'
    return
  }

  // 2. Kiểm tra ngày tháng
  if (totalNights.value <= 0) {
    bookingError.value = 'Ngày trả phòng phải sau ngày nhận phòng.'
    return
  }

  // 3. Kiểm tra số lượng khách
  if (property.value && (guests.value < 1 || guests.value > property.value.maxGuests)) {
    bookingError.value = `Vui lòng chọn số lượng khách từ 1 đến ${property.value.maxGuests}.`
    return
  }

  isBooking.value = true
  try {
    const payload: CreateBookingRequest = {
      propertyId: property.value!.id,
      checkInDate: checkInDate.value,
      checkOutDate: checkOutDate.value,
      numberOfGuests: guests.value,
      totalPrice: totalPrice.value,
    }

    await bookingService.createBooking(payload)

    alert('🎉 Đặt phòng thành công! Chủ nhà sẽ liên hệ với bạn sớm nhất.')

    // Reset Form
    checkInDate.value = ''
    checkOutDate.value = ''
    guests.value = 1
  } catch (error: unknown) {
    // Ép kiểu an toàn (Safe Type Assertion) thay vì dùng any
    const err = error as { response?: { data?: { error?: string } } }
    const backendError = err?.response?.data?.error || 'Có lỗi xảy ra khi gọi API đặt phòng.'

    bookingError.value = backendError
    alert('❌ Không thể đặt phòng: ' + backendError)
  } finally {
    isBooking.value = false
  }
}
</script>
