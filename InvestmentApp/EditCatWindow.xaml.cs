using InvestmentApp.Handlers;
using InvestmentApp.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace InvestmentApp
{
    public partial class EditCatWindow : Window
    {
        private ObservableCollection<Category> Categories;
        private Category MostraTutto = new Category { Name = "Mostra tutto" };

        public EditCatWindow()
        {
            InitializeComponent();

            try
            {
                Categories = new(JsonHandler.ReadCategory());
                Categories.CollectionChanged += Categories_CollectionChanged;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading categories: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Categories = new ObservableCollection<Category>();
            }

            ListViewCat.ItemsSource = Categories;
        }

        private void Categories_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            ListViewCat.Dispatcher.Invoke(() => ListViewCat.ItemsSource = Categories);
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            Category row = (Category)ListViewCat.SelectedItem;
            if (row != null)
            {
                Categories.Remove(row);
                JsonHandler.WriteCategoryAsync(Categories);
            }
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            NewCatWindow newCatWindow = new();
            newCatWindow.Owner = this;
            if (newCatWindow.ShowDialog() == true && newCatWindow.Cat != null)
            {
                Categories.Add(newCatWindow.Cat);
                Categories.Remove(MostraTutto);
                var tmpCategories = Categories.OrderBy(i => i.Name);
                Categories = new(tmpCategories);
                JsonHandler.WriteCategoryAsync(Categories);
                ListViewCat.Dispatcher.Invoke(() => ListViewCat.ItemsSource = Categories);
            }
        }
    }
}
