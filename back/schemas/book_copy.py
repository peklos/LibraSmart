from pydantic import BaseModel


class BookCopyBase(BaseModel):
    book_id: int
    library_id: int
    inventory_number: str
    status: str  # available, on_loan, maintenance, lost


class BookCopyCreate(BookCopyBase):
    pass


class BookCopyUpdate(BaseModel):
    book_id: int | None = None
    library_id: int | None = None
    inventory_number: str | None = None
    status: str | None = None


class BookCopyResponse(BookCopyBase):
    id: int

    model_config = {"from_attributes": True}


class BookCopyWithDetails(BookCopyResponse):
    book_title: str | None = None
    library_name: str | None = None
