<template>
  <div class="max-w-7xl mx-auto py-8 px-4 sm:px-6 lg:px-8">
    <h1 class="text-3xl font-extrabold text-gray-900 mb-8">Căn nhà yêu thích của bạn</h1>

    <div v-if="isLoading" class="text-center py-12 text-gray-500 font-medium">
      <p class="text-lg">Đang tải danh sách yêu thích...</p>
    </div>

    <div v-else-if="error" class="text-center py-12 text-red-500 font-medium">
      <p class="text-lg">{{ error }}</p>
    </div>

    <div
      v-else-if="properties.length === 0"
      class="text-center py-16 bg-white rounded-2xl shadow-sm border border-gray-200"
    >
      <span class="text-5xl block mb-4">❤️</span>
      <h3 class="text-xl font-bold text-gray-900 mb-2">Bạn chưa có căn nhà yêu thích nào</h3>
      <p class="text-gray-500">Hãy tiếp tục khám phá và lưu lại những căn nhà bạn ưng ý nhé.</p>
    </div>

    <div v-else class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-8">
      <PropertyCard
        v-for="prop in properties"
        :key="prop.id"
        :property="prop"
        :is-favorited="true"
        @unfavorited="handleUnfavorite"
      />
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { wishlistService } from '@/services/wishlist.service'
import type { Property } from '@/types/property'
import PropertyCard from '@/components/common/PropertyCard.vue'

const properties = ref<Property[]>([])
const isLoading = ref(true)
const error = ref<string | null>(null)

const fetchWishlist = async () => {
  isLoading.value = true
  error.value = null
  try {
    properties.value = await wishlistService.getMyWishlist()
  } catch {
    error.value = 'Không thể tải danh sách yêu thích. Vui lòng thử lại sau.'
  } finally {
    isLoading.value = false
  }
}

// Loại bỏ căn nhà khỏi giao diện ngay khi bấm hủy thả tim
const handleUnfavorite = (id: number | string) => {
  properties.value = properties.value.filter((p) => p.id !== id)
}

onMounted(() => {
  fetchWishlist()
})
</script>
