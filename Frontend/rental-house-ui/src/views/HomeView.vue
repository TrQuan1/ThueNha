<script setup lang="ts">
import { onMounted, ref } from 'vue'
import PropertyCard from '@/components/PropertyCard.vue'
import { getProperties } from '@/services/property.service'
import type { Property } from '@/types/Property'

const properties = ref<Property[]>([])
const loading = ref(false)
const errorMessage = ref('')

const loadProperties = async () => {
  loading.value = true
  errorMessage.value = ''

  try {
    properties.value = await getProperties()
  } catch (error) {
    const err = error as { message?: string }
    errorMessage.value = err.message || 'Không thể tải danh sách nhà cho thuê.'
  } finally {
    loading.value = false
  }
}

onMounted(loadProperties)
</script>

<template>
  <section class="mx-auto max-w-7xl px-4 py-10 sm:px-6 lg:px-8">
    <div class="mb-8 flex flex-col gap-2">
      <p class="text-sm font-semibold uppercase tracking-wide text-blue-600">RentalHouse</p>
      <h1 class="text-3xl font-bold text-slate-950 sm:text-4xl">Khám phá nhà cho thuê</h1>
      <p class="max-w-2xl text-base leading-7 text-slate-600">
        Tìm không gian phù hợp cho chuyến đi, kỳ nghỉ hoặc nhu cầu lưu trú dài ngày của bạn.
      </p>
    </div>

    <div
      v-if="loading"
      class="flex min-h-64 items-center justify-center rounded-2xl border border-dashed border-slate-300 bg-white text-base font-medium text-slate-500"
    >
      Đang tải...
    </div>

    <div
      v-else-if="errorMessage"
      class="rounded-2xl border border-red-200 bg-red-50 p-5 text-sm font-medium text-red-700"
    >
      {{ errorMessage }}
    </div>

    <div
      v-else-if="properties.length"
      class="grid grid-cols-1 gap-6 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4"
    >
      <PropertyCard v-for="property in properties" :key="property.id" :property="property" />
    </div>

    <div
      v-else
      class="flex min-h-64 items-center justify-center rounded-2xl border border-dashed border-slate-300 bg-white text-base font-medium text-slate-500"
    >
      Chưa có nhà cho thuê nào.
    </div>
  </section>
</template>
