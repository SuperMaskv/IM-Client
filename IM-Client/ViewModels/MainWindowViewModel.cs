using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IM_Client.Services;

namespace IM_Client.ViewModels
{
    public class MainWindowViewModel:ViewModelBase
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
            set { _profilePic = value;
                OnPropertyChanged();    
            }
        }
    }
}
