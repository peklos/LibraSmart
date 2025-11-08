from fastapi import APIRouter, Depends, HTTPException
from sqlalchemy.orm import Session
from db.database import get_db
from db.models import Reader
from schemas.reader import ReaderResponse, ReaderUpdate

router = APIRouter(prefix="/profile", tags=["Reader Profile"])


@router.get("/{reader_id}", response_model=ReaderResponse)
def get_profile(reader_id: int, db: Session = Depends(get_db)):
    """Просмотр профиля читателя"""

    reader = db.query(Reader).filter(Reader.id == reader_id).first()

    if not reader:
        raise HTTPException(status_code=404, detail="Читатель не найден")

    return reader


@router.patch("/{reader_id}", response_model=ReaderResponse)
def update_profile(reader_id: int, update_data: ReaderUpdate, db: Session = Depends(get_db)):
    """Обновление профиля читателя"""

    reader = db.query(Reader).filter(Reader.id == reader_id).first()

    if not reader:
        raise HTTPException(status_code=404, detail="Читатель не найден")

    # Обновляем только переданные поля
    update_dict = update_data.model_dump(exclude_unset=True)

    for key, value in update_dict.items():
        setattr(reader, key, value)

    db.commit()
    db.refresh(reader)

    return reader
