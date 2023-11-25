using InvestmentApp.Converters;
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
                Categories = new(JsonHandler.ReadCategory() ?? new List<Category>());
                Categories.CollectionChanged += Categories_CollectionChanged;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error reading categories from file: {ex.Message}", "Categories", MessageBoxButton.OK, MessageBoxImage.Error);
                Categories = new ObservableCollection<Category>();
            }

            try
            { 
                Items = new(JsonHandler.ReadItems() ?? new List<Item>()); 
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error reading items from file: {ex.Message}", "Objects", MessageBoxButton.OK, MessageBoxImage.Error);
                Items = new ObservableCollection<Item>();
            }

            ComboBoxCats.ItemsSource = Categories;
            Categories.Insert(0, MostraTutto);
            ComboBoxCats.SelectedItem = MostraTutto;

            MainDataGrid.Dispatcher.Invoke(() => GridGUIUpdate());
        }

        /// <summary>
        /// Calcola i valori totali da mostrare in fondo
        /// </summary>
        private void ReloadTotalValues(IEnumerable<Item> items)
        {
            int itemCount = items.Count();
            decimal sumQty = decimal.Round(items.Sum(i => i.Qty), 2);
            decimal sumTotalBuy = decimal.Round(items.Sum(i => i.Total), 2);
            decimal sumNetTotalProfit = decimal.Round(items.Sum(i => i.NetTotalProfit), 2);
            decimal sumTotalValue = decimal.Round(sumTotalBuy + sumNetTotalProfit, 2);
            decimal avgPercentage = decimal.Round(items.Sum(i => i.ProfitPercentage / itemCount), 2);

            LabelQty.Content = "Quantity: " + sumQty.ToString();
            LabelTotal.Content = "Total Payed: " + sumTotalBuy.ToString() + " €";
            LabelAvgPercentage.Content = "Average profit percentage: " + avgPercentage.ToString() + " %";
            LabelNetTotalProfit.Content = "Total Net Profit: " + sumNetTotalProfit.ToString() + " €";
            LabelTotalValue.Content = "Total Value: " + sumTotalValue.ToString() + " €";
        }

        /// <summary>
        /// Controlla se la categoria da visualizzare è cambiata
        /// </summary>
        private void Categories_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            ComboBoxCats.ItemsSource = Categories;
        }

        /// <summary>
        /// Elimina elemento dalla datagridview (contextmenu)
        /// </summary>
        private async void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            var selectedItems = MainDataGrid.SelectedItems;
            if (selectedItems != null && selectedItems.Count > 0)
            {
                for (int i = selectedItems.Count - 1; i >= 0; i--)
                {
                    if (selectedItems[i] is Item selectedItem)
                    {
                        Items.Remove(selectedItem);
                    }
                }
                await JsonHandler.WriteItemsAsync(Items);
            }
            MainDataGrid.Dispatcher.Invoke(() => GridGUIUpdate());
        }

        /// <summary>
        /// Aggiorna l'elemento selezionato
        /// </summary>
        private void UpdateItem_Click(object sender, RoutedEventArgs e)
        {
            var selectedItems = MainDataGrid.SelectedItems;
            if (selectedItems != null && selectedItems.Count > 0)
            {
                foreach (var selectedItem in selectedItems)
                {
                    if (selectedItem is Item row)
                        Scraping(row);
                }
            }
        }

        /// <summary>
        /// Modifica l'elemento selezionato
        /// </summary>
        private void EditItem_Click(object sender, RoutedEventArgs e)
        {
            // TODO: IMPLEMENTATION
        }

        /// <summary>
        /// Aggiunge l'oggetto alla lista e ne aggiorna i prezzi in base a steam
        /// </summary>
        private async void ButtonAddItem_Click(object sender, RoutedEventArgs e)
        {
            AddWindow addWindow = new(ComboBoxCats.Text)
            {
                Owner = this
            };

            if (addWindow.ShowDialog() == true && addWindow.Item != null)
            {
                IEnumerable<Item> queryItems = Items.Where(item => item.Name == addWindow.Item.Name &&
                                                           item.Price == addWindow.Item.Price &&
                                                           item.Category == addWindow.Item.Category);

                if (queryItems.Any())
                {
                    int index = Items.IndexOf(queryItems.First());
                    Items[index].Qty += addWindow.Item.Qty;
                }
                else
                {
                    await PriceHandler.ScrapePriceAsync(addWindow.Item);
                    Items.Add(addWindow.Item);
                }

                await JsonHandler.WriteItemsAsync(Items);
            }
            MainDataGrid.Dispatcher.Invoke(() => GridGUIUpdate());
        }

        /// <summary>
        /// Modifica le categorie
        /// </summary>
        private void ButtonEditCat_Click(object sender, RoutedEventArgs e)
        {
            EditCatWindow newEditWindow = new()
            {
                Owner = this
            };
            newEditWindow.ShowDialog();

            Categories = new(JsonHandler.ReadCategory() ?? new List<Category>());

            ComboBoxCats.ItemsSource = Categories;
            Categories.Insert(0, MostraTutto);
            ComboBoxCats.SelectedIndex = 0;
        }

        /// <summary>
        /// Quando selezioni nuovo elemento combobox, cambia la visualizzazione 
        /// nella gridview in base alla categoria selezionata
        /// </summary>
        private void ComboBoxCats_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MainDataGrid.Dispatcher.Invoke(() => GridGUIUpdate());
        }

        /// <summary>
        /// Quando viene premuto il tasto aggiorna prezzi, i prezzi vengono aggiornati
        /// e viene conseguentemente aggiornata la gridview
        /// </summary>
        private void ButtonReload_Click(object sender, RoutedEventArgs e)
        {
            Scraping();
        }

        /// <summary>
        /// Aggiorna la GUI in base alla categoria attualmente selezionata
        /// </summary>
        private void GridGUIUpdate()
        {
            Items = ObservableCollectionConverter.ConvertIEnumerable(JsonHandler.ReadItems());
            Category selectedCategory = (Category)ComboBoxCats.SelectedItem;
            if (selectedCategory != null)
            {
                if (selectedCategory != MostraTutto)
                {
                    var selected = Items.Where(item => item.Category == selectedCategory.Name);
                    MainDataGrid.ItemsSource = selected;
                    ReloadTotalValues(selected);
                }
                else
                {
                    MainDataGrid.ItemsSource = Items;
                    ReloadTotalValues(Items);
                }
            }
        }

        /// <summary>
        /// Ottiene i prezzi degli item dal mercato di steam
        /// </summary>
        private async void Scraping(Item? selectedItem = null)
        {
            await Task.Run(async () =>
            {
                MainGridProgressBar.Dispatcher.Invoke(() =>
                {
                    MainGridProgressBar.Visibility = Visibility.Visible;
                    MainGridProgressBar.Value = 0;
                    if (ComboBoxCats.SelectedItem != MostraTutto)
                        MainGridProgressBar.Maximum = Items.Count(item => item.Category == ComboBoxCats.Text);
                    else
                        MainGridProgressBar.Maximum = Items.Count;
                });

                string text = string.Empty;
                ComboBoxCats.Dispatcher.Invoke(() => text = ComboBoxCats.Text);

                var items = await PriceHandler.ScrapePricesAsync(Items.Where(item =>
                                                                    selectedItem != null ?
                                                                    selectedItem.Name == item.Name :
                                                                    item.Category == (text != MostraTutto.Name ? text : item.Category))
                                                                .ToList().AsEnumerable(), MainGridProgressBar, LabelInfo);

                var result = Items.Except(items).Concat(items); // remove items and add them at the end
                await JsonHandler.WriteItemsAsync(result);
            });
            MainDataGrid.Dispatcher.Invoke(() => GridGUIUpdate());
        }
    }
}
