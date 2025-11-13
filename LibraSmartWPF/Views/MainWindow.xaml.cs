using System.Windows;
using System.Windows.Controls;
using LibraSmartWPF.Views.Reader;
using LibraSmartWPF.Views.Staff;

namespace LibraSmartWPF.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        Loaded += MainWindow_Loaded;
    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        // Устанавливаем начальную страницу
        if (MenuListBox.Items.Count > 0)
        {
            MenuListBox.SelectedIndex = 0;
        }
    }

    private void MenuListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (MenuListBox.SelectedItem == null)
            return;

        Page? page = null;

        // Страницы для читателей
        if (MenuListBox.SelectedItem == CatalogMenuItem)
        {
            page = new CatalogPage();
        }
        else if (MenuListBox.SelectedItem == MyLoansMenuItem)
        {
            page = new MyLoansPage();
        }
        else if (MenuListBox.SelectedItem == MyReservationsMenuItem)
        {
            page = new MyReservationsPage();
        }
        // Страницы для персонала
        else if (MenuListBox.SelectedItem == BooksManagementMenuItem)
        {
            page = new BooksManagementPage();
        }
        else if (MenuListBox.SelectedItem == LoansManagementMenuItem)
        {
            page = new LoansManagementPage();
        }
        else if (MenuListBox.SelectedItem == ReadersManagementMenuItem)
        {
            page = new ReadersManagementPage();
        }
        else if (MenuListBox.SelectedItem == LibrariesMenuItem)
        {
            page = new LibrariesPage();
        }

        if (page != null)
        {
            ContentFrame.Navigate(page);
        }

        // Закрываем меню после выбора
        MenuToggleButton.IsChecked = false;
    }
}
