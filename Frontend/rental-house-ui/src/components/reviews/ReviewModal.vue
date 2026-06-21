<template>
  <div v-if="isOpen" class="fixed inset-0 z-50 flex items-center justify-center px-4">
    <div class="absolute inset-0 bg-black/50 transition-opacity" @click="closeModal"></div>

    <div
      class="relative bg-white rounded-2xl shadow-xl w-full max-w-lg overflow-hidden flex flex-col animate-fade-in-up"
    >
      <div class="flex items-center justify-between px-6 py-4 border-b border-gray-100">
        <h3 class="text-xl font-bold text-gray-900">Đánh giá chuyến đi của bạn</h3>
        <button
          @click="closeModal"
          class="p-2 text-gray-400 hover:text-gray-600 rounded-full hover:bg-gray-100 transition"
        >
          <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path
              stroke-linecap="round"
              stroke-linejoin="round"
              stroke-width="2"
              d="M6 18L18 6M6 6l12 12"
            />
          </svg>
        </button>
      </div>

      <div class="px-6 py-6 overflow-y-auto">
        <div
          v-if="errorMessage"
          class="mb-4 p-3 rounded-lg bg-red-50 text-red-600 text-sm font-medium border border-red-200"
        >
          {{ errorMessage }}
        </div>

        <form @submit.prevent="handleSubmit" class="space-y-6">
          <div class="flex flex-col items-center">
            <label class="block text-sm font-semibold text-gray-700 mb-3"
              >Mức độ hài lòng của bạn</label
            >
            <div class="flex gap-2" @mouseleave="hoverRating = 0">
              <button
                v-for="star in 5"
                :key="star"
                type="button"
                @mouseover="hoverRating = star"
                @click="rating = star"
                class="focus:outline-none transition-transform hover:scale-110"
              >
                <svg
                  class="w-10 h-10 transition-colors duration-200"
                  :class="(hoverRating || rating) >= star ? 'text-yellow-400' : 'text-gray-200'"
                  fill="currentColor"
                  viewBox="0 0 20 20"
                >
                  <path
                    d="M9.049 2.927c.3-.921 1.603-.921 1.902 0l1.07 3.292a1 1 0 00.95.69h3.462c.969 0 1.371 1.24.588 1.81l-2.8 2.034a1 1 0 00-.364 1.118l1.07 3.292c.3.921-.755 1.688-1.54 1.118l-2.8-2.034a1 1 0 00-1.175 0l-2.8 2.034c-.784.57-1.838-.197-1.539-1.118l1.07-3.292a1 1 0 00-.364-1.118L2.98 8.72c-.783-.57-.38-1.81.588-1.81h3.461a1 1 0 00.951-.69l1.07-3.292z"
                  />
                </svg>
              </button>
            </div>
          </div>

          <div>
            <label class="block text-sm font-semibold text-gray-700 mb-2"
              >Chia sẻ trải nghiệm của bạn</label
            >
            <div class="relative">
              <textarea
                v-model="comment"
                rows="4"
                maxlength="500"
                placeholder="Chỗ ở có sạch sẽ không? Vị trí có thuận tiện không? Chủ nhà có nhiệt tình không?"
                class="w-full px-4 py-3 border border-gray-300 rounded-xl focus:ring-2 focus:ring-blue-500 focus:border-blue-500 outline-none transition resize-none bg-gray-50 focus:bg-white"
              ></textarea>
              <div class="absolute bottom-3 right-3 text-xs text-gray-400 font-medium">
                {{ comment.length }} / 500
              </div>
            </div>
          </div>

          <div class="flex justify-end gap-3 pt-4 border-t border-gray-100">
            <button
              type="button"
              @click="closeModal"
              class="px-5 py-2.5 text-gray-700 font-semibold hover:bg-gray-100 rounded-xl transition"
            >
              Hủy
            </button>
            <button
              type="submit"
              :disabled="rating === 0 || isSubmitting"
              class="px-6 py-2.5 bg-blue-600 text-white font-bold rounded-xl shadow-md hover:bg-blue-700 transition disabled:opacity-50 disabled:cursor-not-allowed flex items-center"
            >
              <svg
                v-if="isSubmitting"
                class="animate-spin -ml-1 mr-2 h-4 w-4 text-white"
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
              {{ isSubmitting ? 'Đang gửi...' : 'Gửi đánh giá' }}
            </button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue'
import { reviewService } from '@/services/review.service'
import type { ReviewDto } from '@/services/review.service'

const props = defineProps<{
  isOpen: boolean
  bookingId: number
  existingReview?: ReviewDto // Nếu có truyền vào -> Chế độ Edit
}>()

const emit = defineEmits<{
  (e: 'close'): void
  (e: 'success'): void
}>()

// States
const rating = ref(0)
const hoverRating = ref(0)
const comment = ref('')
const isSubmitting = ref(false)
const errorMessage = ref<string | null>(null)

// Lắng nghe sự thay đổi của modal để reset form hoặc điền dữ liệu cũ vào form
watch(
  () => props.isOpen,
  (newVal) => {
    if (newVal) {
      errorMessage.value = null
      if (props.existingReview) {
        // Đang sửa review cũ
        rating.value = props.existingReview.rating
        comment.value = props.existingReview.comment
      } else {
        // Đang tạo review mới
        rating.value = 0
        comment.value = ''
      }
    }
  },
)

const closeModal = () => {
  emit('close')
}

const handleSubmit = async () => {
  if (rating.value === 0) return

  isSubmitting.value = true
  errorMessage.value = null

  try {
    if (props.existingReview) {
      // Gọi API cập nhật
      await reviewService.updateReview(props.existingReview.id, {
        rating: rating.value,
        comment: comment.value.trim(),
      })
    } else {
      // Gọi API tạo mới
      await reviewService.createReview({
        bookingId: props.bookingId,
        rating: rating.value,
        comment: comment.value.trim(),
      })
    }

    // Thông báo cho component cha biết để gọi lại danh sách Booking hoặc Review
    emit('success')
    closeModal()
  } catch (error: unknown) {
    const err = error as { response?: { data?: { message?: string } } }
    errorMessage.value =
      err.response?.data?.message || 'Có lỗi xảy ra khi lưu đánh giá. Vui lòng thử lại sau.'
  } finally {
    isSubmitting.value = false
  }
}
</script>

<style scoped>
/* Keyframes tạo hiệu ứng trượt nhẹ lên (Smooth Modal Reveal) */
@keyframes fadeInUp {
  from {
    opacity: 0;
    transform: translateY(10px) scale(0.98);
  }
  to {
    opacity: 1;
    transform: translateY(0) scale(1);
  }
}
.animate-fade-in-up {
  animation: fadeInUp 0.25s ease-out forwards;
}
</style>
