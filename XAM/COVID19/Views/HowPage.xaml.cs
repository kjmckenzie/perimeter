using System;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Perimeter.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class HowPage : ContentPage
    {
        public HowPage()
        {
            InitializeComponent();

            NavigationPage.SetHasNavigationBar(this, false);

        }

        private void btnContinue_Clicked(object sender, EventArgs e)
        {
            //this.Navigation.PushModalAsync(new ProfilePage());
            this.Navigation.PushModalAsync(new CheckPage());

            //if (Device.RuntimePlatform == Device.iOS)
            //    this.Navigation.PushModalAsync(new CheckIOSPage());
            //else
            //    this.Navigation.PushModalAsync(new CheckPage());
        }
    }
}