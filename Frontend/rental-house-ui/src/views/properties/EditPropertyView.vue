<template>
  <div class="min-h-screen bg-gray-50 py-12 px-4 sm:px-6 lg:px-8">
    <div class="max-w-3xl mx-auto bg-white rounded-2xl shadow-xl p-8 border border-gray-100">
      <div class="text-center mb-8">
        <h1 class="text-3xl font-extrabold text-gray-900 tracking-tight">Cập Nhật Thông Tin Nhà</h1>
        <p class="mt-2 text-sm text-gray-600">
          Sửa đổi các thông tin chi tiết để thu hút thêm nhiều khách thuê.
        </p>
      </div>

      <div v-if="isLoadingData" class="text-center py-10 text-gray-500">
        Đang tải dữ liệu căn nhà...
      </div>

      <form v-else @submit.prevent="handleSubmit" class="space-y-6">
        <div>
          <label class="block text-sm font-semibold text-gray-700 mb-2">Tiêu đề bài đăng</label>
          <input
            v-model="formData.title"
            type="text"
            required
            class="w-full px-4 py-3 rounded-xl border border-gray-300 shadow-xs focus:ring-2 focus:ring-blue-500 outline-hidden transition"
          />
        </div>

        <div>
          <label class="block text-sm font-semibold text-gray-700 mb-2">Địa chỉ chính xác</label>
          <input
            v-model="formData.address"
            type="text"
            required
            class="w-full px-4 py-3 rounded-xl border border-gray-300 shadow-xs focus:ring-2 focus:ring-blue-500 outline-hidden transition"
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
              class="w-full px-4 py-3 rounded-xl border border-gray-300 shadow-xs focus:ring-2 focus:ring-blue-500 outline-hidden transition"
            />
          </div>
          <div>
            <label class="block text-sm font-semibold text-gray-700 mb-2">Số khách tối đa</label>
            <input
              v-model.number="formData.maxGuests"
              type="number"
              min="1"
              required
              class="w-full px-4 py-3 rounded-xl border border-gray-300 shadow-xs focus:ring-2 focus:ring-blue-500 outline-hidden transition"
            />
          </div>
        </div>

        <div class="pt-4 border-t border-gray-100">
          <label class="block text-sm font-semibold text-gray-900 mb-4">Tiện ích sẵn có</label>
          <div class="grid grid-cols-2 sm:grid-cols-3 gap-4">
            <label
              v-for="facility in availableFacilities"
              :key="facility.id"
              class="flex items-center gap-3 p-3 border border-gray-200 rounded-xl cursor-pointer hover:bg-blue-50 transition select-none has-checked:border-blue-500 has-checked:bg-blue-50"
            >
              <input
                type="checkbox"
                :value="facility.id"
                v-model="formData.facilityIds"
                class="w-5 h-5 text-blue-600 border-gray-300 rounded focus:ring-blue-500 cursor-pointer"
              />
              <span class="text-gray-700 text-sm font-medium flex items-center gap-2">
                <span
                  v-if="facility.icon"
                  v-html="facility.icon"
                  class="w-5 h-5 flex items-center justify-center"
                ></span>
                <span v-else>✨</span>
                {{ facility.name }}
              </span>
            </label>
          </div>
        </div>

        <div>
          <label class="block text-sm font-semibold text-gray-700 mb-2">Mô tả chi tiết</label>
          <textarea
            v-model="formData.description"
            rows="4"
            required
            class="w-full px-4 py-3 rounded-xl border border-gray-300 shadow-xs focus:ring-2 focus:ring-blue-500 outline-hidden transition resize-none"
          ></textarea>
        </div>

        <div>
          <label class="block text-sm font-semibold text-gray-700 mb-2"
            >Thêm hình ảnh mới (Không bắt buộc)</label
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
              <p v-if="selectedFiles.length > 0" class="text-sm text-green-600 font-medium">
                Đã chọn {{ selectedFiles.length }} ảnh mới
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
          :disabled="isSubmitting"
          class="w-full bg-blue-600 hover:bg-blue-700 text-white font-bold py-3.5 px-4 rounded-xl shadow-md transition disabled:bg-blue-400 cursor-pointer"
        >
          {{ isSubmitting ? 'Đang lưu thay đổi...' : 'Cập nhật tin ngay' }}
        </button>
      </form>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { propertyService } from '@/services/property.service'
import { facilityService } from '@/services/facility.service'
import type { Facility, CreatePropertyRequest } from '@/types/property'

const router = useRouter()
const route = useRoute()

const isLoadingData = ref(true)
const isSubmitting = ref(false)
const errorMessage = ref<string | null>(null)
const selectedFiles = ref<File[]>([])
const availableFacilities = ref<Facility[]>([])

const formData = reactive<CreatePropertyRequest>({
  title: '',
  description: '',
  address: '',
  pricePerNight: 0,
  maxGuests: 1,
  facilityIds: [],
})

const handleFileChange = (event: Event) => {
  const target = event.target as HTMLInputElement
  if (target.files) selectedFiles.value = Array.from(target.files)
}

// Nạp dữ liệu lúc khởi tạo
onMounted(async () => {
  const propertyId = route.params.id as string
  isLoadingData.value = true
  try {
    // Gọi song song để tăng tốc độ load
    const [facilities, property] = await Promise.all([
      facilityService.getAllFacilities(),
      propertyService.getPropertyById(propertyId),
    ])

    availableFacilities.value = facilities

    // Gán dữ liệu cũ vào Form
    formData.title = property.title
    formData.description = property.description
    formData.address = property.address
    formData.pricePerNight = property.pricePerNight
    formData.maxGuests = property.maxGuests

    // Tự động check các tiện ích cũ
    if (property.facilities) {
      formData.facilityIds = property.facilities.map((f) => Number(f.id))
    }
  } catch (error) {
    console.error(error)
    errorMessage.value = 'Lỗi khi tải dữ liệu căn nhà.'
  } finally {
    isLoadingData.value = false
  }
})

const handleSubmit = async () => {
  const propertyId = route.params.id as string
  isSubmitting.value = true
  errorMessage.value = null

  try {
    // Update thông tin
    await propertyService.updateProperty(propertyId, formData)

    // Upload thêm ảnh mới nếu có
    if (selectedFiles.value.length > 0) {
      await propertyService.uploadImages(propertyId, selectedFiles.value)
    }

    // Quay lại trang chi tiết sau khi sửa xong
    router.push(`/properties/${propertyId}`)
  } catch (error: unknown) {
    const err = error as { response?: { data?: { message?: string } } }
    errorMessage.value = err.response?.data?.message || 'Có lỗi xảy ra trong quá trình cập nhật.'
  } finally {
    isSubmitting.value = false
  }
}
</script>
