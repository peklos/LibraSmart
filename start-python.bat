@echo off
chcp 65001 >nul
echo ============================================================
echo    LibraSmart - Full Stack Build (Vue + Python + SQLite)
echo ============================================================
echo.

echo [1/3] Building Vue.js frontend...
cd ..\front
call npm install --silent
call npm run build
if errorlevel 1 (
    echo [ERROR] Frontend build failed
    exit /b 1
)
echo [OK] Frontend built
echo.

echo [2/3] Setting up Python backend...
cd ..\back
if not exist ".env" (
    echo Creating .env file...
    echo DATABASE_URL=sqlite:///./librasmart.db > .env
    echo SECRET_KEY=librasmart_secret_key_2024 >> .env
)

echo Installing Python dependencies...
pip install -r requirements.txt --quiet
if errorlevel 1 (
    echo [ERROR] Failed to install Python dependencies
    exit /b 1
)
echo [OK] Python backend ready
echo.

echo [3/3] Starting application...
echo.
echo ============================================================
echo            Application is starting!
echo ============================================================
echo.
echo Web Interface: http://localhost:8000
echo API Docs: http://localhost:8000/docs
echo.
echo Database: ./librasmart.db
echo.
echo Press Ctrl+C to stop
echo.

python main.py
