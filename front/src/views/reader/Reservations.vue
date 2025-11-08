<template>
  <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
    <h1 class="text-3xl font-bold text-primary-600 mb-6">Мои бронирования</h1>

    <div v-if="loading" class="text-center py-8">
      <div class="text-dark-600">Загрузка...</div>
    </div>

    <div v-else-if="reservations.length === 0" class="card text-center py-8">
      <div class="text-dark-600">У вас пока нет бронирований</div>
    </div>

    <div v-else class="card overflow-hidden">
      <table class="table">
        <thead>
          <tr>
            <th>Книга</th>
            <th>Библиотека</th>
            <th>Дата бронирования</th>
            <th>Статус</th>
            <th>Действия</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="reservation in reservations" :key="reservation.id">
            <td class="font-medium">{{ reservation.book_title }}</td>
            <td>{{ reservation.library_name }}</td>
            <td>{{ formatDate(reservation.reservation_date) }}</td>
            <td>
              <span
                class="badge"
                :class="{
                  'badge-success': reservation.status === 'active',
                  'badge-info': reservation.status === 'completed',
                  'badge-danger': reservation.status === 'cancelled'
                }"
              >
                {{ statusLabel(reservation.status) }}
              </span>
            </td>
            <td>
              <button
                v-if="reservation.status === 'active'"
                @click="cancel(reservation.id)"
                class="btn btn-danger text-xs"
              >
                Отменить
              </button>
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
import { getMyReservations, cancelReservation } from '../../api'
import type { Reservation, Reader } from '../../types'

const authStore = useAuthStore()
const loading = ref(false)
const reservations = ref<Reservation[]>([])

const fetchReservations = async () => {
  loading.value = true
  try {
    const reader = authStore.user as Reader
    const response = await getMyReservations(reader.id)
    reservations.value = response.data
  } catch (error) {
    console.error('Error fetching reservations:', error)
  } finally {
    loading.value = false
  }
}

const cancel = async (id: number) => {
  if (!confirm('Отменить бронирование?')) return
  try {
    await cancelReservation(id)
    fetchReservations()
  } catch (error) {
    console.error('Error cancelling reservation:', error)
    alert('Ошибка отмены бронирования')
  }
}

const formatDate = (dateString: string) => {
  return new Date(dateString).toLocaleDateString('ru-RU')
}

const statusLabel = (status: string) => {
  const labels: Record<string, string> = {
    active: 'Активное',
    completed: 'Завершено',
    cancelled: 'Отменено'
  }
  return labels[status] || status
}

onMounted(fetchReservations)
</script>
