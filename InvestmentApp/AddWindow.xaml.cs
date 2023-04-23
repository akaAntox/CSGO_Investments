using InvestmentApp.Handlers;
using InvestmentApp.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace InvestmentApp
{
    public partial class AddWindow : Window
    {
        private ObservableCollection<Category> Categories;
        private ObservableCollection<Item> Items;

        public AddWindow(string selectedCat)
        {
            InitializeComponent();

            try
            {
                Categories = new(JsonHandler.ReadCategoryAsync());
                Categories.CollectionChanged += Categories_CollectionChanged;
            }
            catch (Exception)
            { Categories = new(); }

            try
            { Items = new(JsonHandler.ReadItems()); }
            catch (Exception)
            { Items = new(); }

            var foundItem = Categories.FirstOrDefault(item => item.Name == selectedCat);
            if (selectedCat != null && foundItem != null)
                ComboBoxCat.SelectedItem = foundItem;

            ComboBoxCat.ItemsSource = Categories;
        }

        private void Categories_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            ComboBoxCat.ItemsSource = Categories;
        }

        public Item? Item { get; set; }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(TextBoxName.Text) || string.IsNullOrEmpty(TextBoxPrice.Text) || string.IsNullOrEmpty(TextBoxQty.Text) || string.IsNullOrEmpty(ComboBoxCat.Text))
            {
                MessageBox.Show("Inserisci tutti i dati!", "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else if (!decimal.TryParse(TextBoxPrice.Text, out decimal _) && !int.TryParse(TextBoxQty.Text, out int _))
            {
                MessageBox.Show("Inserisci un prezzo e una quantità validi", "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else if (!decimal.TryParse(TextBoxPrice.Text, out decimal _))
            {
                MessageBox.Show("Inserisci un prezzo valido", "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else if (!int.TryParse(TextBoxQty.Text, out int _))
            {
                MessageBox.Show("Inserisci una quanità valida", "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Item = new Item
            {
                Name = TextBoxName.Text,
                Qty = Convert.ToInt32(TextBoxQty.Text),
                Price = Convert.ToDecimal(TextBoxPrice.Text.Replace('.', ',')),
                Category = ComboBoxCat.Text
            };

            DialogResult = true;
            Close();
        }

        private void TextBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            TextBox? textBox = sender as TextBox;
            if (textBox != null)
                textBox.Clear();
        }

        private void ButtonEditCat_Click(object sender, RoutedEventArgs e)
        {
            EditCatWindow newEditWindow = new();
            newEditWindow.Owner = this;
            newEditWindow.ShowDialog();

            Categories = new(JsonHandler.ReadCategoryAsync());
            ComboBoxCat.ItemsSource = Categories;
        }
    }
}
