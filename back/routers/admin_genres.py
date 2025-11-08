from fastapi import APIRouter, Depends, HTTPException
from sqlalchemy.orm import Session
from db.database import get_db
from db.models import Genre
from schemas.genre import GenreCreate, GenreUpdate, GenreResponse
from typing import List

router = APIRouter(prefix="/admin/genres", tags=["Admin Genres"])


@router.get("", response_model=List[GenreResponse])
def get_all_genres(db: Session = Depends(get_db)):
    """Все жанры"""
    genres = db.query(Genre).all()
    return genres


@router.post("", response_model=GenreResponse)
def create_genre(genre_data: GenreCreate, db: Session = Depends(get_db)):
    """Добавить жанр"""

    # Проверяем уникальность
    existing = db.query(Genre).filter(Genre.genre_name == genre_data.genre_name).first()
    if existing:
        raise HTTPException(status_code=400, detail="Жанр с таким названием уже существует")

    new_genre = Genre(**genre_data.model_dump())

    db.add(new_genre)
    db.commit()
    db.refresh(new_genre)

    return new_genre


@router.patch("/{genre_id}", response_model=GenreResponse)
def update_genre(genre_id: int, update_data: GenreUpdate, db: Session = Depends(get_db)):
    """Обновить жанр"""

    genre = db.query(Genre).filter(Genre.id == genre_id).first()

    if not genre:
        raise HTTPException(status_code=404, detail="Жанр не найден")

    update_dict = update_data.model_dump(exclude_unset=True)

    for key, value in update_dict.items():
        setattr(genre, key, value)

    db.commit()
    db.refresh(genre)

    return genre


@router.delete("/{genre_id}")
def delete_genre(genre_id: int, db: Session = Depends(get_db)):
    """Удалить жанр"""

    genre = db.query(Genre).filter(Genre.id == genre_id).first()

    if not genre:
        raise HTTPException(status_code=404, detail="Жанр не найден")

    db.delete(genre)
    db.commit()

    return {"message": "Жанр удалён"}
