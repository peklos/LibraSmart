<template>
  <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
    <div class="flex justify-between items-center mb-6">
      <h1 class="text-3xl font-bold text-primary-600">Управление читателями</h1>
      <button @click="openCreateModal" class="btn btn-primary">
        Добавить читателя
      </button>
    </div>

    <div v-if="loading" class="text-center py-8">
      <div class="text-dark-600">Загрузка...</div>
    </div>

    <div v-else-if="readers.length === 0" class="card text-center py-8">
      <div class="text-dark-600">Читатели не найдены</div>
    </div>

    <div v-else class="card overflow-hidden">
      <table class="table">
        <thead>
          <tr>
            <th>ФИО</th>
            <th>Email</th>
            <th>Телефон</th>
            <th>Номер билета</th>
            <th>Действия</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="reader in readers" :key="reader.id">
            <td class="font-medium">{{ reader.full_name }}</td>
            <td>{{ reader.email }}</td>
            <td>{{ reader.phone || '—' }}</td>
            <td>{{ reader.library_card_number }}</td>
            <td>
              <div class="flex space-x-2">
                <button @click="openEditModal(reader)" class="btn btn-secondary text-xs">
                  Изменить
                </button>
                <button @click="deleteReader(reader.id)" class="btn btn-danger text-xs">
                  Удалить
                </button>
              </div>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- Create/Edit Modal -->
    <div v-if="showModal" class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center p-4 z-50">
      <div class="card max-w-md w-full">
        <h3 class="text-xl font-bold text-dark-900 mb-4">
          {{ editingReader ? 'Редактировать читателя' : 'Новый читатель' }}
        </h3>
        <form @submit.prevent="handleSubmit" class="space-y-4">
          <div>
            <label class="block text-sm font-medium text-dark-800 mb-1">ФИО</label>
            <input v-model="form.full_name" type="text" required class="input" />
          </div>
          <div>
            <label class="block text-sm font-medium text-dark-800 mb-1">Email</label>
            <input v-model="form.email" type="email" required class="input" />
          </div>
          <div>
            <label class="block text-sm font-medium text-dark-800 mb-1">Телефон</label>
            <input v-model="form.phone" type="tel" class="input" />
          </div>
          <div v-if="!editingReader">
            <label class="block text-sm font-medium text-dark-800 mb-1">Номер билета</label>
            <input v-model="form.library_card_number" type="text" required class="input" />
          </div>
          <div v-if="!editingReader">
            <label class="block text-sm font-medium text-dark-800 mb-1">Пароль</label>
            <input v-model="form.password" type="password" required class="input" />
          </div>
          <div v-if="modalError" class="text-red-600 text-sm">{{ modalError }}</div>
          <div class="flex space-x-2">
            <button type="button" @click="showModal = false" class="btn btn-secondary flex-1">
              Отмена
            </button>
            <button type="submit" class="btn btn-primary flex-1">
              {{ editingReader ? 'Сохранить' : 'Создать' }}
            </button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { getAllReaders, createReader, updateReader, deleteReader as apiDeleteReader } from '../../api'
import type { Reader } from '../../types'

const loading = ref(false)
const readers = ref<Reader[]>([])
const showModal = ref(false)
const editingReader = ref<Reader | null>(null)
const modalError = ref('')

const form = ref({
  full_name: '',
  email: '',
  phone: '',
  library_card_number: '',
  password: ''
})

const fetchReaders = async () => {
  loading.value = true
  try {
    const response = await getAllReaders()
    readers.value = response.data
  } catch (error) {
    console.error('Error fetching readers:', error)
  } finally {
    loading.value = false
  }
}

const openCreateModal = () => {
  editingReader.value = null
  form.value = {
    full_name: '',
    email: '',
    phone: '',
    library_card_number: '',
    password: ''
  }
  modalError.value = ''
  showModal.value = true
}

const openEditModal = (reader: Reader) => {
  editingReader.value = reader
  form.value = {
    full_name: reader.full_name,
    email: reader.email,
    phone: reader.phone || '',
    library_card_number: reader.library_card_number,
    password: ''
  }
  modalError.value = ''
  showModal.value = true
}

const handleSubmit = async () => {
  modalError.value = ''
  try {
    if (editingReader.value) {
      await updateReader(editingReader.value.id, form.value)
    } else {
      await createReader(form.value)
    }
    showModal.value = false
    fetchReaders()
  } catch (error: any) {
    modalError.value = error.response?.data?.detail || 'Ошибка сохранения'
  }
}

const deleteReader = async (id: number) => {
  if (!confirm('Удалить читателя?')) return
  try {
    await apiDeleteReader(id)
    fetchReaders()
  } catch (error: any) {
    alert(error.response?.data?.detail || 'Ошибка удаления')
  }
}

onMounted(fetchReaders)
</script>
