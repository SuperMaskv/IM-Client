using IM_Client.Commands;
using IM_Client.Enums;
using IM_Client.Protocol.NoServerPacket;
using IM_Client.Services;
using IM_Client.Utils;
using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace IM_Client.ViewModels
{
    public class FileTransferWindowViewModel : ViewModelBase
    {
        private IDialogService dialogService;
        private IChatService chatService;
        private FileTransfer.Send _send;
        private FileTransfer.Receive _receive;
        public CancellationTokenSource CTS;
        private ViewModelLocator locator = (ViewModelLocator)Application.Current.Resources["VMLocator"];

        public IPEndPoint REMOTE;

        public FileTransferWindowViewModel(IDialogService dialogService, IChatService chatService)
        {
            Console.WriteLine("Initial FileTransferWindowViewModel.");
            this.dialogService = dialogService;
            this.chatService = chatService;
            _send = new FileTransfer.Send();
            _receive = new FileTransfer.Receive();
            CTS = new CancellationTokenSource();

            _send.ProgressPercent += delegate (object o, double progress) { SendProgress = progress; };
            _send.StatusMessage += delegate (object o, string status) { SendStatus = status; };
            _send.Cleanup += delegate (object o, EventArgs args) { SendButtonEnable = true; };

            _receive.ProgressPercent += delegate (object o, double progress) { ReceiveProgress = progress; };
            _receive.StatusMessage += delegate (object o, string status) { ReceiveStatus = status; };
        }

        private SendFileMode _sendFileMode;

        public SendFileMode SendFileMode
        {
            get { return _sendFileMode; }
            set
            {
                _sendFileMode = value;
                OnPropertyChanged();
            }
        }

        private string _filePath;

        public string FilePath
        {
            get
            {
                return _filePath;
            }
            set
            {
                _filePath = value;
                OnPropertyChanged();
            }
        }

        private bool _isCheckSum;

        public bool IsCheckSum
        {
            get { return _isCheckSum; }
            set
            {
                _isCheckSum = value;
                OnPropertyChanged();
            }
        }

        #region Send File Without Server Command

        private ICommand _noServerSendFileCommand;

        public ICommand NoServerSendFileCommand
        {
            get
            {
                return _noServerSendFileCommand ?? (_noServerSendFileCommand
                  = new RelayCommand((o) => SendFileRequest()));
            }
        }

        public void SendFileRequest()
        {
            FilePath = dialogService.OpenFile("选择文件");

            var fileName = Path.GetFileName(FilePath);

            NoServerSendFilePacket requestPacket = new NoServerSendFilePacket()
            {
                IsSend = true,
                FileName = fileName,
                Author = locator.MainWindowVM.UserName
            };

            IPEndPoint remote = locator.MainWindowVM.SelectedParticipant.Remote;
            chatService.InvokeUnicastPacketEvent(requestPacket, remote);
        }

        #endregion Send File Without Server Command

        #region Send Fields

        private double _sendProgress;

        public double SendProgress
        {
            get { return _sendProgress; }
            set
            {
                _sendProgress = value;
                OnPropertyChanged();
            }
        }

        private string _sendStatus = "";

        public string SendStatus
        {
            get { return _sendStatus; }
            set
            {
                _sendStatus = value;
                OnPropertyChanged();
            }
        }

        private bool _sendButtonEnable = true;

        public bool SendButtonEnable
        {
            get
            {
                return _sendButtonEnable;
            }
            set
            {
                _sendButtonEnable = value;
                OnPropertyChanged();
            }
        }

        #endregion Send Fields

        #region Receive Fields

        private double _receiveProgress;

        public double ReceiveProgress
        {
            get { return _receiveProgress; }
            set
            {
                _receiveProgress = value;
                OnPropertyChanged();
            }
        }

        private string _receiveStatus;

        public string ReceiveStatus
        {
            get { return _receiveStatus; }
            set
            {
                _receiveStatus = value;
                OnPropertyChanged();
            }
        }

        #endregion Receive Fields

        public void Listen()
        {
            
            ThreadPool.QueueUserWorkItem(new WaitCallback((o) =>
            {
                CancellationToken token = (CancellationToken)o;
                if (token.IsCancellationRequested) return;
                _receive.ReceiveFile(IPAddress.Any, 8888, AppDomain.CurrentDomain.BaseDirectory);
            }), CTS.Token);
        }

        public void Send()
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback((o) =>
            {
                _send.SendFile(REMOTE.Address, 8888, FilePath, IsCheckSum);
            }));
        }

        #region Cancel Receive Command
        private ICommand _cancelReceiveCommand;
        public ICommand CancelReceiveCommand
        {
            get
            {
                return _cancelReceiveCommand ?? (_cancelReceiveCommand
                    = new RelayCommand((o) => CancelReceive()));
            }
        }

        private void CancelReceive()
        {
            CTS.Cancel();
            CTS.Dispose();
            foreach (var window in Application.Current.Windows)
            {
                if (window.GetType() == typeof(FileTransferWindow))
                {
                    ((FileTransferWindow)window).Close();
                }
            }
        }

        #endregion
    }
}