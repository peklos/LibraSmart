from fastapi import APIRouter, Depends, HTTPException
from sqlalchemy.orm import Session
from db.database import get_db
from db.models import Loan
from schemas.loan import LoanResponse, LoanWithDetails
from typing import List
from datetime import date

router = APIRouter(prefix="/loans", tags=["Reader Loans"])


@router.get("/my/{reader_id}", response_model=List[LoanWithDetails])
def get_my_loans(reader_id: int, db: Session = Depends(get_db)):
    """Мои текущие выдачи"""

    loans = db.query(Loan).filter(Loan.reader_id == reader_id).all()

    result = []
    for loan in loans:
        loan_dict = LoanWithDetails.model_validate(loan).model_dump()
        loan_dict["book_title"] = loan.copy.book.title
        loan_dict["reader_name"] = loan.reader.full_name
        loan_dict["staff_name"] = loan.staff.full_name
        loan_dict["inventory_number"] = loan.copy.inventory_number
        result.append(LoanWithDetails(**loan_dict))

    return result


@router.get("/my/{reader_id}/active", response_model=List[LoanWithDetails])
def get_my_active_loans(reader_id: int, db: Session = Depends(get_db)):
    """Активные займы"""

    loans = db.query(Loan).filter(
        Loan.reader_id == reader_id,
        Loan.status.in_(["active", "overdue"])
    ).all()

    result = []
    for loan in loans:
        loan_dict = LoanWithDetails.model_validate(loan).model_dump()
        loan_dict["book_title"] = loan.copy.book.title
        loan_dict["reader_name"] = loan.reader.full_name
        loan_dict["staff_name"] = loan.staff.full_name
        loan_dict["inventory_number"] = loan.copy.inventory_number
        result.append(LoanWithDetails(**loan_dict))

    return result


@router.get("/my/{reader_id}/overdue", response_model=List[LoanWithDetails])
def get_my_overdue_loans(reader_id: int, db: Session = Depends(get_db)):
    """Просроченные займы"""

    # Обновляем статусы просроченных займов
    today = date.today()
    overdue_loans = db.query(Loan).filter(
        Loan.reader_id == reader_id,
        Loan.status == "active",
        Loan.due_date < today
    ).all()

    for loan in overdue_loans:
        loan.status = "overdue"

    db.commit()

    # Получаем все просроченные займы
    loans = db.query(Loan).filter(
        Loan.reader_id == reader_id,
        Loan.status == "overdue"
    ).all()

    result = []
    for loan in loans:
        loan_dict = LoanWithDetails.model_validate(loan).model_dump()
        loan_dict["book_title"] = loan.copy.book.title
        loan_dict["reader_name"] = loan.reader.full_name
        loan_dict["staff_name"] = loan.staff.full_name
        loan_dict["inventory_number"] = loan.copy.inventory_number
        result.append(LoanWithDetails(**loan_dict))

    return result
