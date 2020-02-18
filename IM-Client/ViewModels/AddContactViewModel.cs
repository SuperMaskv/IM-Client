using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using IM_Client.Views;
using IM_Client.Commands;

namespace IM_Client.ViewModels
{
    public class AddContactViewModel : ViewModelBase
    {
        public AddContactViewModel() { }

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
            return !IsDialogOpen;
        }

        private void OpenDialog()
        {
            DialogContent = new AddContactView();
            IsDialogOpen = true;
        }
        #endregion

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
        #endregion

        #region OK Button Command

        private string _contactName;
        public string ContactName
        {
            get { return _contactName; }
            set
            {
                _contactName = value;
                OnPropertyChanged();
            }
        }

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

        private ICommand _okButtonCommand;
        public ICommand OkButtonCommand
        {
            get
            {
                return _okButtonCommand ?? (_okButtonCommand
                    = new RelayCommand((o) => AddContact(), (o) => CanOk()));
            }
        }

        private bool CanOk()
        {
            return !string.IsNullOrEmpty(ContactName);
        }

        private void AddContact()
        {
            //关闭dialog
            IsDialogOpen = false;

        }

        #endregion
    }
}
