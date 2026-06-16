<template>
  <div class="max-w-7xl mx-auto py-8 px-4 sm:px-6 lg:px-8">
    <div class="flex justify-between items-center mb-8">
      <div>
        <h1 class="text-3xl font-extrabold text-gray-900 tracking-tight">Duyệt tin đăng nhà</h1>
        <p class="mt-2 text-sm text-gray-600">Quản lý và kiểm duyệt các bài đăng mới từ chủ nhà.</p>
      </div>
    </div>

    <div v-if="isLoading" class="text-center py-12 text-gray-500 font-medium">
      Đang tải danh sách chờ duyệt...
    </div>

    <div v-else-if="error" class="text-center py-12 text-red-500 font-medium bg-red-50 rounded-xl">
      {{ error }}
    </div>

    <div
      v-else-if="properties.length === 0"
      class="text-center py-16 bg-white rounded-2xl shadow-sm border border-gray-200"
    >
      <span class="text-5xl block mb-4">📭</span>
      <h3 class="text-xl font-bold text-gray-900 mb-2">Không có tin đăng nào cần duyệt</h3>
      <p class="text-gray-500">Tất cả các bài đăng đã được xử lý.</p>
    </div>

    <div v-else class="overflow-x-auto bg-white rounded-2xl shadow-sm border border-gray-200">
      <table class="min-w-full divide-y divide-gray-200">
        <thead class="bg-gray-50">
          <tr>
            <th
              scope="col"
              class="px-6 py-4 text-left text-xs font-bold text-gray-500 uppercase tracking-wider"
            >
              ID
            </th>
            <th
              scope="col"
              class="px-6 py-4 text-left text-xs font-bold text-gray-500 uppercase tracking-wider"
            >
              Tiêu đề / Địa chỉ
            </th>
            <th
              scope="col"
              class="px-6 py-4 text-left text-xs font-bold text-gray-500 uppercase tracking-wider"
            >
              Chủ nhà (Host ID)
            </th>
            <th
              scope="col"
              class="px-6 py-4 text-left text-xs font-bold text-gray-500 uppercase tracking-wider"
            >
              Giá / Đêm
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
          <tr v-for="property in properties" :key="property.id" class="hover:bg-gray-50 transition">
            <td class="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">
              #{{ property.id }}
            </td>
            <td class="px-6 py-4 text-sm text-gray-900">
              <div
                class="font-bold text-blue-600 hover:underline cursor-pointer"
                @click="goToDetail(property.id)"
              >
                {{ property.title }}
              </div>
              <div class="text-gray-500 text-xs mt-1 truncate max-w-xs">{{ property.address }}</div>
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-600">{{ property.hostId }}</td>
            <td class="px-6 py-4 whitespace-nowrap text-sm font-bold text-gray-900">
              {{ property.pricePerNight.toLocaleString() }} đ
            </td>
            <td
              class="px-6 py-4 whitespace-nowrap text-right text-sm font-medium space-x-2 flex justify-end"
            >
              <button
                @click="handleApprove(property.id)"
                :disabled="isProcessing"
                class="bg-green-100 text-green-700 hover:bg-green-500 hover:text-white px-3 py-1.5 rounded-lg font-semibold transition disabled:opacity-50"
              >
                ✅ Duyệt
              </button>
              <button
                @click="handleReject(property.id)"
                :disabled="isProcessing"
                class="bg-red-100 text-red-700 hover:bg-red-500 hover:text-white px-3 py-1.5 rounded-lg font-semibold transition disabled:opacity-50"
              >
                ❌ Từ chối
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
import { propertyService } from '@/services/property.service'
import type { Property } from '@/types/property'

const properties = ref<Property[]>([])
const isLoading = ref(true)
const error = ref<string | null>(null)
const isProcessing = ref(false)

const fetchPendingProperties = async () => {
  isLoading.value = true
  error.value = null
  try {
    properties.value = await propertyService.getPendingProperties()
  } catch (err: unknown) {
    const errorObj = err as { response?: { data?: { message?: string } } }
    error.value = errorObj.response?.data?.message || 'Không thể tải danh sách nhà chờ duyệt.'
  } finally {
    isLoading.value = false
  }
}

onMounted(() => {
  fetchPendingProperties()
})

const goToDetail = (id: string | number) => {
  // Có thể mở sang tab mới hoặc chuyển hướng để Admin xem chi tiết trước khi duyệt
  window.open(`/properties/${id}`, '_blank')
}

const handleApprove = async (id: string | number) => {
  if (!confirm(`Bạn có chắc chắn muốn DUYỆT bài đăng #${id} không?`)) return

  isProcessing.value = true
  try {
    await propertyService.approveProperty(id)
    alert('Đã duyệt bài đăng thành công!')
    await fetchPendingProperties() // Tải lại danh sách
  } catch (err: unknown) {
    const errorObj = err as { response?: { data?: { message?: string } } }
    alert(errorObj.response?.data?.message || 'Có lỗi xảy ra khi duyệt bài đăng.')
  } finally {
    isProcessing.value = false
  }
}

const handleReject = async (id: string | number) => {
  const reason = prompt('Vui lòng nhập lý do từ chối bài đăng này:')

  // Tránh trường hợp người dùng bấm Cancel hoặc để trống
  if (reason === null) return
  if (reason.trim() === '') {
    alert('Bạn phải nhập lý do từ chối!')
    return
  }

  isProcessing.value = true
  try {
    await propertyService.rejectProperty(id, reason.trim())
    alert('Đã từ chối bài đăng thành công!')
    await fetchPendingProperties() // Tải lại danh sách
  } catch (err: unknown) {
    const errorObj = err as { response?: { data?: { message?: string } } }
    alert(errorObj.response?.data?.message || 'Có lỗi xảy ra khi từ chối bài đăng.')
  } finally {
    isProcessing.value = false
  }
}
</script>
