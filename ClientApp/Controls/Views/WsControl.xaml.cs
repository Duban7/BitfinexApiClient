using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ClientApp.Controls.Views
{
    /// <summary>
    /// Логика взаимодействия для WsControl.xaml
    /// </summary>
    public partial class WsControl : UserControl
    {
        public WsControl()
        {
            InitializeComponent();
            ClearButton.IsEnabled = false;
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = Regex.IsMatch(e.Text, "[^0-9.-]+");
        }

        private void TradeButton_Click(object sender, RoutedEventArgs e)
        {
            TradeButton.IsEnabled = false;
            CandleButton.IsEnabled = false;
            CandleComboBox.IsEnabled = false;
            ClearButton.IsEnabled = true;
        }
        private void CandleButton_Click(object sender, RoutedEventArgs e)
        {
            TradeButton.IsEnabled = false;
            CandleButton.IsEnabled = false;
            CandleComboBox.IsEnabled = false;
            ClearButton.IsEnabled = true;
        }
        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            TradeButton.IsEnabled = true;
            CandleButton.IsEnabled = true;
            CandleComboBox.IsEnabled = true;
            ClearButton.IsEnabled = false;
        }
    }
}
