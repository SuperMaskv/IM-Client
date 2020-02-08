using IM_Client.ViewModels;
using System.Collections.ObjectModel;

namespace IM_Client.Models
{
    public class Participant : ViewModelBase
    {
        public string UserName { get; set; }
        public byte[] Photo { get; set; }
        public ObservableCollection<ChatMessage> ChatMessages { get; set; }

        private bool _isLoggedIn = true;

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