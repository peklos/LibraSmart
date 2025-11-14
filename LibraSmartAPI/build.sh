#!/bin/bash
# LibraSmart Hybrid App - Build Script (Bash)
# Собирает Vue фронтенд + C# бэкенд в один .exe файл

echo "╔════════════════════════════════════════════════════════╗"
echo "║   LibraSmart Hybrid - Полная сборка приложения        ║"
echo "╚════════════════════════════════════════════════════════╝"
echo ""

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

# Шаг 1: Сборка Vue.js фронтенда
echo "[1/4] Сборка Vue.js фронтенда..."
FRONT_PATH="$SCRIPT_DIR/../front"

if [ ! -d "$FRONT_PATH" ]; then
    echo "✗ Папка front не найдена!"
    exit 1
fi

cd "$FRONT_PATH"

# Проверка Node.js
if ! command -v node &> /dev/null; then
    echo "✗ Node.js не установлен!"
    exit 1
fi

# Установка зависимостей и сборка
echo "  → Установка зависимостей..."
npm install --silent
if [ $? -ne 0 ]; then
    echo "✗ Ошибка установки зависимостей"
    exit 1
fi

echo "  → Сборка фронтенда..."
npm run build
if [ $? -ne 0 ]; then
    echo "✗ Ошибка сборки фронтенда"
    exit 1
fi

echo "✓ Фронтенд собран"
echo ""

# Шаг 2: Копирование фронтенда в wwwroot
echo "[2/4] Копирование фронтенда в wwwroot..."
DIST_PATH="$FRONT_PATH/dist"
WWWROOT_PATH="$SCRIPT_DIR/wwwroot"

# Очищаем wwwroot
if [ -d "$WWWROOT_PATH" ]; then
    rm -rf "$WWWROOT_PATH"
fi

# Копируем dist в wwwroot
cp -r "$DIST_PATH" "$WWWROOT_PATH"
echo "✓ Фронтенд скопирован"
echo ""

# Шаг 3: Сборка C# бэкенда
echo "[3/4] Сборка C# ASP.NET Core бэкенда..."
cd "$SCRIPT_DIR"

# Проверка .NET SDK
if ! command -v dotnet &> /dev/null; then
    echo "✗ .NET SDK не установлен!"
    exit 1
fi

# Очистка
dotnet clean -c Release --nologo --verbosity quiet
if [ $? -ne 0 ]; then
    echo "✗ Ошибка очистки"
    exit 1
fi

# Восстановление зависимостей
echo "  → Восстановление NuGet пакетов..."
dotnet restore --nologo --verbosity quiet
if [ $? -ne 0 ]; then
    echo "✗ Ошибка восстановления пакетов"
    exit 1
fi

# Публикация в один файл
echo "  → Публикация приложения (это может занять несколько минут)..."
dotnet publish -c Release -r win-x64 \
    --self-contained true \
    /p:PublishSingleFile=true \
    /p:IncludeNativeLibrariesForSelfExtract=true \
    /p:EnableCompressionInSingleFile=true \
    /p:PublishReadyToRun=true \
    /p:PublishTrimmed=false \
    --nologo \
    --verbosity quiet

if [ $? -ne 0 ]; then
    echo "✗ Ошибка публикации"
    exit 1
fi

echo "✓ Бэкенд собран"
echo ""

# Шаг 4: Проверка результата
echo "[4/4] Проверка результата..."
EXE_PATH="$SCRIPT_DIR/bin/Release/net8.0/win-x64/publish/LibraSmart.exe"

if [ -f "$EXE_PATH" ]; then
    FILE_SIZE=$(du -h "$EXE_PATH" | cut -f1)
    echo ""
    echo "╔════════════════════════════════════════════════════════╗"
    echo "║           СБОРКА УСПЕШНО ЗАВЕРШЕНА!                    ║"
    echo "╚════════════════════════════════════════════════════════╝"
    echo ""
    echo "📦 Готовый файл:"
    echo "   $EXE_PATH"
    echo ""
    echo "📊 Размер: $FILE_SIZE"
    echo ""
    echo "🚀 Как запустить:"
    echo "   1. Скопируйте LibraSmart.exe в любую папку"
    echo "   2. Запустите двойным кликом или из командной строки"
    echo "   3. Откройте браузер: http://localhost:5000"
    echo ""
    echo "📝 База данных создается автоматически в:"
    echo "   %APPDATA%\\LibraSmart\\librasmart.db"
    echo ""
else
    echo "✗ Файл не найден: $EXE_PATH"
    exit 1
fi
