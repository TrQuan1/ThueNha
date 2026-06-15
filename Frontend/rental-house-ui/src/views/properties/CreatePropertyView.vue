<template>
  <div class="min-h-screen bg-gray-50 py-12 px-4 sm:px-6 lg:px-8">
    <div class="max-w-2xl mx-auto bg-white rounded-2xl shadow-xl p-8 border border-gray-100">
      <div class="text-center mb-8">
        <h1 class="text-3xl font-extrabold text-gray-900 tracking-tight">Đăng Tin Cho Thuê Nhà</h1>
        <p class="mt-2 text-sm text-gray-600">
          Chia sẻ không gian của bạn với những người đang tìm kiếm nơi dừng chân lý tưởng.
        </p>
      </div>

      <form @submit.prevent="handleSubmit" class="space-y-6">
        <div>
          <label class="block text-sm font-semibold text-gray-700 mb-2">Tiêu đề bài đăng</label>
          <input
            v-model="formData.title"
            type="text"
            required
            class="w-full px-4 py-3 rounded-xl border border-gray-300 shadow-xs focus:ring-2 focus:ring-blue-500 focus:border-blue-500 focus:outline-hidden transition"
          />
        </div>

        <div>
          <label class="block text-sm font-semibold text-gray-700 mb-2">Địa chỉ chính xác</label>
          <input
            v-model="formData.address"
            type="text"
            required
            class="w-full px-4 py-3 rounded-xl border border-gray-300 shadow-xs focus:ring-2 focus:ring-blue-500 focus:border-blue-500 focus:outline-hidden transition"
          />
        </div>

        <div class="grid grid-cols-1 sm:grid-cols-2 gap-6">
          <div>
            <label class="block text-sm font-semibold text-gray-700 mb-2">Giá theo đêm (VND)</label>
            <input
              v-model.number="formData.pricePerNight"
              type="number"
              min="0"
              required
              class="w-full px-4 py-3 rounded-xl border border-gray-300 shadow-xs focus:ring-2 focus:ring-blue-500 focus:border-blue-500 focus:outline-hidden transition"
            />
          </div>
          <div>
            <label class="block text-sm font-semibold text-gray-700 mb-2">Số khách tối đa</label>
            <input
              v-model.number="formData.maxGuests"
              type="number"
              min="1"
              required
              class="w-full px-4 py-3 rounded-xl border border-gray-300 shadow-xs focus:ring-2 focus:ring-blue-500 focus:border-blue-500 focus:outline-hidden transition"
            />
          </div>
        </div>

        <div>
          <label class="block text-sm font-semibold text-gray-700 mb-2">Mô tả chi tiết</label>
          <textarea
            v-model="formData.description"
            rows="5"
            required
            class="w-full px-4 py-3 rounded-xl border border-gray-300 shadow-xs focus:ring-2 focus:ring-blue-500 focus:border-blue-500 focus:outline-hidden transition resize-none"
          ></textarea>
        </div>

        <div>
          <label class="block text-sm font-semibold text-gray-700 mb-2"
            >Hình ảnh căn nhà (Chọn nhiều ảnh)</label
          >
          <div
            class="mt-1 flex justify-center px-6 pt-5 pb-6 border-2 border-gray-300 border-dashed rounded-xl hover:border-blue-500 transition"
          >
            <div class="space-y-1 text-center">
              <span class="text-4xl">📷</span>
              <div class="flex text-sm text-gray-600">
                <label
                  class="relative cursor-pointer bg-white rounded-md font-medium text-blue-600 hover:text-blue-500 focus-within:outline-hidden"
                >
                  <span>Tải ảnh lên</span>
                  <input
                    type="file"
                    multiple
                    accept="image/*"
                    class="sr-only"
                    @change="handleFileChange"
                  />
                </label>
              </div>
              <p class="text-xs text-gray-500">PNG, JPG tối đa 5MB</p>
              <p v-if="selectedFiles.length > 0" class="text-sm text-green-600 font-medium">
                Đã chọn {{ selectedFiles.length }} ảnh
              </p>
            </div>
          </div>
        </div>

        <div
          v-if="errorMessage"
          class="p-4 bg-red-50 border border-red-200 rounded-xl text-sm text-red-600 text-center font-medium"
        >
          {{ errorMessage }}
        </div>

        <button
          type="submit"
          :disabled="isLoading"
          class="w-full bg-blue-600 hover:bg-blue-700 text-white font-bold py-3 px-4 rounded-xl shadow-md transition disabled:bg-blue-400 cursor-pointer"
        >
          {{ isLoading ? 'Đang xử lý...' : 'Đăng tin ngay' }}
        </button>
      </form>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive } from 'vue'
import { useRouter } from 'vue-router'
import { propertyService } from '@/services/property.service'

const router = useRouter()
const isLoading = ref(false)
const errorMessage = ref<string | null>(null)
const selectedFiles = ref<File[]>([])

const formData = reactive({
  title: '',
  description: '',
  address: '',
  pricePerNight: 0,
  maxGuests: 1,
})

const handleFileChange = (event: Event) => {
  const target = event.target as HTMLInputElement
  if (target.files) {
    selectedFiles.value = Array.from(target.files)
  }
}

const handleSubmit = async () => {
  isLoading.value = true
  errorMessage.value = null
  try {
    // 1. Tạo Property trước
    const newProperty = await propertyService.createProperty(formData)
    const propertyId = newProperty.id

    // 2. Nếu có file thì upload ảnh
    if (selectedFiles.value.length > 0) {
      await propertyService.uploadImages(propertyId, selectedFiles.value)
    }

    router.push({ name: 'home' })
  } catch (error: unknown) {
    if (error instanceof Error) {
      errorMessage.value = error.message
    } else {
      errorMessage.value = 'Có lỗi xảy ra trong quá trình đăng tin.'
    }
  } finally {
    isLoading.value = false
  }
}
</script>
