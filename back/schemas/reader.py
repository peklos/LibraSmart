from pydantic import BaseModel, EmailStr
from datetime import datetime


class ReaderBase(BaseModel):
    full_name: str
    email: EmailStr
    phone: str | None = None


class ReaderCreate(ReaderBase):
    password: str
    library_card_number: str


class ReaderRegister(ReaderBase):
    password: str


class ReaderUpdate(BaseModel):
    full_name: str | None = None
    email: EmailStr | None = None
    phone: str | None = None
    password: str | None = None


class ReaderLogin(BaseModel):
    email: EmailStr
    password: str


class ReaderResponse(ReaderBase):
    id: int
    library_card_number: str
    created_at: datetime

    model_config = {"from_attributes": True}
