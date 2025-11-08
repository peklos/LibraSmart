from fastapi import APIRouter, Depends, HTTPException
from sqlalchemy.orm import Session
from sqlalchemy import func
from db.database import get_db
from db.models import Loan, Genre, Book, BookCopy
from schemas.loan import LoanWithDetails
from typing import List

router = APIRouter(prefix="/history", tags=["Reader History"])


@router.get("/{reader_id}", response_model=List[LoanWithDetails])
def get_reading_history(reader_id: int, db: Session = Depends(get_db)):
    """История всех моих выдач (включая возвращенные)"""

    loans = db.query(Loan).filter(Loan.reader_id == reader_id).order_by(Loan.loan_date.desc()).all()

    result = []
    for loan in loans:
        loan_dict = LoanWithDetails.model_validate(loan).model_dump()
        loan_dict["book_title"] = loan.copy.book.title
        loan_dict["reader_name"] = loan.reader.full_name
        loan_dict["staff_name"] = loan.staff.full_name
        loan_dict["inventory_number"] = loan.copy.inventory_number
        result.append(LoanWithDetails(**loan_dict))

    return result


@router.get("/{reader_id}/stats")
def get_reading_stats(reader_id: int, db: Session = Depends(get_db)):
    """Статистика по чтению"""

    # Общее количество прочитанных книг
    total_read = db.query(Loan).filter(
        Loan.reader_id == reader_id,
        Loan.status == "returned"
    ).count()

    # Текущие активные займы
    active_loans = db.query(Loan).filter(
        Loan.reader_id == reader_id,
        Loan.status.in_(["active", "overdue"])
    ).count()

    # Просроченные займы
    overdue_loans = db.query(Loan).filter(
        Loan.reader_id == reader_id,
        Loan.status == "overdue"
    ).count()

    # Любимые жанры (топ 3)
    favorite_genres = db.query(
        Genre.genre_name,
        func.count(Loan.id).label("count")
    ).join(BookCopy, BookCopy.id == Loan.copy_id)\
     .join(Book, Book.id == BookCopy.book_id)\
     .join(Genre, Genre.id == Book.genre_id)\
     .filter(Loan.reader_id == reader_id)\
     .group_by(Genre.id, Genre.genre_name)\
     .order_by(func.count(Loan.id).desc())\
     .limit(3)\
     .all()

    # Всего займов
    total_loans = db.query(Loan).filter(Loan.reader_id == reader_id).count()

    return {
        "reader_id": reader_id,
        "total_books_read": total_read,
        "active_loans": active_loans,
        "overdue_loans": overdue_loans,
        "total_loans": total_loans,
        "favorite_genres": [{"genre": g[0], "count": g[1]} for g in favorite_genres] if favorite_genres else []
    }
