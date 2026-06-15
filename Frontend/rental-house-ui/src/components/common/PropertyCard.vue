<template>
  <article
    class="group overflow-hidden rounded-2xl border border-slate-200 bg-white shadow-sm transition-all duration-300 hover:-translate-y-1 hover:shadow-xl flex flex-col relative cursor-pointer"
    @click="goToDetail"
  >
    <div class="aspect-4/3 overflow-hidden bg-slate-100 relative block w-full h-56">
      <img
        :src="getFullImageUrl(property.imageUrl)"
        :alt="property.title"
        class="h-full w-full object-cover transition-transform duration-500 group-hover:scale-105"
        loading="lazy"
        @error="
          (e) => ((e.target as HTMLImageElement).src = 'https://placehold.co/600x400?text=No+Image')
        "
      />

      <button
        @click.stop.prevent="toggleFavorite"
        class="absolute top-3 right-3 z-10 p-2 rounded-full bg-black/20 hover:bg-black/40 transition-colors focus:outline-hidden cursor-pointer"
        title="Thêm vào danh sách yêu thích"
      >
        <svg
          xmlns="http://www.w3.org/2000/svg"
          :class="[
            'w-6 h-6 transition-colors',
            isLiked
              ? 'text-red-500 fill-current'
              : 'text-white stroke-current stroke-2 fill-transparent',
          ]"
          viewBox="0 0 24 24"
        >
          <path
            stroke-linecap="round"
            stroke-linejoin="round"
            d="M21 8.25c0-2.485-2.099-4.5-4.688-4.5-1.935 0-3.597 1.126-4.312 2.733-.715-1.607-2.377-2.733-4.313-2.733C5.1 3.75 3 5.765 3 8.25c0 7.22 9 12 9 12s9-4.78 9-12z"
          />
        </svg>
      </button>
    </div>

    <div class="flex flex-col grow p-5 space-y-4">
      <div class="space-y-1 mb-auto">
        <h3 class="text-lg font-semibold text-slate-900 line-clamp-2 leading-snug">
          {{ property.title }}
        </h3>
        <p class="text-sm text-slate-500 truncate">📍 {{ property.address }}</p>
        <p v-if="property.maxGuests" class="text-sm font-medium text-slate-600 mt-2">
          Sức chứa: Tối đa {{ property.maxGuests }} khách
        </p>
      </div>

      <div class="flex items-center justify-between pt-2">
        <p class="text-lg font-bold text-blue-600">
          {{ formatCurrency(property.pricePerNight) }}
          <span class="text-sm font-medium text-slate-500">/ đêm</span>
        </p>

        <button
          class="shrink-0 rounded-lg bg-slate-900 px-4 py-2 text-sm font-semibold text-white transition-colors hover:bg-blue-600 focus:outline-none block"
        >
          Xem chi tiết
        </button>
      </div>
    </div>
  </article>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth.store'
import { wishlistService } from '@/services/wishlist.service'
import type { Property } from '@/types/property'

const props = withDefaults(
  defineProps<{
    property: Property
    isFavorited?: boolean
  }>(),
  {
    isFavorited: false,
  },
)

const router = useRouter()
const authStore = useAuthStore()

const isLiked = ref(props.isFavorited)

watch(
  () => props.isFavorited,
  (newVal) => {
    isLiked.value = newVal
  },
)

const BACKEND_URL = import.meta.env.VITE_API_URL || 'https://localhost:7023'
const getFullImageUrl = (relativeUrl?: string) => {
  if (!relativeUrl) return 'https://placehold.co/600x400?text=No+Image'
  if (relativeUrl.startsWith('http')) return relativeUrl
  return `${BACKEND_URL.replace(/\/api\/?$/, '')}/${relativeUrl.replace(/^\//, '')}`
}

const formatCurrency = (value: number): string => {
  if (!value) return '0 VND'
  return new Intl.NumberFormat('vi-VN', {
    style: 'currency',
    currency: 'VND',
    maximumFractionDigits: 0,
  }).format(value)
}

const goToDetail = () => {
  router.push(`/properties/${props.property.id}`)
}

const toggleFavorite = async () => {
  if (authStore.user?.role !== 'Tenant') {
    alert('Chỉ Khách thuê mới có thể thả tim!')
    return
  }

  const previousState = isLiked.value
  isLiked.value = !isLiked.value // Optimistic update: cập nhật UI trước

  try {
    if (isLiked.value) {
      await wishlistService.addWishlist(props.property.id)
    } else {
      await wishlistService.removeWishlist(props.property.id)
    }
  } catch (err: unknown) {
    const errorObj = err as { response?: { status?: number } }

    // Nếu lỗi 400: Căn nhà không có trong danh sách yêu thích
    if (errorObj.response?.status === 400) {
      alert('Căn nhà không có trong danh sách yêu thích.')
      isLiked.value = false // Đồng bộ lại trạng thái là false
    } else {
      alert('Đã xảy ra lỗi, vui lòng thử lại.')
      isLiked.value = previousState // Hoàn tác nếu lỗi khác
    }
  }
}
</script>
