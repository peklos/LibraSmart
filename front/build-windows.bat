@echo off
echo ========================================
echo LibraSmart - Windows Build Script
echo ========================================
echo.

echo Checking Node.js...
node --version >nul 2>&1
if errorlevel 1 (
    echo ERROR: Node.js not found!
    echo Download from: https://nodejs.org/
    pause
    exit /b 1
)

echo Checking Rust...
rustc --version >nul 2>&1
if errorlevel 1 (
    echo ERROR: Rust not found!
    echo Install from: https://rustup.rs/
    pause
    exit /b 1
)

echo.
echo Installing dependencies...
call npm install
if errorlevel 1 (
    echo ERROR: Failed to install dependencies
    pause
    exit /b 1
)

echo.
echo ========================================
echo Building Windows app...
echo This may take 10-30 minutes...
echo ========================================
echo.

call npm run tauri:build:windows
if errorlevel 1 (
    echo ERROR: Build failed
    pause
    exit /b 1
)

echo.
echo ========================================
echo BUILD SUCCESSFUL!
echo ========================================
echo.
echo Your app is ready:
echo.
echo MSI Installer:
echo src-tauri\target\release\bundle\msi\LibraSmart_1.0.0_x64_en-US.msi
echo.
echo NSIS Installer:
echo src-tauri\target\release\bundle\nsis\LibraSmart_1.0.0_x64-setup.exe
echo.
echo Executable:
echo src-tauri\target\release\LibraSmart.exe
echo.
pause
