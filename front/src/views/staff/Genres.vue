<template>
  <div class="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
    <div class="flex justify-between items-center mb-6">
      <h1 class="text-3xl font-bold text-primary-600">Управление жанрами</h1>
      <button @click="openCreateModal" class="btn btn-primary">Добавить жанр</button>
    </div>

    <div v-if="loading" class="text-center py-8">Загрузка...</div>

    <div v-else class="card">
      <div class="flex flex-wrap gap-3">
        <div
          v-for="genre in genres"
          :key="genre.id"
          class="flex items-center space-x-2 px-4 py-2 bg-dark-200 rounded-lg"
        >
          <span class="font-medium">{{ genre.genre_name }}</span>
          <button @click="openEditModal(genre)" class="text-primary-600 hover:text-primary-700">✏️</button>
          <button @click="deleteGenre(genre.id)" class="text-red-600 hover:text-red-700">🗑️</button>
        </div>
      </div>
    </div>

    <!-- Modal -->
    <div v-if="showModal" class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center p-4 z-50">
      <div class="card max-w-sm w-full">
        <h3 class="text-xl font-bold mb-4">{{ editingGenre ? 'Редактировать' : 'Новый жанр' }}</h3>
        <form @submit.prevent="handleSubmit" class="space-y-3">
          <div>
            <label class="block text-sm font-medium mb-1">Название жанра</label>
            <input v-model="form.genre_name" required class="input" />
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
import { getAllGenres, createGenre, updateGenre, deleteGenre as apiDeleteGenre } from '../../api'
import type { Genre } from '../../types'

const loading = ref(false)
const genres = ref<Genre[]>([])
const showModal = ref(false)
const editingGenre = ref<Genre | null>(null)

const form = ref({
  genre_name: ''
})

const fetchGenres = async () => {
  loading.value = true
  try {
    const response = await getAllGenres()
    genres.value = response.data
  } finally {
    loading.value = false
  }
}

const openCreateModal = () => {
  editingGenre.value = null
  form.value = { genre_name: '' }
  showModal.value = true
}

const openEditModal = (genre: Genre) => {
  editingGenre.value = genre
  form.value = { ...genre }
  showModal.value = true
}

const handleSubmit = async () => {
  try {
    if (editingGenre.value) {
      await updateGenre(editingGenre.value.id, form.value)
    } else {
      await createGenre(form.value)
    }
    showModal.value = false
    fetchGenres()
  } catch (error) {
    alert('Ошибка')
  }
}

const deleteGenre = async (id: number) => {
  if (!confirm('Удалить жанр?')) return
  try {
    await apiDeleteGenre(id)
    fetchGenres()
  } catch (error) {
    alert('Ошибка')
  }
}

onMounted(fetchGenres)
</script>
