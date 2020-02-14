using IM_Client.Enums;
using IM_Client.Services;
using System;
using System.Windows.Input;
using IM_Client.Protocol.NoServerPacket;
using System.IO;
using IM_Client.Utils;
using System.Windows;
using System.Net;
using IM_Client.Commands;

namespace IM_Client.ViewModels
{
    public class FileTransferWindowViewModel : ViewModelBase
    {
        private IDialogService dialogService;
        private IChatService chatService;
        private ViewModelLocator locator = (ViewModelLocator)Application.Current.Resources["VMLocator"];

        public FileTransferWindowViewModel(IDialogService dialogService, IChatService chatService)
        {
            Console.WriteLine("Initial FileTransferWindowViewModel.");
            this.dialogService = dialogService;
            this.chatService = chatService;
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

        #region Send File Without Server
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
            var filePath = dialogService.OpenFile("选择文件");
            var fileName = Path.GetFileName(filePath);

            NoServerSendFilePacket requestPacket = new NoServerSendFilePacket()
            {
                IsSend = true,
                FileName = fileName
            };

            IPEndPoint remote = locator.MainWindowVM.SelectedParticipant.Remote;
            chatService.InvokeUnicastPacketEvent(requestPacket, remote);
        }
        #endregion
    }
}