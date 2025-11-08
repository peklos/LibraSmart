from pydantic import BaseModel


class BookBase(BaseModel):
    title: str
    author: str
    publication_year: int | None = None
    genre_id: int
    description: str | None = None
    isbn: str | None = None


class BookCreate(BookBase):
    pass


class BookUpdate(BaseModel):
    title: str | None = None
    author: str | None = None
    publication_year: int | None = None
    genre_id: int | None = None
    description: str | None = None
    isbn: str | None = None


class BookResponse(BookBase):
    id: int

    model_config = {"from_attributes": True}


class BookWithGenre(BookResponse):
    genre_name: str | None = None
