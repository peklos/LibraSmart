<template>
  <div class="max-w-3xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
    <h1 class="text-3xl font-bold text-primary-600 mb-6">Мой профиль</h1>

    <div class="card">
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
        <div>
          <label class="block text-sm font-medium text-dark-800 mb-1">Номер читательского билета</label>
          <input :value="reader?.library_card_number" type="text" disabled class="input bg-dark-200" />
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
import { updateReaderProfile, getReaderProfile } from '../../api'
import type { Reader } from '../../types'

const authStore = useAuthStore()
const loading = ref(false)
const error = ref('')
const success = ref(false)
const reader = ref<Reader | null>(null)

const form = ref({
  full_name: '',
  email: '',
  phone: ''
})

const loadProfile = async () => {
  try {
    const currentReader = authStore.user as Reader
    const response = await getReaderProfile(currentReader.id)
    reader.value = response.data
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
