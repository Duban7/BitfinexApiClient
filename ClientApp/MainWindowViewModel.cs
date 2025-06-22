using ClientApp.Commands;
using ClientApp.Controls.ViewModels;
using ClientApp.Shared;
using Connector.Connectors.Interfaces;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace ClientApp
{
    public class MainWindowViewModel : ObservableObject
    {
        private readonly IRESTBitfinexConnector _restConnector;
        private readonly IWSBitfinexConnector _wsConnector;

        public ICommand OpenRestCommand { get; }
        public ICommand OpenWsCommand { get; }
        public ICommand BackCommand { get; }

        public ObservableCollection<string> AvailablePairs { get; } = new ObservableCollection<string>
        {
            "BTCUSD", "ETHUSD", "XRPUSD", "LTCUSD"
        };

        private string _selectedPair;
        public string SelectedPair
        {
            get => _selectedPair;
            set { _selectedPair = value; OnPropertyChange(); }
        }
        private object _currentVM;
        public object CurrentVM { get => _currentVM; set { _currentVM = value; OnPropertyChange(); } }
        public MainWindowViewModel(IRESTBitfinexConnector restCon, IWSBitfinexConnector wsCon)
        {
            _restConnector = restCon;
            _wsConnector = wsCon;
            
            SelectedPair = AvailablePairs[0];

            OpenRestCommand = new RelayCommand(_ => ShowRest());
            OpenWsCommand = new RelayCommand(_ => ShowWs());
            BackCommand = new RelayCommand(_ => BackToStartCreen());
        }

        private void ShowRest()
        {
            CurrentVM = new RestViewModel(_restConnector, SelectedPair);
        }
        private void ShowWs()
        {
            CurrentVM = new WsViewModel(_wsConnector, SelectedPair);
        }
        private void BackToStartCreen()
        {
            CurrentVM = null;
        }
    }
}
