using InvestmentApp.Handlers;
using InvestmentApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace InvestmentApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ObservableCollection<Item> Items;
        private ObservableCollection<Category> Categories;
        private readonly Category MostraTutto = new() { Name = "Mostra tutto" };

        public MainWindow()
        {
            InitializeComponent();

            try
            {
                Categories = new(JsonHandler.ReadCategory());
                Categories.CollectionChanged += Categories_CollectionChanged;
            }
            catch (Exception)
            { Categories = new(); }

            try
            { Items = new(JsonHandler.ReadItems()); }
            catch (Exception)
            { Items = new(); }

            ComboBoxCats.ItemsSource = Categories;
            Categories.Insert(0, MostraTutto);
            ComboBoxCats.SelectedIndex = 0;

            MainDataGrid.ItemsSource = Items;
        }

        private void Categories_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            ComboBoxCats.ItemsSource = Categories;
        }

        /// <summary>
        /// Elimina elemento dalla datagridview (contextmenu)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Item row = (Item)MainDataGrid.SelectedItem;
            if (row != null)
            {
                Items.Remove(row);
                MainDataGrid.ItemsSource = Items;
                JsonHandler.WriteItems(Items);
            }
        }

        /// <summary>
        /// Aggiunge l'oggetto alla lista e ne aggiorna i prezzi in base a steam
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ButtonAddItem_Click(object sender, RoutedEventArgs e)
        {
            AddWindow addWindow = new();
            addWindow.Owner = this;

            if (addWindow.ShowDialog() == true && addWindow.Item != null)
            {
                IEnumerable<Item> queryItems = Items.Where(item => item.Name == addWindow.Item.Name &&
                                                           item.Price == addWindow.Item.Price &&
                                                           item.Category == addWindow.Item.Category);

                if (queryItems.Any())
                {
                    int index = Items.IndexOf(queryItems.FirstOrDefault());
                    Items[index].Qty += addWindow.Item.Qty;
                }
                else
                {
                    await PriceHandler.ScrapePriceAsync(addWindow.Item);
                    Items.Add(addWindow.Item);
                }

                JsonHandler.WriteItems(Items);
                MainDataGrid.ItemsSource = Items;
            }
        }

        private void ButtonEditCat_Click(object sender, RoutedEventArgs e)
        {
            EditCatWindow newEditWindow = new();
            newEditWindow.Owner = this;
            newEditWindow.ShowDialog();

            Categories = new(JsonHandler.ReadCategory());

            ComboBoxCats.ItemsSource = Categories;
            Categories.Insert(0, MostraTutto);
            ComboBoxCats.SelectedIndex = 0;
        }

        /// <summary>
        /// Quando selezioni nuovo elemento combobox, cambia la visualizzazione 
        /// nella gridview in base alla categoria selezionata
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboBoxCats_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Category selectedCategory = (Category)ComboBoxCats.SelectedItem;
            if (selectedCategory != null)
            {
                if (selectedCategory != MostraTutto)
                    MainDataGrid.ItemsSource = Items.Where(item => item.Category == selectedCategory.Name);
                else
                    MainDataGrid.ItemsSource = Items;
            }
        }

        private void ButtonReload_Click(object sender, RoutedEventArgs e)
        {
            MainDataGrid.ItemsSource = Items;
            Scraping();
        }

        private async void Scraping()
        {
            await Task.Run(async () =>
            {
                MainGridProgressBar.Dispatcher.Invoke(() => MainGridProgressBar.Visibility = Visibility.Visible);
                MainDataGrid.Dispatcher.Invoke(() => MainDataGrid.IsEnabled = false);
                await PriceHandler.ScrapePricesAsync(Items.Where(item => ComboBoxCats.Text != "Mostra tutto" ? 
                                                                         item.Category == ComboBoxCats.Text : true));
                MainGridProgressBar.Dispatcher.Invoke(() => MainGridProgressBar.Visibility = Visibility.Hidden);
                MainDataGrid.Dispatcher.Invoke(() => MainDataGrid.IsEnabled = true);
            });

        }
    }
}
