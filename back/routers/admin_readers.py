from fastapi import APIRouter, Depends, HTTPException
from sqlalchemy.orm import Session
from db.database import get_db
from db.models import Reader, Loan
from schemas.reader import ReaderCreate, ReaderUpdate, ReaderResponse
from schemas.loan import LoanWithDetails
from typing import List
from datetime import datetime

router = APIRouter(prefix="/admin/readers", tags=["Admin Readers"])


@router.get("", response_model=List[ReaderResponse])
def get_all_readers(db: Session = Depends(get_db)):
    """Все читатели"""
    readers = db.query(Reader).all()
    return readers


@router.get("/{reader_id}", response_model=ReaderResponse)
def get_reader(reader_id: int, db: Session = Depends(get_db)):
    """Детали читателя"""
    reader = db.query(Reader).filter(Reader.id == reader_id).first()

    if not reader:
        raise HTTPException(status_code=404, detail="Читатель не найден")

    return reader


@router.post("", response_model=ReaderResponse)
def create_reader(reader_data: ReaderCreate, db: Session = Depends(get_db)):
    """Создать читателя"""

    # Проверяем уникальность email
    existing_email = db.query(Reader).filter(Reader.email == reader_data.email).first()
    if existing_email:
        raise HTTPException(status_code=400, detail="Email уже используется")

    # Проверяем уникальность номера билета
    existing_card = db.query(Reader).filter(Reader.library_card_number == reader_data.library_card_number).first()
    if existing_card:
        raise HTTPException(status_code=400, detail="Номер читательского билета уже используется")

    new_reader = Reader(
        full_name=reader_data.full_name,
        email=reader_data.email,
        password=reader_data.password,
        phone=reader_data.phone,
        library_card_number=reader_data.library_card_number,
        created_at=datetime.utcnow()
    )

    db.add(new_reader)
    db.commit()
    db.refresh(new_reader)

    return new_reader


@router.patch("/{reader_id}", response_model=ReaderResponse)
def update_reader(reader_id: int, update_data: ReaderUpdate, db: Session = Depends(get_db)):
    """Обновить читателя"""

    reader = db.query(Reader).filter(Reader.id == reader_id).first()

    if not reader:
        raise HTTPException(status_code=404, detail="Читатель не найден")

    update_dict = update_data.model_dump(exclude_unset=True)

    for key, value in update_dict.items():
        setattr(reader, key, value)

    db.commit()
    db.refresh(reader)

    return reader


@router.delete("/{reader_id}")
def delete_reader(reader_id: int, db: Session = Depends(get_db)):
    """Удалить читателя"""

    reader = db.query(Reader).filter(Reader.id == reader_id).first()

    if not reader:
        raise HTTPException(status_code=404, detail="Читатель не найден")

    # Проверяем, нет ли активных займов
    active_loans = db.query(Loan).filter(
        Loan.reader_id == reader_id,
        Loan.status.in_(["active", "overdue"])
    ).count()

    if active_loans > 0:
        raise HTTPException(status_code=400, detail="Нельзя удалить читателя с активными займами")

    db.delete(reader)
    db.commit()

    return {"message": "Читатель удалён"}


@router.get("/{reader_id}/loans", response_model=List[LoanWithDetails])
def get_reader_loans_history(reader_id: int, db: Session = Depends(get_db)):
    """История выдач читателя"""

    reader = db.query(Reader).filter(Reader.id == reader_id).first()
    if not reader:
        raise HTTPException(status_code=404, detail="Читатель не найден")

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
