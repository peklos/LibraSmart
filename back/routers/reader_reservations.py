from fastapi import APIRouter, Depends, HTTPException, Query
from sqlalchemy.orm import Session
from db.database import get_db
from db.models import Reservation, Book, Library, Reader
from schemas.reservation import ReservationCreate, ReservationResponse, ReservationWithDetails
from datetime import datetime
from typing import List

router = APIRouter(prefix="/reservations", tags=["Reader Reservations"])


@router.post("", response_model=ReservationResponse)
def create_reservation(
    reservation_data: ReservationCreate,
    reader_id: int = Query(..., description="ID читателя"),
    db: Session = Depends(get_db)
):
    """Создать бронирование"""

    # Проверяем существование читателя
    reader = db.query(Reader).filter(Reader.id == reader_id).first()
    if not reader:
        raise HTTPException(status_code=404, detail="Читатель не найден")

    # Проверяем существование книги
    book = db.query(Book).filter(Book.id == reservation_data.book_id).first()
    if not book:
        raise HTTPException(status_code=404, detail="Книга не найдена")

    # Проверяем существование библиотеки
    library = db.query(Library).filter(Library.id == reservation_data.library_id).first()
    if not library:
        raise HTTPException(status_code=404, detail="Библиотека не найдена")

    # Проверяем, нет ли уже активного бронирования этой книги у читателя
    existing = db.query(Reservation).filter(
        Reservation.reader_id == reader_id,
        Reservation.book_id == reservation_data.book_id,
        Reservation.status == "active"
    ).first()

    if existing:
        raise HTTPException(status_code=400, detail="У вас уже есть активное бронирование этой книги")

    # Создаем бронирование
    new_reservation = Reservation(
        reader_id=reader_id,
        book_id=reservation_data.book_id,
        library_id=reservation_data.library_id,
        reservation_date=datetime.utcnow(),
        status="active"
    )

    db.add(new_reservation)
    db.commit()
    db.refresh(new_reservation)

    return new_reservation


@router.get("/my/{reader_id}", response_model=List[ReservationWithDetails])
def get_my_reservations(reader_id: int, db: Session = Depends(get_db)):
    """Мои бронирования"""

    reservations = db.query(Reservation).filter(Reservation.reader_id == reader_id).all()

    result = []
    for reservation in reservations:
        res_dict = ReservationWithDetails.model_validate(reservation).model_dump()
        res_dict["book_title"] = reservation.book.title
        res_dict["reader_name"] = reservation.reader.full_name
        res_dict["library_name"] = reservation.library.library_name
        result.append(ReservationWithDetails(**res_dict))

    return result


@router.delete("/{reservation_id}")
def cancel_reservation(reservation_id: int, db: Session = Depends(get_db)):
    """Отменить бронирование"""

    reservation = db.query(Reservation).filter(Reservation.id == reservation_id).first()

    if not reservation:
        raise HTTPException(status_code=404, detail="Бронирование не найдено")

    # Меняем статус на cancelled
    reservation.status = "cancelled"
    db.commit()

    return {"message": "Бронирование отменено"}
