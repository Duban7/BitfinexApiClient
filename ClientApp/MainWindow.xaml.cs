using Connector;
using Connector.Connectors.Implementation;
using Connector.Models;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ClientApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        RESTBitfinexConnector con;
        WSBitfinexConnector Wscon;
        public MainWindow()
        {
            InitializeComponent();
            con = new();
            Wscon = new();

            //Wscon.SubscribeCandles("BTCUSD", 60, count: 1);
            //Wscon.CandleSeriesProcessing += NewBuyTradeHandle;
            Wscon.SubscribeTrades("BTCUSD", 1);
            Wscon.NewSellTrade += Wscon_NewSellTrade;
            Wscon.NewBuyTrade += Wscon_NewSellTrade;
        }

        private void Wscon_NewSellTrade(Trade obj)
        {
            Debug.WriteLine(obj.Time);
        }

        private void NewBuyTradeHandle(Candle candle)
        {

            Debug.WriteLine(candle.OpenTime+"-"+candle.OpenPrice.ToString()+"-"+candle.TotalVolume+"--------------------Invoked method");//Присылает текущую(обновляет) и предыдущую свечу
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            label.Content  = (await con.GetTickerInfo("BTCUSD"));

            return;
        }
    }
}