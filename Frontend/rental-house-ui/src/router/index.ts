import { createRouter, createWebHistory } from 'vue-router'
import MainLayout from '@/layouts/MainLayout.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      component: MainLayout,
      children: [
        // {
        //   path: '',
        //   name: 'home',
        //   component: () => import('@/views/HomeView.vue'),
        // },
        {
          path: 'login',
          name: 'login',
          component: () => import('@/views/auth/LoginView.vue'),
        },
        // {
        //   path: 'register',
        //   name: 'register',
        //   component: RegisterView,
        // },
      ],
    },
  ],
})

export default router
