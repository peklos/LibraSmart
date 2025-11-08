<template>
  <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
    <h1 class="text-3xl font-bold text-primary-600 mb-6">Мои книги</h1>

    <!-- Tabs -->
    <div class="flex space-x-2 mb-6">
      <button
        @click="activeTab = 'active'"
        class="px-4 py-2 rounded-lg font-medium transition"
        :class="activeTab === 'active' ? 'bg-primary-600 text-white' : 'bg-dark-200 text-dark-800'"
      >
        Активные
      </button>
      <button
        @click="activeTab = 'overdue'"
        class="px-4 py-2 rounded-lg font-medium transition"
        :class="activeTab === 'overdue' ? 'bg-primary-600 text-white' : 'bg-dark-200 text-dark-800'"
      >
        Просроченные
      </button>
    </div>

    <div v-if="loading" class="text-center py-8">
      <div class="text-dark-600">Загрузка...</div>
    </div>

    <div v-else-if="displayedLoans.length === 0" class="card text-center py-8">
      <div class="text-dark-600">Книг не найдено</div>
    </div>

    <div v-else class="card overflow-hidden">
      <table class="table">
        <thead>
          <tr>
            <th>Книга</th>
            <th>Дата выдачи</th>
            <th>Срок возврата</th>
            <th>Статус</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="loan in displayedLoans" :key="loan.id">
            <td class="font-medium">{{ loan.book_title }}</td>
            <td>{{ formatDate(loan.loan_date) }}</td>
            <td :class="{ 'text-red-600 font-semibold': isOverdue(loan) }">
              {{ formatDate(loan.due_date) }}
            </td>
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
import { ref, computed, onMounted } from 'vue'
import { useAuthStore } from '../../stores/auth'
import { getMyActiveLoans, getMyOverdueLoans } from '../../api'
import type { Loan, Reader } from '../../types'

const authStore = useAuthStore()
const loading = ref(false)
const activeLoans = ref<Loan[]>([])
const overdueLoans = ref<Loan[]>([])
const activeTab = ref<'active' | 'overdue'>('active')

const displayedLoans = computed(() =>
  activeTab.value === 'active' ? activeLoans.value : overdueLoans.value
)

const fetchLoans = async () => {
  loading.value = true
  try {
    const reader = authStore.user as Reader
    const [activeRes, overdueRes] = await Promise.all([
      getMyActiveLoans(reader.id),
      getMyOverdueLoans(reader.id)
    ])
    activeLoans.value = activeRes.data
    overdueLoans.value = overdueRes.data
  } catch (error) {
    console.error('Error fetching loans:', error)
  } finally {
    loading.value = false
  }
}

const formatDate = (dateString: string) => {
  return new Date(dateString).toLocaleDateString('ru-RU')
}

const isOverdue = (loan: Loan) => {
  return loan.status === 'overdue'
}

const statusLabel = (status: string) => {
  const labels: Record<string, string> = {
    active: 'Активна',
    overdue: 'Просрочена',
    returned: 'Возвращена'
  }
  return labels[status] || status
}

onMounted(fetchLoans)
</script>
