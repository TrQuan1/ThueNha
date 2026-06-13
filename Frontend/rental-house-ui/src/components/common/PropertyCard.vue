<script setup lang="ts">
import type { Property } from '@/types/property'

defineProps<{ property: Property }>()

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

const handleImageError = (event: Event) => {
  const image = event.target as HTMLImageElement
  image.src = fallbackImage
}
</script>

<template>
  <article
    class="group overflow-hidden rounded-2xl border border-slate-200 bg-white shadow-sm transition-all duration-300 hover:-translate-y-1 hover:shadow-xl flex flex-col"
  >
    <router-link
      :to="{ name: 'property-detail', params: { id: property.id } }"
      class="aspect-4/3 overflow-hidden bg-slate-100 relative block"
    >
      <img
        :src="property.imageUrl || fallbackImage"
        :alt="property.title"
        class="h-full w-full object-cover transition-transform duration-500 group-hover:scale-105"
        loading="lazy"
        @error="handleImageError"
      />
    </router-link>

    <div class="flex flex-col grow p-5 space-y-4">
      <div class="space-y-1 mb-auto">
        <h3 class="text-lg font-semibold text-slate-900 line-clamp-2 leading-snug">
          {{ property.title }}
        </h3>
        <p class="text-sm text-slate-500 truncate">{{ property.address }}</p>
        <p v-if="property.maxGuests" class="text-sm font-medium text-slate-600 mt-2">
          Sức chứa: Tối đa {{ property.maxGuests }} khách
        </p>
      </div>

      <div class="flex items-center justify-between pt-2">
        <p class="text-lg font-bold text-blue-600">
          {{ formatCurrency(property.pricePerNight) }}
          <span class="text-sm font-medium text-slate-500">/ đêm</span>
        </p>

        <router-link
          :to="{ name: 'property-detail', params: { id: property.id } }"
          class="shrink-0 rounded-lg bg-slate-900 px-4 py-2 text-sm font-semibold text-white transition-colors hover:bg-blue-600 focus:outline-none block"
        >
          Xem chi tiết
        </router-link>
      </div>
    </div>
  </article>
</template>
