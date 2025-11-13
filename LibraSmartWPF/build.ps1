# Скрипт сборки LibraSmart WPF приложения в один .exe файл
# PowerShell скрипт для Windows

Write-Host "╔════════════════════════════════════════════════════════════╗" -ForegroundColor Cyan
Write-Host "║        LibraSmart WPF - Сборка приложения                 ║" -ForegroundColor Cyan
Write-Host "╚════════════════════════════════════════════════════════════╝" -ForegroundColor Cyan
Write-Host ""

# Проверка наличия .NET 8 SDK
Write-Host "Проверка .NET SDK..." -ForegroundColor Yellow
$dotnetVersion = dotnet --version 2>$null
if ($LASTEXITCODE -ne 0) {
    Write-Host "✗ .NET SDK не найден!" -ForegroundColor Red
    Write-Host "Пожалуйста, установите .NET 8 SDK: https://dotnet.microsoft.com/download/dotnet/8.0" -ForegroundColor Red
    exit 1
}
Write-Host "✓ .NET SDK версия: $dotnetVersion" -ForegroundColor Green
Write-Host ""

# Очистка предыдущих сборок
Write-Host "Очистка предыдущих сборок..." -ForegroundColor Yellow
dotnet clean -c Release
if ($LASTEXITCODE -ne 0) {
    Write-Host "✗ Ошибка при очистке проекта" -ForegroundColor Red
    exit 1
}
Write-Host "✓ Очистка завершена" -ForegroundColor Green
Write-Host ""

# Восстановление зависимостей
Write-Host "Восстановление зависимостей..." -ForegroundColor Yellow
dotnet restore
if ($LASTEXITCODE -ne 0) {
    Write-Host "✗ Ошибка при восстановлении зависимостей" -ForegroundColor Red
    exit 1
}
Write-Host "✓ Зависимости восстановлены" -ForegroundColor Green
Write-Host ""

# Сборка проекта
Write-Host "Сборка проекта..." -ForegroundColor Yellow
Write-Host "Это может занять несколько минут..." -ForegroundColor Gray
Write-Host ""

dotnet publish -c Release -r win-x64 `
    --self-contained true `
    /p:PublishSingleFile=true `
    /p:IncludeNativeLibrariesForSelfExtract=true `
    /p:EnableCompressionInSingleFile=true `
    /p:PublishReadyToRun=true `
    /p:PublishTrimmed=false

if ($LASTEXITCODE -ne 0) {
    Write-Host ""
    Write-Host "✗ Ошибка при сборке проекта" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "✓ Сборка успешно завершена!" -ForegroundColor Green
Write-Host ""

# Путь к собранному файлу
$publishPath = "bin\Release\net8.0-windows\win-x64\publish"
$exePath = Join-Path $publishPath "LibraSmart.exe"

if (Test-Path $exePath) {
    $fileSize = (Get-Item $exePath).Length / 1MB
    Write-Host "═══════════════════════════════════════════════════" -ForegroundColor Cyan
    Write-Host "Готовый файл:" -ForegroundColor Green
    Write-Host "  Путь:    $exePath" -ForegroundColor White
    Write-Host "  Размер:  $([math]::Round($fileSize, 2)) МБ" -ForegroundColor White
    Write-Host "═══════════════════════════════════════════════════" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Приложение готово к использованию!" -ForegroundColor Green
    Write-Host "Запустите файл LibraSmart.exe для начала работы." -ForegroundColor White
    Write-Host ""

    # Предложение открыть папку
    $response = Read-Host "Открыть папку с файлом? (Y/N)"
    if ($response -eq 'Y' -or $response -eq 'y') {
        explorer.exe $publishPath
    }
} else {
    Write-Host "✗ Не удалось найти собранный файл" -ForegroundColor Red
    exit 1
}
