import { createRouter, createWebHistory } from 'vue-router'
import MainLayout from '@/layouts/MainLayout.vue'

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
        },
        {
          path: 'host/bookings',
          name: 'host-bookings',
          component: () => import('@/views/properties/HostBookingsView.vue'),
        },
      ],
    },
  ],
})

export default router
