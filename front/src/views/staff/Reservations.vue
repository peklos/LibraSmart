<template>
  <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
    <h1 class="text-3xl font-bold text-primary-600 mb-6">Управление бронированиями</h1>

    <div class="flex space-x-2 mb-6">
      <button @click="filter = 'all'" :class="filter === 'all' ? 'btn btn-primary' : 'btn btn-secondary'">Все</button>
      <button @click="filter = 'active'" :class="filter === 'active' ? 'btn btn-primary' : 'btn btn-secondary'">Активные</button>
    </div>

    <div v-if="loading" class="text-center py-8">Загрузка...</div>

    <div v-else class="card overflow-x-auto">
      <table class="table">
        <thead>
          <tr>
            <th>Читатель</th>
            <th>Книга</th>
            <th>Библиотека</th>
            <th>Дата</th>
            <th>Статус</th>
            <th>Действия</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="res in filteredReservations" :key="res.id">
            <td class="font-medium">{{ res.reader_name }}</td>
            <td>{{ res.book_title }}</td>
            <td>{{ res.library_name }}</td>
            <td>{{ formatDate(res.reservation_date) }}</td>
            <td>
              <span class="badge" :class="{
                'badge-success': res.status === 'active',
                'badge-info': res.status === 'completed',
                'badge-danger': res.status === 'cancelled'
              }">{{ statusLabel(res.status) }}</span>
            </td>
            <td>
              <div class="flex space-x-2">
                <button v-if="res.status === 'active'" @click="updateStatus(res.id, 'completed')" class="btn btn-primary text-xs">
                  Завершить
                </button>
                <button v-if="res.status === 'active'" @click="deleteReservation(res.id)" class="btn btn-danger text-xs">
                  Отменить
                </button>
              </div>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { getAllReservations, getActiveReservations, updateReservation, deleteReservation as apiDeleteReservation } from '../../api'
import type { Reservation } from '../../types'

const loading = ref(false)
const reservations = ref<Reservation[]>([])
const filter = ref('all')

const filteredReservations = computed(() =>
  filter.value === 'active' ? reservations.value.filter(r => r.status === 'active') : reservations.value
)

const fetchReservations = async () => {
  loading.value = true
  try {
    const response = await getAllReservations()
    reservations.value = response.data
  } finally {
    loading.value = false
  }
}

const updateStatus = async (id: number, status: string) => {
  try {
    await updateReservation(id, { status })
    fetchReservations()
  } catch (error) {
    alert('Ошибка')
  }
}

const deleteReservation = async (id: number) => {
  if (!confirm('Отменить бронирование?')) return
  try {
    await apiDeleteReservation(id)
    fetchReservations()
  } catch (error) {
    alert('Ошибка')
  }
}

const formatDate = (date: string) => new Date(date).toLocaleDateString('ru-RU')
const statusLabel = (status: string) => ({ active: 'Активное', completed: 'Завершено', cancelled: 'Отменено' })[status] || status

onMounted(fetchReservations)
</script>
