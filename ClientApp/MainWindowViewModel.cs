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
        public ICommand OpenWalletCommand { get; }
        public ICommand BackCommand { get; }

        public ObservableCollection<string> AvailablePairs { get; } = new ObservableCollection<string>
        {
            "ADABTC","ADAUSD","ADAUST","ALGUSD","APEUSD","APEUST","APTUSD","APTUST","ATOUSD",
            "ATOUST","AVAX:BTC","AVAX:USD","AVAX:UST","BCHN:USD","BTCEUR","BTCGBP","BTCJPY",
            "BTCUSD","BTCUST","COMP:USD","COMP:UST","DAIUSD","DOGE:BTC","DOGE:USD","DOGE:UST",
            "DOTUSD","DOTUST","DSHBTC","DSHUSD","EGLD:USD","EGLD:UST","ETCBTC","ETCUSD","ETCUST",
            "ETHBTC","ETHEUR","ETHGBP","ETHJPY","ETHUSD","ETHUST","ETHW:USD","ETHW:UST","FILUSD","FILUST",
            "IOTBTC","IOTUSD","LEOUSD","LEOUST","LINK:USD","LINK:UST","LTCBTC","LTCUSD","LTCUST","MATIC:USD",
            "MATIC:UST","MKRUSD","NEOUSD","NEOUST","SHIB:USD","SHIB:UST","SOLBTC","SOLUSD","SOLUST","SUIUSD",
            "SUIUST","SUSHI:USD","SUSHI:UST","TESTADA:TESTUSD","TESTALGO:TESTUSD","TESTAPT:TESTUSD","TESTAVAX:TESTUSD",
            "TESTBTC:TESTUSD","TESTBTC:TESTUSDT","TESTDOGE:TESTUSD","TESTDOT:TESTUSD","TESTETH:TESTUSD","TESTFIL:TESTUSD",
            "TESTLTC:TESTUSD","TESTMATIC:TESTUSD","TESTNEAR:TESTUSD","TESTSOL:TESTUSD","TESTXAUT:TESTUSD","TESTXTZ:TESTUSD",
            "TRXUSD","TRXUST","UNIUSD","UNIUST","USTUSD","XAUT:BTC","XAUT:USD","XAUT:UST","XLMBTC","XLMUSD","XLMUST","XMRBTC"
            ,"XMRUSD","XMRUST","XRPBTC","XRPUSD","XRPUST","ZECBTC","ZECUSD","ZRXUSD"
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
            OpenWalletCommand = new RelayCommand(_ => ShowWallet());
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
        private void ShowWallet()
        {
            CurrentVM = new WalletViewModel(_restConnector);
        }

        private void BackToStartCreen()
        {
            CurrentVM = null;
        }
    }
}
