<template>
  <div class="max-w-7xl mx-auto py-8 px-4 sm:px-6 lg:px-8">
    <div class="mb-8">
      <h1 class="text-3xl font-bold text-gray-900">Quản lý người dùng</h1>
      <p class="mt-2 text-gray-600">
        Admin dashboard - Quản lý tài khoản và phân quyền người dùng.
      </p>
    </div>

    <div class="mb-6 flex gap-4">
      <input
        v-model="searchQuery"
        @input="loadUsers"
        placeholder="Tìm kiếm theo tên hoặc email..."
        class="w-full md:w-1/3 p-3 border border-gray-300 rounded-xl focus:ring-2 focus:ring-blue-500 outline-none transition"
      />
    </div>

    <div v-if="isLoading" class="text-center py-10 text-gray-500">
      Đang tải danh sách người dùng...
    </div>

    <div v-else class="bg-white shadow-sm rounded-2xl border border-gray-200 overflow-hidden">
      <table class="min-w-full divide-y divide-gray-200">
        <thead class="bg-gray-50">
          <tr>
            <th class="px-6 py-3 text-left text-xs font-bold text-gray-500 uppercase">ID</th>
            <th class="px-6 py-3 text-left text-xs font-bold text-gray-500 uppercase">Họ tên</th>
            <th class="px-6 py-3 text-left text-xs font-bold text-gray-500 uppercase">Email</th>
            <th class="px-6 py-3 text-left text-xs font-bold text-gray-500 uppercase">Vai trò</th>
            <th class="px-6 py-3 text-left text-xs font-bold text-gray-500 uppercase">
              Trạng thái
            </th>
            <th class="px-6 py-3 text-right text-xs font-bold text-gray-500 uppercase">
              Hành động
            </th>
          </tr>
        </thead>
        <tbody class="divide-y divide-gray-200">
          <tr v-for="user in users" :key="user.id" class="hover:bg-gray-50 transition">
            <td class="px-6 py-4 text-sm text-gray-900">{{ user.id }}</td>
            <td class="px-6 py-4 text-sm text-gray-900 font-medium">{{ user.fullName }}</td>
            <td class="px-6 py-4 text-sm text-gray-600">{{ user.email }}</td>
            <td class="px-6 py-4 text-sm">
              <span class="px-2 py-1 bg-gray-100 rounded-md text-xs font-semibold">
                {{ getRoleText(user.role) }}
              </span>
            </td>
            <td class="px-6 py-4 text-sm">
              <span
                :class="user.status === 1 ? 'text-green-600 font-bold' : 'text-red-600 font-bold'"
              >
                {{ getStatusText(user.status) }}
              </span>
            </td>
            <td class="px-6 py-4 text-right text-sm">
              <button
                v-if="user.status === 1"
                @click="handleBan(user.id)"
                class="text-red-600 hover:text-red-800 font-semibold cursor-pointer"
              >
                Khóa
              </button>
              <button
                v-else
                @click="handleUnban(user.id)"
                class="text-green-600 hover:text-green-800 font-semibold cursor-pointer"
              >
                Mở khóa
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
import { userService } from '@/services/user.service'
import type { User } from '@/types/auth'

const users = ref<User[]>([])
const searchQuery = ref('')
const isLoading = ref(false)

// Logic chuyển đổi số sang chữ (Dựa trên Enum của bạn)
const getStatusText = (status: number) => {
  // Thay đổi các con số (1, 2) cho khớp với Enum UserStatus bên Backend của bạn
  const statusMap: Record<number, string> = {
    1: 'Active',
    2: 'Banned',
  }
  return statusMap[status] || 'Unknown'
}

const getRoleText = (role: number) => {
  // Thay đổi các con số (1, 2, 3) cho khớp với Enum Role bên Backend của bạn
  const roleMap: Record<number, string> = {
    1: 'Admin',
    2: 'Host',
    3: 'Tenant',
  }
  return roleMap[role] || 'Unknown'
}

const loadUsers = async () => {
  isLoading.value = true
  try {
    users.value = await userService.getUsers(searchQuery.value)
  } catch (err) {
    console.error(err)
    alert('Không thể tải danh sách người dùng.')
  } finally {
    isLoading.value = false
  }
}

const handleBan = async (id: number | string) => {
  if (!confirm('Bạn có chắc chắn muốn KHÓA tài khoản này?')) return
  try {
    await userService.banUser(id)
    alert('Đã khóa tài khoản thành công!')
    await loadUsers()
  } catch {
    alert('Có lỗi xảy ra khi khóa tài khoản.')
  }
}

const handleUnban = async (id: number | string) => {
  if (!confirm('Bạn có chắc chắn muốn MỞ KHÓA tài khoản này?')) return
  try {
    await userService.unbanUser(id)
    alert('Đã mở khóa tài khoản thành công!')
    await loadUsers()
  } catch {
    alert('Có lỗi xảy ra khi mở khóa tài khoản.')
  }
}

onMounted(loadUsers)
</script>
