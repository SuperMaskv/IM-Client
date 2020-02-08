using IM_Client.Commands;
using IM_Client.Services;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using IM_Client.Enums;
using IM_Client.Protocol.NoServerPacket;
using System.Collections.ObjectModel;
using IM_Client.Models;
using System.Threading;
using System.Net.Sockets;
using IM_Client.Protocol.Handler;
using IM_Client.Protocol;
using System.Net;

namespace IM_Client.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private IChatService chatService;
        private IDialogService dialogService;

        private static readonly int LISTEN_PORT = 20000;


        public MainWindowViewModel(IChatService chatSvc, IDialogService dialogSvc)
        {
            Console.WriteLine("MainWindowVM is intialed.");
            this.chatService = chatSvc;
            this.dialogService = dialogSvc;
        }

        private string _userName;
        public string UserName
        {
            get { return _userName; }
            set
            {
                _userName = value;
                OnPropertyChanged();
            }
        }

        private string _profilePic;
        public string ProfilePic
        {
            get { return _profilePic; }
            set
            {
                _profilePic = value;
                OnPropertyChanged();
            }
        }

        private UserModes _userMode;
        public UserModes UserMode
        {
            get { return _userMode; }
            set
            {
                _userMode = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<Participant> _participants = new ObservableCollection<Participant>();
        public ObservableCollection<Participant> Participants
        {
            get { return _participants; }
            set
            {
                _participants = value;
                OnPropertyChanged();
            }
        }

        private Participant _selectedParticipant;
        public Participant SelectedParticipant
        {
            get { return _selectedParticipant; }
            set
            {
                _selectedParticipant = value;
                if (_selectedParticipant.HasSentNewMessage) _selectedParticipant.HasSentNewMessage = false;
                OnPropertyChanged();
            }
        }

        #region NoServerLoginCommand
        private ICommand _noServerLoginCommand;
        public ICommand NoServerLoginCommand
        {
            get
            {
                return _noServerLoginCommand ?? (_noServerLoginCommand =
                    new RelayCommandAsync(() => NoServerLogin(), (o) => CanNoServerLogin()));
            }
        }

        private bool CanNoServerLogin()
        {
            return !string.IsNullOrEmpty(UserName) && UserName.Length >= 2;
        }

        private async Task<bool> NoServerLogin()
        {
            NoServerLoginPacket noServerLoginPacket = new NoServerLoginPacket();
            noServerLoginPacket.UserName = UserName;
            noServerLoginPacket.Port = LISTEN_PORT;
            chatService.InvokeBroadcastPacketEvent(noServerLoginPacket);

            UserMode = UserModes.Chat;
            ThreadPool.QueueUserWorkItem(new WaitCallback(UdpListen));

            return true;
        }

        private void UdpListen(object obj)
        {
            Console.WriteLine("Listener is on.");
            Console.WriteLine(Thread.CurrentThread.Name);
            UdpClient rcvClient = new UdpClient(20000);

            IPEndPoint remote = new IPEndPoint(IPAddress.Any, 0);

            var rcvResult = rcvClient.Receive(ref remote);
            NoServerPacketHandler.INSTANCE.INVOKE(PacketCodec.INSTANCE.Decode(rcvResult));
        }

        #endregion

        #region LoginCommand
        private ICommand _loginCommand;
        public ICommand LoginCommand
        {
            get
            {
                return _loginCommand ?? (_loginCommand =
                    new RelayCommand((o) => Login(), (o) => CanLogin()));
            }
        }

        private bool CanLogin()
        {
            return true;
        }

        private void Login()
        {
            dialogService.ShowNotification("Login");
        }
        #endregion
    }
}
