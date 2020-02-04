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
            container.RegisterType<IChatServices, ChatServices>();
        }

        public MainWindowViewModel MainWindowVM()
        {
            return container.Resolve<MainWindowViewModel>();
        }
    }
}