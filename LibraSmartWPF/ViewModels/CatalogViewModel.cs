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

public class BookViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public int? PublicationYear { get; set; }
    public string GenreName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int AvailableCopies { get; set; }
    public int TotalCopies { get; set; }
}

public class CatalogViewModel : ViewModelBase
{
    private ObservableCollection<BookViewModel> _books = new();
    private BookViewModel? _selectedBook;
    private string _searchText = string.Empty;
    private bool _isLoading;

    public ObservableCollection<BookViewModel> Books
    {
        get => _books;
        set => SetProperty(ref _books, value);
    }

    public BookViewModel? SelectedBook
    {
        get => _selectedBook;
        set => SetProperty(ref _selectedBook, value);
    }

    public string SearchText
    {
        get => _searchText;
        set
        {
            if (SetProperty(ref _searchText, value))
            {
                LoadBooks();
            }
        }
    }

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public ICommand ReserveCommand { get; }
    public ICommand RefreshCommand { get; }

    public CatalogViewModel()
    {
        ReserveCommand = new RelayCommand(Reserve, CanReserve);
        RefreshCommand = new RelayCommand(_ => LoadBooks());
        LoadBooks();
    }

    private void LoadBooks()
    {
        IsLoading = true;
        try
        {
            using var context = new LibraryContext();

            var query = context.Books
                .Include(b => b.Genre)
                .Include(b => b.Copies)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                query = query.Where(b =>
                    b.Title.Contains(SearchText) ||
                    b.Author.Contains(SearchText) ||
                    b.Genre.GenreName.Contains(SearchText));
            }

            var books = query.ToList();

            Books = new ObservableCollection<BookViewModel>(books.Select(b => new BookViewModel
            {
                Id = b.Id,
                Title = b.Title,
                Author = b.Author,
                PublicationYear = b.PublicationYear,
                GenreName = b.Genre.GenreName,
                Description = b.Description,
                TotalCopies = b.Copies.Count,
                AvailableCopies = b.Copies.Count(c => c.Status == "available")
            }));
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка загрузки каталога: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            IsLoading = false;
        }
    }

    private bool CanReserve(object? parameter)
    {
        return SelectedBook != null && SelectedBook.AvailableCopies > 0 && AuthenticationService.CurrentReader != null;
    }

    private void Reserve(object? parameter)
    {
        if (SelectedBook == null || AuthenticationService.CurrentReader == null)
            return;

        try
        {
            using var context = new LibraryContext();

            // Проверяем, нет ли уже активного бронирования
            var existingReservation = context.Reservations
                .FirstOrDefault(r => r.ReaderId == AuthenticationService.CurrentReader.Id &&
                                   r.BookId == SelectedBook.Id &&
                                   r.Status == "active");

            if (existingReservation != null)
            {
                MessageBox.Show("У вас уже есть активное бронирование этой книги", "Информация",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // Создаем бронирование (для первой доступной библиотеки с копией)
            var availableCopy = context.BookCopies
                .Include(bc => bc.Library)
                .FirstOrDefault(bc => bc.BookId == SelectedBook.Id && bc.Status == "available");

            if (availableCopy == null)
            {
                MessageBox.Show("К сожалению, все экземпляры книги заняты", "Информация",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var reservation = new Reservation
            {
                ReaderId = AuthenticationService.CurrentReader.Id,
                BookId = SelectedBook.Id,
                LibraryId = availableCopy.LibraryId,
                ReservationDate = DateTime.UtcNow,
                Status = "active"
            };

            context.Reservations.Add(reservation);
            context.SaveChanges();

            MessageBox.Show($"Книга успешно забронирована!\nБиблиотека: {availableCopy.Library.LibraryName}",
                "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

            LoadBooks();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка бронирования: {ex.Message}", "Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
