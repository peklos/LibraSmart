using System.Windows;
using System.Windows.Controls;
using LibraSmartWPF.ViewModels;

namespace LibraSmartWPF.Views.Staff;

public partial class LoansManagementPage : Page
{
    public LoansManagementPage()
    {
        InitializeComponent();
    }

    private void ActiveFilter_Checked(object sender, RoutedEventArgs e)
    {
        if (DataContext is LoansManagementViewModel viewModel)
        {
            viewModel.FilterStatus = "active";
        }
    }

    private void OverdueFilter_Checked(object sender, RoutedEventArgs e)
    {
        if (DataContext is LoansManagementViewModel viewModel)
        {
            viewModel.FilterStatus = "overdue";
        }
    }

    private void ReturnedFilter_Checked(object sender, RoutedEventArgs e)
    {
        if (DataContext is LoansManagementViewModel viewModel)
        {
            viewModel.FilterStatus = "returned";
        }
    }

    private void AllFilter_Checked(object sender, RoutedEventArgs e)
    {
        if (DataContext is LoansManagementViewModel viewModel)
        {
            viewModel.FilterStatus = "all";
        }
    }
}
