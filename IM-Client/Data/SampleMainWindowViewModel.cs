using IM_Client.Models;
using IM_Client.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace IM_Client.Data
{
    public class SampleMainWindowViewModel : ViewModelBase
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

        private ObservableCollection<Participant> _participants = new ObservableCollection<Participant>();

        public ObservableCollection<Participant> Participants
        {
            get { return _participants; }
            set
            {
                _participants = value;
                OnPropertyChanged();
            }
        }

        private Participant _selectedParticipant;

        public Participant SelectedParticipant
        {
            get { return _selectedParticipant; }
            set
            {
                _selectedParticipant = value;
                if (_selectedParticipant.HasSentNewMessage)
                {
                    _selectedParticipant.HasSentNewMessage = false;
                }

                OnPropertyChanged();
            }
        }

        public SampleMainWindowViewModel()
        {
            ObservableCollection<ChatMessage> someChatter = new ObservableCollection<ChatMessage>();
            someChatter.Add(new ChatMessage
            {
                Author = "Batman",
                Message = "What do you think about the Batmobile?",
                Time = DateTime.Now,
                IsOriginNative = true
            });
            someChatter.Add(new ChatMessage
            {
                Author = "Batman",
                Message = "Coolest superhero ride?",
                Time = DateTime.Now,
                IsOriginNative = true
            });
            someChatter.Add(new ChatMessage
            {
                Author = "Superman",
                Message = "Only if you don't have superpowers :P",
                Time = DateTime.Now
            });
            someChatter.Add(new ChatMessage
            {
                Author = "Batman",
                Message = "I'm rich. That's my superpower.",
                Time = DateTime.Now,
                IsOriginNative = true
            });
            someChatter.Add(new ChatMessage
            {
                Author = "Superman",
                Message = ":D Lorem Ipsum something blah blah blah blah blah blah blah blah. Lorem Ipsum something blah blah blah blah.",
                Time = DateTime.Now
            });
            someChatter.Add(new ChatMessage
            {
                Author = "Batman",
                Message = "I have no feelings",
                Time = DateTime.Now,
                IsOriginNative = true
            });
            someChatter.Add(new ChatMessage
            {
                Author = "Batman",
                Message = "How's Martha?",
                Time = DateTime.Now,
                IsOriginNative = true
            });

            Participants.Add(new Participant { UserName = "Superman", ChatMessages = someChatter, IsTyping = true, IsLoggedIn = true });
            Participants.Add(new Participant { UserName = "Wonder Woman", ChatMessages = someChatter, IsLoggedIn = false });
            Participants.Add(new Participant { UserName = "Aquaman", ChatMessages = someChatter, HasSentNewMessage = true });
            Participants.Add(new Participant { UserName = "Captain Canada", ChatMessages = someChatter, HasSentNewMessage = true });
            Participants.Add(new Participant { UserName = "Iron Man", ChatMessages = someChatter, IsTyping = true });

            SelectedParticipant = Participants.First();
        }
    }
}