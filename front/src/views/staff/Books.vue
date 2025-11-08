<template>
  <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
    <div class="flex justify-between items-center mb-6">
      <h1 class="text-3xl font-bold text-primary-600">Управление книгами</h1>
      <button @click="openCreateModal" class="btn btn-primary">Добавить книгу</button>
    </div>

    <div v-if="loading" class="text-center py-8">
      <div class="text-dark-600">Загрузка...</div>
    </div>

    <div v-else class="grid md:grid-cols-2 lg:grid-cols-3 gap-4">
      <div v-for="book in books" :key="book.id" class="card">
        <h3 class="font-bold text-dark-900 mb-1">{{ book.title }}</h3>
        <p class="text-sm text-dark-700">{{ book.author }}</p>
        <p class="text-xs text-dark-600">{{ book.genre_name }}</p>
        <div class="flex space-x-2 mt-3">
          <button @click="openEditModal(book)" class="btn btn-secondary text-xs flex-1">
            Изменить
          </button>
          <button @click="deleteBook(book.id)" class="btn btn-danger text-xs flex-1">
            Удалить
          </button>
        </div>
      </div>
    </div>

    <!-- Modal -->
    <div v-if="showModal" class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center p-4 z-50">
      <div class="card max-w-md w-full max-h-[80vh] overflow-y-auto">
        <h3 class="text-xl font-bold text-dark-900 mb-4">
          {{ editingBook ? 'Редактировать книгу' : 'Новая книга' }}
        </h3>
        <form @submit.prevent="handleSubmit" class="space-y-3">
          <div>
            <label class="block text-sm font-medium text-dark-800 mb-1">Название</label>
            <input v-model="form.title" required class="input" />
          </div>
          <div>
            <label class="block text-sm font-medium text-dark-800 mb-1">Автор</label>
            <input v-model="form.author" required class="input" />
          </div>
          <div>
            <label class="block text-sm font-medium text-dark-800 mb-1">Жанр</label>
            <select v-model="form.genre_id" required class="input">
              <option :value="null">Выберите...</option>
              <option v-for="genre in genres" :key="genre.id" :value="genre.id">
                {{ genre.genre_name }}
              </option>
            </select>
          </div>
          <div>
            <label class="block text-sm font-medium text-dark-800 mb-1">Год издания</label>
            <input v-model.number="form.publication_year" type="number" class="input" />
          </div>
          <div>
            <label class="block text-sm font-medium text-dark-800 mb-1">Описание</label>
            <textarea v-model="form.description" class="input" rows="3"></textarea>
          </div>
          <div v-if="modalError" class="text-red-600 text-sm">{{ modalError }}</div>
          <div class="flex space-x-2">
            <button type="button" @click="showModal = false" class="btn btn-secondary flex-1">
              Отмена
            </button>
            <button type="submit" class="btn btn-primary flex-1">
              {{ editingBook ? 'Сохранить' : 'Создать' }}
            </button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { getAllBooks, createBook, updateBook, deleteBook as apiDeleteBook, getAllGenres } from '../../api'
import type { Book, Genre } from '../../types'

const loading = ref(false)
const books = ref<Book[]>([])
const genres = ref<Genre[]>([])
const showModal = ref(false)
const editingBook = ref<Book | null>(null)
const modalError = ref('')

const form = ref({
  title: '',
  author: '',
  genre_id: null as number | null,
  publication_year: null as number | null,
  description: ''
})

const fetchBooks = async () => {
  loading.value = true
  try {
    const response = await getAllBooks()
    books.value = response.data
  } catch (error) {
    console.error('Error:', error)
  } finally {
    loading.value = false
  }
}

const fetchGenres = async () => {
  try {
    const response = await getAllGenres()
    genres.value = response.data
  } catch (error) {
    console.error('Error:', error)
  }
}

const openCreateModal = () => {
  editingBook.value = null
  form.value = { title: '', author: '', genre_id: null, publication_year: null, description: '' }
  modalError.value = ''
  showModal.value = true
}

const openEditModal = (book: Book) => {
  editingBook.value = book
  form.value = {
    title: book.title,
    author: book.author,
    genre_id: book.genre_id,
    publication_year: book.publication_year || null,
    description: book.description || ''
  }
  modalError.value = ''
  showModal.value = true
}

const handleSubmit = async () => {
  modalError.value = ''
  try {
    const formData = {
      ...form.value,
      genre_id: form.value.genre_id ?? undefined,
      publication_year: form.value.publication_year ?? undefined
    }
    if (editingBook.value) {
      await updateBook(editingBook.value.id, formData)
    } else {
      await createBook(formData)
    }
    showModal.value = false
    fetchBooks()
  } catch (error: any) {
    modalError.value = error.response?.data?.detail || 'Ошибка'
  }
}

const deleteBook = async (id: number) => {
  if (!confirm('Удалить книгу?')) return
  try {
    await apiDeleteBook(id)
    fetchBooks()
  } catch (error: any) {
    alert(error.response?.data?.detail || 'Ошибка')
  }
}

onMounted(() => {
  fetchBooks()
  fetchGenres()
})
</script>
