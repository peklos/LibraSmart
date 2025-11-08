# 📚 LibraSmart Backend API

API для системы управления библиотекой LibraSmart, построенный на FastAPI + SQLAlchemy + PostgreSQL.

## 🎯 Описание проекта

LibraSmart - это полнофункциональная система управления библиотекой с двумя типами пользователей:
- **Читатели** - могут просматривать каталог, бронировать и брать книги
- **Библиотекари** - управляют книгами, читателями, выдачами и статистикой

## 🚀 Быстрый старт

### 1. Установка зависимостей

```bash
cd back
pip install -r requirements.txt
```

### 2. Настройка базы данных

Создайте файл `.env` на основе `.env.example`:

```bash
cp .env.example .env
```

Для **локальной разработки** (SQLite):
```
DATABASE_URL=sqlite:///./librasmart.db
```

Для **продакшена с Neon PostgreSQL**:
```
DATABASE_URL=postgresql://username:password@ep-xxxxx.region.aws.neon.tech/librasmart?sslmode=require
```

### 3. Запуск сервера

```bash
uvicorn main:app --reload
```

Сервер запустится на `http://localhost:8000`

### 4. Документация API

После запуска откройте:
- **Swagger UI**: http://localhost:8000/docs
- **ReDoc**: http://localhost:8000/redoc

## 📊 База данных

### Таблицы

1. **roles** - Роли библиотекарей (Администратор, Старший библиотекарь, Библиотекарь, Помощник)
2. **staff** - Библиотекари/сотрудники
3. **libraries** - Библиотеки (филиалы)
4. **readers** - Читатели
5. **genres** - Жанры книг
6. **books** - Книги
7. **book_copies** - Экземпляры книг (физические копии)
8. **reservations** - Бронирования книг
9. **loans** - Выдачи книг (займы)

### Автоматическая инициализация

При первом запуске база данных автоматически создаётся и заполняется тестовыми данными:
- 4 роли
- 5 библиотек
- 7 библиотекарей
- 10 читателей
- 12 жанров
- 40 книг
- 160+ экземпляров книг
- 10 бронирований
- 15 выдач

## 🔐 Тестовые учётные записи

### Для читателей

```
Email: alekseev@mail.ru
Пароль: reader123
```

Все читатели имеют пароль `reader123`

### Для библиотекарей

**Администратор:**
```
Email: petrova@library.ru
Пароль: admin123
```

**Обычный библиотекарь:**
```
Email: ivanov@library.ru
Пароль: staff123
```

Все сотрудники (кроме администратора) имеют пароль `staff123`

## 📋 API Endpoints

### 🔵 Читатели (Readers)

#### Авторизация (`/auth`)
- `POST /auth/register` - Регистрация нового читателя
- `POST /auth/login` - Вход в систему
- `GET /auth/me/{reader_id}` - Получить данные текущего читателя

#### Профиль (`/profile`)
- `GET /profile/{reader_id}` - Просмотр профиля
- `PATCH /profile/{reader_id}` - Обновление профиля

#### Книги (`/books`)
- `GET /books` - Каталог всех книг (с фильтрацией по жанру, автору, поиску)
- `GET /books/{book_id}` - Детали книги
- `GET /books/{book_id}/availability` - Наличие книги в библиотеках

#### Бронирования (`/reservations`)
- `POST /reservations?reader_id={id}` - Создать бронирование
- `GET /reservations/my/{reader_id}` - Мои бронирования
- `DELETE /reservations/{reservation_id}` - Отменить бронирование

#### Займы (`/loans`)
- `GET /loans/my/{reader_id}` - Мои текущие выдачи
- `GET /loans/my/{reader_id}/active` - Активные займы
- `GET /loans/my/{reader_id}/overdue` - Просроченные займы

#### История (`/history`)
- `GET /history/{reader_id}` - История всех моих выдач
- `GET /history/{reader_id}/stats` - Статистика по чтению

---

### 🔴 Библиотекари/Админы (Staff/Admin)

