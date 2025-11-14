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

public class LoanViewModel
{
    public int Id { get; set; }
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
    public int DaysLeft => (DueDate - DateTime.Now).Days;
    public string DaysLeftDisplay
    {
        get
        {
            if (Status == "returned") return "Возвращено";
            if (DaysLeft < 0) return $"Просрочено на {-DaysLeft} дн.";
            if (DaysLeft == 0) return "Сегодня последний день";
            return $"Осталось {DaysLeft} дн.";
        }
    }
}

public class MyLoansViewModel : ViewModelBase
{
    private ObservableCollection<LoanViewModel> _loans = new();
    private LoanViewModel? _selectedLoan;
    private bool _isLoading;
    private string _filterStatus = "all";

    public ObservableCollection<LoanViewModel> Loans
    {
        get => _loans;
        set => SetProperty(ref _loans, value);
    }

    public LoanViewModel? SelectedLoan
    {
        get => _selectedLoan;
        set => SetProperty(ref _selectedLoan, value);
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

    public MyLoansViewModel()
    {
        RefreshCommand = new RelayCommand(_ => LoadLoans());
        LoadLoans();
    }

    private void LoadLoans()
    {
        if (AuthenticationService.CurrentReader == null)
        {
            MessageBox.Show("Необходимо авторизоваться как читатель", "Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        IsLoading = true;
        try
        {
            using var context = new LibraryContext();

            var query = context.Loans
                .Include(l => l.Copy)
                    .ThenInclude(c => c.Book)
                .Include(l => l.Copy)
                    .ThenInclude(c => c.Library)
                .Where(l => l.ReaderId == AuthenticationService.CurrentReader.Id)
                .AsQueryable();

            // Фильтрация по статусу
            if (FilterStatus != "all")
            {
                query = query.Where(l => l.Status == FilterStatus);
            }

            var loans = query
                .OrderByDescending(l => l.LoanDate)
                .ToList();

            Loans = new ObservableCollection<LoanViewModel>(loans.Select(l => new LoanViewModel
            {
                Id = l.Id,
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
}
