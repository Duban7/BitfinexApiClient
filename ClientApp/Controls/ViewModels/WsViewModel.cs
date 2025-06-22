using Connector.Connectors.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientApp.Controls.ViewModels
{
    public class WsViewModel
    {
        public string Pair { get; set; }
        public WsViewModel(IWSBitfinexConnector connector, string pair)
        {
            Pair = pair;
        }
    }
}
