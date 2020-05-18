using Xamarin.Forms;

namespace Perimeter.Interfaces
{
    public interface INavigate
	{
		void NavigateTo(Page page,int index=-1,INavigation nav=null);

        void PopNavigateTo(Page page, int index = -1);
        void PushModal(Page page, int index = -1);

        void PushNavigateTo(Page page);

        void PopAsync(int index = 2);
    }
}

