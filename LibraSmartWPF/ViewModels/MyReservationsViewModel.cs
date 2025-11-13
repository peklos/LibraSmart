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

public class ReservationViewModel
{
    public int Id { get; set; }
    public string BookTitle { get; set; } = string.Empty;
    public string BookAuthor { get; set; } = string.Empty;
    public string GenreName { get; set; } = string.Empty;
    public string LibraryName { get; set; } = string.Empty;
    public DateTime ReservationDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string StatusDisplay => Status switch
    {
        "active" => "Активно",
        "completed" => "Выполнено",
        "cancelled" => "Отменено",
        _ => Status
    };
}

public class MyReservationsViewModel : ViewModelBase
{
    private ObservableCollection<ReservationViewModel> _reservations = new();
    private ReservationViewModel? _selectedReservation;
    private bool _isLoading;

    public ObservableCollection<ReservationViewModel> Reservations
    {
        get => _reservations;
        set => SetProperty(ref _reservations, value);
    }

    public ReservationViewModel? SelectedReservation
    {
        get => _selectedReservation;
        set => SetProperty(ref _selectedReservation, value);
    }

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public ICommand RefreshCommand { get; }
    public ICommand CancelReservationCommand { get; }

    public MyReservationsViewModel()
    {
        RefreshCommand = new RelayCommand(_ => LoadReservations());
        CancelReservationCommand = new RelayCommand(CancelReservation, CanCancelReservation);
        LoadReservations();
    }

    private void LoadReservations()
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

            var reservations = context.Reservations
                .Include(r => r.Book)
                    .ThenInclude(b => b.Genre)
                .Include(r => r.Library)
                .Where(r => r.ReaderId == AuthenticationService.CurrentReader.Id)
                .OrderByDescending(r => r.ReservationDate)
                .ToList();

            Reservations = new ObservableCollection<ReservationViewModel>(reservations.Select(r => new ReservationViewModel
            {
                Id = r.Id,
                BookTitle = r.Book.Title,
                BookAuthor = r.Book.Author,
                GenreName = r.Book.Genre.GenreName,
                LibraryName = r.Library.LibraryName,
                ReservationDate = r.ReservationDate,
                Status = r.Status
            }));
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка загрузки бронирований: {ex.Message}", "Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            IsLoading = false;
        }
    }

    private bool CanCancelReservation(object? parameter)
    {
        return SelectedReservation != null && SelectedReservation.Status == "active";
    }

    private void CancelReservation(object? parameter)
    {
        if (SelectedReservation == null)
            return;

        var result = MessageBox.Show(
            $"Вы уверены, что хотите отменить бронирование книги\n\"{SelectedReservation.BookTitle}\"?",
            "Подтверждение отмены",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (result != MessageBoxResult.Yes)
            return;

        try
        {
            using var context = new LibraryContext();

            var reservation = context.Reservations.Find(SelectedReservation.Id);
            if (reservation != null)
            {
                reservation.Status = "cancelled";
                context.SaveChanges();

                MessageBox.Show("Бронирование успешно отменено", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                LoadReservations();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка отмены бронирования: {ex.Message}", "Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
