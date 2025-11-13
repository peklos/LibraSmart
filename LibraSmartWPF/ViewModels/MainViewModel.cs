using System.Windows;
using System.Windows.Input;
using LibraSmartWPF.Commands;
using LibraSmartWPF.Helpers;
using LibraSmartWPF.Services;
using LibraSmartWPF.Views;

namespace LibraSmartWPF.ViewModels;

public class MainViewModel : ViewModelBase
{
    private string _currentUserName = string.Empty;
    private string _currentUserRole = string.Empty;

    public string CurrentUserName
    {
        get => _currentUserName;
        set => SetProperty(ref _currentUserName, value);
    }

    public string CurrentUserRole
    {
        get => _currentUserRole;
        set => SetProperty(ref _currentUserRole, value);
    }

    public bool IsReader => AuthenticationService.IsReader;
    public bool IsStaff => AuthenticationService.IsStaff;

    public ICommand LogoutCommand { get; }

    public MainViewModel()
    {
        LogoutCommand = new RelayCommand(Logout);
        LoadCurrentUser();
    }

    private void LoadCurrentUser()
    {
        if (AuthenticationService.CurrentReader != null)
        {
            CurrentUserName = AuthenticationService.CurrentReader.FullName;
            CurrentUserRole = "Читатель";
        }
        else if (AuthenticationService.CurrentStaff != null)
        {
            CurrentUserName = AuthenticationService.CurrentStaff.FullName;
            CurrentUserRole = AuthenticationService.CurrentStaff.Role.Name;
        }
    }

    private void Logout(object? parameter)
    {
        AuthenticationService.Logout();

        var loginWindow = new LoginWindow();
        loginWindow.Show();

        Application.Current.Windows.OfType<MainWindow>().FirstOrDefault()?.Close();
    }
}
