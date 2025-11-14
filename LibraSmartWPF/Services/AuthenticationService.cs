using LibraSmartWPF.Data;
using LibraSmartWPF.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraSmartWPF.Services;

public class AuthenticationService
{
    private static Reader? _currentReader;
    private static Staff? _currentStaff;

    public static Reader? CurrentReader => _currentReader;
    public static Staff? CurrentStaff => _currentStaff;
    public static bool IsReader => _currentReader != null;
    public static bool IsStaff => _currentStaff != null;

    /// <summary>
    /// Вход для читателя
    /// </summary>
    public static (bool success, string? message, Reader? reader) LoginAsReader(string email, string password)
    {
        try
        {
            using var context = new LibraryContext();
            var reader = context.Readers.FirstOrDefault(r => r.Email == email);

            if (reader == null)
            {
                return (false, "Пользователь с таким email не найден", null);
            }

            if (reader.Password != password)
            {
                return (false, "Неверный пароль", null);
            }

            _currentReader = reader;
            _currentStaff = null;
            return (true, null, reader);
        }
        catch (Exception ex)
        {
            return (false, $"Ошибка входа: {ex.Message}", null);
        }
    }

    /// <summary>
    /// Вход для персонала
    /// </summary>
    public static (bool success, string? message, Staff? staff) LoginAsStaff(string email, string password)
    {
        try
        {
            using var context = new LibraryContext();
            var staff = context.Staff
                .Include(s => s.Role)
                .Include(s => s.Library)
                .FirstOrDefault(s => s.Email == email);

            if (staff == null)
            {
                return (false, "Пользователь с таким email не найден", null);
            }

            if (staff.Password != password)
            {
                return (false, "Неверный пароль", null);
            }

            _currentStaff = staff;
            _currentReader = null;
            return (true, null, staff);
        }
        catch (Exception ex)
        {
            return (false, $"Ошибка входа: {ex.Message}", null);
        }
    }

    /// <summary>
    /// Выход из системы
    /// </summary>
    public static void Logout()
    {
        _currentReader = null;
        _currentStaff = null;
    }

    /// <summary>
    /// Обновление данных текущего читателя
    /// </summary>
    public static void UpdateCurrentReader(Reader reader)
    {
        _currentReader = reader;
    }

    /// <summary>
    /// Обновление данных текущего сотрудника
    /// </summary>
    public static void UpdateCurrentStaff(Staff staff)
    {
        _currentStaff = staff;
    }

    /// <summary>
    /// Регистрация нового читателя
    /// </summary>
    public static (bool success, string? message) RegisterReader(string fullName, string email, string password, string phone)
    {
        try
        {
            using var context = new LibraryContext();

            // Проверяем, существует ли уже пользователь с таким email
            if (context.Readers.Any(r => r.Email == email))
            {
                return (false, "Пользователь с таким email уже существует");
            }

            // Генерируем номер читательского билета
            var lastCardNumber = context.Readers
                .OrderByDescending(r => r.Id)
                .Select(r => r.LibraryCardNumber)
                .FirstOrDefault();

            var cardNumber = GenerateCardNumber(lastCardNumber);

            // Создаем нового читателя
            var reader = new Reader
            {
                FullName = fullName,
                Email = email,
                Password = password,
                Phone = phone,
                LibraryCardNumber = cardNumber,
                CreatedAt = DateTime.UtcNow
            };

            context.Readers.Add(reader);
            context.SaveChanges();

            return (true, null);
        }
        catch (Exception ex)
        {
            return (false, $"Ошибка регистрации: {ex.Message}");
        }
    }

    private static string GenerateCardNumber(string? lastCardNumber)
    {
        var year = DateTime.Now.Year;
        var prefix = $"LIB-{year}-";

        if (string.IsNullOrEmpty(lastCardNumber))
        {
            return $"{prefix}001";
        }

        // Извлекаем последний номер
        var parts = lastCardNumber.Split('-');
        if (parts.Length == 3 && int.TryParse(parts[2], out var lastNumber))
        {
            return $"{prefix}{(lastNumber + 1):D3}";
        }

        return $"{prefix}001";
    }
}
