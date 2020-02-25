using IM_Client.Commands;
using IM_Client.Protocol;
using IM_Client.Protocol.ServerPacket;
using IM_Client.Utils;
using IM_Client.Views;
using System.Windows;
using System.Windows.Input;

namespace IM_Client.ViewModels
{
    public class EditContactAliasViewModel : ViewModelBase
    {
        public EditContactAliasViewModel()
        {
        }

        private ViewModelLocator locator = (ViewModelLocator)Application.Current.Resources["VMLocator"];

        private bool _isDialogOpen;

        public bool IsDialogOpen
        {
            get { return _isDialogOpen; }
            set
            {
                _isDialogOpen = value;
                OnPropertyChanged();
            }
        }

        private object _dialogContent;

        public object DialogContent
        {
            get { return _dialogContent; }
            set
            {
                _dialogContent = value;
                OnPropertyChanged();
            }
        }

        #region Open Dialog Command

        private ICommand _openDialogCommand;

        public ICommand OpenDialogCommand
        {
            get
            {
                return _openDialogCommand ?? (_openDialogCommand
                    = new RelayCommand((o) => OpenDialog(), (o) => CanOpenDialog()));
            }
        }

        private bool CanOpenDialog()
        {
            return !IsDialogOpen
                && locator.MainWindowVM.SelectedParticipant != null;
        }

        private void OpenDialog()
        {
            DialogContent = new EditContactAliasView();
            Info = locator.MainWindowVM.SelectedParticipant.TrueName;
            IsDialogOpen = true;
        }

        #endregion Open Dialog Command

        #region Cancel Button Command

        private ICommand _cancelButtonCommand;

        public ICommand CancelButtonCommand
        {
            get
            {
                return _cancelButtonCommand ?? (_cancelButtonCommand
                    = new RelayCommand((o) => Cancel()));
            }
        }

        private void Cancel()
        {
            IsDialogOpen = false;
        }

        #endregion Cancel Button Command

        #region OK Button Command

        private string _alias;

        public string Alias
        {
            get { return _alias; }
            set
            {
                _alias = value;
                OnPropertyChanged();
            }
        }

        private string _info;

        public string Info
        {
            get { return _info; }
            set
            {
                _info = "修改用户 " + value + " 的备注为：";
                OnPropertyChanged();
            }
        }

        private ICommand _editAliasCommand;

        public ICommand EditAliasCommmand
        {
            get
            {
                return _editAliasCommand ?? (_editAliasCommand
                    = new RelayCommand((o) => Edit(), (o) => CanEdit()));
            }
        }

        private bool CanEdit()
        {
            return !string.IsNullOrEmpty(Alias);
        }

        private void Edit()
        {
            //关闭dialog
            IsDialogOpen = false;
            //获取NetworkStream
            var stream = locator.MainWindowVM.TcpClient.GetStream();
            //构建报文
            EditContactAliasPacket packet = new EditContactAliasPacket()
            {
                token = locator.MainWindowVM.Token,
                userName = locator.MainWindowVM.UserName,
                contactName = locator.MainWindowVM.SelectedParticipant.TrueName,
                alias = Alias
            };
            //Encode
            var packetBytes = PacketCodec.INSTANCE.Encode(packet);
            //发送
            if (stream.CanWrite)
            {
                stream.Write(packetBytes, 0, packetBytes.Length);
            }
        }

        #endregion OK Button Command
    }
}