#### Авторизация (`/admin/auth`)
- `POST /admin/auth/login` - Вход библиотекаря
- `GET /admin/auth/me/{staff_id}` - Получить данные текущего сотрудника

#### Читатели (`/admin/readers`)
- `GET /admin/readers` - Все читатели
- `GET /admin/readers/{reader_id}` - Детали читателя
- `POST /admin/readers` - Создать читателя
- `PATCH /admin/readers/{reader_id}` - Обновить читателя
- `DELETE /admin/readers/{reader_id}` - Удалить читателя
- `GET /admin/readers/{reader_id}/loans` - История выдач читателя

#### Книги (`/admin/books`)
- `GET /admin/books` - Все книги
- `GET /admin/books/{book_id}` - Детали книги
- `POST /admin/books` - Добавить книгу
- `PATCH /admin/books/{book_id}` - Обновить книгу
- `DELETE /admin/books/{book_id}` - Удалить книгу

#### Экземпляры (`/admin/copies`)
- `GET /admin/copies` - Все экземпляры
- `GET /admin/copies/library/{library_id}` - Экземпляры в конкретной библиотеке
- `POST /admin/copies` - Добавить экземпляр
- `PATCH /admin/copies/{copy_id}` - Обновить статус экземпляра
- `DELETE /admin/copies/{copy_id}` - Удалить экземпляр

#### Бронирования (`/admin/reservations`)
- `GET /admin/reservations` - Все бронирования
- `GET /admin/reservations/active` - Активные бронирования
- `PATCH /admin/reservations/{reservation_id}` - Обновить статус
- `DELETE /admin/reservations/{reservation_id}` - Удалить бронирование

#### Выдачи (`/admin/loans`)
- `GET /admin/loans` - Все выдачи
- `POST /admin/loans` - Создать выдачу (выдать книгу читателю)
- `PATCH /admin/loans/{loan_id}/return` - Принять возврат книги
- `GET /admin/loans/overdue` - Просроченные выдачи
- `GET /admin/loans/active` - Активные выдачи

#### Сотрудники (`/admin/staff`) 🔒 **Только для администратора**
- `GET /admin/staff?current_staff_id={id}` - Все сотрудники
- `POST /admin/staff?current_staff_id={id}` - Добавить сотрудника
- `PATCH /admin/staff/{staff_id}?current_staff_id={id}` - Обновить сотрудника
- `DELETE /admin/staff/{staff_id}?current_staff_id={id}` - Удалить сотрудника

#### Библиотеки (`/admin/libraries`)
- `GET /admin/libraries` - Все библиотеки
- `POST /admin/libraries` - Добавить библиотеку
- `PATCH /admin/libraries/{library_id}` - Обновить библиотеку
- `DELETE /admin/libraries/{library_id}` - Удалить библиотеку

#### Жанры (`/admin/genres`)
- `GET /admin/genres` - Все жанры
- `POST /admin/genres` - Добавить жанр
- `PATCH /admin/genres/{genre_id}` - Обновить жанр
- `DELETE /admin/genres/{genre_id}` - Удалить жанр

#### Статистика (`/admin/stats`)
- `GET /admin/stats/dashboard` - Общая статистика (количество читателей, книг, выдач, просрочек)
- `GET /admin/stats/popular-books` - Топ популярных книг
- `GET /admin/stats/popular-genres` - Топ жанров
- `GET /admin/stats/active-readers` - Самые активные читатели
- `GET /admin/stats/library/{library_id}` - Статистика по конкретной библиотеке

---

## 📁 Структура проекта

