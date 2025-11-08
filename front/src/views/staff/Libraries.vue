<template>
  <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
    <div class="flex justify-between items-center mb-6">
      <h1 class="text-3xl font-bold text-primary-600">Управление библиотеками</h1>
      <button @click="openCreateModal" class="btn btn-primary">Добавить библиотеку</button>
    </div>

    <div v-if="loading" class="text-center py-8">Загрузка...</div>

    <div v-else class="grid md:grid-cols-2 gap-4">
      <div v-for="lib in libraries" :key="lib.id" class="card">
        <h3 class="font-bold text-dark-900 mb-2">{{ lib.library_name }}</h3>
        <p class="text-sm text-dark-700 mb-1">📍 {{ lib.address }}</p>
        <p class="text-sm text-dark-700 mb-3">📞 {{ lib.phone }}</p>
        <div class="flex space-x-2">
          <button @click="openEditModal(lib)" class="btn btn-secondary text-xs flex-1">Изменить</button>
          <button @click="deleteLibrary(lib.id)" class="btn btn-danger text-xs flex-1">Удалить</button>
        </div>
      </div>
    </div>

    <!-- Modal -->
    <div v-if="showModal" class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center p-4 z-50">
      <div class="card max-w-md w-full">
        <h3 class="text-xl font-bold mb-4">{{ editingLibrary ? 'Редактировать' : 'Новая библиотека' }}</h3>
        <form @submit.prevent="handleSubmit" class="space-y-3">
          <div>
            <label class="block text-sm font-medium mb-1">Название</label>
            <input v-model="form.library_name" required class="input" />
          </div>
          <div>
            <label class="block text-sm font-medium mb-1">Адрес</label>
            <input v-model="form.address" required class="input" />
          </div>
          <div>
            <label class="block text-sm font-medium mb-1">Телефон</label>
            <input v-model="form.phone" required class="input" />
          </div>
          <div class="flex space-x-2">
            <button type="button" @click="showModal = false" class="btn btn-secondary flex-1">Отмена</button>
            <button type="submit" class="btn btn-primary flex-1">Сохранить</button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { getAllLibraries, createLibrary, updateLibrary, deleteLibrary as apiDeleteLibrary } from '../../api'
import type { Library } from '../../types'

const loading = ref(false)
const libraries = ref<Library[]>([])
const showModal = ref(false)
const editingLibrary = ref<Library | null>(null)

const form = ref({
  library_name: '',
  address: '',
  phone: ''
})

const fetchLibraries = async () => {
  loading.value = true
  try {
    const response = await getAllLibraries()
    libraries.value = response.data
  } finally {
    loading.value = false
  }
}

const openCreateModal = () => {
  editingLibrary.value = null
  form.value = { library_name: '', address: '', phone: '' }
  showModal.value = true
}

const openEditModal = (lib: Library) => {
  editingLibrary.value = lib
  form.value = { ...lib }
  showModal.value = true
}

const handleSubmit = async () => {
  try {
    if (editingLibrary.value) {
      await updateLibrary(editingLibrary.value.id, form.value)
    } else {
      await createLibrary(form.value)
    }
    showModal.value = false
    fetchLibraries()
  } catch (error) {
    alert('Ошибка')
  }
}

const deleteLibrary = async (id: number) => {
  if (!confirm('Удалить библиотеку?')) return
  try {
    await apiDeleteLibrary(id)
    fetchLibraries()
  } catch (error) {
    alert('Ошибка')
  }
}

onMounted(fetchLibraries)
</script>
