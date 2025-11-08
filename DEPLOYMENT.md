# 🚀 Инструкция по деплою LibraSmart

Полное руководство по развертыванию LibraSmart на **Netlify + Render + Neon**.

---

## 📦 Часть 1: База данных (Neon PostgreSQL)

### 1.1. Создайте базу данных на Neon

1. Перейдите на [neon.tech](https://neon.tech)
2. Зарегистрируйтесь или войдите
3. Нажмите **"Create Project"**
4. Выберите регион (рекомендуется Frankfurt или ближайший к вам)
5. Назовите проект: `librasmart`
6. Скопируйте **Connection String** (начинается с `postgresql://`)

Пример:
```
postgresql://username:password@ep-xxxxx.eu-central-1.aws.neon.tech/librasmart?sslmode=require
```

**Сохраните эту строку! Она понадобится для Render.**

---

## 🔧 Часть 2: Backend (Render)

### 2.1. Подключите репозиторий к Render

1. Перейдите на [render.com](https://render.com)
2. Зарегистрируйтесь или войдите
3. Нажмите **"New +"** → **"Web Service"**
4. Подключите ваш GitHub репозиторий **peklos/LibraSmart**
5. Выберите ветку: `claude/backend-roadmap-setup-011CUvnFJMogyEpAXojuRVoF`

### 2.2. Настройте Web Service

**Основные настройки:**
- **Name:** `librasmart-backend` (или любое имя)
- **Region:** Frankfurt (или ближайший)
- **Branch:** `claude/backend-roadmap-setup-011CUvnFJMogyEpAXojuRVoF`
- **Root Directory:** `back`
- **Runtime:** Python 3
- **Build Command:**
  ```
  pip install -r requirements.txt
  ```
- **Start Command:**
  ```
  uvicorn main:app --host 0.0.0.0 --port $PORT
  ```

**Environment Variables (переменные окружения):**

Добавьте переменную:
- **Key:** `DATABASE_URL`
- **Value:** ваша connection string из Neon
  ```
  postgresql://username:password@ep-xxxxx.eu-central-1.aws.neon.tech/librasmart?sslmode=require
  ```

**Advanced Settings:**
- **Health Check Path:** `/health`
- **Auto-Deploy:** Yes

### 2.3. Разверните backend

1. Нажмите **"Create Web Service"**
2. Дождитесь завершения деплоя (2-3 минуты)
3. Скопируйте URL вашего backend

Пример URL:
```
https://librasmart-backend.onrender.com
```

### 2.4. Проверьте работу backend

Откройте в браузере:
```
https://librasmart-backend.onrender.com/docs
```

Вы должны увидеть Swagger UI с документацией API.

**Важно:** Первый запуск может занять 30-60 секунд, так как создаются таблицы и заполняются тестовые данные.

---

## 🌐 Часть 3: Frontend (Netlify)

### 3.1. Подключите репозиторий к Netlify

1. Перейдите на [netlify.com](https://www.netlify.com)
2. Зарегистрируйтесь или войдите
3. Нажмите **"Add new site"** → **"Import an existing project"**
4. Выберите **GitHub** и найдите репозиторий **peklos/LibraSmart**
5. Выберите ветку: `claude/backend-roadmap-setup-011CUvnFJMogyEpAXojuRVoF`

### 3.2. Настройте билд

**Build settings:**
- **Base directory:** `front`
- **Build command:** `npm run build`
- **Publish directory:** `front/dist`
- **Node version:** 18

### 3.3. Добавьте переменную окружения

В **Site settings → Environment variables** добавьте:

- **Key:** `VITE_API_URL`
- **Value:** URL вашего backend с Render
  ```
  https://librasmart-backend.onrender.com
  ```

### 3.4. Разверните frontend

1. Нажмите **"Deploy site"**
2. Дождитесь завершения (1-2 минуты)
3. Netlify автоматически сгенерирует URL

Пример URL:
```
https://random-name-12345.netlify.app
```

### 3.5. (Опционально) Настройте свой домен

В **Site settings → Domain management** можно:
- Изменить Netlify subdomain (например, `librasmart.netlify.app`)
- Подключить свой домен

---

## ✅ Часть 4: Проверка работы

### 4.1. Откройте ваш сайт

Перейдите по URL от Netlify, например:
```
https://librasmart.netlify.app
```

### 4.2. Проверьте вход

**Тестовые аккаунты:**

**Читатель:**
- Email: `alekseev@mail.ru`
- Пароль: `reader123`

**Библиотекарь:**
- Email: `ivanov@library.ru`
- Пароль: `staff123`

**Администратор:**
- Email: `petrova@library.ru`
- Пароль: `admin123`

### 4.3. Проверьте функциональность

✅ Войдите как читатель:
- Просмотрите каталог книг
- Забронируйте книгу
- Проверьте "Мои книги"

✅ Войдите как библиотекарь:
- Откройте Dashboard
- Создайте новую книгу
- Создайте выдачу

✅ Войдите как администратор:
- Откройте "Управление персоналом"
- Добавьте нового сотрудника

---

## 🔧 Часть 5: Настройка мониторинга (опционально)

### 5.1. UptimeRobot для backend

1. Зарегистрируйтесь на [uptimerobot.com](https://uptimerobot.com)
2. Создайте новый монитор:
   - **Monitor Type:** HTTP(s)
   - **Friendly Name:** LibraSmart Backend
   - **URL:** `https://librasmart-backend.onrender.com/health`
   - **Monitoring Interval:** 5 minutes
   - **Monitor Timeout:** 30 seconds
   - **HTTP Method:** HEAD ✅

Это будет пинговать ваш backend каждые 5 минут, предотвращая "засыпание" на бесплатном плане Render.

---

## 📊 Архитектура после деплоя

```
┌─────────────┐
│   Browser   │
└──────┬──────┘
       │
       │ HTTPS
       ▼
┌─────────────────┐
│  Netlify (CDN)  │
│  Vue 3 Frontend │
│  Static Files   │
└────────┬────────┘
         │
         │ HTTPS API Calls
         │ (VITE_API_URL)
         ▼
┌──────────────────┐
│  Render Web App  │
│  FastAPI Backend │
│  uvicorn         │
└────────┬─────────┘
         │
         │ PostgreSQL
         ▼
┌──────────────────┐
│  Neon Database   │
│  PostgreSQL      │
│  Serverless      │
└──────────────────┘
```

---

## 🐛 Troubleshooting

### Backend не стартует
1. Проверьте логи в Render Dashboard
2. Убедитесь, что `DATABASE_URL` правильно настроен
3. Проверьте, что Root Directory = `back`

### Frontend показывает ошибки API
1. Откройте DevTools (F12) → Console
2. Проверьте, что `VITE_API_URL` настроен в Netlify
3. Убедитесь, что backend доступен (откройте `/docs`)
4. Проверьте CORS (должен быть настроен в backend)

### База данных не создается
1. Первый запуск занимает 30-60 секунд
2. Проверьте логи Render - должны быть сообщения:
   ```
   ✅ Таблицы базы данных созданы
   ✅ Роли созданы
   ✅ Библиотеки созданы
   ...
   ```

### Free tier Render "засыпает"
- Настройте UptimeRobot (см. Часть 5)
- Или перейдите на платный план ($7/месяц)

---

## 💰 Стоимость (бесплатный план)

| Сервис | План | Лимиты |
|--------|------|--------|
| **Neon** | Free | 0.5 GB storage, 1 project |
| **Render** | Free | 750 часов/месяц, засыпает после 15 мин неактивности |
| **Netlify** | Free | 100 GB bandwidth, 300 build минут |

**Итого:** 0₽/месяц при использовании бесплатных планов

---

## 🎉 Готово!

Ваш проект LibraSmart теперь полностью развернут и доступен в интернете!

**Не забудьте:**
- Сохранить все URL и пароли
- Настроить мониторинг
- Поделиться ссылкой с друзьями! 🚀

---

## 📝 Дополнительные ресурсы

- [Neon Documentation](https://neon.tech/docs)
- [Render Documentation](https://render.com/docs)
- [Netlify Documentation](https://docs.netlify.com)
- [FastAPI Documentation](https://fastapi.tiangolo.com)
- [Vue 3 Documentation](https://vuejs.org)
