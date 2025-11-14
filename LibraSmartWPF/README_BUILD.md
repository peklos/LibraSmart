# 🚀 Быстрая сборка LibraSmart в один .exe файл

## ⚡ Самый простой способ

### На Windows:

1. Откройте PowerShell в папке `LibraSmartWPF`
2. Выполните:
   ```powershell
   .\build.ps1
   ```

### На Linux/WSL:

1. Откройте терминал в папке `LibraSmartWPF`
2. Выполните:
   ```bash
   chmod +x build.sh
   ./build.sh
   ```

## 📦 Где найти готовый файл

После успешной сборки:

```
LibraSmartWPF/bin/Release/net8.0-windows/win-x64/publish/LibraSmart.exe
```

## ✅ Что включено в exe-файл

- ✅ **Фронтенд** - весь WPF интерфейс с Material Design
- ✅ **Бэкенд** - вся бизнес-логика
- ✅ **SQLite** - провайдер базы данных
- ✅ **Entity Framework Core** - ORM
- ✅ **.NET Runtime** - не нужно устанавливать на целевом ПК

## 💾 База данных

База данных создается автоматически при первом запуске в:
```
C:\Users\{ваше_имя}\AppData\Roaming\LibraSmart\librasmart.db
```

Это значит:
- Данные сохраняются между обновлениями программы
- Можно делать резервные копии БД
- Программу можно переустановить без потери данных

## 📋 Требования для сборки

- .NET 8.0 SDK: https://dotnet.microsoft.com/download/dotnet/8.0
- Windows 10/11 или WSL
- ~2 GB свободного места

## ❓ Если что-то не работает

1. Убедитесь, что установлен .NET 8 SDK: `dotnet --version`
2. Проверьте, что версия >= 8.0.0
3. Запустите PowerShell/терминал от имени администратора

## 📖 Подробная документация

См. файл [BUILD_INSTRUCTIONS.md](./BUILD_INSTRUCTIONS.md)
