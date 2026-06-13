<script setup lang="ts">
import { ref, onMounted } from 'vue'
import propertyService from '@/services/property.service'
import type { Property } from '@/types/property'
import PropertyCard from '@/components/common/PropertyCard.vue'

const properties = ref<Property[]>([])
const isLoading = ref(true)
const error = ref<string | null>(null)

onMounted(async () => {
  try {
    const response = await propertyService.getAllProperties()
    properties.value = response.data || response
  } catch {
    error.value = 'Không thể tải dữ liệu nhà đất. Vui lòng thử lại sau.'
  } finally {
    isLoading.value = false
  }
})
</script>

<template>
  <main class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-10 font-sans">
    <h1 class="text-3xl md:text-4xl font-bold text-center text-slate-800 mb-10 tracking-tight">
      Danh sách nhà cho thuê
    </h1>

    <div v-if="isLoading" class="text-center py-16 text-slate-500 text-lg animate-pulse">
      Đang tải dữ liệu...
    </div>

    <div v-else-if="error" class="text-center py-16 text-red-500 text-lg">
      {{ error }}
    </div>

    <div
      v-else-if="properties.length === 0"
      class="text-center py-16 text-slate-500 text-lg italic"
    >
      Chưa có căn nhà nào được đăng.
    </div>

    <div v-else class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6">
      <PropertyCard v-for="prop in properties" :key="prop.id" :property="prop" />
    </div>
  </main>
</template>
