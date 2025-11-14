# LibraSmart Hybrid App - Build Script (PowerShell)
# Собирает Vue фронтенд + C# бэкенд в один .exe файл

Write-Host "================================================================" -ForegroundColor Cyan
Write-Host "   LibraSmart Hybrid - Полная сборка приложения                " -ForegroundColor Cyan
Write-Host "================================================================" -ForegroundColor Cyan
Write-Host ""

# Шаг 1: Сборка Vue.js фронтенда
Write-Host "[1/4] Сборка Vue.js фронтенда..." -ForegroundColor Yellow
$frontPath = Join-Path $PSScriptRoot "..\front"

if (-not (Test-Path $frontPath))
{
    Write-Host "[ERROR] Папка front не найдена!" -ForegroundColor Red
    exit 1
}

Push-Location $frontPath

# Проверка Node.js
if (-not (Get-Command node -ErrorAction SilentlyContinue))
{
    Write-Host "[ERROR] Node.js не установлен!" -ForegroundColor Red
    Pop-Location
    exit 1
}

# Установка зависимостей и сборка
Write-Host "  -> Установка зависимостей..." -ForegroundColor Gray
npm install --silent
if ($LASTEXITCODE -ne 0)
{
    Write-Host "[ERROR] Ошибка установки зависимостей" -ForegroundColor Red
    Pop-Location
    exit 1
}

Write-Host "  -> Сборка фронтенда..." -ForegroundColor Gray
npm run build
if ($LASTEXITCODE -ne 0)
{
    Write-Host "[ERROR] Ошибка сборки фронтенда" -ForegroundColor Red
    Pop-Location
    exit 1
}

Pop-Location
Write-Host "[OK] Фронтенд собран" -ForegroundColor Green
Write-Host ""

# Шаг 2: Копирование фронтенда в wwwroot
Write-Host "[2/4] Копирование фронтенда в wwwroot..." -ForegroundColor Yellow
$distPath = Join-Path $frontPath "dist"
$wwwrootPath = Join-Path $PSScriptRoot "wwwroot"

# Очищаем wwwroot
if (Test-Path $wwwrootPath)
{
    Remove-Item -Path $wwwrootPath -Recurse -Force
}

# Копируем dist в wwwroot
Copy-Item -Path $distPath -Destination $wwwrootPath -Recurse
Write-Host "[OK] Фронтенд скопирован" -ForegroundColor Green
Write-Host ""

# Шаг 3: Сборка C# бэкенда
Write-Host "[3/4] Сборка C# ASP.NET Core бэкенда..." -ForegroundColor Yellow

# Проверка .NET SDK
if (-not (Get-Command dotnet -ErrorAction SilentlyContinue))
{
    Write-Host "[ERROR] .NET SDK не установлен!" -ForegroundColor Red
    exit 1
}

# Очистка
dotnet clean -c Release --nologo --verbosity quiet
if ($LASTEXITCODE -ne 0)
{
    Write-Host "[ERROR] Ошибка очистки" -ForegroundColor Red
    exit 1
}

# Восстановление зависимостей
Write-Host "  -> Восстановление NuGet пакетов..." -ForegroundColor Gray
dotnet restore --nologo --verbosity quiet
if ($LASTEXITCODE -ne 0)
{
    Write-Host "[ERROR] Ошибка восстановления пакетов" -ForegroundColor Red
    exit 1
}

# Публикация в один файл
Write-Host "  -> Публикация приложения (это может занять несколько минут)..." -ForegroundColor Gray
dotnet publish -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true /p:IncludeNativeLibrariesForSelfExtract=true /p:EnableCompressionInSingleFile=true /p:PublishReadyToRun=true /p:PublishTrimmed=false --nologo --verbosity quiet

if ($LASTEXITCODE -ne 0)
{
    Write-Host "[ERROR] Ошибка публикации" -ForegroundColor Red
    exit 1
}

Write-Host "[OK] Бэкенд собран" -ForegroundColor Green
Write-Host ""

# Шаг 4: Проверка результата
Write-Host "[4/4] Проверка результата..." -ForegroundColor Yellow
$exePath = Join-Path $PSScriptRoot "bin\Release\net8.0\win-x64\publish\LibraSmart.exe"

if (Test-Path $exePath)
{
    $fileSize = (Get-Item $exePath).Length / 1MB
    Write-Host ""
    Write-Host "================================================================" -ForegroundColor Green
    Write-Host "           СБОРКА УСПЕШНО ЗАВЕРШЕНА!                            " -ForegroundColor Green
    Write-Host "================================================================" -ForegroundColor Green
    Write-Host ""
    Write-Host "Готовый файл:" -ForegroundColor Cyan
    Write-Host "   $exePath" -ForegroundColor White
    Write-Host ""
    Write-Host "Размер: $([math]::Round($fileSize, 2)) MB" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Как запустить:" -ForegroundColor Cyan
    Write-Host "   1. Скопируйте LibraSmart.exe в любую папку" -ForegroundColor White
    Write-Host "   2. Запустите двойным кликом или из командной строки" -ForegroundColor White
    Write-Host "   3. Откройте браузер: http://localhost:5000" -ForegroundColor White
    Write-Host ""
    Write-Host "База данных создается автоматически в:" -ForegroundColor Cyan
    Write-Host "   %APPDATA%\LibraSmart\librasmart.db" -ForegroundColor White
    Write-Host ""
}
else
{
    Write-Host "[ERROR] Файл не найден: $exePath" -ForegroundColor Red
    exit 1
}
