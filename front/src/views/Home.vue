<template>
  <div class="min-h-screen flex items-center justify-center px-4">
    <div class="max-w-4xl w-full text-center">
      <!-- Content for authenticated users -->
      <div v-if="authStore.isAuthenticated" class="grid md:grid-cols-2 gap-6">
        <router-link
          v-if="authStore.userType === 'reader'"
          to="/reader/catalog"
          class="card hover:shadow-xl transition-shadow cursor-pointer"
        >
          <div class="text-4xl mb-4">📖</div>
          <h3 class="text-xl font-bold text-primary-600 mb-2">Каталог книг</h3>
          <p class="text-dark-700">Просмотр и поиск книг в библиотеке</p>
        </router-link>

        <router-link
          v-if="authStore.userType === 'staff'"
          to="/staff/dashboard"
          class="card hover:shadow-xl transition-shadow cursor-pointer"
        >
          <div class="text-4xl mb-4">📊</div>
          <h3 class="text-xl font-bold text-primary-600 mb-2">Панель управления</h3>
          <p class="text-dark-700">Статистика и управление библиотекой</p>
        </router-link>

        <router-link
          :to="authStore.userType === 'reader' ? '/reader/loans' : '/staff/readers'"
          class="card hover:shadow-xl transition-shadow cursor-pointer"
        >
          <div class="text-4xl mb-4">{{ authStore.userType === 'reader' ? '📚' : '👥' }}</div>
          <h3 class="text-xl font-bold text-primary-600 mb-2">
            {{ authStore.userType === 'reader' ? 'Мои книги' : 'Читатели' }}
          </h3>
          <p class="text-dark-700">
            {{ authStore.userType === 'reader' ? 'Текущие выдачи и история' : 'Управление читателями' }}
          </p>
        </router-link>
      </div>

      <!-- Content for unauthenticated users -->
      <div v-else class="space-y-8">
        <div class="text-6xl mb-6">📚</div>
        <h1 class="text-4xl font-bold text-primary-600 mb-4">LibraSmart</h1>
        <p class="text-xl text-dark-700 mb-8">Система управления библиотекой</p>

        <div class="grid md:grid-cols-3 gap-6 mb-8">
          <div class="card">
            <div class="text-3xl mb-3">📖</div>
            <h3 class="font-semibold text-primary-600 mb-2">Каталог книг</h3>
            <p class="text-sm text-dark-600">Удобный поиск и просмотр доступных книг</p>
          </div>
          <div class="card">
            <div class="text-3xl mb-3">📅</div>
            <h3 class="font-semibold text-primary-600 mb-2">Бронирование</h3>
            <p class="text-sm text-dark-600">Резервирование книг онлайн</p>
          </div>
          <div class="card">
            <div class="text-3xl mb-3">📊</div>
            <h3 class="font-semibold text-primary-600 mb-2">История</h3>
            <p class="text-sm text-dark-600">Отслеживание выдач и возвратов</p>
          </div>
        </div>

        <button
          @click="showLoginModal = true"
          class="btn btn-primary text-lg px-8 py-3"
        >
          Войти в систему
        </button>
      </div>
    </div>

    <LoginModal :isOpen="showLoginModal" @close="showLoginModal = false" />
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useAuthStore } from '../stores/auth'
import LoginModal from '../components/LoginModal.vue'

const authStore = useAuthStore()
const showLoginModal = ref(false)

onMounted(() => {
  if (!authStore.isAuthenticated) {
    showLoginModal.value = true
  }
})
</script>
