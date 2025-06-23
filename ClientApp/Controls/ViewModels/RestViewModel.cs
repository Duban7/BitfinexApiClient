using ClientApp.Commands;
using ClientApp.Shared;
using Connector.Connectors.Interfaces;
using Connector.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ClientApp.Controls.ViewModels
{
    public class RestViewModel : ObservableObject
    {
        private readonly IRESTBitfinexConnector _connector;
        public string Pair { get; set; }
        public ObservableCollection<object> Data { get; set; }
        public ObservableCollection<string> Periods { get; set; } = new ObservableCollection<string> { "1m", "5m", "15m", "30m", "1h", "3h", "6h", "12h", "1D", "1W" };

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

        private string _selectedMaxCount;
        public string SelectedMaxCount
        {
            get { return _selectedMaxCount; }
            set
            {
                _selectedMaxCount = value;
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
        public ICommand TradesCommand { get; set; }
        public ICommand CandlesCommand { get; set; }
        public ICommand TickerCommand { get; set; }
        public RestViewModel(IRESTBitfinexConnector connector, string pair)
        {
            _connector = connector;
            Pair = pair;

            Data = new();
            SelectedPeriod = Periods[0];
            SelectedMaxCount = "10";
            AutoColumns = true;

            CandlesCommand = new RelayCommand(_ => CandlesHandling());
            TradesCommand = new RelayCommand(_ => TradesHandling());
            TickerCommand = new RelayCommand(_ => TickerHandling());
        }

        private async void CandlesHandling()
        {
            Data.Clear();
            AutoColumns = false;
            AutoColumns = true;
            var candles = await _connector.GetCandleSeriesAsync(Pair, SelectedPeriod);
            foreach (var item in candles)
            {
                Data.Add(item);   
            }
            
        }
        private async void TradesHandling()
        {
            Data.Clear();
            AutoColumns = false;
            AutoColumns = true;
            var trades = await _connector.GetNewTradesAsync(Pair, int.Parse(SelectedMaxCount));
            foreach (var item in trades)
            {
                Data.Add(item);
            }
        }
        private async void TickerHandling()
        {
            Data.Clear();
            AutoColumns = false;
            AutoColumns = true;
            var ticker = await _connector.GetTickerInfo(Pair);
            Data.Add(ticker);

        }
    }
}
