<template>
  <div class="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
    <h1 class="text-3xl font-bold text-primary-600 mb-6">Мой профиль</h1>

    <!-- Profile Info Card -->
    <div class="card mb-6">
      <div class="flex items-start space-x-4 mb-6">
        <div class="w-20 h-20 bg-primary-100 rounded-full flex items-center justify-center flex-shrink-0">
          <span class="text-3xl">👤</span>
        </div>
        <div class="flex-1">
          <h2 class="text-2xl font-bold text-dark-900">{{ reader?.full_name }}</h2>
          <p class="text-dark-600">{{ reader?.email }}</p>
          <p class="text-dark-600">{{ reader?.phone }}</p>
          <div class="mt-2">
            <span class="badge badge-info">{{ reader?.library_card_number }}</span>
          </div>
        </div>
      </div>

      <!-- Stats -->
      <div v-if="stats" class="grid grid-cols-2 md:grid-cols-4 gap-4 mb-6 p-4 bg-dark-100 rounded-lg">
        <div class="text-center">
          <div class="text-2xl font-bold text-primary-600">{{ stats.total_books_read }}</div>
          <div class="text-xs text-dark-600">Книг прочитано</div>
        </div>
        <div class="text-center">
          <div class="text-2xl font-bold text-primary-600">{{ stats.active_loans }}</div>
          <div class="text-xs text-dark-600">Активных займов</div>
        </div>
        <div class="text-center">
          <div class="text-2xl font-bold text-primary-600">{{ stats.total_loans }}</div>
          <div class="text-xs text-dark-600">Всего займов</div>
        </div>
        <div class="text-center">
          <div class="text-2xl font-bold text-red-600">{{ stats.overdue_loans }}</div>
          <div class="text-xs text-dark-600">Просрочек</div>
        </div>
      </div>

      <!-- Favorite Genres -->
      <div v-if="stats && stats.favorite_genres && stats.favorite_genres.length > 0" class="mb-4">
        <h3 class="text-sm font-bold text-dark-700 mb-2">Любимые жанры</h3>
        <div class="flex flex-wrap gap-2">
          <span
            v-for="(genre, index) in stats.favorite_genres"
            :key="index"
            class="badge badge-success"
          >
            {{ genre.genre }} ({{ genre.count }})
          </span>
        </div>
      </div>
    </div>

    <!-- Edit Profile Card -->
    <div class="card">
      <h3 class="text-xl font-bold text-dark-900 mb-4">Редактировать профиль</h3>
      <form @submit.prevent="handleUpdate" class="space-y-4">
        <div>
          <label class="block text-sm font-medium text-dark-800 mb-1">ФИО</label>
          <input v-model="form.full_name" type="text" required class="input" />
        </div>
        <div>
          <label class="block text-sm font-medium text-dark-800 mb-1">Email</label>
          <input v-model="form.email" type="email" required class="input" />
        </div>
        <div>
          <label class="block text-sm font-medium text-dark-800 mb-1">Телефон</label>
          <input v-model="form.phone" type="tel" class="input" />
        </div>
        <div v-if="error" class="text-red-600 text-sm">{{ error }}</div>
        <div v-if="success" class="text-primary-600 text-sm">Профиль обновлён!</div>
        <button type="submit" class="btn btn-primary w-full" :disabled="loading">
          {{ loading ? 'Сохранение...' : 'Сохранить' }}
        </button>
      </form>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useAuthStore } from '../../stores/auth'
import { updateReaderProfile, getReaderProfile, getReadingStats } from '../../api'
import type { Reader } from '../../types'

const authStore = useAuthStore()
const loading = ref(false)
const error = ref('')
const success = ref(false)
const reader = ref<Reader | null>(null)
const stats = ref<any>(null)

const form = ref({
  full_name: '',
  email: '',
  phone: ''
})

const loadProfile = async () => {
  try {
    const currentReader = authStore.user as Reader
    const [profileRes, statsRes] = await Promise.all([
      getReaderProfile(currentReader.id),
      getReadingStats(currentReader.id)
    ])
    reader.value = profileRes.data
    stats.value = statsRes.data
    form.value = {
      full_name: reader.value.full_name,
      email: reader.value.email,
      phone: reader.value.phone || ''
    }
  } catch (err) {
    console.error('Error loading profile:', err)
  }
}

const handleUpdate = async () => {
  loading.value = true
  error.value = ''
  success.value = false

  try {
    const currentReader = authStore.user as Reader
    const response = await updateReaderProfile(currentReader.id, form.value)
    authStore.loginAsReader(response.data)
    reader.value = response.data
    success.value = true
    setTimeout(() => {
      success.value = false
    }, 3000)
  } catch (err: any) {
    error.value = err.response?.data?.detail || 'Ошибка обновления профиля'
  } finally {
    loading.value = false
  }
}

onMounted(loadProfile)
</script>
