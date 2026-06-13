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
      ],
    },
  ],
})

export default router
