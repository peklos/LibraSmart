using LibraSmartAPI.Models;

namespace LibraSmartAPI.Data;

public static class DatabaseInitializer
{
    public static void Initialize(LibraryContext context)
    {
        // Проверяем, есть ли уже данные
        if (context.Roles.Any())
        {
            return; // База уже содержит данные
        }

        // 1. Роли
        var roles = new[]
        {
            new Role { Id = 1, Name = "Администратор" },
            new Role { Id = 2, Name = "Старший библиотекарь" },
            new Role { Id = 3, Name = "Библиотекарь" },
            new Role { Id = 4, Name = "Помощник библиотекаря" }
        };
        context.Roles.AddRange(roles);
        context.SaveChanges();

        // 2. Библиотеки
        var libraries = new[]
        {
            new Library { Id = 1, LibraryName = "Центральная городская библиотека", Address = "г. Москва, ул. Ленина, 15", Phone = "+7 (495) 123-45-67" },
            new Library { Id = 2, LibraryName = "Библиотека им. Тургенева", Address = "г. Москва, ул. Тургенева, 8", Phone = "+7 (495) 234-56-78" },
            new Library { Id = 3, LibraryName = "Детская библиотека №1", Address = "г. Москва, пр. Мира, 42", Phone = "+7 (495) 345-67-89" },
            new Library { Id = 4, LibraryName = "Библиотека-филиал №5", Address = "г. Москва, ул. Пушкина, 23", Phone = "+7 (495) 456-78-90" },
            new Library { Id = 5, LibraryName = "Научная библиотека", Address = "г. Москва, Ломоносовский пр., 31", Phone = "+7 (495) 567-89-01" }
        };
        context.Libraries.AddRange(libraries);
        context.SaveChanges();

        // 3. Библиотекари
        var staffMembers = new[]
        {
            new Staff { Id = 1, FullName = "Петрова Анна Сергеевна", Position = "Директор", LibraryId = 1, Email = "petrova@library.ru", Password = "admin123", RoleId = 1 },
            new Staff { Id = 2, FullName = "Иванов Дмитрий Петрович", Position = "Старший библиотекарь", LibraryId = 1, Email = "ivanov@library.ru", Password = "staff123", RoleId = 2 },
            new Staff { Id = 3, FullName = "Смирнова Елена Викторовна", Position = "Библиотекарь", LibraryId = 2, Email = "smirnova@library.ru", Password = "staff123", RoleId = 3 },
            new Staff { Id = 4, FullName = "Козлов Андрей Николаевич", Position = "Библиотекарь", LibraryId = 3, Email = "kozlov@library.ru", Password = "staff123", RoleId = 3 },
            new Staff { Id = 5, FullName = "Морозова Ольга Ивановна", Position = "Старший библиотекарь", LibraryId = 4, Email = "morozova@library.ru", Password = "staff123", RoleId = 2 },
            new Staff { Id = 6, FullName = "Новиков Сергей Александрович", Position = "Библиотекарь", LibraryId = 5, Email = "novikov@library.ru", Password = "staff123", RoleId = 3 },
            new Staff { Id = 7, FullName = "Соколова Мария Дмитриевна", Position = "Помощник библиотекаря", LibraryId = 1, Email = "sokolova@library.ru", Password = "staff123", RoleId = 4 }
        };
        context.Staff.AddRange(staffMembers);
        context.SaveChanges();

        // 4. Читатели
        var readers = new[]
        {
            new Reader { Id = 1, FullName = "Алексеев Владимир Игоревич", Email = "alekseev@mail.ru", Password = "reader123", Phone = "+7 (910) 111-22-33", LibraryCardNumber = "LIB-2024-001" },
            new Reader { Id = 2, FullName = "Волкова Екатерина Павловна", Email = "volkova@mail.ru", Password = "reader123", Phone = "+7 (910) 222-33-44", LibraryCardNumber = "LIB-2024-002" },
            new Reader { Id = 3, FullName = "Федоров Николай Андреевич", Email = "fedorov@mail.ru", Password = "reader123", Phone = "+7 (910) 333-44-55", LibraryCardNumber = "LIB-2024-003" },
            new Reader { Id = 4, FullName = "Лебедева Анастасия Сергеевна", Email = "lebedeva@mail.ru", Password = "reader123", Phone = "+7 (910) 444-55-66", LibraryCardNumber = "LIB-2024-004" },
            new Reader { Id = 5, FullName = "Кузнецов Михаил Дмитриевич", Email = "kuznetsov@mail.ru", Password = "reader123", Phone = "+7 (910) 555-66-77", LibraryCardNumber = "LIB-2024-005" },
            new Reader { Id = 6, FullName = "Павлова Ирина Александровна", Email = "pavlova@mail.ru", Password = "reader123", Phone = "+7 (910) 666-77-88", LibraryCardNumber = "LIB-2024-006" },
            new Reader { Id = 7, FullName = "Соловьев Артем Викторович", Email = "solovyev@mail.ru", Password = "reader123", Phone = "+7 (910) 777-88-99", LibraryCardNumber = "LIB-2024-007" },
            new Reader { Id = 8, FullName = "Егорова Светлана Петровна", Email = "egorova@mail.ru", Password = "reader123", Phone = "+7 (910) 888-99-00", LibraryCardNumber = "LIB-2024-008" },
            new Reader { Id = 9, FullName = "Макаров Денис Иванович", Email = "makarov@mail.ru", Password = "reader123", Phone = "+7 (910) 999-00-11", LibraryCardNumber = "LIB-2024-009" },
            new Reader { Id = 10, FullName = "Титова Юлия Николаевна", Email = "titova@mail.ru", Password = "reader123", Phone = "+7 (910) 000-11-22", LibraryCardNumber = "LIB-2024-010" }
        };
        context.Readers.AddRange(readers);
        context.SaveChanges();

        // 5. Жанры
        var genres = new[]
        {
            new Genre { Id = 1, GenreName = "Художественная литература" },
            new Genre { Id = 2, GenreName = "Научная фантастика" },
            new Genre { Id = 3, GenreName = "Детектив" },
            new Genre { Id = 4, GenreName = "Фэнтези" },
            new Genre { Id = 5, GenreName = "Классическая литература" },
            new Genre { Id = 6, GenreName = "Поэзия" },
            new Genre { Id = 7, GenreName = "Биография" },
            new Genre { Id = 8, GenreName = "История" },
            new Genre { Id = 9, GenreName = "Психология" },
            new Genre { Id = 10, GenreName = "Детская литература" },
            new Genre { Id = 11, GenreName = "Приключения" },
            new Genre { Id = 12, GenreName = "Бизнес" }
        };
        context.Genres.AddRange(genres);
        context.SaveChanges();

        // 6. Книги
        var books = new[]
        {
            // Классика
            new Book { Id = 1, Title = "Война и мир", Author = "Лев Толстой", PublicationYear = 1869, GenreId = 5, Description = "Эпический роман о русском обществе в эпоху войн против Наполеона" },
            new Book { Id = 2, Title = "Преступление и наказание", Author = "Фёдор Достоевский", PublicationYear = 1866, GenreId = 5, Description = "Психологический роман о студенте Раскольникове" },
            new Book { Id = 3, Title = "Мастер и Маргарита", Author = "Михаил Булгаков", PublicationYear = 1967, GenreId = 5, Description = "Мистический роман о визите дьявола в Москву" },
            new Book { Id = 4, Title = "Евгений Онегин", Author = "Александр Пушкин", PublicationYear = 1833, GenreId = 6, Description = "Роман в стихах о любви и жизни дворянства" },
            new Book { Id = 5, Title = "Анна Каренина", Author = "Лев Толстой", PublicationYear = 1877, GenreId = 5, Description = "Роман о трагической любви и семейной жизни" },

            // Научная фантастика
            new Book { Id = 6, Title = "Солярис", Author = "Станислав Лем", PublicationYear = 1961, GenreId = 2, Description = "Философский роман о контакте с внеземным разумом" },
            new Book { Id = 7, Title = "1984", Author = "Джордж Оруэлл", PublicationYear = 1949, GenreId = 2, Description = "Антиутопия о тоталитарном государстве" },
            new Book { Id = 8, Title = "Дюна", Author = "Фрэнк Герберт", PublicationYear = 1965, GenreId = 2, Description = "Эпическая сага о пустынной планете Арракис" },
            new Book { Id = 9, Title = "Пикник на обочине", Author = "Аркадий и Борис Стругацкие", PublicationYear = 1972, GenreId = 2, Description = "Роман о Зоне посещения" },
            new Book { Id = 10, Title = "Трудно быть богом", Author = "Аркадий и Борис Стругацкие", PublicationYear = 1964, GenreId = 2, Description = "О земном учёном на средневековой планете" },

            // Детективы
            new Book { Id = 11, Title = "Убийство в Восточном экспрессе", Author = "Агата Кристи", PublicationYear = 1934, GenreId = 3, Description = "Классический детектив с Эркюлем Пуаро" },
            new Book { Id = 12, Title = "Собака Баскервилей", Author = "Артур Конан Дойл", PublicationYear = 1902, GenreId = 3, Description = "Приключения Шерлока Холмса" },
            new Book { Id = 13, Title = "Десять негритят", Author = "Агата Кристи", PublicationYear = 1939, GenreId = 3, Description = "Загадочные убийства на острове" },

            // Фэнтези
            new Book { Id = 14, Title = "Властелин колец", Author = "Дж. Р. Р. Толкин", PublicationYear = 1954, GenreId = 4, Description = "Эпическое фэнтези о походе в Мордор" },
            new Book { Id = 15, Title = "Гарри Поттер и философский камень", Author = "Дж. К. Роулинг", PublicationYear = 1997, GenreId = 4, Description = "Первая книга о юном волшебнике" },
            new Book { Id = 16, Title = "Хоббит", Author = "Дж. Р. Р. Толкин", PublicationYear = 1937, GenreId = 4, Description = "Приключения Бильбо Бэггинса" },
            new Book { Id = 17, Title = "Ночной Дозор", Author = "Сергей Лукьяненко", PublicationYear = 1998, GenreId = 4, Description = "Городское фэнтези о войне Света и Тьмы" },

            // Детская литература
            new Book { Id = 18, Title = "Винни-Пух", Author = "Алан Милн", PublicationYear = 1926, GenreId = 10, Description = "Истории о медвежонке и его друзьях" },
            new Book { Id = 19, Title = "Маленький принц", Author = "Антуан де Сент-Экзюпери", PublicationYear = 1943, GenreId = 10, Description = "Философская сказка" },
            new Book { Id = 20, Title = "Гарри Поттер и Тайная комната", Author = "Дж. К. Роулинг", PublicationYear = 1998, GenreId = 10, Description = "Вторая книга о Гарри Поттере" },

            // Приключения
            new Book { Id = 21, Title = "Граф Монте-Кристо", Author = "Александр Дюма", PublicationYear = 1844, GenreId = 11, Description = "История мести и справедливости" },
            new Book { Id = 22, Title = "Три мушкетёра", Author = "Александр Дюма", PublicationYear = 1844, GenreId = 11, Description = "Приключения д'Артаньяна" },
            new Book { Id = 23, Title = "Остров сокровищ", Author = "Роберт Стивенсон", PublicationYear = 1883, GenreId = 11, Description = "Пиратские приключения" },

            // Биография
            new Book { Id = 24, Title = "Стив Джобс", Author = "Уолтер Айзексон", PublicationYear = 2011, GenreId = 7, Description = "Биография основателя Apple" },
            new Book { Id = 25, Title = "Илон Маск", Author = "Эшли Вэнс", PublicationYear = 2015, GenreId = 7, Description = "Биография предпринимателя" }
        };
        context.Books.AddRange(books);
        context.SaveChanges();

        // 7. Экземпляры книг
        var bookCopies = new List<BookCopy>();
        int copyId = 1;
        // Создаем по 2-3 экземпляра для каждой книги в разных библиотеках
        foreach (var book in books.Take(15)) // Для первых 15 книг
        {
            bookCopies.Add(new BookCopy { Id = copyId++, BookId = book.Id, LibraryId = 1, InventoryNumber = $"INV-{book.Id:D3}-001", Status = "available" });
            bookCopies.Add(new BookCopy { Id = copyId++, BookId = book.Id, LibraryId = 2, InventoryNumber = $"INV-{book.Id:D3}-002", Status = "available" });
            if (book.Id <= 10)
            {
                bookCopies.Add(new BookCopy { Id = copyId++, BookId = book.Id, LibraryId = 3, InventoryNumber = $"INV-{book.Id:D3}-003", Status = "available" });
            }
        }
        context.BookCopies.AddRange(bookCopies);
        context.SaveChanges();

        // 8. Бронирования
        var reservations = new[]
        {
            new Reservation { Id = 1, ReaderId = 1, BookId = 14, LibraryId = 1, ReservationDate = DateTime.UtcNow.AddDays(-2), Status = "active" },
            new Reservation { Id = 2, ReaderId = 2, BookId = 15, LibraryId = 2, ReservationDate = DateTime.UtcNow.AddDays(-1), Status = "active" },
            new Reservation { Id = 3, ReaderId = 3, BookId = 7, LibraryId = 1, ReservationDate = DateTime.UtcNow.AddDays(-5), Status = "completed" }
        };
        context.Reservations.AddRange(reservations);
        context.SaveChanges();

        // 9. Займы
        var loans = new[]
        {
            new Loan { Id = 1, ReaderId = 1, CopyId = 1, StaffId = 2, LoanDate = DateTime.UtcNow.AddDays(-10), DueDate = DateTime.UtcNow.AddDays(4), Status = "active" },
            new Loan { Id = 2, ReaderId = 2, CopyId = 3, StaffId = 2, LoanDate = DateTime.UtcNow.AddDays(-7), DueDate = DateTime.UtcNow.AddDays(7), Status = "active" },
            new Loan { Id = 3, ReaderId = 3, CopyId = 5, StaffId = 3, LoanDate = DateTime.UtcNow.AddDays(-20), DueDate = DateTime.UtcNow.AddDays(-6), Status = "overdue" },
            new Loan { Id = 4, ReaderId = 4, CopyId = 7, StaffId = 2, LoanDate = DateTime.UtcNow.AddDays(-30), DueDate = DateTime.UtcNow.AddDays(-16), ReturnDate = DateTime.UtcNow.AddDays(-15), Status = "returned" }
        };
        context.Loans.AddRange(loans);
        context.SaveChanges();

        // Обновляем статус экземпляров
        var copy1 = context.BookCopies.Find(1);
        var copy3 = context.BookCopies.Find(3);
        var copy5 = context.BookCopies.Find(5);
        if (copy1 != null) copy1.Status = "on_loan";
        if (copy3 != null) copy3.Status = "on_loan";
        if (copy5 != null) copy5.Status = "on_loan";
        context.SaveChanges();
    }
}
