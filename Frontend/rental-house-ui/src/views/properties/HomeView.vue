<template>
  <div class="home-container py-8 px-4 sm:px-6 lg:px-8 max-w-7xl mx-auto">
    <div class="text-center mb-10">
      <h1 class="text-4xl font-extrabold text-gray-900 tracking-tight">Danh sách nhà cho thuê</h1>
      <p class="mt-2 text-gray-500">Khám phá các không gian tuyệt vời cho chuyến đi của bạn</p>
    </div>

    <div
      class="bg-white p-5 rounded-2xl shadow-sm border border-gray-200 mb-10 flex flex-col lg:flex-row gap-4 items-center transition-all"
    >
      <div class="w-full lg:grow relative">
        <span class="absolute inset-y-0 left-0 pl-4 flex items-center text-gray-400">🔍</span>
        <input
          v-model="searchQuery"
          type="text"
          placeholder="Nhập địa điểm, tên căn nhà..."
          class="w-full pl-11 pr-4 py-3 border border-gray-300 rounded-xl focus:ring-2 focus:ring-blue-500 focus:border-blue-500 outline-hidden transition"
          @keyup.enter="handleSearch"
        />
      </div>

      <div class="w-full lg:w-48">
        <input
          v-model.number="minPrice"
          type="number"
          min="0"
          placeholder="Giá tối thiểu"
          class="w-full px-4 py-3 border border-gray-300 rounded-xl focus:ring-2 focus:ring-blue-500 focus:border-blue-500 outline-hidden transition"
          @keyup.enter="handleSearch"
        />
      </div>

      <div class="w-full lg:w-48">
        <input
          v-model.number="maxPrice"
          type="number"
          min="0"
          placeholder="Giá tối đa"
          class="w-full px-4 py-3 border border-gray-300 rounded-xl focus:ring-2 focus:ring-blue-500 focus:border-blue-500 outline-hidden transition"
          @keyup.enter="handleSearch"
        />
      </div>

      <div class="flex gap-2 w-full lg:w-auto shrink-0">
        <button
          @click="handleSearch"
          class="grow lg:grow-0 bg-blue-600 hover:bg-blue-700 text-white font-semibold px-6 py-3 rounded-xl transition shadow-md shadow-blue-100 cursor-pointer"
        >
          Tìm kiếm
        </button>
        <button
          @click="handleReset"
          class="bg-gray-100 hover:bg-gray-200 text-gray-700 font-semibold px-4 py-3 rounded-xl transition cursor-pointer"
          title="Xóa bộ lọc"
        >
          🔄
        </button>
      </div>
    </div>

    <div v-if="isLoading" class="text-center py-20 text-gray-500 font-medium">
      Đang tải dữ liệu căn nhà...
    </div>

    <div v-else-if="error" class="text-center py-20 text-red-500 font-medium">
      {{ error }}
    </div>

    <div v-else-if="properties.length === 0" class="text-center py-20 text-gray-500 italic">
      Không tìm thấy căn nhà nào phù hợp với bộ lọc.
    </div>

    <div v-else class="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-6">
      <PropertyCard v-for="property in properties" :key="property.id" :property="property" />
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import propertyService from '@/services/property.service'
import type { Property, PropertyFilterParams } from '@/types/property'
import PropertyCard from '@/components/common/PropertyCard.vue'

const properties = ref<Property[]>([])
const isLoading = ref(true)
const error = ref<string | null>(null)

// State Form Filter chính xác kiểu dữ liệu
const searchQuery = ref('')
const minPrice = ref<number | null>(null)
const maxPrice = ref<number | null>(null)

// Hàm Fetch dữ liệu chính từ API
const fetchProperties = async (params?: PropertyFilterParams) => {
  isLoading.value = true
  error.value = null
  try {
    properties.value = await propertyService.getProperties(params)
  } catch (err: unknown) {
    error.value = 'Không thể tải dữ liệu nhà đất. Vui lòng thử lại sau.'
    // Tiêu thụ biến err hợp lệ để sạch lỗi ESLint no-unused-vars
    console.error('Lỗi fetchProperties:', err)
  } finally {
    isLoading.value = false
  }
}

// Thiết lập tham số và loại bỏ so sánh ép kiểu chuỗi sai chuẩn với null
const handleSearch = () => {
  const params: PropertyFilterParams = {}

  if (searchQuery.value.trim()) {
    params.searchTerm = searchQuery.value.trim()
  }
  if (minPrice.value !== null && minPrice.value !== undefined) {
    params.minPrice = minPrice.value
  }
  if (maxPrice.value !== null && maxPrice.value !== undefined) {
    params.maxPrice = maxPrice.value
  }

  fetchProperties(params)
}

// Khôi phục bộ lọc về trạng thái ban đầu
const handleReset = () => {
  searchQuery.value = ''
  minPrice.value = null
  maxPrice.value = null
  fetchProperties()
}

onMounted(() => {
  fetchProperties()
})
</script>
