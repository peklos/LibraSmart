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

public class LoanManagementViewModel
{
    public int Id { get; set; }
    public string ReaderName { get; set; } = string.Empty;
    public string ReaderEmail { get; set; } = string.Empty;
    public string BookTitle { get; set; } = string.Empty;
    public string BookAuthor { get; set; } = string.Empty;
    public string LibraryName { get; set; } = string.Empty;
    public DateTime LoanDate { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string StatusDisplay => Status switch
    {
        "active" => "Активен",
        "overdue" => "Просрочен",
        "returned" => "Возвращён",
        _ => Status
    };
}

public class LoansManagementViewModel : ViewModelBase
{
    private ObservableCollection<LoanManagementViewModel> _loans = new();
    private LoanManagementViewModel? _selectedLoan;
    private string _searchText = string.Empty;
    private bool _isLoading;
    private string _filterStatus = "active";

    public ObservableCollection<LoanManagementViewModel> Loans
    {
        get => _loans;
        set => SetProperty(ref _loans, value);
    }

    public LoanManagementViewModel? SelectedLoan
    {
        get => _selectedLoan;
        set => SetProperty(ref _selectedLoan, value);
    }

    public string SearchText
    {
        get => _searchText;
        set
        {
            if (SetProperty(ref _searchText, value))
            {
                LoadLoans();
            }
        }
    }

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public string FilterStatus
    {
        get => _filterStatus;
        set
        {
            if (SetProperty(ref _filterStatus, value))
            {
                LoadLoans();
            }
        }
    }

    public ICommand RefreshCommand { get; }
    public ICommand ReturnBookCommand { get; }

    public LoansManagementViewModel()
    {
        RefreshCommand = new RelayCommand(_ => LoadLoans());
        ReturnBookCommand = new RelayCommand(ReturnBook, CanReturnBook);
        LoadLoans();
    }

    private void LoadLoans()
    {
        if (AuthenticationService.CurrentStaff == null)
            return;

        IsLoading = true;
        try
        {
            using var context = new LibraryContext();

            var query = context.Loans
                .Include(l => l.Reader)
                .Include(l => l.Copy)
                    .ThenInclude(c => c.Book)
                .Include(l => l.Copy)
                    .ThenInclude(c => c.Library)
                .AsQueryable();

            // Фильтр по библиотеке сотрудника (только своя библиотека)
            query = query.Where(l => l.Copy.LibraryId == AuthenticationService.CurrentStaff.LibraryId);

            // Фильтр по статусу
            if (FilterStatus != "all")
            {
                query = query.Where(l => l.Status == FilterStatus);
            }

            // Поиск
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                query = query.Where(l =>
                    l.Reader.FullName.Contains(SearchText) ||
                    l.Reader.Email.Contains(SearchText) ||
                    l.Copy.Book.Title.Contains(SearchText) ||
                    l.Copy.Book.Author.Contains(SearchText));
            }

            var loans = query
                .OrderByDescending(l => l.LoanDate)
                .ToList();

            // Обновляем статус просроченных займов
            var now = DateTime.Now;
            foreach (var loan in loans.Where(l => l.Status == "active" && l.DueDate < now))
            {
                loan.Status = "overdue";
            }
            context.SaveChanges();

            Loans = new ObservableCollection<LoanManagementViewModel>(loans.Select(l => new LoanManagementViewModel
            {
                Id = l.Id,
                ReaderName = l.Reader.FullName,
                ReaderEmail = l.Reader.Email,
                BookTitle = l.Copy.Book.Title,
                BookAuthor = l.Copy.Book.Author,
                LibraryName = l.Copy.Library.LibraryName,
                LoanDate = l.LoanDate,
                DueDate = l.DueDate,
                ReturnDate = l.ReturnDate,
                Status = l.Status
            }));
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка загрузки займов: {ex.Message}", "Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            IsLoading = false;
        }
    }

    private bool CanReturnBook(object? parameter)
    {
        return SelectedLoan != null && (SelectedLoan.Status == "active" || SelectedLoan.Status == "overdue");
    }

    private void ReturnBook(object? parameter)
    {
        if (SelectedLoan == null)
            return;

        var result = MessageBox.Show(
            $"Подтвердите возврат книги:\n\n" +
            $"Книга: {SelectedLoan.BookTitle}\n" +
            $"Читатель: {SelectedLoan.ReaderName}\n" +
            $"Дата выдачи: {SelectedLoan.LoanDate:dd.MM.yyyy}",
            "Подтверждение возврата",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (result != MessageBoxResult.Yes)
            return;

        try
        {
            using var context = new LibraryContext();

            var loan = context.Loans
                .Include(l => l.Copy)
                .FirstOrDefault(l => l.Id == SelectedLoan.Id);

            if (loan != null)
            {
                loan.ReturnDate = DateTime.Now;
                loan.Status = "returned";

                // Обновляем статус экземпляра книги
                loan.Copy.Status = "available";

                context.SaveChanges();

                MessageBox.Show("Книга успешно возвращена", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                LoadLoans();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка возврата книги: {ex.Message}", "Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
