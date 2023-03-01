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

            GridGUIUpdate();
        }

        private void ReloadTotalValues (IEnumerable<Item> items)
        {
            decimal sumQty = Decimal.Round(items.Sum(i => i.Qty),2);
            decimal sumPrice = Decimal.Round(items.Sum(i => i.Price), 2);
            decimal sumTotal = Decimal.Round(items.Sum(i => i.Total), 2);
            decimal sumSellPrice = Decimal.Round(items.Sum(i => i.SellPrice), 2);
            decimal sumMediumPrice = Decimal.Round(items.Sum(i => i.MediumPrice), 2);
            decimal sumNetProfit = Decimal.Round(items.Sum(i => i.NetProfit), 2);
            decimal sumNetTotalProfit = Decimal.Round(items.Sum(i => i.NetTotalProfit), 2);
                
            LabelQty.Content = "Quantity: " + sumQty.ToString();
            LabelPrice.Content = "Buy Price: " + sumPrice.ToString();
            LabelTotal.Content = "Total Payed: " + sumTotal.ToString();
            LabelSellPrice.Content = "Minimum Price: " + sumSellPrice.ToString();
            LabelMediumPrice.Content = "Medium Price: " + sumMediumPrice.ToString();
            LabelNetProfit.Content = "Net Profit: " + sumNetProfit.ToString();
            LabelNetTotalProfit.Content = "Total Net Profit: " + sumNetTotalProfit.ToString();
        }

        private void Categories_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            ComboBoxCats.ItemsSource = Categories;
        }

        /// <summary>
        /// Elimina elemento dalla datagridview (contextmenu)
        /// </summary>
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Item row = (Item)MainDataGrid.SelectedItem;
            if (row != null)
            {
                Items.Remove(row);
                GridGUIUpdate();
                JsonHandler.WriteItems(Items);
            }
        }

        /// <summary>
        /// Aggiunge l'oggetto alla lista e ne aggiorna i prezzi in base a steam
        /// </summary>
        private async void ButtonAddItem_Click(object sender, RoutedEventArgs e)
        {
            AddWindow addWindow = new(ComboBoxCats.Text);
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
                GridGUIUpdate();
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
        private void ComboBoxCats_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GridGUIUpdate();
        }


        private void ButtonReload_Click(object sender, RoutedEventArgs e)
        {
            GridGUIUpdate();
            Scraping();
        }

        private void GridGUIUpdate()
        {
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

        private async void Scraping()
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
                        MainGridProgressBar.Maximum = Items.Count();
                });
                MainDataGrid.Dispatcher.Invoke(() =>
                {
                    GridGUIUpdate();
                });

                string text = string.Empty;
                ComboBoxCats.Dispatcher.Invoke(() => text = ComboBoxCats.Text);

                var items = await PriceHandler.ScrapePricesAsync(Items.Where(item => item.Category == (text != MostraTutto.Name ? text : item.Category)).ToList().AsEnumerable(), MainGridProgressBar, LabelInfo);
                var result = Items.Except(items).Concat(items);

                MainDataGrid.Dispatcher.Invoke(() =>
                {
                    MainDataGrid.ToolTip = null;
                    GridGUIUpdate();
                });
                JsonHandler.WriteItems(result);
            });
        }
    }
}
