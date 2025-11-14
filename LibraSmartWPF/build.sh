#!/bin/bash
# Скрипт сборки LibraSmart WPF приложения
# Для использования в WSL или Linux (с поддержкой cross-compile)

echo "╔════════════════════════════════════════════════════════════╗"
echo "║        LibraSmart WPF - Сборка приложения                 ║"
echo "╚════════════════════════════════════════════════════════════╝"
echo ""

# Проверка наличия .NET SDK
echo "Проверка .NET SDK..."
if ! command -v dotnet &> /dev/null; then
    echo "✗ .NET SDK не найден!"
    echo "Пожалуйста, установите .NET 8 SDK: https://dotnet.microsoft.com/download/dotnet/8.0"
    exit 1
fi

DOTNET_VERSION=$(dotnet --version)
echo "✓ .NET SDK версия: $DOTNET_VERSION"
echo ""

# Очистка предыдущих сборок
echo "Очистка предыдущих сборок..."
dotnet clean -c Release
if [ $? -ne 0 ]; then
    echo "✗ Ошибка при очистке проекта"
    exit 1
fi
echo "✓ Очистка завершена"
echo ""

# Восстановление зависимостей
echo "Восстановление зависимостей..."
dotnet restore
if [ $? -ne 0 ]; then
    echo "✗ Ошибка при восстановлении зависимостей"
    exit 1
fi
echo "✓ Зависимости восстановлены"
echo ""

# Сборка проекта
echo "Сборка проекта..."
echo "Это может занять несколько минут..."
echo ""

dotnet publish -c Release -r win-x64 \
    --self-contained true \
    /p:PublishSingleFile=true \
    /p:IncludeNativeLibrariesForSelfExtract=true \
    /p:EnableCompressionInSingleFile=true \
    /p:PublishReadyToRun=true \
    /p:PublishTrimmed=false

if [ $? -ne 0 ]; then
    echo ""
    echo "✗ Ошибка при сборке проекта"
    exit 1
fi

echo ""
echo "✓ Сборка успешно завершена!"
echo ""

# Путь к собранному файлу
PUBLISH_PATH="bin/Release/net8.0-windows/win-x64/publish"
EXE_PATH="$PUBLISH_PATH/LibraSmart.exe"

if [ -f "$EXE_PATH" ]; then
    FILE_SIZE=$(du -h "$EXE_PATH" | cut -f1)
    echo "═══════════════════════════════════════════════════"
    echo "Готовый файл:"
    echo "  Путь:    $EXE_PATH"
    echo "  Размер:  $FILE_SIZE"
    echo "═══════════════════════════════════════════════════"
    echo ""
    echo "Приложение готово к использованию!"
    echo "Запустите файл LibraSmart.exe для начала работы."
else
    echo "✗ Не удалось найти собранный файл"
    exit 1
fi
