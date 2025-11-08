from pydantic import BaseModel


class LibraryBase(BaseModel):
    library_name: str
    address: str
    phone: str


class LibraryCreate(LibraryBase):
    pass


class LibraryUpdate(BaseModel):
    library_name: str | None = None
    address: str | None = None
    phone: str | None = None


class LibraryResponse(LibraryBase):
    id: int

    model_config = {"from_attributes": True}
