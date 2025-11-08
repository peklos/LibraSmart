<template>
  <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
    <div class="flex justify-between items-center mb-6">
      <h1 class="text-3xl font-bold text-primary-600">Управление персоналом</h1>
      <button @click="openCreateModal" class="btn btn-primary">Добавить сотрудника</button>
    </div>

    <div v-if="loading" class="text-center py-8">Загрузка...</div>

    <div v-else class="card overflow-x-auto">
      <table class="table">
        <thead>
          <tr>
            <th>ФИО</th>
            <th>Email</th>
            <th>Должность</th>
            <th>Библиотека</th>
            <th>Роль</th>
            <th>Действия</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="staff in staffList" :key="staff.id">
            <td class="font-medium">{{ staff.full_name }}</td>
            <td>{{ staff.email }}</td>
            <td>{{ staff.position }}</td>
            <td>{{ getLibraryName(staff.library_id) }}</td>
            <td>
              <span class="badge badge-info">{{ getRoleName(staff.role_id) }}</span>
            </td>
            <td>
              <button @click="deleteStaff(staff.id)" class="btn btn-danger text-xs">Удалить</button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- Create Modal -->
    <div v-if="showModal" class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center p-4 z-50">
      <div class="card max-w-md w-full">
        <h3 class="text-xl font-bold mb-4">Новый сотрудник</h3>
        <form @submit.prevent="handleSubmit" class="space-y-3">
          <div>
            <label class="block text-sm font-medium mb-1">ФИО</label>
            <input v-model="form.full_name" required class="input" />
          </div>
          <div>
            <label class="block text-sm font-medium mb-1">Email</label>
            <input v-model="form.email" type="email" required class="input" />
          </div>
          <div>
            <label class="block text-sm font-medium mb-1">Пароль</label>
            <input v-model="form.password" type="password" required class="input" />
          </div>
          <div>
            <label class="block text-sm font-medium mb-1">Должность</label>
            <input v-model="form.position" required class="input" />
          </div>
          <div>
            <label class="block text-sm font-medium mb-1">Библиотека</label>
            <select v-model="form.library_id" required class="input">
              <option :value="null">Выберите...</option>
              <option v-for="lib in libraries" :key="lib.id" :value="lib.id">{{ lib.library_name }}</option>
            </select>
          </div>
          <div>
            <label class="block text-sm font-medium mb-1">Роль</label>
            <select v-model="form.role_id" required class="input">
              <option value="1">Администратор</option>
              <option value="2">Старший библиотекарь</option>
              <option value="3">Библиотекарь</option>
              <option value="4">Помощник библиотекаря</option>
            </select>
          </div>
          <div class="flex space-x-2">
            <button type="button" @click="showModal = false" class="btn btn-secondary flex-1">Отмена</button>
            <button type="submit" class="btn btn-primary flex-1">Создать</button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useAuthStore } from '../../stores/auth'
import { getAllStaff, createStaff, deleteStaff as apiDeleteStaff, getAllLibraries } from '../../api'
import type { Staff, Library } from '../../types'

const authStore = useAuthStore()
const loading = ref(false)
const staffList = ref<Staff[]>([])
const libraries = ref<Library[]>([])
const showModal = ref(false)

const form = ref({
  full_name: '',
  email: '',
  password: '',
  position: '',
  library_id: null as number | null,
  role_id: 3
})

const fetchStaff = async () => {
  loading.value = true
  try {
    const currentStaff = authStore.user as Staff
    const response = await getAllStaff(currentStaff.id)
    staffList.value = response.data
  } finally {
    loading.value = false
  }
}

const openCreateModal = async () => {
  const response = await getAllLibraries()
  libraries.value = response.data
  form.value = { full_name: '', email: '', password: '', position: '', library_id: null, role_id: 3 }
  showModal.value = true
}

const handleSubmit = async () => {
  try {
    const currentStaff = authStore.user as Staff
    await createStaff(currentStaff.id, form.value)
    showModal.value = false
    fetchStaff()
  } catch (error) {
    alert('Ошибка')
  }
}

const deleteStaff = async (id: number) => {
  if (!confirm('Удалить сотрудника?')) return
  try {
    const currentStaff = authStore.user as Staff
    await apiDeleteStaff(currentStaff.id, id)
    fetchStaff()
  } catch (error: any) {
    alert(error.response?.data?.detail || 'Ошибка')
  }
}

const getLibraryName = (id: number) => libraries.value.find(l => l.id === id)?.library_name || '—'
const getRoleName = (id: number) => ['Администратор', 'Старший библиотекарь', 'Библиотекарь', 'Помощник'][id - 1] || '—'

onMounted(() => {
  fetchStaff()
  getAllLibraries().then(res => libraries.value = res.data)
})
</script>
