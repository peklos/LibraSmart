# LibraSmart - Библиотечная система управления

Современная система управления библиотекой с веб и десктоп версиями.

## 🚀 Возможности

- **Управление книгами**: каталог, жанры, экземпляры
- **Работа с читателями**: регистрация, выдача книг, история
- **Библиотеки**: управление несколькими библиотеками
- **Персонал**: роли и права доступа
- **Статистика**: аналитика использования

## 📦 Доступные версии

### Windows Desktop приложение

**Готовое приложение для Windows 10/11**

📥 **Скачать**: `releases/windows/LibraSmart.exe` (16 MB)

**Требования:**
- Windows 10/11
- WebView2 Runtime (обычно уже установлен)

**Установка:**
1. Скачайте `LibraSmart.exe`
2. Запустите двойным кликом
3. Приложение откроется как обычная программа

**Примечание**: При первом запуске Windows может показать предупреждение SmartScreen - нажмите "Подробнее" → "Выполнить в любом случае"

### Веб-версия

Доступна онлайн: `https://librasmart.onrender.com`

## 🛠️ Разработка

### Структура проекта

```
LibraSmart/
├── front/          # Vue.js фронтенд + Tauri desktop
│   ├── src/        # Исходники Vue приложения
│   ├── src-tauri/  # Tauri конфигурация и Rust код
│   └── dist/       # Собранный фронтенд
├── back/           # Express.js бэкенд
├── releases/       # Готовые сборки
│   └── windows/    # Windows .exe файлы
└── README.md       # Этот файл
```

### Запуск локально

#### Фронтенд (веб-версия)

```bash
cd front
npm install
npm run dev
```

#### Бэкенд

```bash
cd back
npm install
npm start
```

### Сборка десктоп-приложения

**Windows** (собирается на Windows машине):

```bash
cd front
npm install
npm run tauri build
```

**Linux/RedOS**:

```bash
cd front
npm install
npm run tauri build
```

Готовые файлы будут в `front/src-tauri/target/release/`

## 📖 Документация

Подробная документация по сборке и развертыванию:

- `front/TAURI_BUILD.md` - Инструкции по сборке Tauri приложений
- `front/BUILD_INSTRUCTIONS.md` - Общие инструкции по сборке
- `DEPLOYMENT.md` - Развертывание на production

## 🔧 Технологии

**Фронтенд:**
- Vue 3 + TypeScript
- Vite
- Tailwind CSS
- Pinia (state management)
- Tauri (desktop app)

**Бэкенд:**
- Node.js + Express
- SQLite/PostgreSQL
- JWT авторизация

## 📝 Лицензия

© 2024 LibraSmart

## 🐛 Поддержка

При возникновении проблем создайте issue в репозитории проекта.

---

**Версия:** 1.0.0
**Последнее обновление:** Ноябрь 2024
