from pydantic import BaseModel, EmailStr


class StaffBase(BaseModel):
    full_name: str
    position: str
    library_id: int
    email: EmailStr
    role_id: int


class StaffCreate(StaffBase):
    password: str


class StaffUpdate(BaseModel):
    full_name: str | None = None
    position: str | None = None
    library_id: int | None = None
    email: EmailStr | None = None
    role_id: int | None = None
    password: str | None = None


class StaffLogin(BaseModel):
    email: EmailStr
    password: str


class StaffResponse(StaffBase):
    id: int

    model_config = {"from_attributes": True}
