# LibraSmart Desktop - Сборка приложения

LibraSmart теперь доступен как десктопное приложение для Windows и Linux (включая RedOS) благодаря Tauri.

## Требования

### Для всех платформ:
- Node.js 18+
- npm или yarn
- Rust 1.70+

### Для Windows:
- Microsoft Visual Studio C++ Build Tools
- WebView2 (обычно уже установлен в Windows 10/11)

### Для Linux:
```bash
# Ubuntu/Debian/RedOS
sudo apt update
sudo apt install libwebkit2gtk-4.0-dev \
    build-essential \
    curl \
    wget \
    file \
    libssl-dev \
    libgtk-3-dev \
    libayatana-appindicator3-dev \
    librsvg2-dev
```

## Установка зависимостей

```bash
cd front
npm install
```

## Разработка

Запуск приложения в режиме разработки:

```bash
npm run tauri:dev
```

## Сборка

### Сборка для текущей платформы:
```bash
npm run tauri:build
```

### Сборка для Windows (из Windows):
```bash
npm run tauri:build:windows
```

### Сборка для Linux (из Linux):
```bash
npm run tauri:build:linux
```

## Где найти готовые сборки

После успешной сборки файлы будут находиться в:

### Windows:
- `src-tauri/target/release/LibraSmart.exe` - исполняемый файл
- `src-tauri/target/release/bundle/msi/LibraSmart_1.0.0_x64_en-US.msi` - установщик MSI
- `src-tauri/target/release/bundle/nsis/LibraSmart_1.0.0_x64-setup.exe` - установщик NSIS

### Linux:
- `src-tauri/target/release/librasmart` - исполняемый файл
- `src-tauri/target/release/bundle/deb/librasmart_1.0.0_amd64.deb` - пакет DEB для Debian/Ubuntu/RedOS
- `src-tauri/target/release/bundle/appimage/librasmart_1.0.0_amd64.AppImage` - AppImage (универсальный)

## Особенности десктопной версии

- **Автономная работа**: Приложение работает независимо от браузера
- **Системная интеграция**: Полноценное окно с возможностью сворачивания/разворачивания
- **Безопасность**: CSP настроен для работы с API на https://librasmart.onrender.com
- **Производительность**: Быстрее веб-версии благодаря нативному рендерингу

## API

Приложение использует продакшн API: `https://librasmart.onrender.com`

Если нужно изменить URL API, отредактируйте файл `.env.production`:
```env
VITE_API_URL=https://your-api-url.com
```

## Размер окна

По умолчанию:
- Ширина: 1200px
- Высота: 800px
- Минимальная ширина: 800px
- Минимальная высота: 600px

Можно изменить в `src-tauri/tauri.conf.json`

## Поддержка RedOS

LibraSmart полностью совместим с RedOS (российская операционная система на базе Linux).
Установка через .deb пакет:

```bash
sudo dpkg -i librasmart_1.0.0_amd64.deb
```

Или используйте AppImage для универсальной установки:
```bash
chmod +x librasmart_1.0.0_amd64.AppImage
./librasmart_1.0.0_amd64.AppImage
```

## Проблемы и решения

### Windows: "WebView2 not found"
Установите WebView2 Runtime: https://developer.microsoft.com/en-us/microsoft-edge/webview2/

### Linux: Ошибки при сборке
Убедитесь, что установлены все зависимости (см. раздел "Требования")

### Проблемы с CORS
API уже настроен для работы с десктопным приложением. CSP в Tauri разрешает подключения к https://librasmart.onrender.com

## Лицензия

© 2024 LibraSmart
