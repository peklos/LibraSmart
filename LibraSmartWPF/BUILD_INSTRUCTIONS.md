# Инструкция по сборке LibraSmart в единый исполняемый файл

## Требования

- Windows 10/11 или WSL (Windows Subsystem for Linux)
- .NET 8.0 SDK или выше
- Минимум 2 GB свободного места на диске

## Установка .NET SDK (если еще не установлен)

### Windows
1. Скачайте .NET 8.0 SDK: https://dotnet.microsoft.com/download/dotnet/8.0
2. Запустите установщик и следуйте инструкциям
3. Проверьте установку: откройте PowerShell и выполните `dotnet --version`

### WSL/Linux
```bash
wget https://dot.net/v1/dotnet-install.sh -O dotnet-install.sh
chmod +x dotnet-install.sh
./dotnet-install.sh --channel 8.0
```

## Быстрая сборка

### Вариант 1: Использование готового скрипта (Windows)

Откройте PowerShell в папке `LibraSmartWPF` и выполните:

```powershell
.\build.ps1
```

### Вариант 2: Использование готового скрипта (WSL/Linux)

Откройте терминал в папке `LibraSmartWPF` и выполните:

```bash
chmod +x build.sh
./build.sh
```

### Вариант 3: Ручная сборка

Откройте терминал/PowerShell в папке `LibraSmartWPF` и выполните:

```bash
# Очистка предыдущих сборок
dotnet clean -c Release

# Восстановление зависимостей
dotnet restore

# Публикация в единый исполняемый файл
dotnet publish -c Release -r win-x64 \
    --self-contained true \
    /p:PublishSingleFile=true \
    /p:IncludeNativeLibrariesForSelfExtract=true \
    /p:EnableCompressionInSingleFile=true \
    /p:PublishReadyToRun=true \
    /p:PublishTrimmed=false
```

## Результат сборки

После успешной сборки готовый файл будет находиться здесь:

```
LibraSmartWPF/bin/Release/net8.0-windows/win-x64/publish/LibraSmart.exe
```

**Размер файла:** ~80-120 MB (это нормально, так как включены все зависимости)

## Что включено в exe-файл

✓ Весь фронтенд (WPF интерфейс)
✓ Вся бизнес-логика (backend)
✓ SQLite провайдер для базы данных
✓ Entity Framework Core
✓ Material Design темы
✓ Все нативные библиотеки (.NET Runtime)

## База данных

База данных SQLite **НЕ включена** в exe-файл (это правильно!).
Она автоматически создается при первом запуске в папке:

```
%APPDATA%\LibraSmart\librasmart.db
```

Это позволяет:
- Обновлять приложение без потери данных
- Делать резервные копии БД
- Переносить приложение на другие компьютеры

## Распространение

Вы можете:
1. Скопировать `LibraSmart.exe` на любой компьютер с Windows
2. Запустить двойным кликом
3. Приложение работает без установки .NET на целевом компьютере

## Сборка для других платформ

### Windows x86 (32-bit)
```bash
dotnet publish -c Release -r win-x86 --self-contained true /p:PublishSingleFile=true
```

### Windows ARM64
```bash
dotnet publish -c Release -r win-arm64 --self-contained true /p:PublishSingleFile=true
```

## Оптимизация размера

Если нужен меньший размер файла (с потерей производительности):

```bash
dotnet publish -c Release -r win-x64 \
    --self-contained true \
    /p:PublishSingleFile=true \
    /p:PublishTrimmed=true \
    /p:TrimMode=partial
```

⚠️ **Внимание:** Trimming может удалить нужный код. Тестируйте после сборки!

## Troubleshooting

### Ошибка: "SDK not found"
Установите .NET 8.0 SDK (см. раздел "Установка .NET SDK")

### Ошибка: "Access denied"
Запустите терминал от имени администратора

### Приложение не запускается
1. Проверьте, что целевой ПК имеет Windows 10 1607+ или Windows 11
2. Убедитесь, что антивирус не блокирует exe-файл
3. Попробуйте запустить от имени администратора

### База данных не создается
Проверьте права доступа к папке `%APPDATA%`

## Дополнительные команды

### Проверка зависимостей
```bash
dotnet list package
```

### Анализ размера файла
```bash
dotnet publish -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true /p:DebugType=none /p:DebugSymbols=false
```

### Создание installer (опционально)
Используйте WiX Toolset или Inno Setup для создания MSI/EXE инсталлятора
