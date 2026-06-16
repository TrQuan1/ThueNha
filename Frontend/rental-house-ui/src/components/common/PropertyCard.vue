<template>
  <div
    class="bg-white border border-gray-200 rounded-2xl overflow-hidden hover:shadow-xl transition-shadow cursor-pointer flex flex-col relative"
    @click="goToDetail"
  >
    <div class="relative w-full h-56 bg-gray-200">
      <img
        :src="getFullImageUrl(property.imageUrl)"
        :alt="property.title"
        class="w-full h-full object-cover"
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

    <div class="p-5 flex flex-col grow">
      <h3 class="font-bold text-lg text-gray-900 truncate mb-1" :title="property.title">
        {{ property.title }}
      </h3>
      <p class="text-sm text-gray-500 truncate mb-4">📍 {{ property.address }}</p>
      <div class="mt-auto flex justify-between items-end">
        <p class="text-blue-600 font-extrabold text-lg">
          {{ property.pricePerNight.toLocaleString() }}
          <span class="text-sm font-normal text-gray-500">VND/đêm</span>
        </p>
        <p class="text-xs font-semibold text-gray-400 bg-gray-100 px-2 py-1 rounded-md">
          Tối đa {{ property.maxGuests }} khách
        </p>
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
