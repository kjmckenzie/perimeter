
using Perimeter.Models;
using Perimeter.ViewModels;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Perimeter.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class SidebarPage : PopupPage
    {
        public List<SidebarItem> menuList { get; set; }
        ShieldPageViewModel ViewModel { get; set; }
        public SidebarPage(ShieldPageViewModel vm)
        {
            InitializeComponent();
            menuList = new List<SidebarItem>();
            NavigationPage.SetHasNavigationBar(this, false);

            ViewModel = vm;

            var page1 = new SidebarItem() { Id="profile", Title = "Profile", Icon = "ic_user.png"};
            var page2 = new SidebarItem() { Id="device", Title = "Device", Icon = "ic_settings.png"};
            var page3 = new SidebarItem() { Id="chart",Title = "Chart", Icon = "ic_chart.png" };
            var page4 = new SidebarItem() { Id="about",Title = "About", Icon = "ic_help.png"};
          

            // Adding menu items to menuList
            menuList.Add(page1);
            menuList.Add(page2);
            menuList.Add(page3);
            menuList.Add(page4);

            lst.ItemsSource = menuList;
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();

          
        }

        private void lst_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            try
            {
                SidebarItem item = e.SelectedItem as SidebarItem;

                if(item!=null)
                {
                    switch (item.Id)
                    {
                        case "profile":
                            Application.Current.MainPage.Navigation.PushAsync(new ProfilePage(true));
                            PopupNavigation.Instance.PopAsync();
                            break;
                        case "device":
                            App.OpenBluetoothSettingsAsync();
                            PopupNavigation.Instance.PopAsync();
                            break;
                        case "about":
                            Application.Current.MainPage.Navigation.PushAsync(new AboutPage());

                            PopupNavigation.Instance.PopAsync();
                            break;
                        case "chart":
                            Application.Current.MainPage.Navigation.PushAsync(new ChartPage(ViewModel));
                            PopupNavigation.Instance.PopAsync();
                            break;
                        default:

                            break;
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}