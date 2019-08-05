using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MikrotikManager.Models
{
    public class MkAddress : BindableBase
    {
        private string _name;
        private int _timeout;
        private IPAddress _ipAddress;

        public MkAddress()
        {
            CreationTime = DateTime.Now;
            IPAddress = IPAddress.Any;
        }

        public string Name { get => _name; set => SetProperty(ref _name, value); }
        public IPAddress IPAddress { get => _ipAddress; set => SetProperty(ref _ipAddress, value); }
        public int Timeout { get => _timeout; set => SetProperty(ref _timeout, value); }
        public DateTime CreationTime { get; set; }
    }
}
