<template>
  <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
    <h1 class="text-3xl font-bold text-primary-600 mb-6">Библиотеки</h1>

    <div v-if="loading" class="text-center py-8">
      <div class="text-dark-600">Загрузка...</div>
    </div>

    <div v-else-if="libraries.length === 0" class="card text-center py-8">
      <div class="text-dark-600">Библиотеки не найдены</div>
    </div>

    <div v-else class="grid md:grid-cols-2 lg:grid-cols-3 gap-4">
      <div v-for="lib in libraries" :key="lib.id" class="card hover:shadow-lg transition-shadow">
        <div class="flex items-start space-x-3 mb-3">
          <div class="text-3xl">📚</div>
          <div class="flex-1">
            <h3 class="font-bold text-dark-900 text-lg">{{ lib.library_name }}</h3>
          </div>
        </div>
        <div class="space-y-2">
          <div class="flex items-start space-x-2">
            <span class="text-dark-600">📍</span>
            <p class="text-sm text-dark-700 flex-1">{{ lib.address }}</p>
          </div>
          <div class="flex items-center space-x-2">
            <span class="text-dark-600">📞</span>
            <p class="text-sm text-dark-700">{{ lib.phone }}</p>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { getAllLibraries } from '../../api'
import type { Library } from '../../types'

const loading = ref(false)
const libraries = ref<Library[]>([])

const fetchLibraries = async () => {
  loading.value = true
  try {
    const response = await getAllLibraries()
    libraries.value = response.data
  } catch (error) {
    console.error('Error fetching libraries:', error)
  } finally {
    loading.value = false
  }
}

onMounted(fetchLibraries)
</script>
