using System.Windows.Controls;

namespace LibraSmartWPF.Services;

public class NavigationService
{
    private Frame? _frame;
    private static NavigationService? _instance;

    public static NavigationService Instance => _instance ??= new NavigationService();

    public void SetFrame(Frame frame)
    {
        _frame = frame;
    }

    public void NavigateTo(Page page)
    {
        _frame?.Navigate(page);
    }

    public void NavigateTo(Type pageType)
    {
        if (_frame != null)
        {
            var page = Activator.CreateInstance(pageType) as Page;
            _frame.Navigate(page);
        }
    }

    public void GoBack()
    {
        if (_frame?.CanGoBack == true)
        {
            _frame.GoBack();
        }
    }

    public bool CanGoBack => _frame?.CanGoBack ?? false;
}
