import axios from 'axios'
import type { Reader, Staff, Book, BookCopy, Reservation, Loan, Library, Genre, DashboardStats } from '../types'

const API_URL = import.meta.env.VITE_API_URL || '/api'

const api = axios.create({
  baseURL: API_URL,
})

// ========== READER AUTH ==========
export const readerRegister = (data: {
  full_name: string
  email: string
  password: string
  phone?: string
}) => api.post<Reader>('/auth/register', data)

export const readerLogin = (email: string, password: string) =>
  api.post<Reader>('/auth/login', { email, password })

export const getReaderProfile = (id: number) =>
  api.get<Reader>(`/auth/me/${id}`)

export const updateReaderProfile = (id: number, data: Partial<Reader>) =>
  api.patch<Reader>(`/profile/${id}`, data)

// ========== STAFF AUTH ==========
export const staffLogin = (email: string, password: string) =>
  api.post<Staff>('/admin/auth/login', { email, password })

export const getStaffProfile = (id: number) =>
  api.get<Staff>(`/admin/auth/me/${id}`)

// ========== BOOKS ==========
export const getBooks = (params?: {
  genre_id?: number
  author?: string
  search?: string
}) => api.get<Book[]>('/books', { params })

export const getBook = (id: number) =>
  api.get<Book>(`/books/${id}`)

export const getBookAvailability = (id: number) =>
  api.get(`/books/${id}/availability`)

// ========== RESERVATIONS ==========
export const createReservation = (reader_id: number, data: {
  book_id: number
  library_id: number
}) => api.post<Reservation>(`/reservations?reader_id=${reader_id}`, data)

export const getMyReservations = (reader_id: number) =>
  api.get<Reservation[]>(`/reservations/my/${reader_id}`)

export const cancelReservation = (id: number) =>
  api.delete(`/reservations/${id}`)

// ========== LOANS ==========
export const getMyLoans = (reader_id: number) =>
  api.get<Loan[]>(`/loans/my/${reader_id}`)

export const getMyActiveLoans = (reader_id: number) =>
  api.get<Loan[]>(`/loans/my/${reader_id}/active`)

export const getMyOverdueLoans = (reader_id: number) =>
  api.get<Loan[]>(`/loans/my/${reader_id}/overdue`)

// ========== HISTORY ==========
export const getReadingHistory = (reader_id: number) =>
  api.get<Loan[]>(`/history/${reader_id}`)

export const getReadingStats = (reader_id: number) =>
  api.get(`/history/${reader_id}/stats`)

// ========== ADMIN: READERS ==========
export const getAllReaders = () =>
  api.get<Reader[]>('/admin/readers')

export const getReader = (id: number) =>
  api.get<Reader>(`/admin/readers/${id}`)

export const createReader = (data: any) =>
  api.post<Reader>('/admin/readers', data)

export const updateReader = (id: number, data: Partial<Reader>) =>
  api.patch<Reader>(`/admin/readers/${id}`, data)

export const deleteReader = (id: number) =>
  api.delete(`/admin/readers/${id}`)

export const getReaderLoans = (id: number) =>
  api.get<Loan[]>(`/admin/readers/${id}/loans`)

// ========== ADMIN: BOOKS ==========
export const getAllBooks = () =>
  api.get<Book[]>('/admin/books')

export const createBook = (data: any) =>
  api.post<Book>('/admin/books', data)

export const updateBook = (id: number, data: Partial<Book>) =>
  api.patch<Book>(`/admin/books/${id}`, data)

export const deleteBook = (id: number) =>
  api.delete(`/admin/books/${id}`)

// ========== ADMIN: COPIES ==========
export const getAllCopies = () =>
  api.get<BookCopy[]>('/admin/copies')

export const getCopiesByLibrary = (library_id: number) =>
  api.get<BookCopy[]>(`/admin/copies/library/${library_id}`)

export const createCopy = (data: any) =>
  api.post<BookCopy>('/admin/copies', data)

export const updateCopy = (id: number, data: Partial<BookCopy>) =>
  api.patch<BookCopy>(`/admin/copies/${id}`, data)

export const deleteCopy = (id: number) =>
  api.delete(`/admin/copies/${id}`)

// ========== ADMIN: RESERVATIONS ==========
export const getAllReservations = () =>
  api.get<Reservation[]>('/admin/reservations')

export const getActiveReservations = () =>
  api.get<Reservation[]>('/admin/reservations/active')

export const updateReservation = (id: number, data: { status: string }) =>
  api.patch<Reservation>(`/admin/reservations/${id}`, data)

export const deleteReservation = (id: number) =>
  api.delete(`/admin/reservations/${id}`)

// ========== ADMIN: LOANS ==========
export const getAllLoans = () =>
  api.get<Loan[]>('/admin/loans')

export const createLoan = (data: any) =>
  api.post<Loan>('/admin/loans', data)

export const returnLoan = (id: number) =>
  api.patch<Loan>(`/admin/loans/${id}/return`, {})

export const getOverdueLoans = () =>
  api.get<Loan[]>('/admin/loans/overdue')

export const getActiveLoans = () =>
  api.get<Loan[]>('/admin/loans/active')

// ========== ADMIN: LIBRARIES ==========
export const getAllLibraries = () =>
  api.get<Library[]>('/admin/libraries')

export const createLibrary = (data: any) =>
  api.post<Library>('/admin/libraries', data)

export const updateLibrary = (id: number, data: Partial<Library>) =>
  api.patch<Library>(`/admin/libraries/${id}`, data)

export const deleteLibrary = (id: number) =>
  api.delete(`/admin/libraries/${id}`)

// ========== ADMIN: GENRES ==========
export const getAllGenres = () =>
  api.get<Genre[]>('/admin/genres')

export const createGenre = (data: { genre_name: string }) =>
  api.post<Genre>('/admin/genres', data)

export const updateGenre = (id: number, data: { genre_name: string }) =>
  api.patch<Genre>(`/admin/genres/${id}`, data)

export const deleteGenre = (id: number) =>
  api.delete(`/admin/genres/${id}`)

// ========== ADMIN: STAFF ==========
export const getAllStaff = (current_staff_id: number) =>
  api.get<Staff[]>(`/admin/staff?current_staff_id=${current_staff_id}`)

export const createStaff = (current_staff_id: number, data: any) =>
  api.post<Staff>(`/admin/staff?current_staff_id=${current_staff_id}`, data)

export const updateStaff = (current_staff_id: number, staff_id: number, data: any) =>
  api.patch<Staff>(`/admin/staff/${staff_id}?current_staff_id=${current_staff_id}`, data)

export const deleteStaff = (current_staff_id: number, staff_id: number) =>
  api.delete(`/admin/staff/${staff_id}?current_staff_id=${current_staff_id}`)

// ========== ADMIN: STATS ==========
export const getDashboardStats = () =>
  api.get<DashboardStats>('/admin/stats/dashboard')

export const getPopularBooks = (limit: number = 10) =>
  api.get(`/admin/stats/popular-books?limit=${limit}`)

export const getPopularGenres = (limit: number = 10) =>
  api.get(`/admin/stats/popular-genres?limit=${limit}`)

export const getActiveReaders = (limit: number = 10) =>
  api.get(`/admin/stats/active-readers?limit=${limit}`)

export const getLibraryStats = (library_id: number) =>
  api.get(`/admin/stats/library/${library_id}`)

export default api
