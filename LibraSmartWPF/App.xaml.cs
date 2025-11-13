using System.Windows;
using LibraSmartWPF.Data;
using Microsoft.EntityFrameworkCore;

namespace LibraSmartWPF;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // Инициализация базы данных при старте
        InitializeDatabase();
    }

    private void InitializeDatabase()
    {
        try
        {
            using var context = new LibraryContext();

            // Создаем базу данных если её нет
            context.Database.EnsureCreated();

            // Инициализируем тестовые данные
            DatabaseInitializer.Initialize(context);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Ошибка инициализации базы данных:\n{ex.Message}",
                "Ошибка",
                MessageBoxButton.OK,
                MessageBoxImage.Error
            );
        }
    }
}
