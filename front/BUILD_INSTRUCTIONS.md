# 🚀 Инструкция по сборке LibraSmart Desktop

## Быстрый старт

### Windows
```cmd
build-windows.bat
```

### Linux (включая RedOS)
```bash
./build-linux.sh
```

---

## Требования

### Windows
1. **Node.js 18+** - https://nodejs.org/
2. **Rust** - https://rustup.rs/
3. **Visual Studio C++ Build Tools** (обычно уже есть)
4. **WebView2** (обычно уже есть в Windows 10/11)

### Linux
1. **Node.js 18+**
```bash
sudo apt install nodejs npm
```

2. **Rust**
```bash
curl --proto '=https' --tlsv1.2 -sSf https://sh.rustup.rs | sh
```

3. **Системные зависимости** (скрипт установит автоматически)

---

## Пошаговая инструкция

### Windows

1. Открой PowerShell или CMD в папке `front`
2. Запусти:
```cmd
build-windows.bat
```
3. Подожди 10-30 минут (первая сборка дольше)
4. Готово! Найди файлы в `src-tauri\target\release\bundle\`

**Результат:**
- `msi\LibraSmart_1.0.0_x64_en-US.msi` - установщик MSI
- `nsis\LibraSmart_1.0.0_x64-setup.exe` - установщик NSIS
- `..\LibraSmart.exe` - просто .exe файл

### Linux (Ubuntu/Debian/RedOS)

1. Открой терминал в папке `front`
2. Запусти:
```bash
./build-linux.sh
```
3. Подожди 10-30 минут (первая сборка дольше)
4. Готово! Найди файлы в `src-tauri/target/release/bundle/`

**Результат:**
- `deb/librasmart_1.0.0_amd64.deb` - пакет для Ubuntu/Debian/RedOS
- `appimage/librasmart_1.0.0_amd64.AppImage` - универсальный AppImage
- `../librasmart` - просто исполняемый файл

**Установка DEB (RedOS/Ubuntu/Debian):**
```bash
sudo dpkg -i src-tauri/target/release/bundle/deb/librasmart_1.0.0_amd64.deb
```

**Запуск AppImage:**
```bash
chmod +x src-tauri/target/release/bundle/appimage/librasmart_1.0.0_amd64.AppImage
./src-tauri/target/release/bundle/appimage/librasmart_1.0.0_amd64.AppImage
```

---

## Ручная сборка

Если скрипты не работают:

```bash
cd front
npm install
npm run tauri:build          # текущая платформа
npm run tauri:build:windows  # Windows
npm run tauri:build:linux    # Linux
```

---

## Частые проблемы

### Windows: "WebView2 not found"
Скачай и установи: https://developer.microsoft.com/microsoft-edge/webview2/

### Windows: "MSVC not found"
Установи Visual Studio Build Tools:
https://visualstudio.microsoft.com/downloads/

### Linux: Ошибки компиляции
Запусти скрипт `build-linux.sh` - он установит все зависимости.

### Долго собирается
Первая сборка занимает 10-30 минут. Последующие - быстрее (2-5 минут).

---

## Что получишь

✅ **Готовое десктопное приложение**
- Работает без браузера
- Свое окно, можно сворачивать
- Подключается к API: https://librasmart.onrender.com
- Тот же интерфейс, что на сайте
- Быстрее веб-версии

✅ **Размер окна**
- 1200x800 по умолчанию
- Можно изменять
- Минимум: 800x600

✅ **Установщики**
- Windows: .msi и .exe
- Linux: .deb и .AppImage

---

## Размер файлов

- Windows .exe: ~10-15 MB
- Windows установщик: ~12-18 MB
- Linux .deb: ~10-15 MB
- Linux .AppImage: ~15-20 MB

---

## Поддержка RedOS

Приложение полностью совместимо с RedOS (российская ОС на базе Linux).

**Установка:**
```bash
sudo dpkg -i librasmart_1.0.0_amd64.deb
```

Или используй AppImage для универсальной установки.

---

## Нужна помощь?

1. Проверь, что установлены все требования
2. Запусти готовые скрипты (build-windows.bat или build-linux.sh)
3. Читай ошибки в консоли - они подскажут что не хватает
4. Первая сборка всегда долгая - это нормально!

---

**Готово!** После сборки запускай .exe (Windows) или .deb/.AppImage (Linux) и пользуйся! 🎉
