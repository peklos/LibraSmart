<template>
  <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
    <h1 class="text-3xl font-bold text-primary-600 mb-6">Панель управления</h1>

    <!-- Stats Cards -->
    <div v-if="stats" class="grid md:grid-cols-4 gap-4 mb-8">
      <div class="card">
        <div class="text-3xl font-bold text-primary-600">{{ stats.total_readers }}</div>
        <div class="text-sm text-dark-700">Всего читателей</div>
      </div>
      <div class="card">
        <div class="text-3xl font-bold text-primary-600">{{ stats.total_books }}</div>
        <div class="text-sm text-dark-700">Книг в каталоге</div>
      </div>
      <div class="card">
        <div class="text-3xl font-bold text-primary-600">{{ stats.active_loans }}</div>
        <div class="text-sm text-dark-700">Активных выдач</div>
      </div>
      <div class="card">
        <div class="text-3xl font-bold text-red-600">{{ stats.overdue_loans }}</div>
        <div class="text-sm text-dark-700">Просрочек</div>
      </div>
    </div>

    <div class="grid md:grid-cols-2 gap-6">
      <!-- Popular Books -->
      <div class="card">
        <h3 class="text-lg font-bold text-dark-900 mb-4">Популярные книги</h3>
        <div v-if="popularBooks.length > 0" class="space-y-3">
          <div
            v-for="book in popularBooks"
            :key="book.book_id"
            class="flex justify-between items-center p-3 bg-dark-200 rounded-lg"
          >
            <div>
              <div class="font-medium text-dark-900">{{ book.title }}</div>
              <div class="text-sm text-dark-700">{{ book.author }}</div>
            </div>
            <div class="badge badge-success">{{ book.loan_count }}</div>
          </div>
        </div>
      </div>

      <!-- Active Readers -->
      <div class="card">
        <h3 class="text-lg font-bold text-dark-900 mb-4">Активные читатели</h3>
        <div v-if="activeReaders.length > 0" class="space-y-3">
          <div
            v-for="reader in activeReaders"
            :key="reader.reader_id"
            class="flex justify-between items-center p-3 bg-dark-200 rounded-lg"
          >
            <div>
              <div class="font-medium text-dark-900">{{ reader.full_name }}</div>
              <div class="text-sm text-dark-700">{{ reader.email }}</div>
            </div>
            <div class="badge badge-success">{{ reader.loan_count }}</div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { getDashboardStats, getPopularBooks, getActiveReaders } from '../../api'
import type { DashboardStats } from '../../types'

const stats = ref<DashboardStats | null>(null)
const popularBooks = ref<any[]>([])
const activeReaders = ref<any[]>([])

const fetchData = async () => {
  try {
    const [statsRes, booksRes, readersRes] = await Promise.all([
      getDashboardStats(),
      getPopularBooks(5),
      getActiveReaders(5)
    ])
    stats.value = statsRes.data
    popularBooks.value = booksRes.data
    activeReaders.value = readersRes.data
  } catch (error) {
    console.error('Error fetching dashboard data:', error)
  }
}

onMounted(fetchData)
</script>
