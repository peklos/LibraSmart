<template>
  <div class="min-h-screen flex items-center justify-center px-4">
    <div class="max-w-4xl w-full text-center">
      <div class="text-6xl mb-6">📚</div>
      <h1 class="text-5xl font-bold text-primary-600 mb-4">LibraSmart</h1>
      <p class="text-xl text-dark-700 mb-8">
        Современная система управления библиотекой
      </p>

      <div v-if="!authStore.isAuthenticated" class="space-y-4">
        <button @click="showLoginModal = true" class="btn btn-primary inline-block text-lg px-8 py-3">
          Войти в систему
        </button>
      </div>

      <div v-else class="grid md:grid-cols-2 gap-6">
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
    </div>

    <LoginModal :isOpen="showLoginModal" @close="showLoginModal = false" />
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useAuthStore } from '../stores/auth'
import LoginModal from '../components/LoginModal.vue'

const authStore = useAuthStore()
const showLoginModal = ref(false)
</script>
