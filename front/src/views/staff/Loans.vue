<template>
  <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
    <div class="flex justify-between items-center mb-6">
      <h1 class="text-3xl font-bold text-primary-600">Управление выдачами</h1>
      <button @click="openCreateModal" class="btn btn-primary">Создать выдачу</button>
    </div>

    <div class="flex space-x-2 mb-6">
      <button @click="filter = 'all'" :class="filter === 'all' ? 'btn btn-primary' : 'btn btn-secondary'">Все</button>
      <button @click="filter = 'active'" :class="filter === 'active' ? 'btn btn-primary' : 'btn btn-secondary'">Активные</button>
      <button @click="filter = 'overdue'" :class="filter === 'overdue' ? 'btn btn-primary' : 'btn btn-secondary'">Просроченные</button>
    </div>

    <div v-if="loading" class="text-center py-8">Загрузка...</div>

    <div v-else class="card overflow-x-auto">
      <table class="table">
        <thead>
          <tr>
            <th>Книга</th>
            <th>Читатель</th>
            <th>Выдача</th>
            <th>Срок</th>
            <th>Статус</th>
            <th>Действия</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="loan in filteredLoans" :key="loan.id">
            <td class="font-medium">{{ loan.book_title }}</td>
            <td>{{ loan.reader_name }}</td>
            <td>{{ formatDate(loan.loan_date) }}</td>
            <td :class="{ 'text-red-600': loan.status === 'overdue' }">{{ formatDate(loan.due_date) }}</td>
            <td>
              <span class="badge" :class="{
                'badge-success': loan.status === 'active',
                'badge-danger': loan.status === 'overdue',
                'badge-info': loan.status === 'returned'
              }">{{ statusLabel(loan.status) }}</span>
            </td>
            <td>
              <button
                v-if="loan.status !== 'returned'"
                @click="returnLoan(loan.id)"
                class="btn btn-primary text-xs"
              >Возврат</button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- Create Loan Modal -->
    <div v-if="showModal" class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center p-4 z-50">
      <div class="card max-w-md w-full">
        <h3 class="text-xl font-bold mb-4">Новая выдача</h3>
        <form @submit.prevent="handleSubmit" class="space-y-3">
          <div>
            <label class="block text-sm font-medium mb-1">Читатель</label>
            <select v-model="form.reader_id" required class="input">
              <option :value="null">Выберите...</option>
              <option v-for="reader in readers" :key="reader.id" :value="reader.id">{{ reader.full_name }}</option>
            </select>
          </div>
          <div>
            <label class="block text-sm font-medium mb-1">Экземпляр (доступный)</label>
            <select v-model="form.copy_id" required class="input">
              <option :value="null">Выберите...</option>
              <option v-for="copy in availableCopies" :key="copy.id" :value="copy.id">
                {{ copy.book_title }} ({{ copy.inventory_number }})
              </option>
            </select>
          </div>
          <div>
            <label class="block text-sm font-medium mb-1">Срок возврата</label>
            <input v-model="form.due_date" type="date" required class="input" />
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
import { ref, computed, onMounted } from 'vue'
import { useAuthStore } from '../../stores/auth'
import { getAllLoans, createLoan, returnLoan as apiReturnLoan, getAllReaders, getAllCopies } from '../../api'
import type { Loan, Reader, BookCopy, Staff } from '../../types'

const authStore = useAuthStore()
const loading = ref(false)
const loans = ref<Loan[]>([])
const filter = ref('all')
const showModal = ref(false)
const readers = ref<Reader[]>([])
const copies = ref<BookCopy[]>([])

const form = ref({
  reader_id: null as number | null,
  copy_id: null as number | null,
  due_date: '',
  staff_id: (authStore.user as Staff).id
})

const availableCopies = computed(() => copies.value.filter(c => c.status === 'available'))
const filteredLoans = computed(() => {
  if (filter.value === 'active') return loans.value.filter(l => l.status === 'active')
  if (filter.value === 'overdue') return loans.value.filter(l => l.status === 'overdue')
  return loans.value
})

const fetchLoans = async () => {
  loading.value = true
  try {
    const response = await getAllLoans()
    loans.value = response.data
  } finally {
    loading.value = false
  }
}

const openCreateModal = async () => {
  const [readersRes, copiesRes] = await Promise.all([getAllReaders(), getAllCopies()])
  readers.value = readersRes.data
  copies.value = copiesRes.data
  const today = new Date()
  const twoWeeks = new Date(today.getTime() + 14 * 24 * 60 * 60 * 1000)
  form.value.due_date = twoWeeks.toISOString().split('T')[0]
  showModal.value = true
}

const handleSubmit = async () => {
  try {
    await createLoan(form.value)
    showModal.value = false
    fetchLoans()
  } catch (error) {
    alert('Ошибка')
  }
}

const returnLoan = async (id: number) => {
  if (!confirm('Оформить возврат?')) return
  try {
    await apiReturnLoan(id)
    fetchLoans()
  } catch (error) {
    alert('Ошибка')
  }
}

const formatDate = (date: string) => new Date(date).toLocaleDateString('ru-RU')
const statusLabel = (status: string) => ({ active: 'Активна', overdue: 'Просрочена', returned: 'Возвращена' })[status] || status

onMounted(fetchLoans)
</script>
