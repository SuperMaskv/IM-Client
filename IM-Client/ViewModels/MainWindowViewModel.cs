using IM_Client.Commands;
using IM_Client.Services;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using IM_Client.Enums;

namespace IM_Client.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private IChatService chatService;
        private IDialogService dialogService;

        public MainWindowViewModel(ChatService chatSvc,DialogService dialogSvc)
        {
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
            UserMode = UserModes.Chat;

            new Task(() => chatService.UdpListen());

            return true;
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
