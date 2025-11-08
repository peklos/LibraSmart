from fastapi import APIRouter, Depends, HTTPException
from sqlalchemy.orm import Session
from db.database import get_db
from db.models import Book, Genre
from schemas.book import BookCreate, BookUpdate, BookResponse, BookWithGenre
from typing import List

router = APIRouter(prefix="/admin/books", tags=["Admin Books"])


@router.get("", response_model=List[BookWithGenre])
def get_all_books(db: Session = Depends(get_db)):
    """Все книги"""
    books = db.query(Book).all()

    result = []
    for book in books:
        book_dict = BookWithGenre.model_validate(book).model_dump()
        book_dict["genre_name"] = book.genre.genre_name
        result.append(BookWithGenre(**book_dict))

    return result


@router.get("/{book_id}", response_model=BookWithGenre)
def get_book(book_id: int, db: Session = Depends(get_db)):
    """Детали книги"""
    book = db.query(Book).filter(Book.id == book_id).first()

    if not book:
        raise HTTPException(status_code=404, detail="Книга не найдена")

    book_dict = BookWithGenre.model_validate(book).model_dump()
    book_dict["genre_name"] = book.genre.genre_name

    return BookWithGenre(**book_dict)


@router.post("", response_model=BookResponse)
def create_book(book_data: BookCreate, db: Session = Depends(get_db)):
    """Добавить книгу"""

    # Проверяем существование жанра
    genre = db.query(Genre).filter(Genre.id == book_data.genre_id).first()
    if not genre:
        raise HTTPException(status_code=404, detail="Жанр не найден")

    new_book = Book(**book_data.model_dump())

    db.add(new_book)
    db.commit()
    db.refresh(new_book)

    return new_book


@router.patch("/{book_id}", response_model=BookResponse)
def update_book(book_id: int, update_data: BookUpdate, db: Session = Depends(get_db)):
    """Обновить книгу"""

    book = db.query(Book).filter(Book.id == book_id).first()

    if not book:
        raise HTTPException(status_code=404, detail="Книга не найдена")

    update_dict = update_data.model_dump(exclude_unset=True)

    # Если обновляется жанр, проверяем его существование
    if "genre_id" in update_dict:
        genre = db.query(Genre).filter(Genre.id == update_dict["genre_id"]).first()
        if not genre:
            raise HTTPException(status_code=404, detail="Жанр не найден")

    for key, value in update_dict.items():
        setattr(book, key, value)

    db.commit()
    db.refresh(book)

    return book


@router.delete("/{book_id}")
def delete_book(book_id: int, db: Session = Depends(get_db)):
    """Удалить книгу"""

    book = db.query(Book).filter(Book.id == book_id).first()

    if not book:
        raise HTTPException(status_code=404, detail="Книга не найдена")

    db.delete(book)
    db.commit()

    return {"message": "Книга удалена"}
