<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRoute } from 'vue-router'
import propertyService from '@/services/property.service'
import type { Property } from '@/types/property'

const route = useRoute()
const property = ref<Property | null>(null)
const isLoading = ref(true)
const error = ref<string | null>(null)

const fallbackImage =
  'https://images.unsplash.com/photo-1564013799919-ab600027ffc6?auto=format&fit=crop&w=1200&q=80'

const formatCurrency = (value: number): string => {
  if (!value) return '0 VND'
  return new Intl.NumberFormat('vi-VN', {
    style: 'currency',
    currency: 'VND',
    maximumFractionDigits: 0,
  }).format(value)
}

// Hàm kiểm tra và gắn domain Backend cho ảnh
const getFullImageUrl = (path?: string): string => {
  if (!path) return fallbackImage
  return path.startsWith('/') ? `https://localhost:7023${path}` : path
}

onMounted(async () => {
  const id = route.params.id as string
  try {
    const response = await propertyService.getPropertyById(id)

    // Ép kiểu an toàn không dùng any để tránh lỗi ESLint cấu hình dự án
    const rawData = (response as { data?: Property }).data || (response as Property)
    property.value = rawData
  } catch (err) {
    console.error('Lỗi lấy chi tiết nhà:', err)
    error.value = 'Không thể tải thông tin chi tiết căn nhà này.'
  } finally {
    isLoading.value = false
  }
})
</script>

<template>
  <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8 font-sans">
    <div v-if="isLoading" class="text-center py-20 text-slate-500 animate-pulse text-lg">
      Đang tải thông tin chi tiết...
    </div>
    <div v-else-if="error || !property" class="text-center py-20 text-red-500 text-lg">
      {{ error || 'Không tìm thấy căn nhà yêu cầu.' }}
    </div>

    <div v-else class="space-y-8">
      <div>
        <h1 class="text-2xl md:text-3xl font-bold text-slate-900 mb-2">{{ property.title }}</h1>
        <p class="text-slate-500 flex items-center gap-1">
          <span class="font-medium text-slate-700">📍 Địa chỉ:</span> {{ property.address }}
        </p>
      </div>

      <div class="w-full aspect-21/9 rounded-2xl overflow-hidden bg-slate-100 shadow-sm">
        <img
          :src="getFullImageUrl(property.imageUrl)"
          :alt="property.title"
          class="w-full h-full object-cover"
        />
      </div>

      <div class="grid grid-cols-1 lg:grid-cols-3 gap-8 items-start">
        <div class="lg:col-span-2 space-y-6">
          <div class="border-b border-slate-200 pb-4">
            <h2 class="text-xl font-bold text-slate-900 mb-2">Đặc điểm chỗ ở</h2>
            <div class="flex gap-4 text-sm font-medium text-slate-600">
              <span class="bg-slate-100 px-3 py-1.5 rounded-lg">🏠 Căn hộ dịch vụ</span>
              <span class="bg-slate-100 px-3 py-1.5 rounded-lg"
                >👥 Tối đa {{ property.maxGuests }} khách</span
              >
            </div>
          </div>

          <div class="space-y-3">
            <h2 class="text-xl font-bold text-slate-900">Mô tả chi tiết</h2>
            <p class="text-slate-600 leading-relaxed whitespace-pre-line">
              {{ property.description }}
            </p>
          </div>

          <div class="space-y-3 pt-4 border-t border-slate-200">
            <h2 class="text-xl font-bold text-slate-900">Tiện ích đi kèm</h2>
            <div class="grid grid-cols-2 sm:grid-cols-3 gap-3 text-sm text-slate-600">
              <div class="flex items-center gap-2">📶 Wifi miễn phí</div>
              <div class="flex items-center gap-2">❄️ Điều hòa nhiệt độ</div>
              <div class="flex items-center gap-2">🏍️ Chỗ để xe máy</div>
              <div class="flex items-center gap-2">🧼 Máy giặt tự động</div>
              <div class="flex items-center gap-2">🚿 Nước nóng lạnh</div>
            </div>
          </div>
        </div>

        <div
          class="border border-slate-200 rounded-2xl p-6 bg-white shadow-lg lg:sticky lg:top-6 space-y-4"
        >
          <div>
            <span class="text-2xl font-bold text-blue-600">{{
              formatCurrency(property.pricePerNight)
            }}</span>
            <span class="text-slate-500 text-sm"> / đêm</span>
          </div>

          <div class="border border-slate-200 rounded-xl divide-y divide-slate-200 text-xs">
            <div class="grid grid-cols-2 divide-x divide-slate-200">
              <div class="p-3">
                <label class="font-bold block uppercase text-slate-500 mb-1">Nhận phòng</label>
                <span class="text-sm text-slate-800">Chọn ngày</span>
              </div>
              <div class="p-3">
                <label class="font-bold block uppercase text-slate-500 mb-1">Trả phòng</label>
                <span class="text-sm text-slate-800">Chọn ngày</span>
              </div>
            </div>
            <div class="p-3">
              <label class="font-bold block uppercase text-slate-500 mb-1">Số lượng khách</label>
              <span class="text-sm text-slate-800">1 khách</span>
            </div>
          </div>

          <button
            type="button"
            class="w-full bg-blue-600 hover:bg-blue-700 text-white font-bold py-3 px-4 rounded-xl transition-colors shadow-md shadow-blue-200 text-center"
          >
            Liên hệ đặt phòng
          </button>

          <p class="text-center text-xs text-slate-400">Bạn vẫn chưa bị trừ tiền ở bước này</p>
        </div>
      </div>
    </div>
  </div>
</template>
