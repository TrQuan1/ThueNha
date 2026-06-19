<template>
  <div class="max-w-7xl mx-auto py-8 px-4 sm:px-6 lg:px-8">
    <div v-if="isLoading" class="text-center py-20 text-gray-500">
      Đang tải thông tin chi tiết...
    </div>

    <div v-else-if="error" class="text-center py-20 text-red-500 font-medium">
      {{ error }}
    </div>

    <div v-else-if="property">
      <div class="mb-6 flex flex-col sm:flex-row justify-between items-start gap-4">
        <div>
          <h1 class="text-3xl font-extrabold text-gray-900">{{ property.title }}</h1>
          <p class="text-gray-600 mt-2 flex items-center gap-2">
            <span>📍</span> {{ property.address }}
          </p>
        </div>

        <div
          v-if="authStore.isAuthenticated && authStore.user?.userId === property.hostId"
          class="flex items-center gap-2 shrink-0"
        >
          <!-- <button
            @click="goToEdit"
            class="bg-yellow-500 hover:bg-yellow-600 text-white px-4 py-2.5 rounded-xl font-semibold shadow-sm transition cursor-pointer flex items-center gap-2"
          >
            <span>✏️</span> Sửa tin
          </button> -->
          <button
            @click="handleDelete"
            class="bg-red-500 hover:bg-red-600 text-white px-4 py-2.5 rounded-xl font-semibold shadow-sm transition cursor-pointer flex items-center gap-2"
          >
            <span>🗑️</span> Xóa tin
          </button>
        </div>
      </div>

      <div class="grid grid-cols-1 lg:grid-cols-3 gap-8">
        <div class="lg:col-span-2 space-y-8">
          <div class="w-full h-100 overflow-hidden rounded-2xl bg-gray-200 shadow-sm">
            <img
              :src="getFullImageUrl(property.imageUrl)"
              :alt="property.title"
              class="w-full h-full object-cover"
              @error="
                (e) =>
                  ((e.target as HTMLImageElement).src =
                    'https://placehold.co/800x400?text=No+Image')
              "
            />
          </div>

          <div class="bg-white p-6 rounded-2xl shadow-sm border border-gray-100">
            <h3 class="text-xl font-bold text-gray-900 mb-4">Mô tả chi tiết</h3>
            <div class="prose max-w-none text-gray-700">
              <p class="whitespace-pre-line leading-relaxed">{{ property.description }}</p>
            </div>
          </div>

          <div class="bg-white p-6 rounded-2xl shadow-sm border border-gray-100">
            <h3 class="text-xl font-bold text-gray-900 mb-4">Tiện ích khu vực</h3>

            <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
              <template v-if="property.facilities && property.facilities.length > 0">
                <div
                  v-for="facility in property.facilities"
                  :key="facility.id"
                  class="flex items-center gap-3 text-gray-700"
                >
                  <span class="text-xl flex items-center justify-center w-6">{{
                    facility.icon || facility.icon || '✨'
                  }}</span>
                  <span class="font-medium">{{ facility.name || facility.name }}</span>
                </div>
              </template>

              <template v-else>
                <div
                  class="col-span-1 sm:col-span-2 text-gray-500 italic py-2 bg-gray-50 rounded-lg text-center border border-dashed border-gray-200"
                >
                  Chủ nhà chưa cập nhật thông tin tiện ích cho căn nhà này.
                </div>
              </template>
            </div>
          </div>
        </div>

        <!-- CỘT PHẢI: XỬ LÝ LOGIC ẨN/HIỆN ĐẶT PHÒNG Ở ĐÂY -->
        <div class="lg:col-span-1">
          <div
            v-if="authStore.isAuthenticated && authStore.user?.userId === property.hostId"
            class="sticky top-24 bg-blue-50 p-6 border border-blue-200 rounded-2xl shadow-sm text-center"
          >
            <div class="text-4xl mb-4">🏠</div>
            <h3 class="text-xl font-bold text-blue-900 mb-2">Đây là nhà của bạn</h3>
            <p class="text-sm text-blue-700 mb-6">
              Bạn có thể quản lý, chỉnh sửa thông tin hoặc xóa bài đăng này.
            </p>
            <button
              @click="goToEdit"
              class="w-full bg-blue-600 hover:bg-blue-700 text-white font-bold py-3 rounded-xl transition shadow-md cursor-pointer"
            >
              Cập nhật tin đăng
            </button>
          </div>

          <div
            v-else-if="!authStore.isAuthenticated || authStore.user?.role === 'Tenant'"
            class="sticky top-24 bg-white p-6 border border-gray-200 rounded-2xl shadow-sm"
          >
            <h3 class="text-2xl font-bold text-gray-900 mb-4">
              {{ property.pricePerNight.toLocaleString() }} VND
              <span class="text-base font-normal text-gray-500">/ đêm</span>
            </h3>

            <div class="space-y-4 mb-6">
              <div class="flex flex-col">
                <label class="text-xs font-bold text-gray-700 uppercase mb-1">Nhận phòng</label>
                <el-date-picker
                  v-model="checkInDate"
                  type="date"
                  placeholder="Chọn ngày nhận phòng"
                  format="DD/MM/YYYY"
                  value-format="YYYY-MM-DD"
                  :disabled-date="disabledCheckInDate"
                  size="large"
                  style="width: 100%"
                />
              </div>

              <div class="flex flex-col">
                <label class="text-xs font-bold text-gray-700 uppercase mb-1">Trả phòng</label>
                <el-date-picker
                  v-model="checkOutDate"
                  type="date"
                  placeholder="Chọn ngày trả phòng"
                  format="DD/MM/YYYY"
                  value-format="YYYY-MM-DD"
                  :disabled-date="disabledCheckOutDate"
                  size="large"
                  style="width: 100%"
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
                  class="w-full px-4 py-2.5 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 outline-none transition"
                />
              </div>
            </div>

            <div v-if="totalNights > 0" class="border-t border-gray-200 pt-4 mb-4 space-y-3">
              <div class="flex justify-between text-gray-600">
                <span
                  >{{ property.pricePerNight.toLocaleString() }} VND x {{ totalNights }} đêm</span
                >
                <span>{{ totalPrice.toLocaleString() }} VND</span>
              </div>
              <div
                class="flex justify-between font-bold text-lg text-gray-900 border-t border-gray-200 pt-3 mt-3"
              >
                <span>Tổng cộng</span>
                <span class="text-blue-600">{{ totalPrice.toLocaleString() }} VND</span>
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

          <div
            v-else
            class="sticky top-24 bg-gray-50 p-6 border border-gray-200 rounded-2xl shadow-sm text-center"
          >
            <div class="text-4xl mb-4">⚠️</div>
            <h3 class="text-xl font-bold text-gray-900 mb-2">Chức năng đặt phòng</h3>
            <p class="text-sm text-gray-600">
              Bạn đang dùng tài khoản <strong>Chủ nhà</strong>. Vui lòng đăng nhập bằng tài khoản
              <strong>Khách thuê</strong> để thực hiện đặt phòng.
            </p>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { propertyService } from '@/services/property.service'
