from fastapi import APIRouter, Depends, HTTPException
from sqlalchemy.orm import Session
from sqlalchemy import func, desc
from db.database import get_db
from db.models import Reader, Book, Loan, BookCopy, Library, Genre
from datetime import date

router = APIRouter(prefix="/admin/stats", tags=["Admin Statistics"])


@router.get("/dashboard")
def get_dashboard_stats(db: Session = Depends(get_db)):
    """Общая статистика"""

    # Количество читателей
    total_readers = db.query(Reader).count()

    # Количество книг
    total_books = db.query(Book).count()

    # Количество экземпляров
    total_copies = db.query(BookCopy).count()

    # Активные выдачи
    active_loans = db.query(Loan).filter(Loan.status.in_(["active", "overdue"])).count()

    # Просроченные выдачи
    today = date.today()
    overdue_loans = db.query(Loan).filter(
        Loan.status == "active",
        Loan.due_date < today
    ).count()

    # Всего выдач за всё время
    total_loans = db.query(Loan).count()

    # Доступные экземпляры
    available_copies = db.query(BookCopy).filter(BookCopy.status == "available").count()

    return {
        "total_readers": total_readers,
        "total_books": total_books,
        "total_copies": total_copies,
        "active_loans": active_loans,
        "overdue_loans": overdue_loans,
        "total_loans": total_loans,
        "available_copies": available_copies
    }


@router.get("/popular-books")
def get_popular_books(limit: int = 10, db: Session = Depends(get_db)):
    """Топ популярных книг"""

    popular = db.query(
        Book.id,
        Book.title,
        Book.author,
        func.count(Loan.id).label("loan_count")
    ).join(BookCopy, BookCopy.book_id == Book.id)\
     .join(Loan, Loan.copy_id == BookCopy.id)\
     .group_by(Book.id)\
     .order_by(desc("loan_count"))\
     .limit(limit)\
     .all()

    return [
        {
            "book_id": book.id,
            "title": book.title,
            "author": book.author,
            "loan_count": book.loan_count
        }
        for book in popular
    ]


@router.get("/popular-genres")
def get_popular_genres(limit: int = 10, db: Session = Depends(get_db)):
    """Топ жанров"""

    popular = db.query(
        Genre.id,
        Genre.genre_name,
        func.count(Loan.id).label("loan_count")
    ).join(Book, Book.genre_id == Genre.id)\
     .join(BookCopy, BookCopy.book_id == Book.id)\
     .join(Loan, Loan.copy_id == BookCopy.id)\
     .group_by(Genre.id)\
     .order_by(desc("loan_count"))\
     .limit(limit)\
     .all()

    return [
        {
            "genre_id": genre.id,
            "genre_name": genre.genre_name,
            "loan_count": genre.loan_count
        }
        for genre in popular
    ]


@router.get("/active-readers")
def get_active_readers(limit: int = 10, db: Session = Depends(get_db)):
    """Самые активные читатели"""

    active = db.query(
        Reader.id,
        Reader.full_name,
        Reader.email,
        func.count(Loan.id).label("loan_count")
    ).join(Loan, Loan.reader_id == Reader.id)\
     .group_by(Reader.id)\
     .order_by(desc("loan_count"))\
     .limit(limit)\
     .all()

    return [
        {
            "reader_id": reader.id,
            "full_name": reader.full_name,
            "email": reader.email,
            "loan_count": reader.loan_count
        }
        for reader in active
    ]


@router.get("/library/{library_id}")
def get_library_stats(library_id: int, db: Session = Depends(get_db)):
    """Статистика по конкретной библиотеке"""

    library = db.query(Library).filter(Library.id == library_id).first()
    if not library:
        raise HTTPException(status_code=404, detail="Библиотека не найдена")

    # Экземпляры в библиотеке
    total_copies = db.query(BookCopy).filter(BookCopy.library_id == library_id).count()

    # Доступные экземпляры
    available_copies = db.query(BookCopy).filter(
        BookCopy.library_id == library_id,
        BookCopy.status == "available"
    ).count()

    # Экземпляры на руках
    on_loan_copies = db.query(BookCopy).filter(
        BookCopy.library_id == library_id,
        BookCopy.status == "on_loan"
    ).count()

    return {
        "library_id": library_id,
        "library_name": library.library_name,
        "total_copies": total_copies,
        "available_copies": available_copies,
        "on_loan_copies": on_loan_copies
    }
