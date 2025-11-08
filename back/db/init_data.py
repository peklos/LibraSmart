from sqlalchemy.orm import Session
from db.models import Role, Library, Staff, Reader, Genre, Book, BookCopy, Reservation, Loan
from datetime import datetime, timedelta, date


def init_test_data(db: Session):
    """Инициализация тестовых данных"""

    # Проверяем, есть ли уже данные
    if db.query(Role).first():
        print("✅ База данных уже содержит данные")
        return

    print("🔄 Инициализация тестовых данных...")

    # 1. Роли
    roles_data = [
        {"id": 1, "name": "Администратор"},
        {"id": 2, "name": "Старший библиотекарь"},
        {"id": 3, "name": "Библиотекарь"},
        {"id": 4, "name": "Помощник библиотекаря"}
    ]
    roles = [Role(**data) for data in roles_data]
    db.add_all(roles)
    db.commit()
    print("✅ Роли созданы")

    # 2. Библиотеки
    libraries_data = [
        {"id": 1, "library_name": "Центральная городская библиотека", "address": "г. Москва, ул. Ленина, 15", "phone": "+7 (495) 123-45-67"},
        {"id": 2, "library_name": "Библиотека им. Тургенева", "address": "г. Москва, ул. Тургенева, 8", "phone": "+7 (495) 234-56-78"},
        {"id": 3, "library_name": "Детская библиотека №1", "address": "г. Москва, пр. Мира, 42", "phone": "+7 (495) 345-67-89"},
        {"id": 4, "library_name": "Библиотека-филиал №5", "address": "г. Москва, ул. Пушкина, 23", "phone": "+7 (495) 456-78-90"},
        {"id": 5, "library_name": "Научная библиотека", "address": "г. Москва, Ломоносовский пр., 31", "phone": "+7 (495) 567-89-01"}
    ]
    libraries = [Library(**data) for data in libraries_data]
    db.add_all(libraries)
    db.commit()
    print("✅ Библиотеки созданы")

    # 3. Библиотекари
    staff_data = [
        {"id": 1, "full_name": "Петрова Анна Сергеевна", "position": "Директор", "library_id": 1, "email": "petrova@library.ru", "password": "admin123", "role_id": 1},
        {"id": 2, "full_name": "Иванов Дмитрий Петрович", "position": "Старший библиотекарь", "library_id": 1, "email": "ivanov@library.ru", "password": "staff123", "role_id": 2},
        {"id": 3, "full_name": "Смирнова Елена Викторовна", "position": "Библиотекарь", "library_id": 2, "email": "smirnova@library.ru", "password": "staff123", "role_id": 3},
        {"id": 4, "full_name": "Козлов Андрей Николаевич", "position": "Библиотекарь", "library_id": 3, "email": "kozlov@library.ru", "password": "staff123", "role_id": 3},
        {"id": 5, "full_name": "Морозова Ольга Ивановна", "position": "Старший библиотекарь", "library_id": 4, "email": "morozova@library.ru", "password": "staff123", "role_id": 2},
        {"id": 6, "full_name": "Новиков Сергей Александрович", "position": "Библиотекарь", "library_id": 5, "email": "novikov@library.ru", "password": "staff123", "role_id": 3},
        {"id": 7, "full_name": "Соколова Мария Дмитриевна", "position": "Помощник библиотекаря", "library_id": 1, "email": "sokolova@library.ru", "password": "staff123", "role_id": 4}
    ]
    staff = [Staff(**data) for data in staff_data]
    db.add_all(staff)
    db.commit()
    print("✅ Библиотекари созданы")

    # 4. Читатели
    readers_data = [
        {"id": 1, "full_name": "Алексеев Владимир Игоревич", "email": "alekseev@mail.ru", "password": "reader123", "phone": "+7 (910) 111-22-33", "library_card_number": "LIB-2024-001"},
        {"id": 2, "full_name": "Волкова Екатерина Павловна", "email": "volkova@mail.ru", "password": "reader123", "phone": "+7 (910) 222-33-44", "library_card_number": "LIB-2024-002"},
        {"id": 3, "full_name": "Федоров Николай Андреевич", "email": "fedorov@mail.ru", "password": "reader123", "phone": "+7 (910) 333-44-55", "library_card_number": "LIB-2024-003"},
        {"id": 4, "full_name": "Лебедева Анастасия Сергеевна", "email": "lebedeva@mail.ru", "password": "reader123", "phone": "+7 (910) 444-55-66", "library_card_number": "LIB-2024-004"},
        {"id": 5, "full_name": "Кузнецов Михаил Дмитриевич", "email": "kuznetsov@mail.ru", "password": "reader123", "phone": "+7 (910) 555-66-77", "library_card_number": "LIB-2024-005"},
        {"id": 6, "full_name": "Павлова Ирина Александровна", "email": "pavlova@mail.ru", "password": "reader123", "phone": "+7 (910) 666-77-88", "library_card_number": "LIB-2024-006"},
        {"id": 7, "full_name": "Соловьев Артем Викторович", "email": "solovyev@mail.ru", "password": "reader123", "phone": "+7 (910) 777-88-99", "library_card_number": "LIB-2024-007"},
        {"id": 8, "full_name": "Егорова Светлана Петровна", "email": "egorova@mail.ru", "password": "reader123", "phone": "+7 (910) 888-99-00", "library_card_number": "LIB-2024-008"},
        {"id": 9, "full_name": "Макаров Денис Иванович", "email": "makarov@mail.ru", "password": "reader123", "phone": "+7 (910) 999-00-11", "library_card_number": "LIB-2024-009"},
        {"id": 10, "full_name": "Титова Юлия Николаевна", "email": "titova@mail.ru", "password": "reader123", "phone": "+7 (910) 000-11-22", "library_card_number": "LIB-2024-010"}
    ]
    readers = [Reader(**data) for data in readers_data]
    db.add_all(readers)
    db.commit()
    print("✅ Читатели созданы")

    # 5. Жанры
    genres_data = [
        {"id": 1, "genre_name": "Художественная литература"},
        {"id": 2, "genre_name": "Научная фантастика"},
        {"id": 3, "genre_name": "Детектив"},
        {"id": 4, "genre_name": "Фэнтези"},
        {"id": 5, "genre_name": "Классическая литература"},
        {"id": 6, "genre_name": "Поэзия"},
        {"id": 7, "genre_name": "Биография"},
        {"id": 8, "genre_name": "История"},
        {"id": 9, "genre_name": "Психология"},
        {"id": 10, "genre_name": "Детская литература"},
        {"id": 11, "genre_name": "Приключения"},
        {"id": 12, "genre_name": "Бизнес"}
    ]
    genres = [Genre(**data) for data in genres_data]
    db.add_all(genres)
    db.commit()
    print("✅ Жанры созданы")

    # 6. Книги
    books_data = [
        # Классика
        {"id": 1, "title": "Война и мир", "author": "Лев Толстой", "publication_year": 1869, "genre_id": 5, "description": "Эпический роман о русском обществе в эпоху войн против Наполеона"},
        {"id": 2, "title": "Преступление и наказание", "author": "Фёдор Достоевский", "publication_year": 1866, "genre_id": 5, "description": "Психологический роман о студенте Раскольникове"},
        {"id": 3, "title": "Мастер и Маргарита", "author": "Михаил Булгаков", "publication_year": 1967, "genre_id": 5, "description": "Мистический роман о визите дьявола в Москву"},
        {"id": 4, "title": "Евгений Онегин", "author": "Александр Пушкин", "publication_year": 1833, "genre_id": 6, "description": "Роман в стихах о любви и жизни дворянства"},
        {"id": 5, "title": "Анна Каренина", "author": "Лев Толстой", "publication_year": 1877, "genre_id": 5, "description": "Роман о трагической любви и семейной жизни"},

        # Научная фантастика
        {"id": 6, "title": "Солярис", "author": "Станислав Лем", "publication_year": 1961, "genre_id": 2, "description": "Философский роман о контакте с внеземным разумом"},
        {"id": 7, "title": "1984", "author": "Джордж Оруэлл", "publication_year": 1949, "genre_id": 2, "description": "Антиутопия о тоталитарном государстве"},
        {"id": 8, "title": "Дюна", "author": "Фрэнк Герберт", "publication_year": 1965, "genre_id": 2, "description": "Эпическая сага о пустынной планете Арракис"},
        {"id": 9, "title": "Пикник на обочине", "author": "Аркадий и Борис Стругацкие", "publication_year": 1972, "genre_id": 2, "description": "Роман о Зоне посещения"},
        {"id": 10, "title": "Трудно быть богом", "author": "Аркадий и Борис Стругацкие", "publication_year": 1964, "genre_id": 2, "description": "О земном учёном на средневековой планете"},

        # Детективы
        {"id": 11, "title": "Убийство в Восточном экспрессе", "author": "Агата Кристи", "publication_year": 1934, "genre_id": 3, "description": "Классический детектив с Эркюлем Пуаро"},
        {"id": 12, "title": "Собака Баскервилей", "author": "Артур Конан Дойл", "publication_year": 1902, "genre_id": 3, "description": "Приключения Шерлока Холмса"},
        {"id": 13, "title": "Десять негритят", "author": "Агата Кристи", "publication_year": 1939, "genre_id": 3, "description": "Загадочные убийства на острове"},

        # Фэнтези
        {"id": 14, "title": "Властелин колец", "author": "Дж. Р. Р. Толкин", "publication_year": 1954, "genre_id": 4, "description": "Эпическое фэнтези о походе в Мордор"},
        {"id": 15, "title": "Гарри Поттер и философский камень", "author": "Дж. К. Роулинг", "publication_year": 1997, "genre_id": 4, "description": "Первая книга о юном волшебнике"},
        {"id": 16, "title": "Хоббит", "author": "Дж. Р. Р. Толкин", "publication_year": 1937, "genre_id": 4, "description": "Приключения Бильбо Бэггинса"},
        {"id": 17, "title": "Ночной Дозор", "author": "Сергей Лукьяненко", "publication_year": 1998, "genre_id": 4, "description": "Городское фэнтези о войне Света и Тьмы"},

        # Детская литература
        {"id": 18, "title": "Винни-Пух", "author": "Алан Милн", "publication_year": 1926, "genre_id": 10, "description": "Истории о медвежонке и его друзьях"},
        {"id": 19, "title": "Маленький принц", "author": "Антуан де Сент-Экзюпери", "publication_year": 1943, "genre_id": 10, "description": "Философская сказка"},
        {"id": 20, "title": "Гарри Поттер и Тайная комната", "author": "Дж. К. Роулинг", "publication_year": 1998, "genre_id": 10, "description": "Вторая книга о Гарри Поттере"},
        {"id": 21, "title": "Незнайка на Луне", "author": "Николай Носов", "publication_year": 1965, "genre_id": 10, "description": "Приключения Незнайки"},

        # Приключения
        {"id": 22, "title": "Граф Монте-Кристо", "author": "Александр Дюма", "publication_year": 1844, "genre_id": 11, "description": "История мести и справедливости"},
        {"id": 23, "title": "Три мушкетёра", "author": "Александр Дюма", "publication_year": 1844, "genre_id": 11, "description": "Приключения д'Артаньяна"},
        {"id": 24, "title": "Остров сокровищ", "author": "Роберт Стивенсон", "publication_year": 1883, "genre_id": 11, "description": "Пиратские приключения"},

        # Биография
        {"id": 25, "title": "Стив Джобс", "author": "Уолтер Айзексон", "publication_year": 2011, "genre_id": 7, "description": "Биография основателя Apple"},
        {"id": 26, "title": "Илон Маск", "author": "Эшли Вэнс", "publication_year": 2015, "genre_id": 7, "description": "Биография предпринимателя"},

        # История
        {"id": 27, "title": "ГУЛАГ", "author": "Энн Эпплбаум", "publication_year": 2003, "genre_id": 8, "description": "История советских лагерей"},
        {"id": 28, "title": "Sapiens", "author": "Юваль Ной Харари", "publication_year": 2011, "genre_id": 8, "description": "Краткая история человечества"},

        # Психология
        {"id": 29, "title": "Думай медленно... Решай быстро", "author": "Даниэль Канеман", "publication_year": 2011, "genre_id": 9, "description": "О механизмах мышления"},
        {"id": 30, "title": "Человек в поисках смысла", "author": "Виктор Франкл", "publication_year": 1946, "genre_id": 9, "description": "Психологические заметки узника"},

        # Бизнес
        {"id": 31, "title": "От нуля к единице", "author": "Питер Тиль", "publication_year": 2014, "genre_id": 12, "description": "О создании стартапов"},
        {"id": 32, "title": "Искусство войны", "author": "Сунь-Цзы", "publication_year": -500, "genre_id": 12, "description": "Древний трактат о стратегии"},

        # Еще художественная литература
        {"id": 33, "title": "Старик и море", "author": "Эрнест Хемингуэй", "publication_year": 1952, "genre_id": 1, "description": "Повесть о рыбаке"},
        {"id": 34, "title": "Над пропастью во ржи", "author": "Джером Сэлинджер", "publication_year": 1951, "genre_id": 1, "description": "Роман о подростке"},
        {"id": 35, "title": "Великий Гэтсби", "author": "Фрэнсис Скотт Фицджеральд", "publication_year": 1925, "genre_id": 1, "description": "Роман о американской мечте"},
        {"id": 36, "title": "Процесс", "author": "Франц Кафка", "publication_year": 1925, "genre_id": 1, "description": "Абсурдистский роман"},
        {"id": 37, "title": "Портрет Дориана Грея", "author": "Оскар Уайльд", "publication_year": 1890, "genre_id": 1, "description": "Философский роман о красоте и морали"},
        {"id": 38, "title": "Шум и ярость", "author": "Уильям Фолкнер", "publication_year": 1929, "genre_id": 1, "description": "Модернистский роман"},
        {"id": 39, "title": "Лолита", "author": "Владимир Набоков", "publication_year": 1955, "genre_id": 1, "description": "Скандальный роман"},
        {"id": 40, "title": "Улисс", "author": "Джеймс Джойс", "publication_year": 1922, "genre_id": 1, "description": "Модернистский роман о одном дне"}
    ]
    books = [Book(**data) for data in books_data]
    db.add_all(books)
    db.commit()
    print("✅ Книги созданы")

    # 7. Экземпляры книг (по 3-5 экземпляров каждой книги)
    book_copies = []
    inventory_counter = 1
    for book in books_data:
        # Распределяем экземпляры по разным библиотекам
        copies_count = 4  # по 4 экземпляра каждой книги
        for i in range(copies_count):
            library_id = (i % 5) + 1  # Распределяем по библиотекам 1-5
            status = "available"
            if inventory_counter % 15 == 0:
                status = "on_loan"
            elif inventory_counter % 25 == 0:
                status = "maintenance"

            book_copies.append(BookCopy(
                book_id=book["id"],
                library_id=library_id,
                inventory_number=f"INV-{inventory_counter:05d}",
                status=status
            ))
            inventory_counter += 1

    db.add_all(book_copies)
    db.commit()
    print(f"✅ Экземпляры книг созданы ({len(book_copies)} шт)")

    # 8. Бронирования
    reservations_data = [
        {"reader_id": 1, "book_id": 14, "library_id": 1, "reservation_date": datetime.utcnow() - timedelta(days=2), "status": "active"},
        {"reader_id": 2, "book_id": 7, "library_id": 1, "reservation_date": datetime.utcnow() - timedelta(days=1), "status": "active"},
        {"reader_id": 3, "book_id": 15, "library_id": 2, "reservation_date": datetime.utcnow() - timedelta(days=3), "status": "active"},
        {"reader_id": 4, "book_id": 11, "library_id": 3, "reservation_date": datetime.utcnow() - timedelta(days=1), "status": "active"},
        {"reader_id": 5, "book_id": 25, "library_id": 4, "reservation_date": datetime.utcnow() - timedelta(days=4), "status": "active"},
        {"reader_id": 1, "book_id": 1, "library_id": 1, "reservation_date": datetime.utcnow() - timedelta(days=15), "status": "completed"},
        {"reader_id": 2, "book_id": 3, "library_id": 2, "reservation_date": datetime.utcnow() - timedelta(days=20), "status": "completed"},
        {"reader_id": 6, "book_id": 20, "library_id": 3, "reservation_date": datetime.utcnow() - timedelta(days=5), "status": "cancelled"},
        {"reader_id": 7, "book_id": 28, "library_id": 1, "reservation_date": datetime.utcnow(), "status": "active"},
        {"reader_id": 8, "book_id": 33, "library_id": 2, "reservation_date": datetime.utcnow() - timedelta(hours=12), "status": "active"}
    ]
    reservations = [Reservation(**data) for data in reservations_data]
    db.add_all(reservations)
    db.commit()
    print("✅ Бронирования созданы")

    # 9. Выдачи/Займы
    loans_data = [
        # Активные займы
        {"reader_id": 1, "copy_id": 1, "staff_id": 2, "loan_date": date.today() - timedelta(days=5), "due_date": date.today() + timedelta(days=9), "return_date": None, "status": "active"},
        {"reader_id": 2, "copy_id": 10, "staff_id": 2, "loan_date": date.today() - timedelta(days=3), "due_date": date.today() + timedelta(days=11), "return_date": None, "status": "active"},
        {"reader_id": 3, "copy_id": 25, "staff_id": 3, "loan_date": date.today() - timedelta(days=7), "due_date": date.today() + timedelta(days=7), "return_date": None, "status": "active"},
        {"reader_id": 4, "copy_id": 35, "staff_id": 4, "loan_date": date.today() - timedelta(days=2), "due_date": date.today() + timedelta(days=12), "return_date": None, "status": "active"},
        {"reader_id": 5, "copy_id": 50, "staff_id": 5, "loan_date": date.today() - timedelta(days=10), "due_date": date.today() + timedelta(days=4), "return_date": None, "status": "active"},

        # Просроченные займы
        {"reader_id": 6, "copy_id": 60, "staff_id": 2, "loan_date": date.today() - timedelta(days=20), "due_date": date.today() - timedelta(days=6), "return_date": None, "status": "overdue"},
        {"reader_id": 7, "copy_id": 75, "staff_id": 3, "loan_date": date.today() - timedelta(days=25), "due_date": date.today() - timedelta(days=11), "return_date": None, "status": "overdue"},
        {"reader_id": 8, "copy_id": 90, "staff_id": 4, "loan_date": date.today() - timedelta(days=18), "due_date": date.today() - timedelta(days=4), "return_date": None, "status": "overdue"},

        # Возвращенные займы для пользователя alekseev (reader_id: 1)
        {"reader_id": 1, "copy_id": 5, "staff_id": 2, "loan_date": date.today() - timedelta(days=30), "due_date": date.today() - timedelta(days=16), "return_date": date.today() - timedelta(days=18), "status": "returned"},
        {"reader_id": 1, "copy_id": 9, "staff_id": 2, "loan_date": date.today() - timedelta(days=45), "due_date": date.today() - timedelta(days=31), "return_date": date.today() - timedelta(days=32), "status": "returned"},
        {"reader_id": 1, "copy_id": 13, "staff_id": 2, "loan_date": date.today() - timedelta(days=60), "due_date": date.today() - timedelta(days=46), "return_date": date.today() - timedelta(days=47), "status": "returned"},
        {"reader_id": 1, "copy_id": 17, "staff_id": 3, "loan_date": date.today() - timedelta(days=75), "due_date": date.today() - timedelta(days=61), "return_date": date.today() - timedelta(days=60), "status": "returned"},
        {"reader_id": 1, "copy_id": 21, "staff_id": 2, "loan_date": date.today() - timedelta(days=90), "due_date": date.today() - timedelta(days=76), "return_date": date.today() - timedelta(days=75), "status": "returned"},
        {"reader_id": 1, "copy_id": 29, "staff_id": 2, "loan_date": date.today() - timedelta(days=105), "due_date": date.today() - timedelta(days=91), "return_date": date.today() - timedelta(days=90), "status": "returned"},
        {"reader_id": 1, "copy_id": 33, "staff_id": 3, "loan_date": date.today() - timedelta(days=120), "due_date": date.today() - timedelta(days=106), "return_date": date.today() - timedelta(days=105), "status": "returned"},
        {"reader_id": 1, "copy_id": 41, "staff_id": 2, "loan_date": date.today() - timedelta(days=135), "due_date": date.today() - timedelta(days=121), "return_date": date.today() - timedelta(days=120), "status": "returned"},

        # Возвращенные займы для других пользователей
        {"reader_id": 2, "copy_id": 15, "staff_id": 2, "loan_date": date.today() - timedelta(days=35), "due_date": date.today() - timedelta(days=21), "return_date": date.today() - timedelta(days=22), "status": "returned"},
        {"reader_id": 3, "copy_id": 20, "staff_id": 3, "loan_date": date.today() - timedelta(days=40), "due_date": date.today() - timedelta(days=26), "return_date": date.today() - timedelta(days=25), "status": "returned"},
        {"reader_id": 9, "copy_id": 100, "staff_id": 5, "loan_date": date.today() - timedelta(days=50), "due_date": date.today() - timedelta(days=36), "return_date": date.today() - timedelta(days=35), "status": "returned"},
        {"reader_id": 10, "copy_id": 110, "staff_id": 6, "loan_date": date.today() - timedelta(days=45), "due_date": date.today() - timedelta(days=31), "return_date": date.today() - timedelta(days=30), "status": "returned"},
        {"reader_id": 4, "copy_id": 40, "staff_id": 4, "loan_date": date.today() - timedelta(days=60), "due_date": date.today() - timedelta(days=46), "return_date": date.today() - timedelta(days=44), "status": "returned"},
        {"reader_id": 5, "copy_id": 55, "staff_id": 5, "loan_date": date.today() - timedelta(days=55), "due_date": date.today() - timedelta(days=41), "return_date": date.today() - timedelta(days=40), "status": "returned"}
    ]
    loans = [Loan(**data) for data in loans_data]
    db.add_all(loans)
    db.commit()
    print("✅ Выдачи/Займы созданы")

    print("🎉 Все тестовые данные успешно созданы!")
