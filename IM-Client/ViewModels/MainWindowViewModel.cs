using IM_Client.Commands;
using IM_Client.Enums;
using IM_Client.Models;
using IM_Client.Protocol;
using IM_Client.Protocol.Handler;
using IM_Client.Protocol.ServerPacket;
using IM_Client.Protocol.NoServerPacket;
using IM_Client.Services;
using IM_Client.Utils;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace IM_Client.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private IChatService chatService;
        private IDialogService dialogService;
        private int MAX_IMAGE_WIDTH = 1024;
        private int MAX_IMAGE_HEIGHT = 1024;
        private ViewModelLocator locator = (ViewModelLocator)Application.Current.Resources["VMLocator"];


        public IPEndPoint REMOTE = new IPEndPoint(IPAddress.Any, 0);
        public UdpClient RcvCient;
        public IPEndPoint LOCAL;
        public TcpClient TcpClient;

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

        //private string _userPwd;
        //public string UserPwd
        //{
        //    get { return _userPwd; }
        //    set
        //    {
        //        _userPwd = value;
        //        OnPropertyChanged();
        //    }
        //}

        private string _serverAddress;
        public string ServerAddress
        {
            get { return _serverAddress; }
            set
            {
                _serverAddress = value;
                OnPropertyChanged();
            }
        }

        private string _serverPort;
        public string ServerPort
        {
            get { return _serverPort; }
            set
            {
                _serverPort = value;
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

        #region command mode switcher
        private bool _hasServer;

        public bool HasServer
        {
            get { return _hasServer; }
            set
            {
                _hasServer = value;
                LoginButtonCommand = _hasServer ? LoginCommand : NoServerLoginCommand;
                ClosingEventCommand = _hasServer ? LogoutCommand : NoServerLogoutCommand;
                SendTxtMsgButtonCommand = _hasServer ? SendTextMsgCommand : NoServerTextMsgCommand;
                OnPropertyChanged();
            }
        }

        private ICommand _loginButtonCommand;

        public ICommand LoginButtonCommand
        {
            get
            {
                return _loginButtonCommand ?? (_loginButtonCommand
                    = NoServerLoginCommand);
            }

            set
            {
                _loginButtonCommand = value;
                OnPropertyChanged();
            }
        }

        private ICommand _closingEventCommand;
        public ICommand ClosingEventCommand
        {
            get
            {
                return _closingEventCommand ?? (_closingEventCommand
                    = NoServerLogoutCommand);
            }
            set
            {
                _closingEventCommand = value;
                OnPropertyChanged();
            }
        }

        private ICommand _sendTxtMsgButtonCommand;
        public ICommand SendTxtMsgButtonCommand
        {
            get
            {
                return _sendTxtMsgButtonCommand ?? (_sendTxtMsgButtonCommand
                    = NoServerTextMsgCommand);
            }
            set
            {
                _sendTxtMsgButtonCommand = value;
                OnPropertyChanged();
            }
        }
        #endregion

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
                if (_selectedParticipant != null)
                    if (_selectedParticipant.HasSentNewMessage)
                        _selectedParticipant.HasSentNewMessage = false;
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
                var img = System.Drawing.Image.FromFile(pic);
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
            //关闭应用程序
            Application.Current.Shutdown();
        }

        #endregion NoServerLogoutCommand

        #region NoServerTextMsgCommand

        private ICommand _noServerTextMsgCommand;

        public ICommand NoServerTextMsgCommand
        {
            get
            {
                return _noServerTextMsgCommand ?? (_noServerTextMsgCommand
                  = new RelayCommand((o) => SendTextMsgWithoutServer(), (o) => CanSendTextMsgWithoutServer()));
            }
        }

        private void SendTextMsgWithoutServer()
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

        private bool CanSendTextMsgWithoutServer()
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
                    new RelayCommand((o) => Login(o), (o) => CanLogin()));
            }
        }

        private bool CanLogin()
        {
            IPAddress ipTest;
            int intTest;
            return !string.IsNullOrEmpty(UserName)
                //&& !string.IsNullOrEmpty(UserPwd)
                && !string.IsNullOrEmpty(ServerAddress)
                && !string.IsNullOrEmpty(ServerPort)
                && IPAddress.TryParse(ServerAddress, out ipTest)
                && Int32.TryParse(ServerPort, out intTest);
        }

        private Thread tcpPacketHandler;

        private void Login(object o)
        {
            try
            {
                var pwd = ((PasswordBox)o).Password;
                //与服务器建立tcp连接
                TcpClient = new TcpClient(ServerAddress, int.Parse(ServerPort));
                //获取stream对象
                NetworkStream stream = TcpClient.GetStream();
                //创建登录报文
                LoginPacket loginPacket = new LoginPacket()
                {
                    userName = UserName,
                    userPwd = pwd
                };
                //Encode报文
                var bytes = PacketCodec.INSTANCE.Encode(loginPacket);
                //发送报文
                if (stream.CanWrite)
                {
                    stream.Write(bytes, 0, bytes.Length);

                }
                //建立监听线程
                tcpPacketHandler = new Thread(new ThreadStart(Spliter));
                tcpPacketHandler.Start();
            }
            catch (Exception ex)
            {
                locator.InfoDialogViewModel.Info = ex.Message;
                locator.InfoDialogViewModel.OpenDialogCommand.Execute(new object());
            }
        }
        //tcp监听，拆包
        private void Spliter()
        {
            try
            {
                NetworkStream stream = TcpClient.GetStream();
                while (stream.CanRead)
                {
                    if (!stream.DataAvailable) continue;
                    //尝试读取报文头部
                    byte[] headerBytes = new byte[8];
                    stream.Read(headerBytes, 0, 8);

                    byte[] magicBytes = new byte[2];
                    magicBytes[0] = headerBytes[1];
                    magicBytes[1] = headerBytes[0];
                    //检查报文头部的合法性
                    if (BitConverter.ToInt16(magicBytes, 0) != unchecked((short)0xabcd))
                    {
                        TcpClient.Close();
                        TcpClient.Connect(ServerAddress, int.Parse(ServerPort));
                        continue;
                    }
                    //读取报文body
                    byte[] length = new byte[] { headerBytes[7], headerBytes[6], headerBytes[5], headerBytes[4] };

                    var bodyLength = BitConverter.ToUInt32(length, 0);
                    byte[] bodyBytes = new byte[bodyLength];
                    stream.Read(bodyBytes, 0, (int)bodyLength);
                    //合并header和body
                    byte[] packetBytes = new byte[bodyLength + 8];
                    Buffer.BlockCopy(headerBytes, 0, packetBytes, 0, 8);
                    Buffer.BlockCopy(bodyBytes, 0, packetBytes, 8, (int)bodyLength);
                    //将合并后的报文交给解码器
                    var packet = PacketCodec.INSTANCE.Decode(packetBytes);
                    //将报文交给处理器处理
                    ServerPacketHandler.INSTANCE.handlers?.Invoke(packet);
                }
            }
            catch (Exception ex)
            {
                App.Current.Dispatcher.Invoke(delegate
                {
                    locator.InfoDialogViewModel.Info = ex.Message;
                    locator.InfoDialogViewModel.OpenDialogCommand.Execute(new object());
                });
            }
        }

        public long Token;

        #endregion LoginCommand

        #region Logout Command
        private ICommand _logoutCommand;
        public ICommand LogoutCommand
        {
            get
            {
                return _logoutCommand ?? (_logoutCommand
                    = new RelayCommand((o) => Logout()));
            }
        }

        private void Logout()
        {
            try
            {
                //获取NetworkStream
                var stream = TcpClient.GetStream();
                //创建Logout报文
                LogoutPacket logoutPacket = new LogoutPacket();
                logoutPacket.token = Token;
                logoutPacket.userName = UserName;
                //Encode报文
                var packetBytes = PacketCodec.INSTANCE.Encode(logoutPacket);
                //发送报文
                if (stream.CanWrite)
                    stream.Write(packetBytes, 0, packetBytes.Length);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                //关闭tcp连接
                TcpClient.Close();
                //关闭监听线程
                tcpPacketHandler.Abort();
                //关闭Application
                Application.Current.Shutdown();
            }

        }
        #endregion

        #region Remove Contact Command
        private ICommand _removeContactCommand;
        public ICommand RemoveContactCommand
        {
            get
            {
                return _removeContactCommand ?? (_removeContactCommand
                    = new RelayCommand((o) => RemoveContact(), (o) => CanRemoveContact()));
            }
        }

        private bool CanRemoveContact()
        {
            return SelectedParticipant != null;
        }

        private void RemoveContact()
        {
            //获取NetworkStream
            var stream = TcpClient.GetStream();
            //构建报文
            RemoveContactPacket removeContactPacket = new RemoveContactPacket()
            {
                token = Token,
                userName = UserName,
                contactName = SelectedParticipant.TrueName
            };
            //Encode
            var packetBytes = PacketCodec.INSTANCE.Encode(removeContactPacket);
            //发送报文
            if (stream.CanWrite)
            {
                stream.Write(packetBytes, 0, packetBytes.Length);
            }
        }
        #endregion

        #region Send Text Message With Server
        private ICommand _sendTextMsgCommand;
        public ICommand SendTextMsgCommand
        {
            get
            {
                return _sendTextMsgCommand ?? (_sendTextMsgCommand
                    = new RelayCommand((o) => SendTextMsg(), (o) => CanSendTextMsg()));
            }
        }

        private bool CanSendTextMsg()
        {
            return !string.IsNullOrEmpty(TextMessage)
                && SelectedParticipant != null;
        }
        private void SendTextMsg()
        {
            //获取NetworkStream
            var stream = TcpClient.GetStream();
            //构建报文
            ToUserMessagePacket textMsg = new ToUserMessagePacket()
            {
                token = Token,
                msgSender = UserName,
                msgRecipient = SelectedParticipant.TrueName,
                msgContent = TextMessage
            };
            //Encode
            var packetBytes = PacketCodec.INSTANCE.Encode(textMsg);
            //发送报文
            if (stream.CanWrite)
            {
                stream.Write(packetBytes, 0, packetBytes.Length);
            }
            //在聊天框显示发送的消息
            SelectedParticipant.ChatMessages.Add(new ChatMessage()
            {
                Author = UserName,
                Message = TextMessage,
                Time = DateTime.Now,
                IsOriginNative = true
            });
            //清空输入框
            TextMessage = string.Empty;
        }
        #endregion
    }
}