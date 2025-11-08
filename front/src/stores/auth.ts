import { defineStore } from 'pinia'
import { ref } from 'vue'
import type { Reader, Staff } from '../types'

export const useAuthStore = defineStore('auth', () => {
  const user = ref<Reader | Staff | null>(null)
  const userType = ref<'reader' | 'staff' | null>(null)
  const isAuthenticated = ref(false)

  function loginAsReader(readerData: Reader) {
    user.value = readerData
    userType.value = 'reader'
    isAuthenticated.value = true
    localStorage.setItem('user', JSON.stringify(readerData))
    localStorage.setItem('userType', 'reader')
  }

  function loginAsStaff(staffData: Staff) {
    user.value = staffData
    userType.value = 'staff'
    isAuthenticated.value = true
    localStorage.setItem('user', JSON.stringify(staffData))
    localStorage.setItem('userType', 'staff')
  }

  function logout() {
    user.value = null
    userType.value = null
    isAuthenticated.value = false
    localStorage.removeItem('user')
    localStorage.removeItem('userType')
  }

  function loadFromStorage() {
    const storedUser = localStorage.getItem('user')
    const storedUserType = localStorage.getItem('userType')

    if (storedUser && storedUserType) {
      user.value = JSON.parse(storedUser)
      userType.value = storedUserType as 'reader' | 'staff'
      isAuthenticated.value = true
    }
  }

  function isAdmin() {
    if (userType.value === 'staff' && user.value) {
      return (user.value as Staff).role_id === 1
    }
    return false
  }

  return {
    user,
    userType,
    isAuthenticated,
    loginAsReader,
    loginAsStaff,
    logout,
    loadFromStorage,
    isAdmin
  }
})
