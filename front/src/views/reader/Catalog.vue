<template>
  <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
    <h1 class="text-3xl font-bold text-primary-600 mb-6">Каталог книг</h1>

    <!-- Search and Filters -->
    <div class="card mb-6">
      <div class="grid md:grid-cols-3 gap-4">
        <div>
          <label class="block text-sm font-medium text-dark-800 mb-1">Поиск</label>
          <input
            v-model="searchQuery"
            @input="fetchBooks"
            type="text"
            class="input"
            placeholder="Название или автор..."
          />
        </div>
        <div>
          <label class="block text-sm font-medium text-dark-800 mb-1">Жанр</label>
          <select v-model="selectedGenre" @change="fetchBooks" class="input">
            <option :value="null">Все жанры</option>
            <option v-for="genre in genres" :key="genre.id" :value="genre.id">
              {{ genre.genre_name }}
            </option>
          </select>
        </div>
        <div>
          <label class="block text-sm font-medium text-dark-800 mb-1">Автор</label>
          <input
            v-model="authorQuery"
            @input="fetchBooks"
            type="text"
            class="input"
            placeholder="Имя автора..."
          />
        </div>
      </div>
    </div>

    <!-- Books Grid -->
    <div v-if="loading" class="text-center py-8">
      <div class="text-dark-600">Загрузка...</div>
    </div>

    <div v-else-if="books.length === 0" class="text-center py-8">
      <div class="text-dark-600">Книги не найдены</div>
    </div>

    <div v-else class="grid md:grid-cols-2 lg:grid-cols-3 gap-6">
      <div v-for="book in books" :key="book.id" class="card hover:shadow-xl transition-shadow">
        <h3 class="text-lg font-bold text-dark-900 mb-2">{{ book.title }}</h3>
        <p class="text-sm text-dark-700 mb-1">Автор: {{ book.author }}</p>
        <p class="text-sm text-dark-700 mb-2">Жанр: {{ book.genre_name }}</p>
        <p v-if="book.publication_year" class="text-xs text-dark-600 mb-3">
          Год издания: {{ book.publication_year }}
        </p>
        <p v-if="book.description" class="text-sm text-dark-700 mb-4">
          {{ book.description.substring(0, 100) }}{{ book.description.length > 100 ? '...' : '' }}
        </p>
        <div class="flex space-x-2">
          <button @click="viewAvailability(book)" class="btn btn-secondary text-sm flex-1">
            Наличие
          </button>
          <button @click="openReservationModal(book)" class="btn btn-primary text-sm flex-1">
            Забронировать
          </button>
        </div>
      </div>
    </div>

    <!-- Availability Modal -->
    <div v-if="showAvailabilityModal" class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center p-4 z-50">
      <div class="card max-w-2xl w-full max-h-[80vh] overflow-y-auto">
        <h3 class="text-xl font-bold text-dark-900 mb-4">
          Наличие: {{ selectedBook?.title }}
        </h3>
        <div v-if="availability" class="space-y-3">
          <div v-for="lib in availability.libraries" :key="lib.library_id" class="p-4 bg-dark-200 rounded-lg">
            <div class="font-medium text-dark-900">{{ lib.library_name }}</div>
            <div class="text-sm text-dark-700 mt-1">
              Всего: {{ lib.total }} | Доступно: <span class="text-primary-600 font-semibold">{{ lib.available }}</span>
            </div>
          </div>
        </div>
        <button @click="showAvailabilityModal = false" class="btn btn-secondary w-full mt-4">
          Закрыть
        </button>
      </div>
    </div>

    <!-- Reservation Modal -->
    <div v-if="showReservationModal" class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center p-4 z-50">
      <div class="card max-w-md w-full">
        <h3 class="text-xl font-bold text-dark-900 mb-4">Бронирование книги</h3>
        <p class="text-dark-800 mb-4">{{ selectedBook?.title }}</p>
        <div class="mb-4">
          <label class="block text-sm font-medium text-dark-800 mb-1">Выберите библиотеку</label>
          <select v-model="reservationLibraryId" class="input">
            <option :value="null">Выберите...</option>
            <option v-for="lib in libraries" :key="lib.id" :value="lib.id">
              {{ lib.library_name }}
            </option>
          </select>
        </div>
        <div v-if="reservationError" class="text-red-600 text-sm mb-4">{{ reservationError }}</div>
        <div v-if="reservationSuccess" class="text-primary-600 text-sm mb-4">Бронирование создано!</div>
        <div class="flex space-x-2">
          <button @click="showReservationModal = false" class="btn btn-secondary flex-1">
            Отмена
          </button>
          <button @click="createReservation" class="btn btn-primary flex-1" :disabled="!reservationLibraryId">
            Забронировать
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useAuthStore } from '../../stores/auth'
import { getBooks, getBookAvailability, createReservation as apiCreateReservation, getAllGenres, getAllLibraries } from '../../api'
import type { Book, Genre, Library, Reader } from '../../types'

const authStore = useAuthStore()
const loading = ref(false)
const books = ref<Book[]>([])
const genres = ref<Genre[]>([])
const libraries = ref<Library[]>([])

const searchQuery = ref('')
const selectedGenre = ref<number | null>(null)
const authorQuery = ref('')

const showAvailabilityModal = ref(false)
const showReservationModal = ref(false)
const selectedBook = ref<Book | null>(null)
const availability = ref<any>(null)

const reservationLibraryId = ref<number | null>(null)
const reservationError = ref('')
const reservationSuccess = ref(false)

const fetchBooks = async () => {
  loading.value = true
  try {
    const params: any = {}
    if (searchQuery.value) params.search = searchQuery.value
    if (selectedGenre.value) params.genre_id = selectedGenre.value
    if (authorQuery.value) params.author = authorQuery.value

    const response = await getBooks(params)
    books.value = response.data
  } catch (error) {
    console.error('Error fetching books:', error)
  } finally {
    loading.value = false
  }
}

const fetchGenres = async () => {
  try {
    const response = await getAllGenres()
    genres.value = response.data
  } catch (error) {
    console.error('Error fetching genres:', error)
  }
}

const fetchLibraries = async () => {
  try {
    const response = await getAllLibraries()
    libraries.value = response.data
  } catch (error) {
    console.error('Error fetching libraries:', error)
  }
}

const viewAvailability = async (book: Book) => {
  selectedBook.value = book
  try {
    const response = await getBookAvailability(book.id)
    availability.value = response.data
    showAvailabilityModal.value = true
  } catch (error) {
    console.error('Error fetching availability:', error)
  }
}

const openReservationModal = (book: Book) => {
  selectedBook.value = book
  reservationLibraryId.value = null
  reservationError.value = ''
  reservationSuccess.value = false
  showReservationModal.value = true
}

const createReservation = async () => {
  if (!selectedBook.value || !reservationLibraryId.value) return

  const reader = authStore.user as Reader
  try {
    await apiCreateReservation(reader.id, {
      book_id: selectedBook.value.id,
      library_id: reservationLibraryId.value
    })
    reservationSuccess.value = true
    reservationError.value = ''
    setTimeout(() => {
      showReservationModal.value = false
    }, 1500)
  } catch (error: any) {
    reservationError.value = error.response?.data?.detail || 'Ошибка бронирования'
  }
}

onMounted(() => {
  fetchBooks()
  fetchGenres()
  fetchLibraries()
})
</script>
