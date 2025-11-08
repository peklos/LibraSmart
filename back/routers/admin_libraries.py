from fastapi import APIRouter, Depends, HTTPException
from sqlalchemy.orm import Session
from db.database import get_db
from db.models import Library
from schemas.library import LibraryCreate, LibraryUpdate, LibraryResponse
from typing import List

router = APIRouter(prefix="/admin/libraries", tags=["Admin Libraries"])


@router.get("", response_model=List[LibraryResponse])
def get_all_libraries(db: Session = Depends(get_db)):
    """Все библиотеки"""
    libraries = db.query(Library).all()
    return libraries


@router.post("", response_model=LibraryResponse)
def create_library(library_data: LibraryCreate, db: Session = Depends(get_db)):
    """Добавить библиотеку"""

    new_library = Library(**library_data.model_dump())

    db.add(new_library)
    db.commit()
    db.refresh(new_library)

    return new_library


@router.patch("/{library_id}", response_model=LibraryResponse)
def update_library(library_id: int, update_data: LibraryUpdate, db: Session = Depends(get_db)):
    """Обновить библиотеку"""

    library = db.query(Library).filter(Library.id == library_id).first()

    if not library:
        raise HTTPException(status_code=404, detail="Библиотека не найдена")

    update_dict = update_data.model_dump(exclude_unset=True)

    for key, value in update_dict.items():
        setattr(library, key, value)

    db.commit()
    db.refresh(library)

    return library


@router.delete("/{library_id}")
def delete_library(library_id: int, db: Session = Depends(get_db)):
    """Удалить библиотеку"""

    library = db.query(Library).filter(Library.id == library_id).first()

    if not library:
        raise HTTPException(status_code=404, detail="Библиотека не найдена")

    db.delete(library)
    db.commit()

    return {"message": "Библиотека удалена"}
