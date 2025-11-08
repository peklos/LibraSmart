<template>
  <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
    <div class="flex justify-between items-center mb-6">
      <h1 class="text-3xl font-bold text-primary-600">Экземпляры книг</h1>
      <button @click="openCreateModal" class="btn btn-primary">Добавить экземпляр</button>
    </div>

    <div v-if="loading" class="text-center py-8">Загрузка...</div>

    <div v-else class="card overflow-x-auto">
      <table class="table">
        <thead>
          <tr>
            <th>Книга</th>
            <th>Библиотека</th>
            <th>Инв. номер</th>
            <th>Статус</th>
            <th>Действия</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="copy in copies" :key="copy.id">
            <td class="font-medium">{{ copy.book_title }}</td>
            <td>{{ copy.library_name }}</td>
            <td>{{ copy.inventory_number }}</td>
            <td>
              <span class="badge" :class="{
                'badge-success': copy.status === 'available',
                'badge-warning': copy.status === 'on_loan',
                'badge-danger': copy.status === 'maintenance'
              }">
                {{ statusLabel(copy.status) }}
              </span>
            </td>
            <td>
              <div class="flex space-x-2">
                <button @click="openEditModal(copy)" class="btn btn-secondary text-xs">Изменить</button>
                <button @click="deleteCopy(copy.id)" class="btn btn-danger text-xs">Удалить</button>
              </div>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- Modal -->
    <div v-if="showModal" class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center p-4 z-50">
      <div class="card max-w-md w-full">
        <h3 class="text-xl font-bold mb-4">{{ editingCopy ? 'Редактировать' : 'Новый экземпляр' }}</h3>
        <form @submit.prevent="handleSubmit" class="space-y-3">
          <div>
            <label class="block text-sm font-medium mb-1">Книга</label>
            <select v-model="form.book_id" required class="input">
              <option :value="null">Выберите...</option>
              <option v-for="book in books" :key="book.id" :value="book.id">{{ book.title }}</option>
            </select>
          </div>
          <div>
            <label class="block text-sm font-medium mb-1">Библиотека</label>
            <select v-model="form.library_id" required class="input">
              <option :value="null">Выберите...</option>
              <option v-for="lib in libraries" :key="lib.id" :value="lib.id">{{ lib.library_name }}</option>
            </select>
          </div>
          <div>
            <label class="block text-sm font-medium mb-1">Инвентарный номер</label>
            <input v-model="form.inventory_number" required class="input" />
          </div>
          <div>
            <label class="block text-sm font-medium mb-1">Статус</label>
            <select v-model="form.status" required class="input">
              <option value="available">Доступен</option>
              <option value="on_loan">На руках</option>
              <option value="maintenance">На обслуживании</option>
              <option value="lost">Утерян</option>
            </select>
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
import { getAllCopies, createCopy, updateCopy, deleteCopy as apiDeleteCopy, getAllBooks, getAllLibraries } from '../../api'
import type { BookCopy, Book, Library } from '../../types'

const loading = ref(false)
const copies = ref<BookCopy[]>([])
const books = ref<Book[]>([])
const libraries = ref<Library[]>([])
const showModal = ref(false)
const editingCopy = ref<BookCopy | null>(null)

const form = ref({
  book_id: null as number | null,
  library_id: null as number | null,
  inventory_number: '',
  status: 'available'
})

const fetchCopies = async () => {
  loading.value = true
  try {
    const response = await getAllCopies()
    copies.value = response.data
  } finally {
    loading.value = false
  }
}

const openCreateModal = () => {
  editingCopy.value = null
  form.value = { book_id: null, library_id: null, inventory_number: '', status: 'available' }
  showModal.value = true
}

const openEditModal = (copy: BookCopy) => {
  editingCopy.value = copy
  form.value = { ...copy }
  showModal.value = true
}

const handleSubmit = async () => {
  try {
    const formData = {
      ...form.value,
      book_id: form.value.book_id ?? undefined,
      library_id: form.value.library_id ?? undefined
    }
    if (editingCopy.value) {
      await updateCopy(editingCopy.value.id, formData)
    } else {
      await createCopy(formData)
    }
    showModal.value = false
    fetchCopies()
  } catch (error) {
    alert('Ошибка')
  }
}

const deleteCopy = async (id: number) => {
  if (!confirm('Удалить?')) return
  try {
    await apiDeleteCopy(id)
    fetchCopies()
  } catch (error) {
    alert('Ошибка')
  }
}

const statusLabel = (status: string) => {
  const labels: Record<string, string> = {
    available: 'Доступен',
    on_loan: 'На руках',
    maintenance: 'Обслуживание',
    lost: 'Утерян'
  }
  return labels[status] || status
}

onMounted(async () => {
  fetchCopies()
  const [booksRes, libsRes] = await Promise.all([getAllBooks(), getAllLibraries()])
  books.value = booksRes.data
  libraries.value = libsRes.data
})
</script>
