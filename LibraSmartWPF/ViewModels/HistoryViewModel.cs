using System.Collections.ObjectModel;
using System.Windows.Input;
using LibraSmartWPF.Commands;
using LibraSmartWPF.Data;
using LibraSmartWPF.Helpers;
using LibraSmartWPF.Services;
using Microsoft.EntityFrameworkCore;

namespace LibraSmartWPF.ViewModels;

public class HistoryItemViewModel
{
    public string Type { get; set; } = string.Empty; // "loan" или "reservation"
    public string TypeDisplay => Type == "loan" ? "Займ" : "Бронирование";
    public string BookTitle { get; set; } = string.Empty;
    public string BookAuthor { get; set; } = string.Empty;
    public string LibraryName { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string StatusDisplay => Status switch
    {
        "active" => "Активно",
        "overdue" => "Просрочено",
        "returned" => "Возвращено",
        "completed" => "Выполнено",
        "cancelled" => "Отменено",
        _ => Status
    };
}

public class HistoryViewModel : ViewModelBase
{
    private ObservableCollection<HistoryItemViewModel> _items = new();
    private HistoryItemViewModel? _selectedItem;
    private bool _isLoading;
    private string _filterType = "all"; // all, loans, reservations
    private string _searchText = string.Empty;

    public ObservableCollection<HistoryItemViewModel> Items
    {
        get => _items;
        set => SetProperty(ref _items, value);
    }

    public HistoryItemViewModel? SelectedItem
    {
        get => _selectedItem;
        set => SetProperty(ref _selectedItem, value);
    }

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public string FilterType
    {
        get => _filterType;
        set
        {
            if (SetProperty(ref _filterType, value))
            {
                LoadHistory();
            }
        }
    }

    public string SearchText
    {
        get => _searchText;
        set
        {
            if (SetProperty(ref _searchText, value))
            {
                LoadHistory();
            }
        }
    }

    public ICommand RefreshCommand { get; }

    public HistoryViewModel()
    {
        RefreshCommand = new RelayCommand(_ => LoadHistory());
        LoadHistory();
    }

    private void LoadHistory()
    {
        if (AuthenticationService.CurrentReader == null)
            return;

        IsLoading = true;
        try
        {
            using var context = new LibraryContext();
            var items = new List<HistoryItemViewModel>();

            // Загружаем займы
            if (FilterType == "all" || FilterType == "loans")
            {
                var loans = context.Loans
                    .Include(l => l.Copy)
                        .ThenInclude(c => c.Book)
                    .Include(l => l.Copy)
                        .ThenInclude(c => c.Library)
                    .Where(l => l.ReaderId == AuthenticationService.CurrentReader.Id)
                    .ToList();

                foreach (var loan in loans)
                {
                    var item = new HistoryItemViewModel
                    {
                        Type = "loan",
                        BookTitle = loan.Copy.Book.Title,
                        BookAuthor = loan.Copy.Book.Author,
                        LibraryName = loan.Copy.Library.LibraryName,
                        Date = loan.LoanDate,
                        DueDate = loan.DueDate,
                        ReturnDate = loan.ReturnDate,
                        Status = loan.Status
                    };

                    if (string.IsNullOrWhiteSpace(SearchText) ||
                        item.BookTitle.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                        item.BookAuthor.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                    {
                        items.Add(item);
                    }
                }
            }

            // Загружаем бронирования
            if (FilterType == "all" || FilterType == "reservations")
            {
                var reservations = context.Reservations
                    .Include(r => r.Book)
                    .Include(r => r.Library)
                    .Where(r => r.ReaderId == AuthenticationService.CurrentReader.Id)
                    .ToList();

                foreach (var reservation in reservations)
                {
                    var item = new HistoryItemViewModel
                    {
                        Type = "reservation",
                        BookTitle = reservation.Book.Title,
                        BookAuthor = reservation.Book.Author,
                        LibraryName = reservation.Library.LibraryName,
                        Date = reservation.ReservationDate,
                        Status = reservation.Status
                    };

                    if (string.IsNullOrWhiteSpace(SearchText) ||
                        item.BookTitle.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                        item.BookAuthor.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                    {
                        items.Add(item);
                    }
                }
            }

            // Сортируем по дате (новые сверху)
            Items = new ObservableCollection<HistoryItemViewModel>(
                items.OrderByDescending(i => i.Date));
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show($"Ошибка загрузки истории: {ex.Message}", "Ошибка",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
        }
        finally
        {
            IsLoading = false;
        }
    }
}
