<script setup lang="ts">
import type { Property } from '@/types/Property'

defineProps<{
  property: Property
}>()

const fallbackImage =
  'https://images.unsplash.com/photo-1564013799919-ab600027ffc6?auto=format&fit=crop&w=1200&q=80'

const formatCurrency = (value: number): string =>
  new Intl.NumberFormat('vi-VN', {
    style: 'currency',
    currency: 'VND',
    maximumFractionDigits: 0,
  }).format(value)

const handleImageError = (event: Event) => {
  const image = event.target as HTMLImageElement
  image.src = fallbackImage
}
</script>

<template>
  <article
    class="group overflow-hidden rounded-2xl border border-slate-200 bg-white shadow-sm transition-all duration-300 hover:-translate-y-1 hover:shadow-xl"
  >
    <div class="aspect-[4/3] overflow-hidden bg-slate-100">
      <img
        :src="property.imageUrl || fallbackImage"
        :alt="property.title"
        class="h-full w-full object-cover transition-transform duration-500 group-hover:scale-105"
        loading="lazy"
        @error="handleImageError"
      />
    </div>

    <div class="space-y-4 p-5">
      <div class="space-y-2">
        <h3 class="text-lg font-semibold text-slate-950">{{ property.title }}</h3>
        <p class="text-sm leading-6 text-slate-500">{{ property.address }}</p>
      </div>

      <div class="flex items-center justify-between gap-4">
        <p class="text-base font-bold text-blue-600">
          {{ formatCurrency(property.price) }}
          <span class="text-sm font-medium text-slate-500">/ đêm</span>
        </p>

        <button
          type="button"
          class="shrink-0 rounded-xl bg-slate-950 px-4 py-2 text-sm font-semibold text-white transition-colors hover:bg-blue-600 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2"
        >
          Xem chi tiết
        </button>
      </div>
    </div>
  </article>
</template>
