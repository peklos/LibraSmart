# 🚀 LibraSmart Hybrid - Гибридное приложение

Современная библиотечная система с **Vue.js** фронтендом и **C# ASP.NET Core** бэкендом, упакованная в один исполняемый файл!

## ✨ Особенности

- **Frontend**: Vue.js 3 + TypeScript + Tailwind CSS
- **Backend**: C# ASP.NET Core Web API
- **Database**: SQLite (создается автоматически)
- **Упаковка**: Все в один .exe файл (~50-80 MB)
- **Запуск**: Просто двойной клик, никаких дополнительных установок!

## 📋 Требования для сборки

### Windows
- .NET 8.0 SDK: https://dotnet.microsoft.com/download/dotnet/8.0
- Node.js 18+: https://nodejs.org/
- PowerShell

### Linux/WSL
- .NET 8.0 SDK
- Node.js 18+
- Bash

## 🛠️ Сборка приложения

### Windows (Рекомендуется)

**Вариант 1: BAT файл (самый надежный)**
```cmd
cd LibraSmartAPI
build.bat
```

**Вариант 2: PowerShell**
```powershell
cd LibraSmartAPI
.\build-simple.ps1
```

### Linux/WSL

```bash
cd LibraSmartAPI
chmod +x build.sh
./build.sh
```

Скрипт автоматически:
1. ✅ Соберет Vue.js фронтенд
2. ✅ Скопирует его в wwwroot
3. ✅ Соберет C# бэкенд со встроенным фронтендом
4. ✅ Создаст один .exe файл

## 📦 Готовый файл

После сборки найдете здесь:

```
LibraSmartAPI/bin/Release/net8.0/win-x64/publish/LibraSmart.exe
```

## 🚀 Запуск приложения

### Вариант 1: Двойной клик
Просто запустите `LibraSmart.exe` двойным кликом!

### Вариант 2: Командная строка
```bash
./LibraSmart.exe
```

Приложение запустится на **http://localhost:5000**

## 🌐 Использование

После запуска:

1. **Веб-интерфейс**: http://localhost:5000
2. **API Документация (Swagger)**: http://localhost:5000/api/docs

### Тестовые данные

**Читатель:**
- Email: `alekseev@mail.ru`
- Пароль: `reader123`

**Персонал:**
- Email: `petrova@library.ru`
- Пароль: `admin123`

## 💾 База данных

База данных SQLite создается автоматически при первом запуске в:

```
Windows: C:\Users\{имя}\AppData\Roaming\LibraSmart\librasmart.db
Linux: ~/.local/share/LibraSmart/librasmart.db
```

**Преимущества:**
- ✅ Данные сохраняются между запусками
- ✅ Можно делать резервные копии
- ✅ Можно удалить БД для сброса к исходному состоянию

## 📂 Структура проекта

```
LibraSmartAPI/
├── Controllers/         # API контроллеры
│   ├── AuthController.cs
│   ├── BooksController.cs
│   ├── GenresController.cs
│   └── LibrariesController.cs
├── Data/               # Контекст БД и инициализация
│   ├── LibraryContext.cs
│   └── DatabaseInitializer.cs
├── Models/             # Модели данных
├── Services/           # Бизнес-логика
├── wwwroot/            # Vue.js фронтенд (генерируется при сборке)
├── Program.cs          # Точка входа
├── build.ps1           # Скрипт сборки (Windows)
├── build.sh            # Скрипт сборки (Linux)
└── README.md           # Этот файл
```

## 🔧 Разработка

### Запуск в режиме разработки

#### Backend
```bash
cd LibraSmartAPI
dotnet run
```
API будет доступно на http://localhost:5000

#### Frontend (отдельно)
```bash
cd front
npm install
npm run dev
```
Фронтенд будет доступен на http://localhost:5173

### API Endpoints

**Аутентификация:**
- `POST /api/auth/login/reader` - Вход для читателя
- `POST /api/auth/login/staff` - Вход для персонала

**Книги:**
- `GET /api/books` - Список всех книг
- `GET /api/books/{id}` - Информация о книге
- `POST /api/books` - Добавить книгу
- `PUT /api/books/{id}` - Обновить книгу
- `DELETE /api/books/{id}` - Удалить книгу

**Жанры:**
- `GET /api/genres` - Список жанров

**Библиотеки:**
- `GET /api/libraries` - Список библиотек

## 🎯 Преимущества данного подхода

### ✅ Для пользователей
- Один .exe файл - легко распространять
- Не нужно устанавливать Node.js, .NET Runtime
- Работает "из коробки"
- Современный веб-интерфейс

### ✅ Для разработчиков
- Vue.js - современный фронтенд фреймворк
- C# - мощный и типизированный бэкенд
- SQLite - простая и надежная БД
- REST API - легко расширять
- Swagger - автодокументирование API

## 📝 Лицензия

© 2024 LibraSmart Team

## 🐛 Поддержка

При возникновении проблем:
1. Проверьте логи приложения
2. Проверьте, что порт 5000 свободен
3. Создайте issue в репозитории

---

**Версия:** 1.0.0
**Последнее обновление:** Ноябрь 2024
