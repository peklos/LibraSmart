from fastapi import APIRouter, Depends, HTTPException
from sqlalchemy.orm import Session
from db.database import get_db
from db.models import Reader
from schemas.reader import ReaderRegister, ReaderLogin, ReaderResponse
from datetime import datetime

router = APIRouter(prefix="/auth", tags=["Reader Auth"])


@router.post("/register", response_model=ReaderResponse)
def register_reader(reader_data: ReaderRegister, db: Session = Depends(get_db)):
    """Регистрация нового читателя"""

    # Проверяем, есть ли уже читатель с таким email
    existing_reader = db.query(Reader).filter(Reader.email == reader_data.email).first()
    if existing_reader:
        raise HTTPException(status_code=400, detail="Email уже зарегистрирован")

    # Генерируем номер читательского билета
    last_reader = db.query(Reader).order_by(Reader.id.desc()).first()
    if last_reader:
        last_number = int(last_reader.library_card_number.split("-")[-1])
        new_number = f"LIB-2024-{last_number + 1:03d}"
    else:
        new_number = "LIB-2024-001"

    # Создаем нового читателя
    new_reader = Reader(
        full_name=reader_data.full_name,
        email=reader_data.email,
        password=reader_data.password,
        phone=reader_data.phone,
        library_card_number=new_number,
        created_at=datetime.utcnow()
    )

    db.add(new_reader)
    db.commit()
    db.refresh(new_reader)

    return new_reader


@router.post("/login", response_model=ReaderResponse)
def login_reader(credentials: ReaderLogin, db: Session = Depends(get_db)):
    """Вход читателя в систему"""

    reader = db.query(Reader).filter(Reader.email == credentials.email).first()

    if not reader or reader.password != credentials.password:
        raise HTTPException(status_code=401, detail="Неверный email или пароль")

    return reader


@router.get("/me/{reader_id}", response_model=ReaderResponse)
def get_current_reader(reader_id: int, db: Session = Depends(get_db)):
    """Получить данные текущего читателя"""

    reader = db.query(Reader).filter(Reader.id == reader_id).first()

    if not reader:
        raise HTTPException(status_code=404, detail="Читатель не найден")

    return reader
