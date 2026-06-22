import { createRouter, createWebHistory } from 'vue-router'
import MainLayout from '@/layouts/MainLayout.vue'

// Khai báo mở rộng RouteMeta để đảm bảo TypeScript kiểm soát chặt chẽ kiểu dữ liệu
declare module 'vue-router' {
  interface RouteMeta {
    requiresAuth?: boolean
    allowedRoles?: string[]
  }
}

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    // 2 Trang này để ra ngoài, không dính líu đến Header/Menu của MainLayout
    {
      path: '/login',
      name: 'login',
      component: () => import('@/views/auth/LoginView.vue'),
    },
    // Các trang chính của hệ thống
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
          meta: { requiresAuth: true, allowedRoles: ['Host'] },
        },
        {
          path: 'host/bookings',
          name: 'host-bookings',
          component: () => import('@/views/bookings/HostBookingsView.vue'),
          meta: { requiresAuth: true, allowedRoles: ['Host'] },
        },
        {
          path: 'my-bookings',
          name: 'my-bookings',
          component: () => import('@/views/bookings/TenantBookingsView.vue'),
          meta: { requiresAuth: true, allowedRoles: ['Tenant', 'Host'] },
        },
        {
          path: 'wishlist',
          name: 'wishlist',
          component: () => import('@/views/wishlists/WishlistView.vue'),
          meta: { requiresAuth: true, allowedRoles: ['Tenant'] },
        },
        {
          path: 'properties/:id/edit',
          name: 'property-edit',
          component: () => import('@/views/properties/EditPropertyView.vue'),
          meta: { requiresAuth: true, allowedRoles: ['Host'] },
        },
        {
          path: 'host/properties',
          name: 'host-properties',
          component: () => import('@/views/properties/HostPropertiesView.vue'),
          meta: { requiresAuth: true, allowedRoles: ['Host'] },
        },
        {
          path: '/host/dashboard',
          name: 'HostDashboard',
          component: () => import('@/views/host/HostDashboardView.vue'),
          meta: { requiresAuth: true },
        },
        {
          path: 'admin/properties',
          name: 'admin-properties',
          component: () => import('@/views/admin/AdminPropertiesView.vue'),
          meta: { requiresAuth: true, allowedRoles: ['Admin'] },
        },
        {
          path: '/admin/dashboard',
          name: 'admin-dashboard',
          component: () => import('@/views/admin/AdminDashboardView.vue'),
          meta: { requiresAuth: true, role: 'Admin' }, // 👉 Chặn không cho user thường vào
        },
        {
          path: '/admin/users',
          name: 'admin-users',
          component: () => import('@/views/admin/AdminUsersView.vue'),
          meta: { requiresAuth: true, allowedRoles: ['Admin'] },
        },
        {
          path: 'chat',
          name: 'chat',
          component: () => import('@/views/chat/ChatView.vue'),
          meta: {
            requiresAuth: true, // Bắt buộc đăng nhập mới được chat
          },
        },
        {
          path: 'account/profile',
          name: 'account-profile',
          component: () => import('@/views/account/ProfileView.vue'),
          meta: {
            requiresAuth: true,
          },
        },
      ],
    },
  ],
})

// Global Before Guard - Sạch sẽ và không còn tham số next()
router.beforeEach((to) => {
  const token = localStorage.getItem('token')
  const userStr = localStorage.getItem('user')

  if (to.meta.requiresAuth) {
    if (!token || !userStr) {
      alert('Vui lòng đăng nhập để tiếp tục.')
      return '/'
    }

    if (to.meta.allowedRoles && to.meta.allowedRoles.length > 0) {
      try {
        const user = JSON.parse(userStr) as { role: string }

        // Kiểm tra xem Role chữ (Host/Tenant) của user có nằm trong danh sách cho phép không
        if (!to.meta.allowedRoles.includes(user.role)) {
          alert('Bạn không có quyền truy cập trang này.')
          return '/'
        }
      } catch {
        alert('Lỗi cấu trúc dữ liệu người dùng.')
        return '/'
      }
    }
  }

  return true
})

export default router