```
back/
│
├── .env.example          # Пример конфигурации
├── .gitignore           # Игнорируемые файлы
├── requirements.txt     # Зависимости Python
├── main.py              # Главный файл приложения
├── README.md            # Документация
│
├── db/
│   ├── __init__.py
│   ├── database.py      # Подключение к БД
│   ├── models.py        # SQLAlchemy модели
│   └── init_data.py     # Тестовые данные
│
├── schemas/
│   ├── __init__.py
│   ├── reader.py        # Pydantic схемы для читателей
│   ├── staff.py
│   ├── role.py
│   ├── library.py
│   ├── genre.py
│   ├── book.py
│   ├── book_copy.py
│   ├── reservation.py
│   └── loan.py
│
└── routers/
    ├── __init__.py
    │
    # Роутеры для читателей
    ├── reader_auth.py
    ├── reader_profile.py
    ├── reader_books.py
    ├── reader_reservations.py
    ├── reader_loans.py
    ├── reader_history.py
    │
    # Роутеры для библиотекарей/админов
    ├── staff_auth.py
    ├── admin_readers.py
    ├── admin_books.py
    ├── admin_copies.py
    ├── admin_reservations.py
    ├── admin_loans.py
    ├── admin_staff.py
    ├── admin_libraries.py
    ├── admin_genres.py
    └── admin_stats.py
```

## 🛠️ Технологии

- **FastAPI** - Современный веб-фреймворк для Python
- **SQLAlchemy** - ORM для работы с базой данных
- **PostgreSQL** - Основная БД (через Neon для продакшена)
- **SQLite** - БД для локальной разработки
- **Pydantic** - Валидация данных
- **Uvicorn** - ASGI сервер

## 🚢 Деплой на Render + Neon

### 1. Создайте базу данных на Neon

1. Зарегистрируйтесь на [neon.tech](https://neon.tech)
2. Создайте новый проект
3. Скопируйте connection string

### 2. Создайте Web Service на Render

1. Зарегистрируйтесь на [render.com](https://render.com)
2. Создайте новый **Web Service**
3. Подключите ваш GitHub репозиторий
4. Настройте:
   - **Root Directory**: `back`
   - **Build Command**: `pip install -r requirements.txt`
   - **Start Command**: `uvicorn main:app --host 0.0.0.0 --port $PORT`
5. Добавьте переменную окружения:
   - `DATABASE_URL` = ваш connection string из Neon

## 🔧 Особенности

### Без JWT токенов
Простая авторизация по email/password (для учебного проекта)

### Без хеширования паролей
Пароли хранятся в открытом виде (для учебного проекта)

### Разграничение прав
- Обычные библиотекари могут управлять книгами, читателями, выдачами
- Только администраторы (role_id = 1) могут управлять сотрудниками

### Автоматическое обновление статусов
- Займы автоматически помечаются как "просроченные" при запросах
- Статусы экземпляров обновляются при выдаче/возврате

## 📝 Примеры использования

### Регистрация читателя

```bash
curl -X POST "http://localhost:8000/auth/register" \
  -H "Content-Type: application/json" \
  -d '{
    "full_name": "Иванов Иван Иванович",
    "email": "ivanov@example.com",
    "password": "mypassword",
    "phone": "+7 (900) 123-45-67"
  }'
```

### Поиск книг

```bash
curl "http://localhost:8000/books?search=Толстой"
```

### Создание бронирования

```bash
curl -X POST "http://localhost:8000/reservations?reader_id=1" \
  -H "Content-Type: application/json" \
  -d '{
    "book_id": 14,
    "library_id": 1
  }'
```

### Выдача книги (библиотекарь)

```bash
curl -X POST "http://localhost:8000/admin/loans" \
  -H "Content-Type: application/json" \
  -d '{
    "reader_id": 1,
    "copy_id": 10,
    "staff_id": 2,
    "due_date": "2025-12-01"
  }'
```

## 🎯 Roadmap

- [x] Полный CRUD для всех сущностей
- [x] Система бронирования
- [x] Система выдачи книг
- [x] Учёт экземпляров по библиотекам
- [x] История операций
- [x] Статистика и аналитика
- [ ] JWT авторизация (для продакшена)
- [ ] Хеширование паролей
- [ ] Уведомления о просроченных книгах
- [ ] Система отзывов о книгах

## 📄 Лицензия

Учебный проект для системы управления библиотекой.

## 👨‍💻 Автор

Created with ❤️ by Claude