import { bookingService } from '@/services/booking.service'
import { useAuthStore } from '@/stores/auth.store'
import type { Property } from '@/types/property'
import type { CreateBookingRequest } from '@/types/booking'

const route = useRoute()
const router = useRouter()
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
const bookedDates = ref<string[]>([])

// --- LOGIC KHÓA NGÀY (ELEMENT PLUS) ---
const disabledCheckInDate = (time: Date): boolean => {
  const today = new Date()
  today.setHours(0, 0, 0, 0)

  // Khóa các ngày trong quá khứ
  if (time < today) return true

  // Định dạng ngày đang xét thành YYYY-MM-DD
  const year = time.getFullYear()
  const month = String(time.getMonth() + 1).padStart(2, '0')
  const day = String(time.getDate()).padStart(2, '0')
  const dateString = `${year}-${month}-${day}`

  // Khóa ngày nếu nó nằm trong danh sách đã được đặt (từ API)
  return bookedDates.value.includes(dateString)
}

const disabledCheckOutDate = (time: Date): boolean => {
  // Trả phòng cũng phải tuân thủ luật của Nhận phòng (bị khóa nếu đã có người thuê)
  if (disabledCheckInDate(time)) return true

  // Khóa thêm: Ngày trả phòng KHÔNG ĐƯỢC nhỏ hơn hoặc bằng Ngày nhận phòng
  if (checkInDate.value) {
    const checkIn = new Date(checkInDate.value)
    checkIn.setHours(0, 0, 0, 0)
    if (time <= checkIn) return true
  }

  return false
}

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

