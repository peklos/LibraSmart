from fastapi import APIRouter, Depends, HTTPException
from sqlalchemy.orm import Session
from db.database import get_db
from db.models import Staff
from schemas.staff import StaffLogin, StaffResponse

router = APIRouter(prefix="/admin/auth", tags=["Staff Auth"])


@router.post("/login", response_model=StaffResponse)
def login_staff(credentials: StaffLogin, db: Session = Depends(get_db)):
    """Вход библиотекаря в систему"""

    staff = db.query(Staff).filter(Staff.email == credentials.email).first()

    if not staff or staff.password != credentials.password:
        raise HTTPException(status_code=401, detail="Неверный email или пароль")

    return staff


@router.get("/me/{staff_id}", response_model=StaffResponse)
def get_current_staff(staff_id: int, db: Session = Depends(get_db)):
    """Получить данные текущего сотрудника"""

    staff = db.query(Staff).filter(Staff.id == staff_id).first()

    if not staff:
        raise HTTPException(status_code=404, detail="Сотрудник не найден")

    return staff
