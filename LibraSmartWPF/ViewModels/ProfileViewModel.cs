using System.Windows;
using System.Windows.Input;
using LibraSmartWPF.Commands;
using LibraSmartWPF.Data;
using LibraSmartWPF.Helpers;
using LibraSmartWPF.Services;

namespace LibraSmartWPF.ViewModels;

public class ProfileViewModel : ViewModelBase
{
    private string _fullName = string.Empty;
    private string _email = string.Empty;
    private string _phone = string.Empty;
    private string _libraryCardNumber = string.Empty;
    private string _currentPassword = string.Empty;
    private string _newPassword = string.Empty;
    private string _confirmPassword = string.Empty;
    private bool _isEditMode;
    private int _totalLoans;
    private int _activeLoans;
    private int _totalReservations;
    private int _activeReservations;

    public string FullName
    {
        get => _fullName;
        set => SetProperty(ref _fullName, value);
    }

    public string Email
    {
        get => _email;
        set => SetProperty(ref _email, value);
    }

    public string Phone
    {
        get => _phone;
        set => SetProperty(ref _phone, value);
    }

    public string LibraryCardNumber
    {
        get => _libraryCardNumber;
        set => SetProperty(ref _libraryCardNumber, value);
    }

    public string CurrentPassword
    {
        get => _currentPassword;
        set => SetProperty(ref _currentPassword, value);
    }

    public string NewPassword
    {
        get => _newPassword;
        set => SetProperty(ref _newPassword, value);
    }

    public string ConfirmPassword
    {
        get => _confirmPassword;
        set => SetProperty(ref _confirmPassword, value);
    }

    public bool IsEditMode
    {
        get => _isEditMode;
        set => SetProperty(ref _isEditMode, value);
    }

    public int TotalLoans
    {
        get => _totalLoans;
        set => SetProperty(ref _totalLoans, value);
    }

    public int ActiveLoans
    {
        get => _activeLoans;
        set => SetProperty(ref _activeLoans, value);
    }

    public int TotalReservations
    {
        get => _totalReservations;
        set => SetProperty(ref _totalReservations, value);
    }

    public int ActiveReservations
    {
        get => _activeReservations;
        set => SetProperty(ref _activeReservations, value);
    }

    public ICommand EditProfileCommand { get; }
    public ICommand SaveProfileCommand { get; }
    public ICommand CancelEditCommand { get; }
    public ICommand ChangePasswordCommand { get; }
    public ICommand RefreshCommand { get; }

    public ProfileViewModel()
    {
        EditProfileCommand = new RelayCommand(_ => StartEdit());
        SaveProfileCommand = new RelayCommand(_ => SaveProfile());
        CancelEditCommand = new RelayCommand(_ => CancelEdit());
        ChangePasswordCommand = new RelayCommand(_ => ChangePassword());
        RefreshCommand = new RelayCommand(_ => LoadProfile());

        LoadProfile();
    }

    private void LoadProfile()
    {
        if (AuthenticationService.CurrentReader == null)
        {
            MessageBox.Show("Необходимо авторизоваться", "Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            using var context = new LibraryContext();
            var reader = context.Readers.Find(AuthenticationService.CurrentReader.Id);

            if (reader != null)
            {
                FullName = reader.FullName;
                Email = reader.Email;
                Phone = reader.Phone ?? string.Empty;
                LibraryCardNumber = reader.LibraryCardNumber;

                // Загружаем статистику
                var loans = context.Loans.Where(l => l.ReaderId == reader.Id).ToList();
                TotalLoans = loans.Count;
                ActiveLoans = loans.Count(l => l.Status == "active" || l.Status == "overdue");

                var reservations = context.Reservations.Where(r => r.ReaderId == reader.Id).ToList();
                TotalReservations = reservations.Count;
                ActiveReservations = reservations.Count(r => r.Status == "active");
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка загрузки профиля: {ex.Message}", "Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void StartEdit()
    {
        IsEditMode = true;
    }

    private void SaveProfile()
    {
        if (string.IsNullOrWhiteSpace(FullName) || string.IsNullOrWhiteSpace(Email))
        {
            MessageBox.Show("Заполните обязательные поля: ФИО и Email", "Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (AuthenticationService.CurrentReader == null)
            return;

        try
        {
            using var context = new LibraryContext();
            var reader = context.Readers.Find(AuthenticationService.CurrentReader.Id);

            if (reader != null)
            {
                reader.FullName = FullName;
                reader.Email = Email;
                reader.Phone = string.IsNullOrWhiteSpace(Phone) ? null : Phone;

                context.SaveChanges();

                // Обновляем текущего пользователя
                AuthenticationService.UpdateCurrentReader(reader);

                MessageBox.Show("Профиль успешно обновлён", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                IsEditMode = false;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка сохранения профиля: {ex.Message}", "Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void CancelEdit()
    {
        IsEditMode = false;
        LoadProfile();
        CurrentPassword = string.Empty;
        NewPassword = string.Empty;
        ConfirmPassword = string.Empty;
    }

    private void ChangePassword()
    {
        if (string.IsNullOrWhiteSpace(CurrentPassword) ||
            string.IsNullOrWhiteSpace(NewPassword) ||
            string.IsNullOrWhiteSpace(ConfirmPassword))
        {
            MessageBox.Show("Заполните все поля для смены пароля", "Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (NewPassword != ConfirmPassword)
        {
            MessageBox.Show("Новый пароль и подтверждение не совпадают", "Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (AuthenticationService.CurrentReader == null)
            return;

        try
        {
            using var context = new LibraryContext();
            var reader = context.Readers.Find(AuthenticationService.CurrentReader.Id);

            if (reader != null)
            {
                if (reader.Password != CurrentPassword)
                {
                    MessageBox.Show("Текущий пароль неверен", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                reader.Password = NewPassword;
                context.SaveChanges();

                MessageBox.Show("Пароль успешно изменён", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                CurrentPassword = string.Empty;
                NewPassword = string.Empty;
                ConfirmPassword = string.Empty;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка смены пароля: {ex.Message}", "Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
