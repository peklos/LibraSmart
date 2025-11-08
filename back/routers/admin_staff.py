from fastapi import APIRouter, Depends, HTTPException
from sqlalchemy.orm import Session
from db.database import get_db
from db.models import Staff, Role, Library
from schemas.staff import StaffCreate, StaffUpdate, StaffResponse
from typing import List

router = APIRouter(prefix="/admin/staff", tags=["Admin Staff"])


def check_admin_role(staff_id: int, db: Session):
    """Проверка, является ли сотрудник администратором"""
    staff = db.query(Staff).filter(Staff.id == staff_id).first()
    if not staff or staff.role_id != 1:
        raise HTTPException(status_code=403, detail="Доступ запрещён. Требуются права администратора")
    return staff


@router.get("", response_model=List[StaffResponse])
def get_all_staff(current_staff_id: int, db: Session = Depends(get_db)):
    """Все сотрудники (только для администратора)"""

    # Проверяем права
    check_admin_role(current_staff_id, db)

    staff_list = db.query(Staff).all()
    return staff_list


@router.post("", response_model=StaffResponse)
def create_staff(current_staff_id: int, staff_data: StaffCreate, db: Session = Depends(get_db)):
    """Добавить сотрудника (только для администратора)"""

    # Проверяем права
    check_admin_role(current_staff_id, db)

    # Проверяем уникальность email
    existing = db.query(Staff).filter(Staff.email == staff_data.email).first()
    if existing:
        raise HTTPException(status_code=400, detail="Email уже используется")

    # Проверяем существование библиотеки
    library = db.query(Library).filter(Library.id == staff_data.library_id).first()
    if not library:
        raise HTTPException(status_code=404, detail="Библиотека не найдена")

    # Проверяем существование роли
    role = db.query(Role).filter(Role.id == staff_data.role_id).first()
    if not role:
        raise HTTPException(status_code=404, detail="Роль не найдена")

    new_staff = Staff(**staff_data.model_dump())

    db.add(new_staff)
    db.commit()
    db.refresh(new_staff)

    return new_staff


@router.patch("/{staff_id}", response_model=StaffResponse)
def update_staff(current_staff_id: int, staff_id: int, update_data: StaffUpdate, db: Session = Depends(get_db)):
    """Обновить сотрудника (только для администратора)"""

    # Проверяем права
    check_admin_role(current_staff_id, db)

    staff = db.query(Staff).filter(Staff.id == staff_id).first()

    if not staff:
        raise HTTPException(status_code=404, detail="Сотрудник не найден")

    update_dict = update_data.model_dump(exclude_unset=True)

    for key, value in update_dict.items():
        setattr(staff, key, value)

    db.commit()
    db.refresh(staff)

    return staff


@router.delete("/{staff_id}")
def delete_staff(current_staff_id: int, staff_id: int, db: Session = Depends(get_db)):
    """Удалить сотрудника (только для администратора)"""

    # Проверяем права
    check_admin_role(current_staff_id, db)

    staff = db.query(Staff).filter(Staff.id == staff_id).first()

    if not staff:
        raise HTTPException(status_code=404, detail="Сотрудник не найден")

    # Нельзя удалить самого себя
    if staff.id == current_staff_id:
        raise HTTPException(status_code=400, detail="Нельзя удалить самого себя")

    db.delete(staff)
    db.commit()

    return {"message": "Сотрудник удалён"}
