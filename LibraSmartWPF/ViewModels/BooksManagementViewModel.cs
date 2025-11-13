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

public class BookManagementViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public int? PublicationYear { get; set; }
    public string GenreName { get; set; } = string.Empty;
    public int GenreId { get; set; }
    public string? Description { get; set; }
    public string? ISBN { get; set; }
    public int CopiesCount { get; set; }
}

public class BooksManagementViewModel : ViewModelBase
{
    private ObservableCollection<BookManagementViewModel> _books = new();
    private ObservableCollection<Genre> _genres = new();
    private BookManagementViewModel? _selectedBook;
    private string _searchText = string.Empty;
    private bool _isLoading;
    private bool _isEditMode;

    // Поля для редактирования/добавления
    private string _editTitle = string.Empty;
    private string _editAuthor = string.Empty;
    private string _editPublicationYear = string.Empty;
    private Genre? _editSelectedGenre;
    private string _editDescription = string.Empty;
    private string _editISBN = string.Empty;

    public ObservableCollection<BookManagementViewModel> Books
    {
        get => _books;
        set => SetProperty(ref _books, value);
    }

    public ObservableCollection<Genre> Genres
    {
        get => _genres;
        set => SetProperty(ref _genres, value);
    }

    public BookManagementViewModel? SelectedBook
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

    public bool IsEditMode
    {
        get => _isEditMode;
        set => SetProperty(ref _isEditMode, value);
    }

    public string EditTitle
    {
        get => _editTitle;
        set => SetProperty(ref _editTitle, value);
    }

    public string EditAuthor
    {
        get => _editAuthor;
        set => SetProperty(ref _editAuthor, value);
    }

    public string EditPublicationYear
    {
        get => _editPublicationYear;
        set => SetProperty(ref _editPublicationYear, value);
    }

    public Genre? EditSelectedGenre
    {
        get => _editSelectedGenre;
        set => SetProperty(ref _editSelectedGenre, value);
    }

    public string EditDescription
    {
        get => _editDescription;
        set => SetProperty(ref _editDescription, value);
    }

    public string EditISBN
    {
        get => _editISBN;
        set => SetProperty(ref _editISBN, value);
    }

    public ICommand RefreshCommand { get; }
    public ICommand AddBookCommand { get; }
    public ICommand EditBookCommand { get; }
    public ICommand DeleteBookCommand { get; }
    public ICommand SaveBookCommand { get; }
    public ICommand CancelEditCommand { get; }

    public BooksManagementViewModel()
    {
        RefreshCommand = new RelayCommand(_ => LoadBooks());
        AddBookCommand = new RelayCommand(_ => StartAddBook());
        EditBookCommand = new RelayCommand(_ => StartEditBook(), _ => SelectedBook != null);
        DeleteBookCommand = new RelayCommand(_ => DeleteBook(), _ => SelectedBook != null);
        SaveBookCommand = new RelayCommand(_ => SaveBook());
        CancelEditCommand = new RelayCommand(_ => CancelEdit());

        LoadGenres();
        LoadBooks();
    }

    private void LoadGenres()
    {
        try
        {
            using var context = new LibraryContext();
            Genres = new ObservableCollection<Genre>(context.Genres.OrderBy(g => g.GenreName).ToList());
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка загрузки жанров: {ex.Message}", "Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void LoadBooks()
    {
        if (AuthenticationService.CurrentStaff == null)
            return;

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

            var books = query.OrderBy(b => b.Title).ToList();

            Books = new ObservableCollection<BookManagementViewModel>(books.Select(b => new BookManagementViewModel
            {
                Id = b.Id,
                Title = b.Title,
                Author = b.Author,
                PublicationYear = b.PublicationYear,
                GenreName = b.Genre.GenreName,
                GenreId = b.GenreId,
                Description = b.Description,
                ISBN = b.ISBN,
                CopiesCount = b.Copies.Count
            }));
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка загрузки книг: {ex.Message}", "Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void StartAddBook()
    {
        ClearEditFields();
        IsEditMode = true;
    }

    private void StartEditBook()
    {
        if (SelectedBook == null)
            return;

        EditTitle = SelectedBook.Title;
        EditAuthor = SelectedBook.Author;
        EditPublicationYear = SelectedBook.PublicationYear?.ToString() ?? string.Empty;
        EditSelectedGenre = Genres.FirstOrDefault(g => g.Id == SelectedBook.GenreId);
        EditDescription = SelectedBook.Description ?? string.Empty;
        EditISBN = SelectedBook.ISBN ?? string.Empty;

        IsEditMode = true;
    }

    private void DeleteBook()
    {
        if (SelectedBook == null)
            return;

        var result = MessageBox.Show(
            $"Вы уверены, что хотите удалить книгу\n\"{SelectedBook.Title}\"?\n\nВНИМАНИЕ: Это также удалит все экземпляры этой книги!",
            "Подтверждение удаления",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning);

        if (result != MessageBoxResult.Yes)
            return;

        try
        {
            using var context = new LibraryContext();

            var book = context.Books
                .Include(b => b.Copies)
                .Include(b => b.Reservations)
                .FirstOrDefault(b => b.Id == SelectedBook.Id);

            if (book != null)
            {
                // Проверяем, есть ли активные бронирования или займы
                if (book.Reservations.Any(r => r.Status == "active"))
                {
                    MessageBox.Show("Невозможно удалить книгу с активными бронированиями", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                context.Books.Remove(book);
                context.SaveChanges();

                MessageBox.Show("Книга успешно удалена", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                LoadBooks();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка удаления книги: {ex.Message}", "Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void SaveBook()
    {
        if (string.IsNullOrWhiteSpace(EditTitle) || string.IsNullOrWhiteSpace(EditAuthor) || EditSelectedGenre == null)
        {
            MessageBox.Show("Заполните обязательные поля: Название, Автор, Жанр", "Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            using var context = new LibraryContext();

            Book book;
            if (SelectedBook != null)
            {
                // Редактирование существующей книги
                book = context.Books.Find(SelectedBook.Id)!;
                book.Title = EditTitle;
                book.Author = EditAuthor;
                book.GenreId = EditSelectedGenre.Id;
                book.Description = string.IsNullOrWhiteSpace(EditDescription) ? null : EditDescription;
                book.ISBN = string.IsNullOrWhiteSpace(EditISBN) ? null : EditISBN;

                if (int.TryParse(EditPublicationYear, out var year))
                    book.PublicationYear = year;
                else
                    book.PublicationYear = null;
            }
            else
            {
                // Добавление новой книги
                book = new Book
                {
                    Title = EditTitle,
                    Author = EditAuthor,
                    GenreId = EditSelectedGenre.Id,
                    Description = string.IsNullOrWhiteSpace(EditDescription) ? null : EditDescription,
                    ISBN = string.IsNullOrWhiteSpace(EditISBN) ? null : EditISBN
                };

                if (int.TryParse(EditPublicationYear, out var year))
                    book.PublicationYear = year;

                context.Books.Add(book);
            }

            context.SaveChanges();

            MessageBox.Show("Книга успешно сохранена", "Успех",
                MessageBoxButton.OK, MessageBoxImage.Information);

            IsEditMode = false;
            LoadBooks();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка сохранения книги: {ex.Message}", "Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void CancelEdit()
    {
        IsEditMode = false;
        ClearEditFields();
    }

    private void ClearEditFields()
    {
        EditTitle = string.Empty;
        EditAuthor = string.Empty;
        EditPublicationYear = string.Empty;
        EditSelectedGenre = null;
        EditDescription = string.Empty;
        EditISBN = string.Empty;
    }
}
