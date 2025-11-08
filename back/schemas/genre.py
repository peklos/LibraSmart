from pydantic import BaseModel


class GenreBase(BaseModel):
    genre_name: str


class GenreCreate(GenreBase):
    pass


class GenreUpdate(BaseModel):
    genre_name: str | None = None


class GenreResponse(GenreBase):
    id: int

    model_config = {"from_attributes": True}
