# LibraSmart Hybrid - Build Script
# Builds Vue frontend + C# backend into single .exe

Write-Host "============================================================" -ForegroundColor Cyan
Write-Host "   LibraSmart Hybrid - Full Build                          " -ForegroundColor Cyan
Write-Host "============================================================" -ForegroundColor Cyan
Write-Host ""

# Step 1: Build Vue.js frontend
Write-Host "[1/4] Building Vue.js frontend..." -ForegroundColor Yellow
$frontPath = Join-Path $PSScriptRoot "..\front"

if (-not (Test-Path $frontPath)) {
    Write-Host "[ERROR] Front folder not found!" -ForegroundColor Red
    exit 1
}

Set-Location $frontPath

if (-not (Get-Command node -ErrorAction SilentlyContinue)) {
    Write-Host "[ERROR] Node.js is not installed!" -ForegroundColor Red
    exit 1
}

Write-Host "  Installing dependencies..." -ForegroundColor Gray
npm install --silent 2>&1 | Out-Null
if ($LASTEXITCODE -ne 0) {
    Write-Host "[ERROR] Failed to install dependencies" -ForegroundColor Red
    exit 1
}

Write-Host "  Building frontend..." -ForegroundColor Gray
npm run build
if ($LASTEXITCODE -ne 0) {
    Write-Host "[ERROR] Frontend build failed" -ForegroundColor Red
    exit 1
}

Write-Host "[OK] Frontend built successfully" -ForegroundColor Green
Write-Host ""

# Step 2: Copy frontend to wwwroot
Write-Host "[2/4] Copying frontend to wwwroot..." -ForegroundColor Yellow
$distPath = Join-Path $frontPath "dist"
$wwwrootPath = Join-Path $PSScriptRoot "wwwroot"

if (Test-Path $wwwrootPath) {
    Remove-Item -Path $wwwrootPath -Recurse -Force
}

Copy-Item -Path $distPath -Destination $wwwrootPath -Recurse
Write-Host "[OK] Frontend copied" -ForegroundColor Green
Write-Host ""

# Step 3: Build C# backend
Write-Host "[3/4] Building C# backend..." -ForegroundColor Yellow
Set-Location $PSScriptRoot

if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
    Write-Host "[ERROR] .NET SDK is not installed!" -ForegroundColor Red
    exit 1
}

dotnet clean -c Release --nologo --verbosity quiet
if ($LASTEXITCODE -ne 0) {
    Write-Host "[ERROR] Clean failed" -ForegroundColor Red
    exit 1
}

Write-Host "  Restoring NuGet packages..." -ForegroundColor Gray
dotnet restore --nologo --verbosity quiet
if ($LASTEXITCODE -ne 0) {
    Write-Host "[ERROR] Restore failed" -ForegroundColor Red
    exit 1
}

Write-Host "  Publishing (this may take a few minutes)..." -ForegroundColor Gray
dotnet publish -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true /p:IncludeNativeLibrariesForSelfExtract=true /p:EnableCompressionInSingleFile=true /p:PublishReadyToRun=true /p:PublishTrimmed=false --nologo --verbosity quiet

if ($LASTEXITCODE -ne 0) {
    Write-Host "[ERROR] Publish failed" -ForegroundColor Red
    exit 1
}

Write-Host "[OK] Backend built successfully" -ForegroundColor Green
Write-Host ""

# Step 4: Check result
Write-Host "[4/4] Checking result..." -ForegroundColor Yellow
$exePath = Join-Path $PSScriptRoot "bin\Release\net8.0\win-x64\publish\LibraSmart.exe"

if (Test-Path $exePath) {
    $fileSize = [math]::Round((Get-Item $exePath).Length / 1MB, 2)
    Write-Host ""
    Write-Host "============================================================" -ForegroundColor Green
    Write-Host "           BUILD COMPLETED SUCCESSFULLY!                    " -ForegroundColor Green
    Write-Host "============================================================" -ForegroundColor Green
    Write-Host ""
    Write-Host "Output file:" -ForegroundColor Cyan
    Write-Host "   $exePath" -ForegroundColor White
    Write-Host ""
    Write-Host "Size: $fileSize MB" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "How to run:" -ForegroundColor Cyan
    Write-Host "   1. Copy LibraSmart.exe to any folder" -ForegroundColor White
    Write-Host "   2. Run it by double-click or from command line" -ForegroundColor White
    Write-Host "   3. Open browser: http://localhost:5000" -ForegroundColor White
    Write-Host ""
    Write-Host "Database will be created automatically in:" -ForegroundColor Cyan
    Write-Host "   $env:APPDATA\LibraSmart\librasmart.db" -ForegroundColor White
    Write-Host ""
} else {
    Write-Host "[ERROR] File not found: $exePath" -ForegroundColor Red
    exit 1
}
