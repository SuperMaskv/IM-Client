using Unity;
using IM_Client.Services;
using IM_Client.ViewModels;

namespace IM_Client.Utils
{
    public class ViewModelLocator
    {
        private UnityContainer container;

        public ViewModelLocator()
        {
            container = new UnityContainer();
            container.RegisterType<IChatService, ChatService>();
            container.RegisterType<IDialogService, DialogService>();
        }

        public MainWindowViewModel MainWindowVM
        {
            get { return container.Resolve<MainWindowViewModel>(); }
        }
    }
}