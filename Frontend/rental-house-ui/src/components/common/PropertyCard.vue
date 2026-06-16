<template>
  <div
    class="bg-white border border-gray-200 rounded-2xl overflow-hidden hover:shadow-xl transition-shadow cursor-pointer flex flex-col relative group"
    @click="goToDetail"
  >
    <div class="relative w-full h-56 bg-gray-200 overflow-hidden">
      <img
        :src="getFullImageUrl(property.imageUrl)"
        :alt="property.title"
        class="w-full h-full object-cover transition-transform duration-500 group-hover:scale-105"
        @error="
          (e) => ((e.target as HTMLImageElement).src = 'https://placehold.co/600x400?text=No+Image')
        "
      />

      <button
        v-if="authStore.isAuthenticated && authStore.user?.role === 'Tenant'"
        @click.stop.prevent="toggleFavorite"
        class="absolute top-3 right-3 z-10 p-2 rounded-full bg-black/20 hover:bg-black/40 transition-colors focus:outline-hidden cursor-pointer"
        title="Thêm/Xóa danh sách yêu thích"
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

    <div class="p-5 flex flex-col grow">
      <h3 class="font-bold text-lg text-gray-900 truncate mb-1" :title="property.title">
        {{ property.title }}
      </h3>
      <p class="text-sm text-gray-500 truncate mb-2">📍 {{ property.address }}</p>

      <p v-if="property.maxGuests" class="text-xs font-medium text-gray-500 mb-4">
        👥 Sức chứa: Tối đa {{ property.maxGuests }} khách
      </p>

      <div class="mt-auto flex justify-between items-center border-t border-gray-100 pt-4">
        <p class="text-blue-600 font-extrabold text-lg">
          {{ property.pricePerNight.toLocaleString() }}
          <span class="text-sm font-normal text-gray-500">VND/đêm</span>
        </p>

        <button
          class="shrink-0 bg-gray-900 hover:bg-blue-600 text-white px-4 py-2 rounded-lg text-sm font-semibold transition-colors"
        >
          Xem chi tiết
        </button>
      </div>
    </div>
  </div>
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

// Khai báo emits để báo cho Parent Component biết khi nào tim bị tắt/bật
const emit = defineEmits<{
  (e: 'unfavorited', id: number | string): void
  (e: 'favorited', id: number | string): void
}>()

const router = useRouter()
const authStore = useAuthStore()
const isLiked = ref(props.isFavorited)

watch(
  () => props.isFavorited,
  (newVal) => {
    isLiked.value = newVal
  },
)

const BACKEND_URL = import.meta.env.VITE_API_URL || 'https://localhost:7023/'
const getFullImageUrl = (relativeUrl?: string) => {
  if (!relativeUrl) return 'https://placehold.co/600x400?text=No+Image'
  if (relativeUrl.startsWith('http')) return relativeUrl
  return `${BACKEND_URL.replace('/api', '')}${relativeUrl}`
}

const goToDetail = () => {
  router.push(`/properties/${props.property.id}`)
}

const toggleFavorite = async () => {
  if (authStore.user?.role !== 'Tenant') {
    alert('Chỉ Khách thuê mới có thể thả tim!')
    return
  }

  try {
    if (isLiked.value) {
      await wishlistService.removeWishlist(props.property.id)
      isLiked.value = false
      emit('unfavorited', props.property.id) // Phát sự kiện unfavorited
    } else {
      await wishlistService.addWishlist(props.property.id)
      isLiked.value = true
      emit('favorited', props.property.id) // Phát sự kiện favorited
    }
  } catch {
    alert('Có lỗi xảy ra khi cập nhật danh sách yêu thích.')
  }
}
</script>
