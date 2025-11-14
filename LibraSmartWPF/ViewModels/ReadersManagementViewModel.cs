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

public class ReaderManagementViewModel
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string LibraryCardNumber { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public int ActiveLoans { get; set; }
    public int TotalLoans { get; set; }
}

public class ReadersManagementViewModel : ViewModelBase
{
    private ObservableCollection<ReaderManagementViewModel> _readers = new();
    private ReaderManagementViewModel? _selectedReader;
    private string _searchText = string.Empty;
    private bool _isLoading;

    public ObservableCollection<ReaderManagementViewModel> Readers
    {
        get => _readers;
        set => SetProperty(ref _readers, value);
    }

    public ReaderManagementViewModel? SelectedReader
    {
        get => _selectedReader;
        set => SetProperty(ref _selectedReader, value);
    }

    public string SearchText
    {
        get => _searchText;
        set
        {
            if (SetProperty(ref _searchText, value))
            {
                LoadReaders();
            }
        }
    }

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public ICommand RefreshCommand { get; }
    public ICommand ViewReaderDetailsCommand { get; }

    public ReadersManagementViewModel()
    {
        RefreshCommand = new RelayCommand(_ => LoadReaders());
        ViewReaderDetailsCommand = new RelayCommand(ViewReaderDetails, _ => SelectedReader != null);
        LoadReaders();
    }

    private void LoadReaders()
    {
        if (AuthenticationService.CurrentStaff == null)
            return;

        IsLoading = true;
        try
        {
            using var context = new LibraryContext();

            var query = context.Readers
                .Include(r => r.Loans)
                .AsQueryable();

            // Поиск
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                query = query.Where(r =>
                    r.FullName.Contains(SearchText) ||
                    r.Email.Contains(SearchText) ||
                    r.LibraryCardNumber.Contains(SearchText) ||
                    r.Phone!.Contains(SearchText));
            }

            var readers = query.OrderBy(r => r.FullName).ToList();

            Readers = new ObservableCollection<ReaderManagementViewModel>(readers.Select(r => new ReaderManagementViewModel
            {
                Id = r.Id,
                FullName = r.FullName,
                Email = r.Email,
                Phone = r.Phone ?? "-",
                LibraryCardNumber = r.LibraryCardNumber,
                CreatedAt = r.CreatedAt,
                ActiveLoans = r.Loans.Count(l => l.Status == "active" || l.Status == "overdue"),
                TotalLoans = r.Loans.Count
            }));
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка загрузки читателей: {ex.Message}", "Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void ViewReaderDetails(object? parameter)
    {
        if (SelectedReader == null)
            return;

        try
        {
            using var context = new LibraryContext();

            var reader = context.Readers
                .Include(r => r.Loans)
                    .ThenInclude(l => l.Copy)
                        .ThenInclude(c => c.Book)
                .Include(r => r.Reservations)
                    .ThenInclude(res => res.Book)
                .FirstOrDefault(r => r.Id == SelectedReader.Id);

            if (reader == null)
                return;

            var details = $"Информация о читателе:\n\n" +
                         $"ФИО: {reader.FullName}\n" +
                         $"Email: {reader.Email}\n" +
                         $"Телефон: {reader.Phone ?? "-"}\n" +
                         $"Номер билета: {reader.LibraryCardNumber}\n" +
                         $"Дата регистрации: {reader.CreatedAt:dd.MM.yyyy}\n\n" +
                         $"Активных займов: {reader.Loans.Count(l => l.Status == "active" || l.Status == "overdue")}\n" +
                         $"Активных бронирований: {reader.Reservations.Count(r => r.Status == "active")}\n" +
                         $"Всего займов: {reader.Loans.Count}\n";

            MessageBox.Show(details, "Информация о читателе",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка получения информации: {ex.Message}", "Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
