using ClientApp.Commands;
using ClientApp.Shared;
using Connector.Connectors.Interfaces;
using Connector.Models;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace ClientApp.Controls.ViewModels
{
    public class WsViewModel : ObservableObject
    {

        private readonly IWSBitfinexConnector _connector;
        public string Pair { get; set; }
        public ObservableCollection<object> Data { get; set; }
        public ObservableCollection<string> Periods { get; set; } = new ObservableCollection<string> { "1m", "5m", "15m", "30m", "1h", "3h", "6h", "12h", "1D", "1W" };

        private bool isCandles;

        private string _selectedPeriod;
        public string SelectedPeriod
        {
            get { return _selectedPeriod; }
            set
            {
                _selectedPeriod = value;
                OnPropertyChange();
            }
        }

        private bool _autoColumns;
        public bool AutoColumns
        {
            get { return _autoColumns; }
            set
            {
                _autoColumns = value;
                OnPropertyChange();
            }
        }
        public ICommand SubscribeTradesCommand { get; set; }
        public ICommand SubscribeCandlesCommand { get; set; }
        public ICommand ClearCommand { get; set; }
        public WsViewModel(IWSBitfinexConnector connector, string pair)
        {
            _connector = connector;
            Pair = pair;

            Data = new();
            SelectedPeriod = Periods[0];
            AutoColumns = true;

            SubscribeTradesCommand = new RelayCommand(_ => SubscribeTrades());
            SubscribeCandlesCommand = new RelayCommand(_ => SubscribeCandles());
            ClearCommand = new RelayCommand(_ => Clear());
        }

        private void Clear()
        {
            if (isCandles)
            { 
                _connector.UnsubscribeCandles(Pair);
                _connector.CandleSeriesProcessing -= CandleHandling;
            }
            else
            {
                _connector.NewBuyTrade -= TradeHandling;
                _connector.NewSellTrade -= TradeHandling;
                _connector.UnsubscribeTrades(Pair);
            }
            Data.Clear();
            AutoColumns = false;
            AutoColumns = true;
        }

        private void SubscribeTrades()
        {
            if (Data.Count > 0)
                _connector.UnsubscribeCandles(Pair);

            Data.Clear();
            AutoColumns = false;
            AutoColumns = true;
            isCandles = false;

            _connector.SubscribeTrades(Pair);
            _connector.NewBuyTrade += TradeHandling;
            _connector.NewSellTrade += TradeHandling;
        }

        private void TradeHandling(Trade trade)
        {
            if (!trade.IsExecuted) return;

            Application.Current.Dispatcher.Invoke(() =>
            {
                Data.Add(trade);
            });
        }

        private void SubscribeCandles()
        {
            if (Data.Count > 0)
                _connector.UnsubscribeTrades(Pair);

            Data.Clear();
            AutoColumns = false;
            AutoColumns = true;
            isCandles = true;

            _connector.SubscribeCandles(Pair, SelectedPeriod);
            _connector.CandleSeriesProcessing += CandleHandling;
        }

        private void CandleHandling(Candle candle)
        {
            Candle? foundCandle = (Candle?)Data.FirstOrDefault(c => (c as Candle)!.OpenTime == candle.OpenTime);
            if(foundCandle == null )
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Data.Add(candle);
                });
                return;
            }
            if (foundCandle!.TotalVolume == candle.TotalVolume) return;
            else
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Data.Remove(foundCandle);
                    Data.Add(candle);
                });
            }

        }
    }
}
