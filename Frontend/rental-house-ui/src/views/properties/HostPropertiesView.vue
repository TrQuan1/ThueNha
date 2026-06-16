<template>
  <div class="max-w-7xl mx-auto py-8 px-4 sm:px-6 lg:px-8">
    <div class="flex justify-between items-center mb-8">
      <div>
        <h1 class="text-3xl font-extrabold text-gray-900 tracking-tight">Nhà của tôi</h1>
        <p class="mt-2 text-sm text-gray-600">Quản lý danh sách các căn nhà bạn đang cho thuê.</p>
      </div>
    </div>

    <div v-if="isLoading" class="text-center py-12 text-gray-500 font-medium">
      Đang tải danh sách nhà của bạn...
    </div>

    <div v-else-if="error" class="text-center py-12 text-red-500 font-medium bg-red-50 rounded-xl">
      {{ error }}
    </div>

    <div
      v-else-if="properties.length === 0"
      class="text-center py-16 bg-white rounded-2xl shadow-sm border border-gray-200"
    >
      <span class="text-5xl block mb-4">🏠</span>
      <h3 class="text-xl font-bold text-gray-900 mb-2">Bạn chưa đăng căn nhà nào</h3>
      <p class="text-gray-500 mb-6">
        Hãy bắt đầu chia sẻ không gian của bạn để đón những vị khách đầu tiên.
      </p>
    </div>

    <div v-else class="overflow-x-auto bg-white rounded-2xl shadow-sm border border-gray-200">
      <table class="min-w-full divide-y divide-gray-200">
        <thead class="bg-gray-50">
          <tr>
            <th
              scope="col"
              class="px-6 py-4 text-left text-xs font-bold text-gray-500 uppercase tracking-wider"
            >
              Hình ảnh
            </th>
            <th
              scope="col"
              class="px-6 py-4 text-left text-xs font-bold text-gray-500 uppercase tracking-wider"
            >
              Thông tin nhà
            </th>
            <th
              scope="col"
              class="px-6 py-4 text-left text-xs font-bold text-gray-500 uppercase tracking-wider"
            >
              Giá / Đêm
            </th>
            <th
              scope="col"
              class="px-6 py-4 text-left text-xs font-bold text-gray-500 uppercase tracking-wider"
            >
              Trạng thái
            </th>
            <th
              scope="col"
              class="px-6 py-4 text-right text-xs font-bold text-gray-500 uppercase tracking-wider"
            >
              Hành động
            </th>
          </tr>
        </thead>
        <tbody class="bg-white divide-y divide-gray-200">
          <tr v-for="prop in properties" :key="prop.id" class="hover:bg-gray-50 transition">
            <td class="px-6 py-4 whitespace-nowrap">
              <div class="w-16 h-16 rounded-lg overflow-hidden bg-gray-200">
                <img
                  :src="getFullImageUrl(prop.imageUrl)"
                  class="w-full h-full object-cover"
                  @error="
                    (e) =>
                      ((e.target as HTMLImageElement).src =
                        'https://placehold.co/100x100?text=No+Image')
                  "
                />
              </div>
            </td>
            <td class="px-6 py-4 text-sm text-gray-900">
              <div
                class="font-bold text-blue-600 hover:underline cursor-pointer"
                @click="router.push(`/properties/${prop.id}`)"
              >
                {{ prop.title }}
              </div>
              <div class="text-gray-500 text-xs mt-1 truncate max-w-xs">📍 {{ prop.address }}</div>
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm font-bold text-gray-900">
              {{ prop.pricePerNight.toLocaleString() }} đ
            </td>
            <td class="px-6 py-4 whitespace-nowrap">
              <span :class="getStatusClass(prop.status)">{{ getStatusText(prop.status) }}</span>
            </td>
            <td
              class="px-6 py-4 whitespace-nowrap text-right text-sm font-medium space-x-2 flex justify-end items-center h-full"
            >
              <button
                @click="router.push(`/properties/${prop.id}/edit`)"
                class="bg-yellow-100 text-yellow-700 hover:bg-yellow-500 hover:text-white px-3 py-1.5 rounded-lg font-semibold transition"
              >
                ✏️ Sửa
              </button>
              <button
                @click="handleDelete(prop.id)"
                :disabled="isProcessing"
                class="bg-red-100 text-red-700 hover:bg-red-500 hover:text-white px-3 py-1.5 rounded-lg font-semibold transition disabled:opacity-50"
              >
                🗑️ Xóa
              </button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { propertyService } from '@/services/property.service'
import type { Property } from '@/types/property'

const router = useRouter()
const properties = ref<Property[]>([])
const isLoading = ref(true)
const error = ref<string | null>(null)
const isProcessing = ref(false)

const BACKEND_URL = import.meta.env.VITE_API_URL || 'https://localhost:7023/'
const getFullImageUrl = (relativeUrl?: string) => {
  if (!relativeUrl) return 'https://placehold.co/100x100?text=No+Image'
  if (relativeUrl.startsWith('http')) return relativeUrl
  return `${BACKEND_URL.replace('/api', '')}${relativeUrl}`
}

const fetchMyProperties = async () => {
  isLoading.value = true
  error.value = null
  try {
    properties.value = await propertyService.getMyProperties()
  } catch (err: unknown) {
    const errorObj = err as { response?: { data?: { message?: string } } }
    error.value = errorObj.response?.data?.message || 'Không thể tải danh sách nhà của bạn.'
  } finally {
    isLoading.value = false
  }
}

onMounted(() => {
  fetchMyProperties()
})

const handleDelete = async (id: string | number) => {
  if (!confirm('Bạn có chắc chắn muốn xóa bài đăng này không? Thao tác này không thể hoàn tác.'))
    return

  isProcessing.value = true
  try {
    await propertyService.deleteProperty(id)
    alert('Đã xóa tin đăng thành công!')
    await fetchMyProperties()
  } catch (err: unknown) {
    const errorObj = err as { response?: { data?: { message?: string } } }
    alert(errorObj.response?.data?.message || 'Có lỗi xảy ra khi xóa bài đăng.')
  } finally {
    isProcessing.value = false
  }
}

// Cấu hình Helper Status (Tùy thuộc vào Enum Backend)
const getStatusText = (status?: string | number) => {
  if (status === 1 || status === 'Pending') return 'Chờ duyệt'
  if (status === 2 || status === 'Active') return 'Đang hoạt động'
  if (status === 3 || status === 'Rejected') return 'Bị từ chối'
  return 'Không xác định'
}

const getStatusClass = (status?: string | number) => {
  const baseClass = 'px-3 py-1 inline-flex text-xs leading-5 font-semibold rounded-full '
  if (status === 1 || status === 'Pending') return baseClass + 'bg-yellow-100 text-yellow-800'
  if (status === 2 || status === 'Active') return baseClass + 'bg-green-100 text-green-800'
  if (status === 3 || status === 'Rejected') return baseClass + 'bg-red-100 text-red-800'
  return baseClass + 'bg-gray-100 text-gray-800'
}
</script>
