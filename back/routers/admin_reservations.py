from fastapi import APIRouter, Depends, HTTPException
from sqlalchemy.orm import Session
from db.database import get_db
from db.models import Reservation
from schemas.reservation import ReservationUpdate, ReservationWithDetails
from typing import List

router = APIRouter(prefix="/admin/reservations", tags=["Admin Reservations"])


@router.get("", response_model=List[ReservationWithDetails])
def get_all_reservations(db: Session = Depends(get_db)):
    """Все бронирования"""
    reservations = db.query(Reservation).all()

    result = []
    for reservation in reservations:
        res_dict = ReservationWithDetails.model_validate(reservation).model_dump()
        res_dict["book_title"] = reservation.book.title
        res_dict["reader_name"] = reservation.reader.full_name
        res_dict["library_name"] = reservation.library.library_name
        result.append(ReservationWithDetails(**res_dict))

    return result


@router.get("/active", response_model=List[ReservationWithDetails])
def get_active_reservations(db: Session = Depends(get_db)):
    """Активные бронирования"""
    reservations = db.query(Reservation).filter(Reservation.status == "active").all()

    result = []
    for reservation in reservations:
        res_dict = ReservationWithDetails.model_validate(reservation).model_dump()
        res_dict["book_title"] = reservation.book.title
        res_dict["reader_name"] = reservation.reader.full_name
        res_dict["library_name"] = reservation.library.library_name
        result.append(ReservationWithDetails(**res_dict))

    return result


@router.patch("/{reservation_id}", response_model=ReservationWithDetails)
def update_reservation(reservation_id: int, update_data: ReservationUpdate, db: Session = Depends(get_db)):
    """Обновить статус бронирования"""

    reservation = db.query(Reservation).filter(Reservation.id == reservation_id).first()

    if not reservation:
        raise HTTPException(status_code=404, detail="Бронирование не найдено")

    update_dict = update_data.model_dump(exclude_unset=True)

    for key, value in update_dict.items():
        setattr(reservation, key, value)

    db.commit()
    db.refresh(reservation)

    res_dict = ReservationWithDetails.model_validate(reservation).model_dump()
    res_dict["book_title"] = reservation.book.title
    res_dict["reader_name"] = reservation.reader.full_name
    res_dict["library_name"] = reservation.library.library_name

    return ReservationWithDetails(**res_dict)


@router.delete("/{reservation_id}")
def delete_reservation(reservation_id: int, db: Session = Depends(get_db)):
    """Удалить бронирование"""

    reservation = db.query(Reservation).filter(Reservation.id == reservation_id).first()

    if not reservation:
        raise HTTPException(status_code=404, detail="Бронирование не найдено")

    db.delete(reservation)
    db.commit()

    return {"message": "Бронирование удалено"}
