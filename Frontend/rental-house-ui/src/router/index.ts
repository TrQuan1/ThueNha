import { createRouter, createWebHistory } from 'vue-router'
import MainLayout from '@/layouts/MainLayout.vue'

// Khai báo mở rộng RouteMeta để đảm bảo TypeScript kiểm soát chặt chẽ kiểu dữ liệu
declare module 'vue-router' {
  interface RouteMeta {
    requiresAuth?: boolean
    allowedRoles?: number[]
  }
}

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      component: MainLayout,
      children: [
        {
          path: '',
          name: 'home',
          component: () => import('@/views/properties/HomeView.vue'),
        },
        {
          path: 'properties/:id',
          name: 'property-detail',
          component: () => import('@/views/properties/PropertyDetailView.vue'),
        },
        {
          path: 'host/properties/create',
          name: 'property-create',
          component: () => import('@/views/properties/CreatePropertyView.vue'),
          meta: { requiresAuth: true, allowedRoles: [2] },
        },
        {
          path: 'host/bookings',
          name: 'host-bookings',
          component: () => import('@/views/properties/HostBookingsView.vue'),
          meta: { requiresAuth: true, allowedRoles: [2] },
        },
        {
          path: 'my-bookings',
          name: 'my-bookings',
          component: () => import('@/views/properties/TenantBookingsView.vue'),
          meta: { requiresAuth: true, allowedRoles: [3] },
        },
      ],
    },
  ],
})

// Global Before Guard xử lý phân quyền theo chuẩn Vue Router 4 (bỏ hoàn toàn callback next)
router.beforeEach((to) => {
  const token = localStorage.getItem('token')
  const userStr = localStorage.getItem('user')

  // 1. Kiểm tra nếu trang yêu cầu quyền đăng nhập
  if (to.meta.requiresAuth) {
    if (!token || !userStr) {
      alert('Vui lòng đăng nhập để tiếp tục.')
      return '/'
    }

    // 2. Kiểm tra danh sách quyền được phép truy cập (allowedRoles)
    if (to.meta.allowedRoles && to.meta.allowedRoles.length > 0) {
      try {
        const user = JSON.parse(userStr) as { role: string | number }
        const userRole = Number(user.role)

        if (!to.meta.allowedRoles.includes(userRole)) {
          alert('Bạn không có quyền truy cập trang này.')
          return '/'
        }
      } catch {
        alert('Lỗi cấu trúc dữ liệu người dùng.')
        return '/'
      }
    }
  }

  // Cho phép điều hướng đi tiếp hợp lệ
  return true
})

export default router
