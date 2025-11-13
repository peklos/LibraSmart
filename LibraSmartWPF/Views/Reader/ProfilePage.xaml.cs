using System.Windows.Controls;
using LibraSmartWPF.ViewModels;

namespace LibraSmartWPF.Views.Reader;

public partial class ProfilePage : Page
{
    public ProfilePage()
    {
        InitializeComponent();
    }

    private void CurrentPasswordBox_PasswordChanged(object sender, System.Windows.RoutedEventArgs e)
    {
        if (DataContext is ProfileViewModel vm && sender is PasswordBox pb)
        {
            vm.CurrentPassword = pb.Password;
        }
    }

    private void NewPasswordBox_PasswordChanged(object sender, System.Windows.RoutedEventArgs e)
    {
        if (DataContext is ProfileViewModel vm && sender is PasswordBox pb)
        {
            vm.NewPassword = pb.Password;
        }
    }

    private void ConfirmPasswordBox_PasswordChanged(object sender, System.Windows.RoutedEventArgs e)
    {
        if (DataContext is ProfileViewModel vm && sender is PasswordBox pb)
        {
            vm.ConfirmPassword = pb.Password;
        }
    }
}
