<template>
  <div class="py-8 border-t border-gray-200 mt-8">
    <div class="flex items-center gap-2 mb-8">
      <svg class="w-6 h-6 text-yellow-500" fill="currentColor" viewBox="0 0 20 20">
        <path
          d="M9.049 2.927c.3-.921 1.603-.921 1.902 0l1.07 3.292a1 1 0 00.95.69h3.462c.969 0 1.371 1.24.588 1.81l-2.8 2.034a1 1 0 00-.364 1.118l1.07 3.292c.3.921-.755 1.688-1.54 1.118l-2.8-2.034a1 1 0 00-1.175 0l-2.8 2.034c-.784.57-1.838-.197-1.539-1.118l1.07-3.292a1 1 0 00-.364-1.118L2.98 8.72c-.783-.57-.38-1.81.588-1.81h3.461a1 1 0 00.951-.69l1.07-3.292z"
        />
      </svg>
      <h2 class="text-2xl font-bold text-gray-900">
        <span v-if="reviews.length > 0"
          >{{ averageRating.toFixed(1) }} · {{ reviews.length }} đánh giá</span
        >
        <span v-else>Chưa có đánh giá nào</span>
      </h2>
    </div>

    <div v-if="isLoading" class="text-gray-500 py-4 animate-pulse">Đang tải đánh giá...</div>
    <div v-else-if="error" class="text-red-500 py-4 font-medium">{{ error }}</div>

    <div v-else-if="reviews.length === 0" class="text-gray-500 italic py-4">
      Chưa có đánh giá nào cho chỗ ở này. Hãy là người đầu tiên trải nghiệm và chia sẻ!
    </div>

    <div v-else class="grid grid-cols-1 md:grid-cols-2 gap-x-12 gap-y-10">
      <div v-for="review in reviews" :key="review.id" class="flex flex-col">
        <div class="flex items-center mb-4">
          <div
            class="w-12 h-12 rounded-full bg-gray-800 text-white flex items-center justify-center text-lg font-bold shrink-0"
          >
            {{ getInitial(review.tenantName) }}
          </div>
          <div class="ml-4">
            <h3 class="text-base font-semibold text-gray-900">{{ review.tenantName }}</h3>
            <p class="text-sm text-gray-500">{{ formatDate(review.createdAt) }}</p>
          </div>
        </div>

        <div class="flex items-center mb-2">
          <div class="flex text-yellow-500">
            <svg
              v-for="star in 5"
              :key="star"
              class="w-4 h-4"
              :class="star <= review.rating ? 'text-yellow-500' : 'text-gray-300'"
              fill="currentColor"
              viewBox="0 0 20 20"
            >
              <path
                d="M9.049 2.927c.3-.921 1.603-.921 1.902 0l1.07 3.292a1 1 0 00.95.69h3.462c.969 0 1.371 1.24.588 1.81l-2.8 2.034a1 1 0 00-.364 1.118l1.07 3.292c.3.921-.755 1.688-1.54 1.118l-2.8-2.034a1 1 0 00-1.175 0l-2.8 2.034c-.784.57-1.838-.197-1.539-1.118l1.07-3.292a1 1 0 00-.364-1.118L2.98 8.72c-.783-.57-.38-1.81.588-1.81h3.461a1 1 0 00.951-.69l1.07-3.292z"
              />
            </svg>
          </div>
        </div>
        <p class="text-gray-700 leading-relaxed wrap-break-word whitespace-pre-line">
          {{ review.comment }}
        </p>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { reviewService } from '@/services/review.service'
import type { ReviewDto } from '@/services/review.service'

const props = defineProps<{
  propertyId: number | string
  averageRating: number
}>()

const reviews = ref<ReviewDto[]>([])
const isLoading = ref(true)
const error = ref<string | null>(null)

// Format ngày chuẩn tiếng Việt (VD: Tháng 6 năm 2026)
const formatDate = (dateString: string) => {
  if (!dateString) return ''
  const date = new Date(dateString)
  return new Intl.DateTimeFormat('vi-VN', { month: 'long', year: 'numeric' }).format(date)
}

// Lấy chữ cái đầu tiên của tên làm Avatar ảo
const getInitial = (name: string) => {
  return name && name.trim().length > 0 ? name.trim()[0].toUpperCase() : 'U'
}

onMounted(async () => {
  try {
    isLoading.value = true
    reviews.value = await reviewService.getReviewsByProperty(props.propertyId)
  } catch (err: unknown) {
    console.error(err)
    error.value = 'Không thể tải danh sách đánh giá. Vui lòng thử lại sau.'
  } finally {
    isLoading.value = false
  }
})
</script>
