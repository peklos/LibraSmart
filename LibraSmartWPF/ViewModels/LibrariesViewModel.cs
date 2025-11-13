using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using LibraSmartWPF.Commands;
using LibraSmartWPF.Data;
using LibraSmartWPF.Helpers;
using LibraSmartWPF.Models;
using LibraSmartWPF.Services;
using Microsoft.EntityFrameworkCore;

namespace LibraSmartWPF.ViewModels;

public class LibraryViewModel
{
    public int Id { get; set; }
    public string LibraryName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public int StaffCount { get; set; }
    public int BooksCount { get; set; }
    public int ActiveLoansCount { get; set; }
}

public class LibrariesViewModel : ViewModelBase
{
    private ObservableCollection<LibraryViewModel> _libraries = new();
    private LibraryViewModel? _selectedLibrary;
    private bool _isLoading;

    public ObservableCollection<LibraryViewModel> Libraries
    {
        get => _libraries;
        set => SetProperty(ref _libraries, value);
    }

    public LibraryViewModel? SelectedLibrary
    {
        get => _selectedLibrary;
        set => SetProperty(ref _selectedLibrary, value);
    }

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public ICommand RefreshCommand { get; }
    public ICommand ViewLibraryDetailsCommand { get; }

    public LibrariesViewModel()
    {
        RefreshCommand = new RelayCommand(_ => LoadLibraries());
        ViewLibraryDetailsCommand = new RelayCommand(ViewLibraryDetails, _ => SelectedLibrary != null);
        LoadLibraries();
    }

    private void LoadLibraries()
    {
        if (AuthenticationService.CurrentStaff == null)
            return;

        IsLoading = true;
        try
        {
            using var context = new LibraryContext();

            var libraries = context.Libraries
                .Include(l => l.StaffMembers)
                .Include(l => l.BookCopies)
                    .ThenInclude(bc => bc.Loans)
                .ToList();

            Libraries = new ObservableCollection<LibraryViewModel>(libraries.Select(l => new LibraryViewModel
            {
                Id = l.Id,
                LibraryName = l.LibraryName,
                Address = l.Address,
                Phone = l.Phone,
                StaffCount = l.StaffMembers.Count,
                BooksCount = l.BookCopies.Count,
                ActiveLoansCount = l.BookCopies.SelectMany(bc => bc.Loans)
                    .Count(loan => loan.Status == "active" || loan.Status == "overdue")
            }));
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка загрузки библиотек: {ex.Message}", "Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void ViewLibraryDetails(object? parameter)
    {
        if (SelectedLibrary == null)
            return;

        try
        {
            using var context = new LibraryContext();

            var library = context.Libraries
                .Include(l => l.StaffMembers)
                    .ThenInclude(s => s.Role)
                .Include(l => l.BookCopies)
                    .ThenInclude(bc => bc.Book)
                .FirstOrDefault(l => l.Id == SelectedLibrary.Id);

            if (library == null)
                return;

            var availableBooks = library.BookCopies.Count(bc => bc.Status == "available");
            var onLoanBooks = library.BookCopies.Count(bc => bc.Status == "on_loan");

            var staffList = string.Join("\n", library.StaffMembers.Select(s =>
                $"  - {s.FullName} ({s.Role.Name})"));

            var details = $"Информация о библиотеке:\n\n" +
                         $"Название: {library.LibraryName}\n" +
                         $"Адрес: {library.Address}\n" +
                         $"Телефон: {library.Phone}\n\n" +
                         $"Статистика:\n" +
                         $"  Сотрудников: {library.StaffMembers.Count}\n" +
                         $"  Всего экземпляров книг: {library.BookCopies.Count}\n" +
                         $"  Доступно: {availableBooks}\n" +
                         $"  На руках: {onLoanBooks}\n\n" +
                         $"Сотрудники:\n{staffList}";

            MessageBox.Show(details, "Информация о библиотеке",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка получения информации: {ex.Message}", "Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
