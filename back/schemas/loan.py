from pydantic import BaseModel
from datetime import date


class LoanBase(BaseModel):
    reader_id: int
    copy_id: int
    staff_id: int
    loan_date: date
    due_date: date
    status: str = "active"  # active, returned, overdue


class LoanCreate(BaseModel):
    reader_id: int
    copy_id: int
    staff_id: int
    due_date: date


class LoanUpdate(BaseModel):
    return_date: date | None = None
    status: str | None = None


class LoanResponse(LoanBase):
    id: int
    return_date: date | None = None

    model_config = {"from_attributes": True}


class LoanWithDetails(LoanResponse):
    book_title: str | None = None
    reader_name: str | None = None
    staff_name: str | None = None
    inventory_number: str | None = None
