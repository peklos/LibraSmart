import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '../stores/auth'

const router = createRouter({
  history: createWebHistory(),
  routes: [
    {
      path: '/',
      name: 'home',
      component: () => import('../views/Home.vue')
    },
    {
      path: '/login',
      name: 'login',
      component: () => import('../views/Login.vue')
    },
    // READER ROUTES
    {
      path: '/reader',
      meta: { requiresAuth: true, userType: 'reader' },
      children: [
        {
          path: 'catalog',
          name: 'reader-catalog',
          component: () => import('../views/reader/Catalog.vue')
        },
        {
          path: 'reservations',
          name: 'reader-reservations',
          component: () => import('../views/reader/Reservations.vue')
        },
        {
          path: 'loans',
          name: 'reader-loans',
          component: () => import('../views/reader/Loans.vue')
        },
        {
          path: 'history',
          name: 'reader-history',
          component: () => import('../views/reader/History.vue')
        },
        {
          path: 'profile',
          name: 'reader-profile',
          component: () => import('../views/reader/Profile.vue')
        }
      ]
    },
    // STAFF ROUTES
    {
      path: '/staff',
      meta: { requiresAuth: true, userType: 'staff' },
      children: [
        {
          path: 'dashboard',
          name: 'staff-dashboard',
          component: () => import('../views/staff/Dashboard.vue')
        },
        {
          path: 'readers',
          name: 'staff-readers',
          component: () => import('../views/staff/Readers.vue')
        },
        {
          path: 'books',
          name: 'staff-books',
          component: () => import('../views/staff/Books.vue')
        },
        {
          path: 'copies',
          name: 'staff-copies',
          component: () => import('../views/staff/Copies.vue')
        },
        {
          path: 'reservations',
          name: 'staff-reservations',
          component: () => import('../views/staff/Reservations.vue')
        },
        {
          path: 'loans',
          name: 'staff-loans',
          component: () => import('../views/staff/Loans.vue')
        },
        {
          path: 'staff-management',
          name: 'staff-management',
          component: () => import('../views/staff/StaffManagement.vue'),
          meta: { requiresAdmin: true }
        },
        {
          path: 'libraries',
          name: 'staff-libraries',
          component: () => import('../views/staff/Libraries.vue')
        },
        {
          path: 'genres',
          name: 'staff-genres',
          component: () => import('../views/staff/Genres.vue')
        }
      ]
    }
  ]
})

router.beforeEach((to, from, next) => {
  const authStore = useAuthStore()

  if (to.meta.requiresAuth && !authStore.isAuthenticated) {
    next('/login')
  } else if (to.meta.userType && to.meta.userType !== authStore.userType) {
    next('/')
  } else if (to.meta.requiresAdmin && !authStore.isAdmin()) {
    next('/staff/dashboard')
  } else {
    next()
  }
})

export default router