// Lớp phòng thủ số 2: Kiểm tra xem user có chọn ngày Check-out vắt ngang qua 1 ngày đã bị Book không
const isValidDateRange = computed(() => {
  if (!checkInDate.value || !checkOutDate.value) return true

  const start = new Date(checkInDate.value)
  const end = new Date(checkOutDate.value)

  for (let d = new Date(start); d < end; d.setDate(d.getDate() + 1)) {
    const dateStr = d.toISOString().split('T')[0]
    if (bookedDates.value.includes(dateStr)) {
      return false
    }
  }
  return true
})

// Cảnh báo nếu khoảng ngày vắt ngang ngày đã Book
watch([checkInDate, checkOutDate], () => {
  if (!isValidDateRange.value) {
    bookingError.value =
      'Khoảng thời gian bạn chọn có chứa ngày đã được đặt. Vui lòng chọn ngày khác.'
  } else {
    bookingError.value = null
  }
})

// Lấy dữ liệu khi load trang
onMounted(async () => {
  const id = route.params.id as string
  try {
    const [propertyData, datesData] = await Promise.all([
      propertyService.getPropertyById(id),
      propertyService.getBookedDates(id),
    ])

    property.value = propertyData
    bookedDates.value = datesData
  } catch {
    error.value = 'Không thể tải thông tin chi tiết căn nhà.'
  } finally {
    isLoading.value = false
  }
})

// Xử lý gửi Form Đặt phòng
const handleBooking = async () => {
  bookingError.value = null

  if (!authStore.isAuthenticated) {
    bookingError.value = 'Vui lòng Đăng nhập để tiến hành đặt phòng.'
    return
  }

  if (!isValidDateRange.value) {
    bookingError.value = 'Ngày chọn không hợp lệ (vướng lịch đã đặt).'
    return
  }

  if (totalNights.value <= 0) {
    bookingError.value = 'Ngày trả phòng phải sau ngày nhận phòng.'
    return
  }

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

    // Tự động tải lại trang để Lịch cập nhật ngay lập tức các ngày vừa book
    const newDates = await propertyService.getBookedDates(property.value!.id)
    bookedDates.value = newDates // Lịch Element Plus sẽ tự động bôi xám ngày vừa đặt!

    // Reset Form
    checkInDate.value = ''
    checkOutDate.value = ''
    guests.value = 1
  } catch (error: unknown) {
    const err = error as { response?: { data?: { error?: string } } }
    const backendError = err?.response?.data?.error || 'Có lỗi xảy ra khi gọi API đặt phòng.'

    bookingError.value = backendError
    alert('❌ Không thể đặt phòng: ' + backendError)
  } finally {
    isBooking.value = false
  }
}

const goToEdit = () => {
  if (property.value) {
    router.push(`/properties/${property.value.id}/edit`)
  }
}

const handleDelete = async () => {
  if (!property.value) return

  if (confirm('Bạn có chắc chắn muốn xóa bài đăng này không? Thao tác này không thể hoàn tác.')) {
    try {
      await propertyService.deleteProperty(property.value.id)
      alert('Đã xóa tin đăng thành công!')
      router.push('/')
    } catch (error: unknown) {
      const err = error as { response?: { data?: { message?: string } } }
      alert(err.response?.data?.message || 'Có lỗi xảy ra khi xóa bài đăng. Vui lòng thử lại.')
    }
  }
}
</script>
