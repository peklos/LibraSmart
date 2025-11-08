from fastapi import FastAPI
from fastapi.middleware.cors import CORSMiddleware
from db.database import engine, SessionLocal, Base
from db.init_data import init_test_data

# Import всех роутеров
from routers import (
    reader_auth,
    reader_profile,
    reader_books,
    reader_reservations,
    reader_loans,
    reader_history,
    staff_auth,
    admin_readers,
    admin_books,
    admin_copies,
    admin_reservations,
    admin_loans,
    admin_staff,
    admin_libraries,
    admin_genres,
    admin_stats
)

# Создаем приложение FastAPI
app = FastAPI(
    title="LibraSmart API",
    description="API для системы управления библиотекой LibraSmart",
    version="1.0.0"
)

# Настройка CORS
app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],  # В продакшене указать конкретные домены
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)


# Создание таблиц и инициализация данных при старте
@app.on_event("startup")
def startup():
    """Инициализация БД при старте приложения"""
    print("🚀 Запуск LibraSmart API...")

    # Создаем все таблицы
    Base.metadata.create_all(bind=engine)
    print("✅ Таблицы базы данных созданы")

    # Инициализируем тестовые данные
    db = SessionLocal()
    try:
        init_test_data(db)
    finally:
        db.close()


# Корневой endpoint
@app.get("/")
def root():
    """Корневой endpoint с информацией об API"""
    return {
        "message": "LibraSmart API",
        "version": "1.0.0",
        "documentation": "/docs",
        "description": "API для системы управления библиотекой",
        "endpoints": {
            "readers": {
                "auth": "/auth",
                "profile": "/profile",
                "books": "/books",
                "reservations": "/reservations",
                "loans": "/loans",
                "history": "/history"
            },
            "admin": {
                "auth": "/admin/auth",
                "readers": "/admin/readers",
                "books": "/admin/books",
                "copies": "/admin/copies",
                "reservations": "/admin/reservations",
                "loans": "/admin/loans",
                "staff": "/admin/staff",
                "libraries": "/admin/libraries",
                "genres": "/admin/genres",
                "stats": "/admin/stats"
            }
        }
    }


# Healthcheck endpoint
@app.get("/health")
def health_check():
    """Проверка работоспособности API"""
    return {"status": "healthy", "service": "LibraSmart API"}


# Подключение всех роутеров для читателей
app.include_router(reader_auth.router)
app.include_router(reader_profile.router)
app.include_router(reader_books.router)
app.include_router(reader_reservations.router)
app.include_router(reader_loans.router)
app.include_router(reader_history.router)

# Подключение всех роутеров для админов/библиотекарей
app.include_router(staff_auth.router)
app.include_router(admin_readers.router)
app.include_router(admin_books.router)
app.include_router(admin_copies.router)
app.include_router(admin_reservations.router)
app.include_router(admin_loans.router)
app.include_router(admin_staff.router)
app.include_router(admin_libraries.router)
app.include_router(admin_genres.router)
app.include_router(admin_stats.router)


if __name__ == "__main__":
    import uvicorn
    uvicorn.run(app, host="0.0.0.0", port=8000)
