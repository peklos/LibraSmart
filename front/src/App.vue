<template>
  <div class="min-h-screen bg-dark-50">
    <Navbar v-if="authStore.isAuthenticated" />
    <main :class="authStore.isAuthenticated ? 'pt-16' : ''">
      <router-view />
    </main>
  </div>
</template>

<script setup lang="ts">
import { useAuthStore } from './stores/auth'
import Navbar from './components/Navbar.vue'

const authStore = useAuthStore()
</script>
