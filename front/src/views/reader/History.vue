<template>
  <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
    <h1 class="text-3xl font-bold text-primary-600 mb-6">История чтения</h1>

    <!-- Stats Cards -->
    <div v-if="stats" class="grid md:grid-cols-4 gap-4 mb-6">
      <div class="card">
        <div class="text-2xl font-bold text-primary-600">{{ stats.total_books_read }}</div>
        <div class="text-sm text-dark-700">Книг прочитано</div>
      </div>
      <div class="card">
        <div class="text-2xl font-bold text-primary-600">{{ stats.active_loans }}</div>
        <div class="text-sm text-dark-700">Активных займов</div>
      </div>
      <div class="card">
        <div class="text-2xl font-bold text-red-600">{{ stats.overdue_loans }}</div>
        <div class="text-sm text-dark-700">Просрочек</div>
      </div>
      <div class="card">
        <div class="text-2xl font-bold text-primary-600">{{ stats.total_loans }}</div>
        <div class="text-sm text-dark-700">Всего займов</div>
      </div>
    </div>

    <!-- Favorite Genres -->
    <div v-if="stats && stats.favorite_genres.length > 0" class="card mb-6">
      <h3 class="text-lg font-bold text-dark-900 mb-3">Любимые жанры</h3>
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

    <!-- History Table -->
    <div v-if="loading" class="text-center py-8">
      <div class="text-dark-600">Загрузка...</div>
    </div>

    <div v-else-if="history.length === 0" class="card text-center py-8">
      <div class="text-dark-600">История пуста</div>
    </div>

    <div v-else class="card overflow-hidden">
      <h3 class="text-lg font-bold text-dark-900 mb-4 px-6 pt-4">Все выдачи</h3>
      <table class="table">
        <thead>
          <tr>
            <th>Книга</th>
            <th>Дата выдачи</th>
            <th>Срок возврата</th>
            <th>Дата возврата</th>
            <th>Статус</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="loan in history" :key="loan.id">
            <td class="font-medium">{{ loan.book_title }}</td>
            <td>{{ formatDate(loan.loan_date) }}</td>
            <td>{{ formatDate(loan.due_date) }}</td>
            <td>{{ loan.return_date ? formatDate(loan.return_date) : '—' }}</td>
            <td>
              <span
                class="badge"
                :class="{
                  'badge-success': loan.status === 'active',
                  'badge-danger': loan.status === 'overdue',
                  'badge-info': loan.status === 'returned'
                }"
              >
                {{ statusLabel(loan.status) }}
              </span>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useAuthStore } from '../../stores/auth'
import { getReadingHistory, getReadingStats } from '../../api'
import type { Loan, Reader } from '../../types'

const authStore = useAuthStore()
const loading = ref(false)
const history = ref<Loan[]>([])
const stats = ref<any>(null)

const fetchData = async () => {
  loading.value = true
  try {
    const reader = authStore.user as Reader
    const [historyRes, statsRes] = await Promise.all([
      getReadingHistory(reader.id),
      getReadingStats(reader.id)
    ])
    history.value = historyRes.data
    stats.value = statsRes.data
  } catch (error) {
    console.error('Error fetching history:', error)
  } finally {
    loading.value = false
  }
}

const formatDate = (dateString: string) => {
  return new Date(dateString).toLocaleDateString('ru-RU')
}

const statusLabel = (status: string) => {
  const labels: Record<string, string> = {
    active: 'Активна',
    overdue: 'Просрочена',
    returned: 'Возвращена'
  }
  return labels[status] || status
}

onMounted(fetchData)
</script>
