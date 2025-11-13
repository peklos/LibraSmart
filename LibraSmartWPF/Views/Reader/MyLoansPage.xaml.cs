using System.Windows;
using System.Windows.Controls;
using LibraSmartWPF.ViewModels;

namespace LibraSmartWPF.Views.Reader;

public partial class MyLoansPage : Page
{
    public MyLoansPage()
    {
        InitializeComponent();
    }

    private void ActiveFilter_Checked(object sender, RoutedEventArgs e)
    {
        if (DataContext is MyLoansViewModel viewModel)
        {
            viewModel.FilterStatus = "active";
        }
    }

    private void ReturnedFilter_Checked(object sender, RoutedEventArgs e)
    {
        if (DataContext is MyLoansViewModel viewModel)
        {
            viewModel.FilterStatus = "returned";
        }
    }
}
