<template>
  <div class="layout-wrapper">
    <header class="app-header">
      <div class="logo">
        <h2>MyLogo</h2>
      </div>
      <div class="auth-actions">
        <template v-if="!authStore.isAuthenticated">
          <button class="login-trigger-btn" @click="isAuthModalOpen = true">
            Đăng nhập / Đăng ký
          </button>
        </template>
        <template v-else>
          <span class="welcome-text">Xin chào, {{ authStore.user?.name }}</span>
          <button class="logout-btn" @click="handleLogout">Đăng xuất</button>
        </template>
      </div>
    </header>

    <main class="main-content flex-grow">
      <RouterView />
    </main>

    <footer class="app-footer">
      <p>&copy; 2026 My Application. All rights reserved.</p>
    </footer>

    <AuthModal v-model="isAuthModalOpen" />
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { RouterView } from 'vue-router'
import { useAuthStore } from '../stores/auth.store'
import AuthModal from '../components/common/AuthModal.vue'

const authStore = useAuthStore()
const isAuthModalOpen = ref(false)

const handleLogout = () => {
  authStore.logout()
}
</script>

<style scoped>
.layout-wrapper {
  display: flex;
  flex-direction: column;
  min-height: 100vh;
}

.app-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1rem 2rem;
  background-color: #ffffff;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
}

.logo h2 {
  margin: 0;
  color: #111827;
}

.auth-actions {
  display: flex;
  align-items: center;
  gap: 1rem;
}

.welcome-text {
  font-weight: 500;
  color: #374151;
}

.login-trigger-btn {
  background-color: #3b82f6;
  color: white;
  border: none;
  padding: 0.5rem 1rem;
  border-radius: 4px;
  cursor: pointer;
  font-weight: 600;
}

.logout-btn {
  background-color: transparent;
  color: #ef4444;
  border: 1px solid #ef4444;
  padding: 0.5rem 1rem;
  border-radius: 4px;
  cursor: pointer;
  font-weight: 600;
}

.logout-btn:hover {
  background-color: #fef2f2;
}

.main-content {
  flex-grow: 1;
  padding: 2rem;
  background-color: #f9fafb;
}

.flex-grow {
  flex-grow: 1;
}

.app-footer {
  text-align: center;
  padding: 1rem;
  background-color: #1f2937;
  color: #9ca3af;
}
</style>
