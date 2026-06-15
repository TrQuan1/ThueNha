<template>
  <div class="py-8">
    <div class="mb-10 text-center">
      <h1 class="text-4xl font-extrabold tracking-tight text-slate-900">Danh sách nhà cho thuê</h1>
      <p class="mt-3 text-lg text-slate-500">Khám phá không gian tuyệt vời cho chuyến đi của bạn</p>
    </div>

    <div
      class="mb-10 rounded-2xl border border-slate-200 bg-white p-4 shadow-sm transition-all hover:shadow-md lg:p-5 overflow-hidden"
    >
      <div class="flex flex-col gap-4 lg:flex-row lg:items-center flex-wrap xl:flex-nowrap">
        <div class="relative w-full shrink lg:grow">
          <div class="pointer-events-none absolute inset-y-0 left-0 flex items-center pl-4">
            <svg
              class="h-5 w-5 text-slate-400"
              xmlns="http://www.w3.org/2000/svg"
              viewBox="0 0 20 20"
              fill="currentColor"
            >
              <path
                fill-rule="evenodd"
                d="M9 3.5a5.5 5.5 0 100 11 5.5 5.5 0 000-11zM2 9a7 7 0 1112.452 4.391l3.328 3.329a.75.75 0 11-1.06 1.06l-3.329-3.328A7 7 0 012 9z"
                clip-rule="evenodd"
              />
            </svg>
          </div>
          <input
            v-model="searchQuery"
            type="text"
            placeholder="Nhập địa điểm, tên căn nhà..."
            class="block w-full rounded-xl border-0 py-3.5 pl-11 pr-4 text-slate-900 shadow-sm ring-1 ring-inset ring-slate-300 placeholder:text-slate-400 focus:ring-2 focus:ring-inset focus:ring-blue-600 transition-shadow outline-none"
            @keyup.enter="handleSearch"
          />
        </div>

        <div class="w-full lg:w-44 xl:w-48 shrink-0">
          <input
            v-model.number="minPrice"
            type="number"
            min="0"
            placeholder="Giá tối thiểu"
            class="block w-full rounded-xl border-0 py-3.5 px-4 text-slate-900 shadow-sm ring-1 ring-inset ring-slate-300 placeholder:text-slate-400 focus:ring-2 focus:ring-inset focus:ring-blue-600 transition-shadow outline-none"
            @keyup.enter="handleSearch"
          />
        </div>

        <div class="w-full lg:w-44 xl:w-48 shrink-0">
          <input
            v-model.number="maxPrice"
            type="number"
            min="0"
            placeholder="Giá tối đa"
            class="block w-full rounded-xl border-0 py-3.5 px-4 text-slate-900 shadow-sm ring-1 ring-inset ring-slate-300 placeholder:text-slate-400 focus:ring-2 focus:ring-inset focus:ring-blue-600 transition-shadow outline-none"
            @keyup.enter="handleSearch"
          />
        </div>

        <div class="flex w-full gap-3 lg:w-auto shrink-0">
          <button
            @click="handleSearch"
            class="flex-1 cursor-pointer rounded-xl bg-blue-600 px-6 py-3.5 text-center font-semibold text-white shadow-sm transition-colors hover:bg-blue-500 focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-blue-600 lg:flex-none"
          >
            Tìm kiếm
          </button>
          <button
            @click="handleReset"
            class="flex-1 cursor-pointer rounded-xl bg-slate-100 px-6 py-3.5 text-center font-semibold text-slate-900 shadow-sm transition-colors hover:bg-slate-200 lg:flex-none"
          >
            Xóa lọc
          </button>
        </div>
      </div>
    </div>

    <div
      v-if="isLoading"
      class="flex min-h-100 flex-col items-center justify-center rounded-2xl border border-slate-200 bg-white"
    >
      <svg
        class="h-10 w-10 animate-spin text-blue-600"
        xmlns="http://www.w3.org/2000/svg"
        fill="none"
        viewBox="0 0 24 24"
      >
        <circle
          class="opacity-25"
          cx="12"
          cy="12"
          r="10"
          stroke="currentColor"
          stroke-width="4"
        ></circle>
        <path
          class="opacity-75"
          fill="currentColor"
          d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"
        ></path>
      </svg>
      <p class="mt-4 font-medium text-slate-500">Đang tải dữ liệu nhà đất...</p>
    </div>

    <div
      v-else-if="error"
      class="flex min-h-100 flex-col items-center justify-center rounded-2xl border border-red-200 bg-red-50 p-6 text-center"
    >
      <p class="text-lg font-semibold text-red-800">{{ error }}</p>
      <button
        @click="handleReset"
        class="mt-4 rounded-lg bg-red-100 px-4 py-2 text-sm font-semibold text-red-800 hover:bg-red-200"
      >
        Thử lại
      </button>
    </div>

    <div
      v-else-if="properties.length === 0"
      class="flex min-h-100 flex-col items-center justify-center rounded-2xl border border-slate-200 bg-slate-50 p-6 text-center"
    >
      <span class="text-6xl mb-4 opacity-50">🏠</span>
      <h3 class="text-xl font-bold text-slate-900">Không tìm thấy căn nhà nào</h3>
      <button
        @click="handleReset"
        class="mt-6 rounded-lg bg-blue-50 px-4 py-2 text-sm font-semibold text-blue-600 hover:bg-blue-100"
      >
        Xóa bộ lọc
      </button>
    </div>

    <div v-else class="grid grid-cols-1 gap-6 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4">
      <PropertyCard
        v-for="prop in properties"
        :key="prop.id"
        :property="prop"
        :is-favorited="favoriteIds.includes(prop.id)"
      />
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, watch } from 'vue'
import { propertyService } from '@/services/property.service'
import { wishlistService } from '@/services/wishlist.service'
import { useAuthStore } from '@/stores/auth.store'
import type { Property, PropertyFilterParams } from '@/types/property'
import PropertyCard from '@/components/common/PropertyCard.vue'

const properties = ref<Property[]>([])
const favoriteIds = ref<(string | number)[]>([])
const isLoading = ref(true)
const error = ref<string | null>(null)

const searchQuery = ref('')
const minPrice = ref<number | null>(null)
const maxPrice = ref<number | null>(null)

const authStore = useAuthStore()

// Theo dõi thay đổi trạng thái auth để reset dữ liệu Wishlist
watch(
  () => authStore.isAuthenticated,
  () => {
    favoriteIds.value = []
    fetchWishlist()
  },
)

const fetchWishlist = async () => {
  if (authStore.isAuthenticated && authStore.user?.role === 'Tenant') {
    try {
      const data = await wishlistService.getMyWishlist()
      favoriteIds.value = data.map((p) => p.id)
    } catch (err: unknown) {
      console.error('Lỗi khi tải wishlist:', err)
      favoriteIds.value = [] // Reset nếu lỗi
    }
  }
}

const fetchProperties = async (params?: PropertyFilterParams) => {
  isLoading.value = true
  error.value = null
  try {
    properties.value = await propertyService.getProperties(params)
  } catch {
    error.value = 'Không thể tải dữ liệu nhà đất. Vui lòng thử lại sau.'
  } finally {
    isLoading.value = false
  }
}

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

const handleReset = () => {
  searchQuery.value = ''
  minPrice.value = null
  maxPrice.value = null
  fetchProperties()
}

onMounted(() => {
  fetchProperties()
  fetchWishlist()
})
</script>
