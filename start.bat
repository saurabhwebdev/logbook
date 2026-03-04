@echo off
title Logbook - Launcher
echo ============================================
echo    Logbook - Starting Application
echo ============================================
echo.
echo  Backend API:   http://localhost:5034
echo  Swagger UI:    http://localhost:5034/swagger
echo  Frontend UI:   http://localhost:5173
echo  Login:         admin@coreengine.local / Admin@123
echo.
echo ============================================
echo  Starting backend (.NET 9)...
echo ============================================

start "Logbook API" cmd /k "cd /d C:\Webdev\coreenginelogbook && dotnet run --project src/CoreEngine.API --launch-profile http"

echo  Waiting for backend to initialize...
timeout /t 5 /nobreak >nul

echo ============================================
echo  Starting frontend (React + Vite)...
echo ============================================

start "Logbook Frontend" cmd /k "cd /d C:\Webdev\coreenginelogbook\frontend && npm run dev"

timeout /t 3 /nobreak >nul

echo.
echo ============================================
echo  Both servers starting! Opening browser...
echo ============================================
start http://localhost:5173

echo.
echo  Press any key to exit this launcher window.
echo  (The servers will keep running in their own windows)
pause >nul
