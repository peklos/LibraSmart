from fastapi import APIRouter, Depends, HTTPException, Query
from sqlalchemy.orm import Session
from sqlalchemy import or_
from db.database import get_db
from db.models import Book, BookCopy, Genre
from schemas.book import BookResponse, BookWithGenre
from typing import List

router = APIRouter(prefix="/books", tags=["Reader Books"])


@router.get("", response_model=List[BookWithGenre])
def get_books_catalog(
    genre_id: int | None = None,
    author: str | None = None,
    search: str | None = None,
    db: Session = Depends(get_db)
):
    """Каталог всех книг с фильтрацией"""

    query = db.query(Book).join(Genre)

    # Фильтр по жанру
    if genre_id:
        query = query.filter(Book.genre_id == genre_id)

    # Фильтр по автору
    if author:
        query = query.filter(Book.author.ilike(f"%{author}%"))

    # Поиск по названию или автору
    if search:
        query = query.filter(
            or_(
                Book.title.ilike(f"%{search}%"),
                Book.author.ilike(f"%{search}%")
            )
        )

    books = query.all()

    # Добавляем название жанра к каждой книге
    result = []
    for book in books:
        book_dict = BookWithGenre.model_validate(book).model_dump()
        book_dict["genre_name"] = book.genre.genre_name
        result.append(BookWithGenre(**book_dict))

    return result


@router.get("/{book_id}", response_model=BookWithGenre)
def get_book_details(book_id: int, db: Session = Depends(get_db)):
    """Детали книги"""

    book = db.query(Book).filter(Book.id == book_id).first()

    if not book:
        raise HTTPException(status_code=404, detail="Книга не найдена")

    book_dict = BookWithGenre.model_validate(book).model_dump()
    book_dict["genre_name"] = book.genre.genre_name

    return BookWithGenre(**book_dict)


@router.get("/{book_id}/availability")
def get_book_availability(book_id: int, db: Session = Depends(get_db)):
    """Наличие книги в библиотеках"""

    book = db.query(Book).filter(Book.id == book_id).first()

    if not book:
        raise HTTPException(status_code=404, detail="Книга не найдена")

    # Получаем все экземпляры книги
    copies = db.query(BookCopy).filter(BookCopy.book_id == book_id).all()

    # Группируем по библиотекам
    availability = {}
    for copy in copies:
        library_id = copy.library_id
        library_name = copy.library.library_name

        if library_id not in availability:
            availability[library_id] = {
                "library_id": library_id,
                "library_name": library_name,
                "total": 0,
                "available": 0,
                "on_loan": 0,
                "maintenance": 0
            }

        availability[library_id]["total"] += 1

        if copy.status == "available":
            availability[library_id]["available"] += 1
        elif copy.status == "on_loan":
            availability[library_id]["on_loan"] += 1
        elif copy.status == "maintenance":
            availability[library_id]["maintenance"] += 1

    return {
        "book_id": book_id,
        "book_title": book.title,
        "libraries": list(availability.values())
    }
