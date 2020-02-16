using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IM_Client.Commands;
using System.Windows.Input;
using IM_Client.Views;

namespace IM_Client.ViewModels
{
    public class InfoDialogViewModel : ViewModelBase
    {
        public InfoDialogViewModel()
        {

        }

        private bool _isInfoDialogOpen;
        public bool IsInfoDialogOpen
        {
            get { return _isInfoDialogOpen; }
            set
            {
                _isInfoDialogOpen = value;
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

        private string _info;
        public string Info
        {
            get { return _info; }
            set { _info = value; }
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

        private void OpenDialog()
        {
            IsInfoDialogOpen = true;
            DialogContent = new InfoDialogView();
        }

        private bool CanOpenDialog()
        {
            return DialogContent != null
                && !IsInfoDialogOpen;
        }
        #endregion

        private ICommand _cancelCommand;
        public ICommand CancelCommand
        {
            get
            {
                return _cancelCommand ?? (_cancelCommand
                    = new RelayCommand((o) => CancelDialog()));
            }
        }

        private void CancelDialog()
        {
            IsInfoDialogOpen = false;
        }
    }
}
