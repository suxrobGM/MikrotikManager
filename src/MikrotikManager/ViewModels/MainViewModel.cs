using MikrotikManager.Models;
using MikrotikManager.Services;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using tik4net;

namespace MikrotikManager.ViewModels
{
    public class MainViewModel : BindableBase
    {
        private readonly SecureString _password;
        private string _connectionAddress;
        private string _username;
        private bool _defaultGateway;
        private bool _isBusy;
        private MikrotikService mkService;
        private StringBuilder _statusMessages;
        private ITikConnection _connection;

        public string ConnectionAddress
        {
            get => _connectionAddress;
            set
            {
                SetProperty(ref _connectionAddress, value);
                ConnectCommand.RaiseCanExecuteChanged();
            }
        }
        public string Username
        {
            get => _username;
            set
            {
                SetProperty(ref _username, value);
                ConnectCommand.RaiseCanExecuteChanged();
            }
        }
        public string StatusMessages { get => _statusMessages.ToString(); }
        public bool DefaultGateway { get => _defaultGateway; set => SetProperty(ref _defaultGateway, value); }
        public bool IsBusy { get => _isBusy; set => SetProperty(ref _isBusy, value); }
        public ObservableCollection<MkAddress> MkAddresses { get; set; }
        public DelegateCommand<PasswordBox> PasswordChangedCommand { get; }
        public DelegateCommand ConnectCommand { get; }

        public MainViewModel()
        {
            MkAddresses = new ObservableCollection<MkAddress>();
            _password = new SecureString();
            _statusMessages = new StringBuilder();

            for (int i = 1; i <= 9; i++)
            {
                MkAddresses.Add(new MkAddress
                {
                    Name = $"IP{i}",
                    IPAddress = IPAddress.Parse("192.168.1.2")
                });
            }

            PasswordChangedCommand = new DelegateCommand<PasswordBox>(passBox =>
            {
                _password.Clear();

                foreach (var ch in passBox.Password)
                {
                    _password.AppendChar(ch);
                }
                ConnectCommand.RaiseCanExecuteChanged();
            });

            ConnectCommand = new DelegateCommand(async () =>
            {
                await Task.Run(() =>
                {
                    try
                    {
                        IsBusy = true;
                        //mkService = new MikrotikService(ConnectionAddress);
                        //var loginSucceeded = mkService.Login(_username, new NetworkCredential(_username, _password).Password);

                        _connection = ConnectionFactory.CreateConnection(TikConnectionType.Api);
                        _connection.Open(ConnectionAddress, Username, new NetworkCredential(Username, _password).Password);

                        _statusMessages.AppendLine("Successfully loged");
                        RaisePropertyChanged(nameof(StatusMessages));
                        IsBusy = false;                       
                    }
                    catch (Exception ex)
                    {
                        _statusMessages.AppendLine("Could not log in");
                        _statusMessages.AppendLine("Error message: ");
                        _statusMessages.AppendLine(ex.Message);
                        RaisePropertyChanged(nameof(StatusMessages));
                        IsBusy = false;
                    }
                });                               
            }, CanExecuteConnectCommand);
        }
        
        private bool CanExecuteConnectCommand()
        {
            return IPAddress.TryParse(ConnectionAddress, out IPAddress ip) && !string.IsNullOrEmpty(Username) && _password.Length > 0;                     
        }
    }
}
