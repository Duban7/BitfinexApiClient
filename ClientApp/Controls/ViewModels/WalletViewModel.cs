using ClientApp.Commands;
using ClientApp.Shared;
using Connector.Connectors.Interfaces;
using System.Windows.Input;

namespace ClientApp.Controls.ViewModels
{
    class WalletViewModel : ObservableObject
    {
        private readonly IRESTBitfinexConnector _connector;

        private decimal _BCTtoUSDcourse;
        private decimal _XRPtoUSDcourse;
        private decimal _XMRtoUSDcourse;
        private decimal _DSHtoUSDcourse;

        public decimal BalanceBTC { get; set; }
        public decimal BalanceXRP { get; set; }
        public decimal BalanceXMR { get; set; }
        public decimal BalanceDSH { get; set; }

        private decimal _totalConvertedBalance;
        public decimal TotalConvertedBalance 
        {
            get=>_totalConvertedBalance;
            set
            {
                _totalConvertedBalance = value;
                OnPropertyChange();
            } 
        }

        public ICommand BTCCommand { get; set; }
        public ICommand XRPCommand { get; set; }
        public ICommand XMRCommand { get; set; }
        public ICommand DSHCommand { get; set; }

        public WalletViewModel(IRESTBitfinexConnector connector)
        {
            _connector = connector;

            TotalConvertedBalance = 0;
            BalanceBTC = 1;
            BalanceXRP = 15000;
            BalanceXMR = 50;
            BalanceDSH = 30;

            BTCCommand = new RelayCommand(_ => ConvertToBTC());
            XRPCommand = new RelayCommand(_ => ConvertToXRP());
            XMRCommand = new RelayCommand(_ => ConvertToXMR());
            DSHCommand = new RelayCommand(_ => ConvertToDASH());
        }

        private async void ConvertToDASH()
        {
            await SetCurrentCourse();

            TotalConvertedBalance = 0;
            TotalConvertedBalance += BalanceDSH;
            TotalConvertedBalance += BalanceXRP * _XRPtoUSDcourse / _DSHtoUSDcourse;
            TotalConvertedBalance += BalanceBTC * _BCTtoUSDcourse / _DSHtoUSDcourse;
            TotalConvertedBalance += BalanceXMR * _XMRtoUSDcourse / _DSHtoUSDcourse;
        }

        private async void ConvertToXMR()
        {
            await SetCurrentCourse();

            TotalConvertedBalance = 0;
            TotalConvertedBalance += BalanceXMR;
            TotalConvertedBalance += BalanceXRP * _XRPtoUSDcourse / _XMRtoUSDcourse;
            TotalConvertedBalance += BalanceBTC * _BCTtoUSDcourse / _XMRtoUSDcourse;
            TotalConvertedBalance += BalanceDSH * _DSHtoUSDcourse / _XMRtoUSDcourse;
        }

        private async void ConvertToXRP()
        {
            await SetCurrentCourse();

            TotalConvertedBalance = 0;
            TotalConvertedBalance += BalanceXRP;
            TotalConvertedBalance += BalanceDSH * _DSHtoUSDcourse / _XRPtoUSDcourse;
            TotalConvertedBalance += BalanceBTC * _BCTtoUSDcourse / _XRPtoUSDcourse;
            TotalConvertedBalance += BalanceXMR * _XMRtoUSDcourse / _XRPtoUSDcourse;
        }

        private async void ConvertToBTC()
        {
            await SetCurrentCourse();

            TotalConvertedBalance = 0;
            TotalConvertedBalance += BalanceBTC;
            TotalConvertedBalance += BalanceXRP * _XRPtoUSDcourse / _BCTtoUSDcourse;
            TotalConvertedBalance += BalanceDSH * _DSHtoUSDcourse / _BCTtoUSDcourse;
            TotalConvertedBalance += BalanceXMR * _XMRtoUSDcourse / _BCTtoUSDcourse;
        }

        private async Task SetCurrentCourse()
        {
            _BCTtoUSDcourse = (await _connector.GetTickerInfo("BTCUSD")).LastPrice;
            _XRPtoUSDcourse = (await _connector.GetTickerInfo("XRPUSD")).LastPrice;
            _XMRtoUSDcourse = (await _connector.GetTickerInfo("XMRUSD")).LastPrice;
            _DSHtoUSDcourse = (await _connector.GetTickerInfo("DSHUSD")).LastPrice;
        }
    }
}
