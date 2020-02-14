using IM_Client.Enums;
using IM_Client.Services;

namespace IM_Client.ViewModels
{
    public class SendFileWindowViewModel : ViewModelBase
    {
        private IDialogService dialogService;

        public SendFileWindowViewModel(IDialogService dialogService)
        {
            this.dialogService = dialogService;
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
    }
}