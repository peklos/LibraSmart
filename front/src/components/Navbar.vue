<template>
  <nav class="fixed top-0 left-0 right-0 bg-dark-100 border-b border-dark-200 z-50">
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
      <div class="flex justify-between h-16">
        <div class="flex items-center">
          <router-link to="/" class="flex items-center space-x-3">
            <div class="text-2xl">📚</div>
            <span class="text-xl font-bold text-primary-600">LibraSmart</span>
          </router-link>

          <!-- Reader Menu -->
          <div v-if="authStore.userType === 'reader'" class="hidden md:flex ml-10 space-x-4">
            <router-link
              to="/reader/catalog"
              class="px-3 py-2 rounded-md text-sm font-medium hover:bg-dark-200 transition"
              :class="isActive('/reader/catalog') ? 'bg-dark-200 text-primary-600' : 'text-dark-800'"
            >
              Каталог
            </router-link>
            <router-link
              to="/reader/reservations"
              class="px-3 py-2 rounded-md text-sm font-medium hover:bg-dark-200 transition"
              :class="isActive('/reader/reservations') ? 'bg-dark-200 text-primary-600' : 'text-dark-800'"
            >
              Бронирования
            </router-link>
            <router-link
              to="/reader/loans"
              class="px-3 py-2 rounded-md text-sm font-medium hover:bg-dark-200 transition"
              :class="isActive('/reader/loans') ? 'bg-dark-200 text-primary-600' : 'text-dark-800'"
            >
              Мои книги
            </router-link>
            <router-link
              to="/reader/history"
              class="px-3 py-2 rounded-md text-sm font-medium hover:bg-dark-200 transition"
              :class="isActive('/reader/history') ? 'bg-dark-200 text-primary-600' : 'text-dark-800'"
            >
              История
            </router-link>
          </div>

          <!-- Staff Menu -->
          <div v-if="authStore.userType === 'staff'" class="hidden md:flex ml-10 space-x-4">
            <router-link
              to="/staff/dashboard"
              class="px-3 py-2 rounded-md text-sm font-medium hover:bg-dark-200 transition"
              :class="isActive('/staff/dashboard') ? 'bg-dark-200 text-primary-600' : 'text-dark-800'"
            >
              Панель
            </router-link>
            <router-link
              to="/staff/readers"
              class="px-3 py-2 rounded-md text-sm font-medium hover:bg-dark-200 transition"
              :class="isActive('/staff/readers') ? 'bg-dark-200 text-primary-600' : 'text-dark-800'"
            >
              Читатели
            </router-link>
            <router-link
              to="/staff/books"
              class="px-3 py-2 rounded-md text-sm font-medium hover:bg-dark-200 transition"
              :class="isActive('/staff/books') ? 'bg-dark-200 text-primary-600' : 'text-dark-800'"
            >
              Книги
            </router-link>
            <router-link
              to="/staff/loans"
              class="px-3 py-2 rounded-md text-sm font-medium hover:bg-dark-200 transition"
              :class="isActive('/staff/loans') ? 'bg-dark-200 text-primary-600' : 'text-dark-800'"
            >
              Выдачи
            </router-link>
          </div>
        </div>

        <div class="flex items-center space-x-4">
          <div class="text-sm">
            <div class="font-medium text-dark-900">{{ user?.full_name }}</div>
            <div class="text-xs text-dark-500">{{ userTypeLabel }}</div>
          </div>
          <button @click="handleLogout" class="btn btn-secondary text-sm">
            Выйти
          </button>
        </div>
      </div>
    </div>
  </nav>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { useAuthStore } from '../stores/auth'

const router = useRouter()
const route = useRoute()
const authStore = useAuthStore()

const user = computed(() => authStore.user)
const userTypeLabel = computed(() =>
  authStore.userType === 'reader' ? 'Читатель' : 'Сотрудник'
)

const isActive = (path: string) => route.path.startsWith(path)

const handleLogout = () => {
  authStore.logout()
  router.push('/login')
}
</script>
