using IM_Client.ViewModels;
using System.Collections.ObjectModel;
using System.Net;

namespace IM_Client.Models
{
    public class Participant : ViewModelBase
    {
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

        public string TrueName { get; set; }
        public byte[] Photo { get; set; }
        public ObservableCollection<ChatMessage> ChatMessages { get; set; }

        private bool _isLoggedIn = true;

        public IPEndPoint Remote { get; set; }

        public bool IsLoggedIn
        {
            get { return _isLoggedIn; }
            set
            {
                _isLoggedIn = value;
                OnPropertyChanged();
            }
        }

        private bool _hasSentNewMessage;

        public bool HasSentNewMessage
        {
            get { return _hasSentNewMessage; }
            set
            {
                _hasSentNewMessage = value;
                OnPropertyChanged();
            }
        }

        public Participant()
        {
            ChatMessages = new ObservableCollection<ChatMessage>();
        }
    }
}