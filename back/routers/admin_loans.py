from fastapi import APIRouter, Depends, HTTPException
from sqlalchemy.orm import Session
from db.database import get_db
from db.models import Loan, BookCopy, Reader, Staff
from schemas.loan import LoanCreate, LoanUpdate, LoanWithDetails
from typing import List
from datetime import date

router = APIRouter(prefix="/admin/loans", tags=["Admin Loans"])


@router.get("", response_model=List[LoanWithDetails])
def get_all_loans(db: Session = Depends(get_db)):
    """Все выдачи"""
    loans = db.query(Loan).all()

    result = []
    for loan in loans:
        loan_dict = LoanWithDetails.model_validate(loan).model_dump()
        loan_dict["book_title"] = loan.copy.book.title
        loan_dict["reader_name"] = loan.reader.full_name
        loan_dict["staff_name"] = loan.staff.full_name
        loan_dict["inventory_number"] = loan.copy.inventory_number
        result.append(LoanWithDetails(**loan_dict))

    return result


@router.post("", response_model=LoanWithDetails)
def create_loan(loan_data: LoanCreate, db: Session = Depends(get_db)):
    """Создать выдачу (выдать книгу читателю)"""

    # Проверяем существование читателя
    reader = db.query(Reader).filter(Reader.id == loan_data.reader_id).first()
    if not reader:
        raise HTTPException(status_code=404, detail="Читатель не найден")

    # Проверяем существование экземпляра
    copy = db.query(BookCopy).filter(BookCopy.id == loan_data.copy_id).first()
    if not copy:
        raise HTTPException(status_code=404, detail="Экземпляр книги не найден")

    # Проверяем доступность экземпляра
    if copy.status != "available":
        raise HTTPException(status_code=400, detail="Экземпляр недоступен для выдачи")

    # Проверяем существование сотрудника
    staff = db.query(Staff).filter(Staff.id == loan_data.staff_id).first()
    if not staff:
        raise HTTPException(status_code=404, detail="Сотрудник не найден")

    # Создаем выдачу
    new_loan = Loan(
        reader_id=loan_data.reader_id,
        copy_id=loan_data.copy_id,
        staff_id=loan_data.staff_id,
        loan_date=date.today(),
        due_date=loan_data.due_date,
        status="active"
    )

    # Обновляем статус экземпляра
    copy.status = "on_loan"

    db.add(new_loan)
    db.commit()
    db.refresh(new_loan)

    loan_dict = LoanWithDetails.model_validate(new_loan).model_dump()
    loan_dict["book_title"] = new_loan.copy.book.title
    loan_dict["reader_name"] = new_loan.reader.full_name
    loan_dict["staff_name"] = new_loan.staff.full_name
    loan_dict["inventory_number"] = new_loan.copy.inventory_number

    return LoanWithDetails(**loan_dict)


@router.patch("/{loan_id}/return", response_model=LoanWithDetails)
def return_loan(loan_id: int, db: Session = Depends(get_db)):
    """Принять возврат книги"""

    loan = db.query(Loan).filter(Loan.id == loan_id).first()

    if not loan:
        raise HTTPException(status_code=404, detail="Выдача не найдена")

    if loan.status == "returned":
        raise HTTPException(status_code=400, detail="Книга уже возвращена")

    # Обновляем выдачу
    loan.return_date = date.today()
    loan.status = "returned"

    # Обновляем статус экземпляра
    copy = db.query(BookCopy).filter(BookCopy.id == loan.copy_id).first()
    if copy:
        copy.status = "available"

    db.commit()
    db.refresh(loan)

    loan_dict = LoanWithDetails.model_validate(loan).model_dump()
    loan_dict["book_title"] = loan.copy.book.title
    loan_dict["reader_name"] = loan.reader.full_name
    loan_dict["staff_name"] = loan.staff.full_name
    loan_dict["inventory_number"] = loan.copy.inventory_number

    return LoanWithDetails(**loan_dict)


@router.get("/overdue", response_model=List[LoanWithDetails])
def get_overdue_loans(db: Session = Depends(get_db)):
    """Просроченные выдачи"""

    # Обновляем статусы просроченных займов
    today = date.today()
    overdue_loans = db.query(Loan).filter(
        Loan.status == "active",
        Loan.due_date < today
    ).all()

    for loan in overdue_loans:
        loan.status = "overdue"

    db.commit()

    # Получаем все просроченные
    loans = db.query(Loan).filter(Loan.status == "overdue").all()

    result = []
    for loan in loans:
        loan_dict = LoanWithDetails.model_validate(loan).model_dump()
        loan_dict["book_title"] = loan.copy.book.title
        loan_dict["reader_name"] = loan.reader.full_name
        loan_dict["staff_name"] = loan.staff.full_name
        loan_dict["inventory_number"] = loan.copy.inventory_number
        result.append(LoanWithDetails(**loan_dict))

    return result


@router.get("/active", response_model=List[LoanWithDetails])
def get_active_loans(db: Session = Depends(get_db)):
    """Активные выдачи"""

    loans = db.query(Loan).filter(Loan.status.in_(["active", "overdue"])).all()

    result = []
    for loan in loans:
        loan_dict = LoanWithDetails.model_validate(loan).model_dump()
        loan_dict["book_title"] = loan.copy.book.title
        loan_dict["reader_name"] = loan.reader.full_name
        loan_dict["staff_name"] = loan.staff.full_name
        loan_dict["inventory_number"] = loan.copy.inventory_number
        result.append(LoanWithDetails(**loan_dict))

    return result
