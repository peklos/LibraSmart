from fastapi import APIRouter, Depends, HTTPException
from sqlalchemy.orm import Session
from db.database import get_db
from db.models import BookCopy, Book, Library
from schemas.book_copy import BookCopyCreate, BookCopyUpdate, BookCopyResponse, BookCopyWithDetails
from typing import List

router = APIRouter(prefix="/admin/copies", tags=["Admin Book Copies"])


@router.get("", response_model=List[BookCopyWithDetails])
def get_all_copies(db: Session = Depends(get_db)):
    """Все экземпляры"""
    copies = db.query(BookCopy).all()

    result = []
    for copy in copies:
        copy_dict = BookCopyWithDetails.model_validate(copy).model_dump()
        copy_dict["book_title"] = copy.book.title
        copy_dict["library_name"] = copy.library.library_name
        result.append(BookCopyWithDetails(**copy_dict))

    return result


@router.get("/library/{library_id}", response_model=List[BookCopyWithDetails])
def get_copies_by_library(library_id: int, db: Session = Depends(get_db)):
    """Экземпляры в конкретной библиотеке"""

    library = db.query(Library).filter(Library.id == library_id).first()
    if not library:
        raise HTTPException(status_code=404, detail="Библиотека не найдена")

    copies = db.query(BookCopy).filter(BookCopy.library_id == library_id).all()

    result = []
    for copy in copies:
        copy_dict = BookCopyWithDetails.model_validate(copy).model_dump()
        copy_dict["book_title"] = copy.book.title
        copy_dict["library_name"] = copy.library.library_name
        result.append(BookCopyWithDetails(**copy_dict))

    return result


@router.post("", response_model=BookCopyResponse)
def create_copy(copy_data: BookCopyCreate, db: Session = Depends(get_db)):
    """Добавить экземпляр"""

    # Проверяем существование книги
    book = db.query(Book).filter(Book.id == copy_data.book_id).first()
    if not book:
        raise HTTPException(status_code=404, detail="Книга не найдена")

    # Проверяем существование библиотеки
    library = db.query(Library).filter(Library.id == copy_data.library_id).first()
    if not library:
        raise HTTPException(status_code=404, detail="Библиотека не найдена")

    # Проверяем уникальность инвентарного номера
    existing = db.query(BookCopy).filter(BookCopy.inventory_number == copy_data.inventory_number).first()
    if existing:
        raise HTTPException(status_code=400, detail="Инвентарный номер уже используется")

    new_copy = BookCopy(**copy_data.model_dump())

    db.add(new_copy)
    db.commit()
    db.refresh(new_copy)

    return new_copy


@router.patch("/{copy_id}", response_model=BookCopyResponse)
def update_copy(copy_id: int, update_data: BookCopyUpdate, db: Session = Depends(get_db)):
    """Обновить статус экземпляра"""

    copy = db.query(BookCopy).filter(BookCopy.id == copy_id).first()

    if not copy:
        raise HTTPException(status_code=404, detail="Экземпляр не найден")

    update_dict = update_data.model_dump(exclude_unset=True)

    for key, value in update_dict.items():
        setattr(copy, key, value)

    db.commit()
    db.refresh(copy)

    return copy


@router.delete("/{copy_id}")
def delete_copy(copy_id: int, db: Session = Depends(get_db)):
    """Удалить экземпляр"""

    copy = db.query(BookCopy).filter(BookCopy.id == copy_id).first()

    if not copy:
        raise HTTPException(status_code=404, detail="Экземпляр не найден")

    db.delete(copy)
    db.commit()

    return {"message": "Экземпляр удалён"}
