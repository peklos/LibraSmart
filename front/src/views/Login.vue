<template>
  <div class="min-h-screen flex items-center justify-center px-4 py-12">
    <div class="max-w-md w-full">
      <div class="text-center mb-8">
        <div class="text-5xl mb-4">📚</div>
        <h2 class="text-3xl font-bold text-primary-600">LibraSmart</h2>
        <p class="text-dark-700 mt-2">Вход в систему</p>
      </div>

      <div class="card">
        <!-- Tabs -->
        <div class="flex space-x-2 mb-6">
          <button
            @click="activeTab = 'reader'"
            class="flex-1 py-2 px-4 rounded-lg font-medium transition"
            :class="activeTab === 'reader' ? 'bg-primary-600 text-white' : 'bg-dark-200 text-dark-800'"
          >
            Читатель
          </button>
          <button
            @click="activeTab = 'staff'"
            class="flex-1 py-2 px-4 rounded-lg font-medium transition"
            :class="activeTab === 'staff' ? 'bg-primary-600 text-white' : 'bg-dark-200 text-dark-800'"
          >
            Сотрудник
          </button>
        </div>

        <!-- Reader Login -->
        <form v-if="activeTab === 'reader'" @submit.prevent="handleReaderLogin" class="space-y-4">
          <div>
            <label class="block text-sm font-medium text-dark-800 mb-1">Email</label>
            <input
              v-model="readerEmail"
              type="email"
              required
              class="input"
              placeholder="alekseev@mail.ru"
            />
          </div>
          <div>
            <label class="block text-sm font-medium text-dark-800 mb-1">Пароль</label>
            <input
              v-model="readerPassword"
              type="password"
              required
              class="input"
              placeholder="reader123"
            />
          </div>
          <div v-if="error" class="text-red-600 text-sm">{{ error }}</div>
          <button type="submit" class="btn btn-primary w-full" :disabled="loading">
            {{ loading ? 'Вход...' : 'Войти' }}
          </button>
          <div class="text-xs text-dark-600 mt-2 p-3 bg-dark-200 rounded">
            <strong>Тестовый аккаунт:</strong><br>
            Email: alekseev@mail.ru<br>
            Пароль: reader123
          </div>
        </form>

        <!-- Staff Login -->
        <form v-if="activeTab === 'staff'" @submit.prevent="handleStaffLogin" class="space-y-4">
          <div>
            <label class="block text-sm font-medium text-dark-800 mb-1">Email</label>
            <input
              v-model="staffEmail"
              type="email"
              required
              class="input"
              placeholder="petrova@library.ru"
            />
          </div>
          <div>
            <label class="block text-sm font-medium text-dark-800 mb-1">Пароль</label>
            <input
              v-model="staffPassword"
              type="password"
              required
              class="input"
              placeholder="admin123"
            />
          </div>
          <div v-if="error" class="text-red-600 text-sm">{{ error }}</div>
          <button type="submit" class="btn btn-primary w-full" :disabled="loading">
            {{ loading ? 'Вход...' : 'Войти' }}
          </button>
          <div class="text-xs text-dark-600 mt-2 p-3 bg-dark-200 rounded">
            <strong>Тестовые аккаунты:</strong><br>
            <strong>Администратор:</strong><br>
            Email: petrova@library.ru<br>
            Пароль: admin123<br><br>
            <strong>Библиотекарь:</strong><br>
            Email: ivanov@library.ru<br>
            Пароль: staff123
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '../stores/auth'
import { readerLogin, staffLogin } from '../api'

const router = useRouter()
const authStore = useAuthStore()

const activeTab = ref<'reader' | 'staff'>('reader')
const loading = ref(false)
const error = ref('')

// Reader credentials
const readerEmail = ref('alekseev@mail.ru')
const readerPassword = ref('reader123')

// Staff credentials
const staffEmail = ref('petrova@library.ru')
const staffPassword = ref('admin123')

const handleReaderLogin = async () => {
  loading.value = true
  error.value = ''
  try {
    const response = await readerLogin(readerEmail.value, readerPassword.value)
    authStore.loginAsReader(response.data)
    router.push('/reader/catalog')
  } catch (err: any) {
    error.value = err.response?.data?.detail || 'Ошибка входа'
  } finally {
    loading.value = false
  }
}

const handleStaffLogin = async () => {
  loading.value = true
  error.value = ''
  try {
    const response = await staffLogin(staffEmail.value, staffPassword.value)
    authStore.loginAsStaff(response.data)
    router.push('/staff/dashboard')
  } catch (err: any) {
    error.value = err.response?.data?.detail || 'Ошибка входа'
  } finally {
    loading.value = false
  }
}
</script>
