using IM_Client.Commands;
using IM_Client.Enums;
using IM_Client.Models;
using IM_Client.Protocol;
using IM_Client.Protocol.Handler;
using IM_Client.Protocol.NoServerPacket;
using IM_Client.Services;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using IM_Client.Utils;
using System.Windows;

namespace IM_Client.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private IChatService chatService;
        private IDialogService dialogService;
        private int MAX_IMAGE_WIDTH = 1024;
        private int MAX_IMAGE_HEIGHT = 1024;

        public IPEndPoint REMOTE = new IPEndPoint(IPAddress.Any, 0);
        public UdpClient RcvCient;
        public IPEndPoint LOCAL;

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

        private string _textMessage;

        public string TextMessage
        {
            get { return _textMessage; }
            set
            {
                _textMessage = value;
                OnPropertyChanged();
            }
        }

        private bool _isNoServer;

        public bool IsNoServer
        {
            get
            {
                return _isNoServer;
            }
            set
            {
                _isNoServer = value;
                OnPropertyChanged();
            }
        }

        private bool _hasServer;

        public bool HasServer
        {
            get { return _hasServer; }
            set
            {
                _hasServer = value;
                OnPropertyChanged();
            }
        }

        private ICommand _loginButtonCommand;

        public ICommand LoginButtonCommand
        {
            get
            {
                if (HasServer) return LoginCommand;
                else return NoServerLoginCommand;
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

        #endregion Fields

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

        #endregion SelectProfilePicCommand

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
            var img = chatMessage.Picture;
            if (string.IsNullOrEmpty(img) || !File.Exists(img)) return;
            Process.Start(img);
        }

        #endregion OpenImageCommand

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
            RcvCient = new UdpClient(20000);
            LOCAL = (IPEndPoint)RcvCient.Client.LocalEndPoint;

            NoServerLoginPacket noServerLoginPacket = new NoServerLoginPacket();
            noServerLoginPacket.UserName = UserName;
            noServerLoginPacket.Port = LOCAL.Port;

            if (!string.IsNullOrEmpty(ProfilePic))
                noServerLoginPacket.Avator = File.ReadAllBytes(ProfilePic);

            noServerLoginPacket.IsReply = false;
            chatService.InvokeBroadcastPacketEvent(noServerLoginPacket);

            IsNoServer = true;
            UserMode = UserModes.Chat;

            Thread Listener = new Thread(new ThreadStart(UdpListen));
            Listener.IsBackground = true;
            Listener.Start();
            return true;
        }

        private void UdpListen()
        {
            Console.WriteLine("Listener is on.");

            while (true)
            {
                var rcvResult = RcvCient.Receive(ref REMOTE);
                NoServerPacketHandler.INSTANCE.INVOKE(PacketCodec.INSTANCE.Decode(rcvResult));
            }
        }

        #endregion NoServerLoginCommand

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

        #endregion NoServerLogoutCommand

        #region NoServerTextMsgCommand

        private ICommand _noServerTextMsgCommand;

        public ICommand NoServerTextMsgCommand
        {
            get
            {
                return _noServerTextMsgCommand ?? (_noServerTextMsgCommand
                  = new RelayCommand((o) => SendTextMsg(), (o) => CanSendTextMsg()));
            }
        }

        private void SendTextMsg()
        {
            TextMessagePacket textMessagePacket = new TextMessagePacket();
            textMessagePacket.TextMessage = TextMessage;
            textMessagePacket.Author = UserName;

            chatService.InvokeUnicastPacketEvent(textMessagePacket, SelectedParticipant.Remote);

            ChatMessage chatMessage = new ChatMessage();
            chatMessage.Author = UserName;
            chatMessage.Message = TextMessage;
            chatMessage.Time = DateTime.Now;
            chatMessage.IsOriginNative = true;

            SelectedParticipant.ChatMessages.Add(chatMessage);

            TextMessage = string.Empty;
        }

        private bool CanSendTextMsg()
        {
            return !string.IsNullOrEmpty(TextMessage)
                        && TextMessage.Length > 2
                        && SelectedParticipant != null
                        && SelectedParticipant.IsLoggedIn == true;
        }

        #endregion NoServerTextMsgCommand

        #region Send Picture Message Without Server

        private ICommand _noServerPicMsgCommand;

        public ICommand NoServerPicMsgCommand
        {
            get
            {
                return _noServerPicMsgCommand ?? (_noServerPicMsgCommand
                    = new RelayCommandAsync(() => SendNoServerPicMsg(), (o) => CanSendNoServerPicMsg()));
            }
        }

        private bool CanSendNoServerPicMsg()
        {
            return SelectedParticipant != null
                        && SelectedParticipant.IsLoggedIn == true;
        }

        private async Task<bool> SendNoServerPicMsg()
        {
            var pic = dialogService.OpenFile("选择图片", "Images (*.jpg;*.png)|*.jpg;*.png");
            if (string.IsNullOrEmpty(pic)) return false;

            var img = await Task.Run(() => File.ReadAllBytes(pic));

            if (img.Length > 61440)
            {
                dialogService.ShowNotification("图片过大，请选择小于等于60KB的图片");
                return false;
            }

            try
            {
                NoServerPicMsgPacket picMsgPacket = new NoServerPicMsgPacket();
                picMsgPacket.Pic = img;
                picMsgPacket.Author = UserName;

                chatService.InvokeUnicastPacketEvent(picMsgPacket, SelectedParticipant.Remote);

                return true;
            }
            catch (Exception) { return false; }
            finally
            {
                ChatMessage chatMessage = new ChatMessage()
                {
                    Author = UserName,
                    Picture = pic,
                    Time = DateTime.Now,
                    IsOriginNative = true
                };
                SelectedParticipant.ChatMessages.Add(chatMessage);
            }
        }

        #endregion Send Picture Message Without Server

        #region Open File Transfer Window

        private ICommand _openFileTransferWindow;

        public ICommand OpenFileTransferWindow
        {
            get
            {
                return _openFileTransferWindow ?? (_openFileTransferWindow
                    = new RelayCommand((o) => OpenWindow(), (o) => CanOpenFileTransferWindow()));
            }
        }

        public bool CanOpenFileTransferWindow()
        {
            return SelectedParticipant != null;
        }

        public void OpenWindow()
        {
            FileTransferWindow window = new FileTransferWindow();
            var locator = (ViewModelLocator)Application.Current.Resources["VMLocator"];
            locator.FileTransferWindowViewModel.SendFileMode = SendFileMode.Send;
            locator.FileTransferWindowViewModel.REMOTE = SelectedParticipant.Remote;
            window.Show();
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

        #endregion LoginCommand
    }
}