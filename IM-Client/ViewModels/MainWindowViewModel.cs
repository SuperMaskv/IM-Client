using IM_Client.Commands;
using IM_Client.Services;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace IM_Client.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private IChatServices chatSvc;

        public MainWindowViewModel(ChatServices chatSvc)
        {
            this.chatSvc = chatSvc;
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
            throw new NotImplementedException();
        }

        private async Task<bool> NoServerLogin()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
