<template>
  <div class="max-w-3xl mx-auto py-10 px-4 sm:px-6 lg:px-8">
    <div class="bg-white shadow-xl rounded-2xl overflow-hidden border border-gray-100">
      <div class="bg-gray-50 px-8 py-6 border-b border-gray-100">
        <h1 class="text-2xl font-extrabold text-gray-900">Thông tin cá nhân</h1>
        <p class="text-sm text-gray-500 mt-1">
          Cập nhật thông tin cơ bản và phương thức liên hệ của bạn.
        </p>
      </div>

      <div v-if="isFetching" class="p-12 text-center text-gray-500">Đang tải dữ liệu...</div>

      <div v-else class="p-8">
        <div
          v-if="alertMessage"
          :class="[
            'mb-6 p-4 rounded-xl text-sm font-medium border transition',
            alertType === 'success'
              ? 'bg-green-50 text-green-700 border-green-200'
              : 'bg-red-50 text-red-700 border-red-200',
          ]"
        >
          {{ alertMessage }}
        </div>

        <form @submit.prevent="handleSubmit" class="space-y-6">
          <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
            <div>
              <label class="block text-sm font-semibold text-gray-700 mb-2">Địa chỉ Email</label>
              <input
                type="email"
                :value="formData.email"
                disabled
                class="w-full px-4 py-3 rounded-xl border border-gray-200 bg-gray-100 text-gray-500 cursor-not-allowed"
              />
            </div>

            <div>
              <label class="block text-sm font-semibold text-gray-700 mb-2"
                >Vai trò tài khoản</label
              >
              <input
                type="text"
                :value="getRoleText(formData.role)"
                disabled
                class="w-full px-4 py-3 rounded-xl border border-gray-200 bg-gray-100 text-gray-500 font-medium cursor-not-allowed"
              />
            </div>
          </div>

          <div>
            <label class="block text-sm font-semibold text-gray-700 mb-2"
              >Họ và tên <span class="text-red-500">*</span></label
            >
            <input
              v-model="formData.fullName"
              type="text"
              required
              class="w-full px-4 py-3 rounded-xl border border-gray-300 focus:ring-2 focus:ring-blue-500 focus:border-blue-500 outline-none transition"
              placeholder="Nhập họ và tên..."
            />
          </div>

          <div>
            <label class="block text-sm font-semibold text-gray-700 mb-2"
              >Số điện thoại <span class="text-red-500">*</span></label
            >
            <input
              v-model="formData.phoneNumber"
              type="tel"
              required
              class="w-full px-4 py-3 rounded-xl border border-gray-300 focus:ring-2 focus:ring-blue-500 focus:border-blue-500 outline-none transition"
              placeholder="VD: 0912345678"
            />
          </div>

          <div class="pt-4 flex justify-end">
            <button
              type="submit"
              :disabled="isSubmitting"
              class="bg-blue-600 hover:bg-blue-700 text-white font-bold py-3 px-8 rounded-xl shadow-md transition disabled:bg-blue-400 disabled:cursor-not-allowed cursor-pointer"
            >
              {{ isSubmitting ? 'Đang lưu...' : 'Lưu thay đổi' }}
            </button>
          </div>
        </form>
      </div>
    </div>

    <div class="bg-white shadow-xl rounded-2xl overflow-hidden border border-gray-100 mt-8">
      <div class="bg-gray-50 px-8 py-6 border-b border-gray-100">
        <h2 class="text-xl font-extrabold text-gray-900">Đổi mật khẩu</h2>
        <p class="text-sm text-gray-500 mt-1">
          Đảm bảo tài khoản của bạn đang sử dụng mật khẩu dài và an toàn.
        </p>
      </div>

      <div class="p-8">
        <div
          v-if="passAlertMessage"
          :class="[
            'mb-6 p-4 rounded-xl text-sm font-medium border transition',
            passAlertType === 'success'
              ? 'bg-green-50 text-green-700 border-green-200'
              : 'bg-red-50 text-red-700 border-red-200',
          ]"
        >
          {{ passAlertMessage }}
        </div>

        <form @submit.prevent="handleChangePassword" class="space-y-6">
          <div>
            <label class="block text-sm font-semibold text-gray-700 mb-2"
              >Mật khẩu hiện tại <span class="text-red-500">*</span></label
            >
            <input
              v-model="passFormData.currentPassword"
              type="password"
              required
              class="w-full px-4 py-3 rounded-xl border border-gray-300 focus:ring-2 focus:ring-blue-500 focus:border-blue-500 outline-none transition"
              placeholder="Nhập mật khẩu hiện tại"
            />
          </div>

          <div>
            <label class="block text-sm font-semibold text-gray-700 mb-2"
              >Mật khẩu mới <span class="text-red-500">*</span></label
            >
            <input
              v-model="passFormData.newPassword"
              type="password"
              required
              minlength="6"
              class="w-full px-4 py-3 rounded-xl border border-gray-300 focus:ring-2 focus:ring-blue-500 focus:border-blue-500 outline-none transition"
              placeholder="Nhập mật khẩu mới (ít nhất 6 ký tự)"
            />
          </div>

          <div>
            <label class="block text-sm font-semibold text-gray-700 mb-2"
              >Xác nhận mật khẩu mới <span class="text-red-500">*</span></label
            >
            <input
              v-model="passFormData.confirmNewPassword"
              type="password"
              required
              minlength="6"
              class="w-full px-4 py-3 rounded-xl border border-gray-300 focus:ring-2 focus:ring-blue-500 focus:border-blue-500 outline-none transition"
              placeholder="Nhập lại mật khẩu mới"
            />
          </div>

          <div class="pt-4 flex justify-end">
            <button
              type="submit"
              :disabled="isChangingPassword"
              class="bg-gray-800 hover:bg-gray-900 text-white font-bold py-3 px-8 rounded-xl shadow-md transition disabled:bg-gray-400 disabled:cursor-not-allowed cursor-pointer"
            >
              {{ isChangingPassword ? 'Đang xử lý...' : 'Cập nhật mật khẩu' }}
            </button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { userService } from '@/services/user.service'
