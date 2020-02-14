﻿using IM_Client.Services;
using IM_Client.ViewModels;
using System;
using Unity;

namespace IM_Client.Utils
{
    public class ViewModelLocator
    {
        private UnityContainer container;

        public ViewModelLocator()
        {
            Console.WriteLine("ViewModelLocator is initialed.");
            container = new UnityContainer();
            container.RegisterType<IChatService, ChatService>();
            container.RegisterType<IDialogService, DialogService>();
        }

        private MainWindowViewModel mainWindowViewModel;

        public MainWindowViewModel MainWindowVM
        {
            get
            {
                return mainWindowViewModel ?? (mainWindowViewModel
                    = container.Resolve<MainWindowViewModel>());
            }
        }

        private FileTransferWindowViewModel _fileTransferWindowViewModel;

        public FileTransferWindowViewModel FileTransferWindowViewModel
        {
            get
            {
                return _fileTransferWindowViewModel ?? (_fileTransferWindowViewModel
                    = container.Resolve<FileTransferWindowViewModel>());
            }
        }
    }
}