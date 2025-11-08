from pydantic import BaseModel
from datetime import datetime


class ReservationBase(BaseModel):
    reader_id: int
    book_id: int
    library_id: int
    status: str = "active"  # active, completed, cancelled


class ReservationCreate(BaseModel):
    book_id: int
    library_id: int


class ReservationUpdate(BaseModel):
    status: str | None = None


class ReservationResponse(ReservationBase):
    id: int
    reservation_date: datetime

    model_config = {"from_attributes": True}


class ReservationWithDetails(ReservationResponse):
    book_title: str | None = None
    reader_name: str | None = None
    library_name: str | None = None
