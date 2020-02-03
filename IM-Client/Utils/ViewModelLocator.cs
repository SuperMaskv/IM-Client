using Unity;

namespace IM_Client.Utils
{
    public class ViewModelLocator
    {
        private UnityContainer container;

        public ViewModelLocator()
        {
            container = new UnityContainer();
        }
    }
}