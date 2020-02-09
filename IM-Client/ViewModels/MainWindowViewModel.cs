using IM_Client.Commands;
using IM_Client.Enums;
using IM_Client.Models;
using IM_Client.Protocol;
using IM_Client.Protocol.Handler;
using IM_Client.Protocol.NoServerPacket;
using IM_Client.Services;
using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Diagnostics;
using System.Drawing.Imaging;

namespace IM_Client.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private IChatService chatService;
        private IDialogService dialogService;
        private int MAX_IMAGE_WIDTH = 1024;
        private int MAX_IMAGE_HEIGHT = 1024;

        private static readonly int LISTEN_PORT = 20000;


        public MainWindowViewModel(IChatService chatSvc, IDialogService dialogSvc)
        {
            Console.WriteLine("MainWindowVM is intialed.");
            this.chatService = chatSvc;
            this.dialogService = dialogSvc;
        }

        #region Fields

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
        #endregion

        #region SelectProfilePicCommand
        private ICommand _selectProfilePicCommand;
        public ICommand SelectedProfilePicCommand
        {
            get
            {
                return _selectProfilePicCommand ?? (_selectProfilePicCommand
                    = new RelayCommand((o) => SelectProfilePic()));
            }
        }

        private void SelectProfilePic()
        {
            var pic = dialogService.OpenFile("Select image file", "Images (*.jpg;*.png)|*.jpg;*.png");
            if (!string.IsNullOrEmpty(pic))
            {
                var img = Image.FromFile(pic);
                if (img.Width > MAX_IMAGE_WIDTH || img.Height > MAX_IMAGE_HEIGHT)
                {
                    dialogService.ShowNotification($"Image size should be {MAX_IMAGE_WIDTH} x {MAX_IMAGE_HEIGHT} or less.");
                    return;
                }
                ProfilePic = pic;
            }
        }
        #endregion

        #region OpenImageCommand
        private ICommand _openImageCommand;
        public ICommand OpenImageCommand
        {
            get
            {
                return _openImageCommand ?? (_openImageCommand
                    = new RelayCommand<ChatMessage>((m) => OpenImage(m)));
            }
        }
        private void OpenImage(ChatMessage chatMessage)
        {
            var img = new MemoryStream(chatMessage.Picture);
            if (img != null)
            {
                Image image = Image.FromStream(img);
                var imgName = DateTime.Now.ToFileTimeUtc();
                image.Save(imgName.ToString(), ImageFormat.Jpeg);
                Process.Start(imgName.ToString());
            }

        }
        #endregion

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

            if (!string.IsNullOrEmpty(ProfilePic))
                noServerLoginPacket.Avator = File.ReadAllBytes(ProfilePic);

            noServerLoginPacket.IsReply = false;
            chatService.InvokeBroadcastPacketEvent(noServerLoginPacket);

            UserMode = UserModes.Chat;
            Thread Listener = new Thread(new ThreadStart(UdpListen));
            Listener.IsBackground = true;
            Listener.Start();

            return true;
        }

        private void UdpListen()
        {
            Console.WriteLine("Listener is on.");
            UdpClient rcvClient = new UdpClient(20000);

            IPEndPoint remote = new IPEndPoint(IPAddress.Any, 0);

            while (true)
            {
                var rcvResult = rcvClient.Receive(ref remote);
                NoServerPacketHandler.INSTANCE.INVOKE(PacketCodec.INSTANCE.Decode(rcvResult));
            }
        }

        #endregion

        #region NoServerLogoutCommand
        private ICommand _noServerLogoutCommand;
        public ICommand NoServerLogoutCommand
        {
            get
            {
                return _noServerLogoutCommand ?? (_noServerLogoutCommand =
                  new RelayCommandAsync(() => NoServerLogout()));
            }
        }

        private async Task NoServerLogout()
        {
            NoServerLogoutPacket logoutPacket = new NoServerLogoutPacket();
            logoutPacket.UserName = UserName;

            chatService.InvokeBroadcastPacketEvent(logoutPacket);
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