import { useAuthStore } from '@/stores/auth.store'
import type { ProfileDto } from '@/types/auth'

const authStore = useAuthStore()

// --- LOGIC: THÔNG TIN CÁ NHÂN ---
const isFetching = ref(true)
const isSubmitting = ref(false)
const alertMessage = ref<string | null>(null)
const alertType = ref<'success' | 'error'>('success')

const formData = reactive<ProfileDto>({
  id: 0,
  fullName: '',
  email: '',
  phoneNumber: '',
  role: 0,
})

const getRoleText = (roleId: number): string => {
  switch (roleId) {
    case 1:
      return 'Admin (Quản trị viên)'
    case 2:
      return 'Host (Chủ nhà)'
    case 3:
      return 'Tenant (Người thuê)'
    default:
      return 'Chưa phân quyền'
  }
}

const showAlert = (message: string, type: 'success' | 'error') => {
  alertMessage.value = message
  alertType.value = type
  setTimeout(() => {
    alertMessage.value = null
  }, 4000)
}

onMounted(async () => {
  try {
    const data = await userService.getProfile()
    formData.id = data.id
    formData.fullName = data.fullName
    formData.email = data.email
    formData.phoneNumber = data.phoneNumber || ''
    formData.role = data.role
  } catch {
    showAlert('Lỗi khi tải thông tin hồ sơ.', 'error')
  } finally {
    isFetching.value = false
  }
})

const handleSubmit = async () => {
  alertMessage.value = null
  formData.fullName = formData.fullName.trim()
  formData.phoneNumber = formData.phoneNumber ? formData.phoneNumber.trim() : ''

  if (formData.fullName.length < 2) {
    showAlert('Họ và tên phải dài ít nhất 2 ký tự.', 'error')
    return
  }

  const phoneRegex = /(84|0[3|5|7|8|9])+([0-9]{8})\b/
  if (!phoneRegex.test(formData.phoneNumber)) {
    showAlert('Vui lòng nhập đúng định dạng số điện thoại.', 'error')
    return
  }

  isSubmitting.value = true
  try {
    await userService.updateProfile({
      fullName: formData.fullName,
      phoneNumber: formData.phoneNumber,
    })

    authStore.updateCurrentUser(formData.fullName)
    showAlert('Hồ sơ cá nhân đã được lưu thành công!', 'success')
  } catch (error: unknown) {
    const err = error as { response?: { data?: { message?: string } } }
    showAlert(
      err.response?.data?.message || 'Có lỗi xảy ra, không thể cập nhật thông tin.',
      'error',
    )
  } finally {
    isSubmitting.value = false
  }
}

// --- 👉 THÊM MỚI LOGIC: ĐỔI MẬT KHẨU ---
const isChangingPassword = ref(false)
const passAlertMessage = ref<string | null>(null)
const passAlertType = ref<'success' | 'error'>('success')

const passFormData = reactive({
  currentPassword: '',
  newPassword: '',
  confirmNewPassword: '',
})

const showPassAlert = (message: string, type: 'success' | 'error') => {
  passAlertMessage.value = message
  passAlertType.value = type
  setTimeout(() => {
    passAlertMessage.value = null
  }, 4000)
}

const handleChangePassword = async () => {
  passAlertMessage.value = null

  if (passFormData.newPassword !== passFormData.confirmNewPassword) {
    showPassAlert('Mật khẩu mới và xác nhận mật khẩu không khớp.', 'error')
    return
  }

  if (passFormData.newPassword.length < 6) {
    showPassAlert('Mật khẩu mới phải có ít nhất 6 ký tự.', 'error')
    return
  }

  isChangingPassword.value = true
  try {
    await userService.changePassword({
      currentPassword: passFormData.currentPassword,
      newPassword: passFormData.newPassword,
      confirmNewPassword: passFormData.confirmNewPassword,
    })

    showPassAlert('Đổi mật khẩu thành công! Lần đăng nhập sau hãy dùng mật khẩu mới.', 'success')

    // Đổi xong thì làm rỗng các ô nhập liệu
    passFormData.currentPassword = ''
    passFormData.newPassword = ''
    passFormData.confirmNewPassword = ''
  } catch (error: unknown) {
    const err = error as { response?: { data?: { message?: string } } }
    showPassAlert(
      err.response?.data?.message || 'Mật khẩu hiện tại không chính xác hoặc có lỗi xảy ra.',
      'error',
    )
  } finally {
    isChangingPassword.value = false
  }
}
</script>
