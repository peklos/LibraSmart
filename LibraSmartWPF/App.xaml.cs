using System.Windows;
using System.Threading.Tasks;
using System.IO;
using LibraSmartWPF.Data;
using Microsoft.EntityFrameworkCore;

namespace LibraSmartWPF;

public partial class App : Application
{
    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // Отключаем автоматический показ главного окна
        this.ShutdownMode = ShutdownMode.OnExplicitShutdown;

        // Инициализация базы данных при старте (асинхронно)
        await InitializeDatabaseAsync();

        // Включаем обычный режим закрытия
        this.ShutdownMode = ShutdownMode.OnMainWindowClose;

        // Теперь показываем окно входа
        var loginWindow = new Views.LoginWindow();
        this.MainWindow = loginWindow;
        loginWindow.Show();
    }

    private async Task InitializeDatabaseAsync()
    {
        try
        {
            // Логируем начало инициализации
            LogInfo("Starting database initialization...");

            await Task.Run(() =>
            {
                using var context = new LibraryContext();

                // Создаем базу данных если её нет
                context.Database.EnsureCreated();
                LogInfo("Database created/verified");

                // Инициализируем тестовые данные
                DatabaseInitializer.Initialize(context);
                LogInfo("Test data initialized");
            });

            LogInfo("Database initialization completed successfully");
        }
        catch (Exception ex)
        {
            LogError($"Database initialization error: {ex}");

            MessageBox.Show(
                $"Ошибка инициализации базы данных:\n{ex.Message}\n\nПроверьте файл лога: {GetLogPath()}",
                "Ошибка",
                MessageBoxButton.OK,
                MessageBoxImage.Error
            );

            // Закрываем приложение при критической ошибке
            this.Shutdown(1);
        }
    }

    private void LogInfo(string message)
    {
        try
        {
            var logPath = GetLogPath();
            var logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] INFO: {message}";
            File.AppendAllText(logPath, logMessage + Environment.NewLine);
        }
        catch
        {
            // Игнорируем ошибки логирования
        }
    }

    private void LogError(string message)
    {
        try
        {
            var logPath = GetLogPath();
            var logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] ERROR: {message}";
            File.AppendAllText(logPath, logMessage + Environment.NewLine);
        }
        catch
        {
            // Игнорируем ошибки логирования
        }
    }

    private string GetLogPath()
    {
        var appDataPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "LibraSmart"
        );
        Directory.CreateDirectory(appDataPath);
        return Path.Combine(appDataPath, "app.log");
    }
}
