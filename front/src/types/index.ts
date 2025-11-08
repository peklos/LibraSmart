export interface Reader {
  id: number
  full_name: string
  email: string
  phone?: string
  library_card_number: string
  created_at: string
}

export interface Staff {
  id: number
  full_name: string
  position: string
  library_id: number
  email: string
  role_id: number
}

export interface Book {
  id: number
  title: string
  author: string
  publication_year?: number
  genre_id: number
  description?: string
  isbn?: string
  genre_name?: string
}

export interface BookCopy {
  id: number
  book_id: number
  library_id: number
  inventory_number: string
  status: string
  book_title?: string
  library_name?: string
}

export interface Reservation {
  id: number
  reader_id: number
  book_id: number
  library_id: number
  reservation_date: string
  status: string
  book_title?: string
  reader_name?: string
  library_name?: string
}

export interface Loan {
  id: number
  reader_id: number
  copy_id: number
  staff_id: number
  loan_date: string
  due_date: string
  return_date?: string
  status: string
  book_title?: string
  reader_name?: string
  staff_name?: string
  inventory_number?: string
}

export interface Library {
  id: number
  library_name: string
  address: string
  phone: string
}

export interface Genre {
  id: number
  genre_name: string
}

export interface DashboardStats {
  total_readers: number
  total_books: number
  total_copies: number
  active_loans: number
  overdue_loans: number
  total_loans: number
  available_copies: number
}
