from sqlalchemy import Column, Integer, String, ForeignKey, Date, DateTime, Text
from sqlalchemy.orm import relationship
from datetime import datetime
from db.database import Base


class Role(Base):
    """Роли библиотекарей"""
    __tablename__ = "roles"

    id = Column(Integer, primary_key=True, index=True)
    name = Column(String(100), unique=True, nullable=False)

    # Связи
    staff_members = relationship("Staff", back_populates="role")


class Library(Base):
    """Библиотеки (филиалы)"""
    __tablename__ = "libraries"

    id = Column(Integer, primary_key=True, index=True)
    library_name = Column(String(200), nullable=False)
    address = Column(String(300), nullable=False)
    phone = Column(String(20), nullable=False)

    # Связи
    staff_members = relationship("Staff", back_populates="library")
    book_copies = relationship("BookCopy", back_populates="library")
    reservations = relationship("Reservation", back_populates="library")


class Staff(Base):
    """Библиотекари/сотрудники"""
    __tablename__ = "staff"

    id = Column(Integer, primary_key=True, index=True)
    full_name = Column(String(200), nullable=False)
    position = Column(String(100), nullable=False)
    library_id = Column(Integer, ForeignKey("libraries.id"), nullable=False)
    email = Column(String(100), unique=True, nullable=False, index=True)
    password = Column(String(100), nullable=False)
    role_id = Column(Integer, ForeignKey("roles.id"), nullable=False)

    # Связи
    role = relationship("Role", back_populates="staff_members")
    library = relationship("Library", back_populates="staff_members")
    loans = relationship("Loan", back_populates="staff")


class Reader(Base):
    """Читатели"""
    __tablename__ = "readers"

    id = Column(Integer, primary_key=True, index=True)
    full_name = Column(String(200), nullable=False)
    email = Column(String(100), unique=True, nullable=False, index=True)
    password = Column(String(100), nullable=False)
    phone = Column(String(20), nullable=True)
    library_card_number = Column(String(50), unique=True, nullable=False, index=True)
    created_at = Column(DateTime, default=datetime.utcnow)

    # Связи
    reservations = relationship("Reservation", back_populates="reader")
    loans = relationship("Loan", back_populates="reader")


class Genre(Base):
    """Жанры книг"""
    __tablename__ = "genres"

    id = Column(Integer, primary_key=True, index=True)
    genre_name = Column(String(100), unique=True, nullable=False)

    # Связи
    books = relationship("Book", back_populates="genre")


class Book(Base):
    """Книги"""
    __tablename__ = "books"

    id = Column(Integer, primary_key=True, index=True)
    title = Column(String(300), nullable=False, index=True)
    author = Column(String(200), nullable=False, index=True)
    publication_year = Column(Integer, nullable=True)
    genre_id = Column(Integer, ForeignKey("genres.id"), nullable=False)
    description = Column(Text, nullable=True)
    isbn = Column(String(20), nullable=True, unique=True)

    # Связи
    genre = relationship("Genre", back_populates="books")
    copies = relationship("BookCopy", back_populates="book")
    reservations = relationship("Reservation", back_populates="book")


class BookCopy(Base):
    """Экземпляры книг (физические копии в библиотеках)"""
    __tablename__ = "book_copies"

    id = Column(Integer, primary_key=True, index=True)
    book_id = Column(Integer, ForeignKey("books.id"), nullable=False)
    library_id = Column(Integer, ForeignKey("libraries.id"), nullable=False)
    inventory_number = Column(String(50), unique=True, nullable=False, index=True)
    status = Column(String(20), nullable=False, default="available", index=True)  # available, on_loan, maintenance, lost

    # Связи
    book = relationship("Book", back_populates="copies")
    library = relationship("Library", back_populates="book_copies")
    loans = relationship("Loan", back_populates="copy")


class Reservation(Base):
    """Бронирования книг"""
    __tablename__ = "reservations"

    id = Column(Integer, primary_key=True, index=True)
    reader_id = Column(Integer, ForeignKey("readers.id"), nullable=False)
    book_id = Column(Integer, ForeignKey("books.id"), nullable=False)
    library_id = Column(Integer, ForeignKey("libraries.id"), nullable=False)
    reservation_date = Column(DateTime, default=datetime.utcnow)
    status = Column(String(20), nullable=False, default="active", index=True)  # active, completed, cancelled

    # Связи
    reader = relationship("Reader", back_populates="reservations")
    book = relationship("Book", back_populates="reservations")
    library = relationship("Library", back_populates="reservations")


class Loan(Base):
    """Выдачи книг (активные займы)"""
    __tablename__ = "loans"

    id = Column(Integer, primary_key=True, index=True)
    reader_id = Column(Integer, ForeignKey("readers.id"), nullable=False)
    copy_id = Column(Integer, ForeignKey("book_copies.id"), nullable=False)
    staff_id = Column(Integer, ForeignKey("staff.id"), nullable=False)
    loan_date = Column(Date, nullable=False)
    due_date = Column(Date, nullable=False)
    return_date = Column(Date, nullable=True)
    status = Column(String(20), nullable=False, default="active", index=True)  # active, returned, overdue

    # Связи
    reader = relationship("Reader", back_populates="loans")
    copy = relationship("BookCopy", back_populates="loans")
    staff = relationship("Staff", back_populates="loans")
