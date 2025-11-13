using System.Windows.Controls;
using LibraSmartWPF.ViewModels;

namespace LibraSmartWPF.Views.Reader;

public partial class HistoryPage : Page
{
    public HistoryPage()
    {
        InitializeComponent();
    }

    private void AllFilter_Checked(object sender, System.Windows.RoutedEventArgs e)
    {
        if (DataContext is HistoryViewModel vm)
        {
            vm.FilterType = "all";
        }
    }

    private void LoansFilter_Checked(object sender, System.Windows.RoutedEventArgs e)
    {
        if (DataContext is HistoryViewModel vm)
        {
            vm.FilterType = "loans";
        }
    }

    private void ReservationsFilter_Checked(object sender, System.Windows.RoutedEventArgs e)
    {
        if (DataContext is HistoryViewModel vm)
        {
            vm.FilterType = "reservations";
        }
    }
}
