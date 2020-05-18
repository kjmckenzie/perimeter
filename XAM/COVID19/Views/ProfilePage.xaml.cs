using Perimeter.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Perimeter.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class ProfilePage : ContentPage
    {
     
        public ProfilePage(bool child = false)
        {
            InitializeComponent();

            if(child)
            {
                btnContinue.IsVisible = false;

            } else
            {
                NavigationPage.SetHasNavigationBar(this, false);

            }


            entryName.KeyBoardAction = new Command( () =>
            {
                if (Device.RuntimePlatform == Device.Android)
                    entryName.Unfocus();
                
               // btnContinue_Clicked(null,null);

            });


            seg.SelectedSegment = App.IsInfectedValue.ToLower().Equals("true") ? 1 : 0;

            
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (App.IsInfectedValue.ToLower().Equals("true"))
                seg.SelectedSegment = 1;
            else
                seg.SelectedSegment = 0;

            if (!string.IsNullOrEmpty(App.DeviceNicknameValue))
                entryName.Text = App.DeviceNicknameValue;
            else
                entryName.Focus();

        }


        protected override async void OnDisappearing()
        {
            base.OnDisappearing();

            await UpdateProfile();
        }

        private async void btnContinue_Clicked(object sender, EventArgs e)
        {
           await  UpdateProfile();

            //this.Navigation.PushModalAsync( new ShieldPage());       
            Application.Current.MainPage = new NavigationPage(new ShieldPage());
        }

       private async Task UpdateProfile()
        {
            if (!string.IsNullOrEmpty(entryName.Text))
            {
                if(App.CurrentUser==null)
                {
                    App.CurrentUser = new User();
                }

                App.CurrentUser.UserId = entryName.Text;
                App.CurrentUser.IsInfected = seg.SelectedSegment == 0 ? false : true;

                App.DeviceMACValue = App.CurrentUser.Id;
                App.DeviceNicknameValue = entryName.Text;
                //App.DeviceContactValue = entryEMail.Text;
                App.IsInfectedValue = seg.SelectedSegment == 0 ? "false" : "true";

                //refresh iOS Gatt
                //App.StartGattServer();

                await App.SetUser(App.CurrentUser);
            }

        }
    }
}