using System.Windows;
using System.Windows.Input;
using LibraSmartWPF.Commands;
using LibraSmartWPF.Helpers;
using LibraSmartWPF.Services;
using LibraSmartWPF.Views;

namespace LibraSmartWPF.ViewModels;

public class LoginViewModel : ViewModelBase
{
    private string _email = string.Empty;
    private string _password = string.Empty;
    private bool _isReaderMode = true;
    private string _errorMessage = string.Empty;

    public string Email
    {
        get => _email;
        set => SetProperty(ref _email, value);
    }

    public string Password
    {
        get => _password;
        set => SetProperty(ref _password, value);
    }

    public bool IsReaderMode
    {
        get => _isReaderMode;
        set
        {
            if (SetProperty(ref _isReaderMode, value))
            {
                OnPropertyChanged(nameof(ModeText));
                OnPropertyChanged(nameof(SwitchModeText));
            }
        }
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    public string ModeText => IsReaderMode ? "Вход для читателей" : "Вход для персонала";
    public string SwitchModeText => IsReaderMode ? "Войти как персонал" : "Войти как читатель";

    public ICommand LoginCommand { get; }
    public ICommand SwitchModeCommand { get; }
    public ICommand RegisterCommand { get; }

    public LoginViewModel()
    {
        LoginCommand = new RelayCommand(Login, CanLogin);
        SwitchModeCommand = new RelayCommand(_ => IsReaderMode = !IsReaderMode);
        RegisterCommand = new RelayCommand(Register, _ => IsReaderMode);
    }

    private bool CanLogin(object? parameter)
    {
        return !string.IsNullOrWhiteSpace(Email) && !string.IsNullOrWhiteSpace(Password);
    }

    private void Login(object? parameter)
    {
        ErrorMessage = string.Empty;

        if (IsReaderMode)
        {
            var (success, message, reader) = AuthenticationService.LoginAsReader(Email, Password);
            if (success && reader != null)
            {
                // Открываем главное окно для читателя
                var mainWindow = new MainWindow();
                mainWindow.Show();
                Application.Current.Windows.OfType<LoginWindow>().FirstOrDefault()?.Close();
            }
            else
            {
                ErrorMessage = message ?? "Ошибка входа";
            }
        }
        else
        {
            var (success, message, staff) = AuthenticationService.LoginAsStaff(Email, Password);
            if (success && staff != null)
            {
                // Открываем главное окно для персонала
                var mainWindow = new MainWindow();
                mainWindow.Show();
                Application.Current.Windows.OfType<LoginWindow>().FirstOrDefault()?.Close();
            }
            else
            {
                ErrorMessage = message ?? "Ошибка входа";
            }
        }
    }

    private void Register(object? parameter)
    {
        // TODO: Открыть окно регистрации
        MessageBox.Show("Регистрация пока не реализована", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
    }
}
