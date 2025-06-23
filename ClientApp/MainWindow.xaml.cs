using Connector.Connectors.Implementation;
using Connector.Connectors.Interfaces;
using System.Windows;

namespace ClientApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            IRESTBitfinexConnector rest = new RESTBitfinexConnector();
            IWSBitfinexConnector ws = new WSBitfinexConnector();
            this.DataContext = new MainWindowViewModel(rest, ws);
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            ContentControl.Visibility = Visibility.Collapsed;
            BackButton.Visibility = Visibility.Collapsed;
            StartScreen.Visibility = Visibility.Visible;
        }

        private void ContentButton_Click(object sender, RoutedEventArgs e)
        {
            StartScreen.Visibility = Visibility.Collapsed;
            ContentControl.Visibility = Visibility.Visible;
            BackButton.Visibility = Visibility.Visible;
        }

        //private void Wscon_NewSellTrade(Trade obj)
        //{
        //    Debug.WriteLine("TRADE:----|"+obj.Time+"---------------invoke method");// te - trade executed tu - trade uodated
        //}

        //private void NewBuyTradeHandle(Candle candle)
        //{

        //    Debug.WriteLine("CANDLE:----|"+candle.OpenTime+"-"+candle.OpenPrice.ToString()+"-"+candle.TotalVolume+"--------------------Invoked method");//Присылает текущую(обновляет) и предыдущую свечу
        //}
    }
}