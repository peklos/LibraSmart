# LibraSmart WPF Build Script
# Builds application as a single executable file

Write-Host "================================================================" -ForegroundColor Cyan
Write-Host "        LibraSmart WPF - Build Script                          " -ForegroundColor Cyan
Write-Host "================================================================" -ForegroundColor Cyan
Write-Host ""

# Check .NET SDK
Write-Host "Checking .NET SDK..." -ForegroundColor Yellow
$dotnetVersion = dotnet --version 2>$null
if ($LASTEXITCODE -ne 0) {
    Write-Host "[ERROR] .NET SDK not found!" -ForegroundColor Red
    Write-Host "Please install .NET 8 SDK: https://dotnet.microsoft.com/download/dotnet/8.0" -ForegroundColor Red
    exit 1
}
Write-Host "[OK] .NET SDK version: $dotnetVersion" -ForegroundColor Green
Write-Host ""

# Clean previous builds
Write-Host "Cleaning previous builds..." -ForegroundColor Yellow
dotnet clean -c Release
if ($LASTEXITCODE -ne 0) {
    Write-Host "[ERROR] Failed to clean project" -ForegroundColor Red
    exit 1
}
Write-Host "[OK] Clean completed" -ForegroundColor Green
Write-Host ""

# Restore dependencies
Write-Host "Restoring dependencies..." -ForegroundColor Yellow
dotnet restore
if ($LASTEXITCODE -ne 0) {
    Write-Host "[ERROR] Failed to restore dependencies" -ForegroundColor Red
    exit 1
}
Write-Host "[OK] Dependencies restored" -ForegroundColor Green
Write-Host ""

# Build project
Write-Host "Building project..." -ForegroundColor Yellow
Write-Host "This may take several minutes..." -ForegroundColor Gray
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
    Write-Host "[ERROR] Build failed" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "[OK] Build completed successfully!" -ForegroundColor Green
Write-Host ""

# Path to built file
$publishPath = "bin\Release\net8.0-windows\win-x64\publish"
$exePath = Join-Path $publishPath "LibraSmart.exe"

if (Test-Path $exePath) {
    $fileSize = (Get-Item $exePath).Length / 1MB
    Write-Host "================================================================" -ForegroundColor Cyan
    Write-Host "Build Result:" -ForegroundColor Green
    Write-Host "  Path:    $exePath" -ForegroundColor White
    Write-Host "  Size:    $([math]::Round($fileSize, 2)) MB" -ForegroundColor White
    Write-Host "================================================================" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Application is ready to use!" -ForegroundColor Green
    Write-Host "Run LibraSmart.exe to start the application." -ForegroundColor White
    Write-Host ""

    # Offer to open folder
    $response = Read-Host "Open output folder? (Y/N)"
    if ($response -eq 'Y' -or $response -eq 'y') {
        explorer.exe $publishPath
    }
} else {
    Write-Host "[ERROR] Built file not found" -ForegroundColor Red
    exit 1
}
