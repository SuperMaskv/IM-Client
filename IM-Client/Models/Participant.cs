using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IM_Client.ViewModels;

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

        private bool _isTyping;

        public bool IsTyping
        {
            get { return _isTyping; }
            set
            {
                _isTyping = value;
                OnPropertyChanged();
            }
        }

        public Participant()
        {
            ChatMessages = new ObservableCollection<ChatMessage>();
        }
    }
}