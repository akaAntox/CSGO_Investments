using InvestmentApp.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace InvestmentApp
{
    /// <summary>
    /// Logica di interazione per NewCatWindow.xaml
    /// </summary>
    public partial class NewCatWindow : Window
    {
        public NewCatWindow()
        {
            InitializeComponent();
        }

        public Category? Cat { get; set; }

        private void ButtonNewCat_Click(object sender, RoutedEventArgs e)
        {
            Cat = new Category { Name = TextBoxName.Text };
            DialogResult = true;
        }

        private void TextBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            TextBox? textBox = sender as TextBox;
            if (textBox != null)
                textBox.Clear();
        }
    }
}
