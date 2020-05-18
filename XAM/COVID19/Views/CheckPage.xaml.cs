
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Perimeter.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class CheckPage : ContentPage
    {
        public CheckPage()
        {
            InitializeComponent();

            NavigationPage.SetHasNavigationBar(this, false);

            MessagingCenter.Subscribe<CheckPage>(this, "GotoBluetoothSettings",  (sender)  =>
            {
                App.OpenBluetoothSettingsAsync();
            });

            MessagingCenter.Subscribe<CheckPage>(this, "GotoLocationSettings", (sender) =>
            {
                App.OpenLocationSettingsAsync();
            });

        }
        protected override void OnAppearing()
        {
            base.OnAppearing();

            Device.StartTimer(TimeSpan.FromMilliseconds(3000), OnTimerCancelAnimation);

        }
        private bool OnTimerCancelAnimation()
        {
            pulse.IsRun = false;
            pulse.IsVisible = false;
            scroll.IsVisible = true;

            {
                if (Plugin.BLE.CrossBluetoothLE.Current.IsAvailable)
                {
                    imgBluetoothAvailable.Source = "assets_tick.png";
                }
                imgBluetoothAvailable.IsVisible = true;

                if (Device.RuntimePlatform == Device.iOS && App.IsGATTServiceRunning)
                {
                    imgBluetoothOn.Source = "assets_tick.png";
                }
                else if (Plugin.BLE.CrossBluetoothLE.Current.IsOn)
                {
                    imgBluetoothOn.Source = "assets_tick.png";
                }
                imgBluetoothOn.IsVisible = true;
                imgBluetoothSettings.IsVisible = true;

                if (App.DefaultLocation!=null)
                {
                    imgLocationFeatures.Source = "assets_tick.png";
                }
                imgLocationFeatures.IsVisible = true;
                imgLocationSettings.IsVisible = true;

                if (App.IsGATTServiceRunning)
                {
                    //good
                    imgBluetoothFeatures.Source = "assets_tick.png";

                    if (Device.RuntimePlatform == Device.iOS)
                    {
                        lblStatus.Text = string.Format("Perimeter broadcasting as");
                        lblBroadcast.Text = string.Format("{0}", App.ManufacturerNameValue);
                    }
                    else
                    {
                        lblStatus.Text = string.Format("Perimeter broadcasting as");
                        lblBroadcast.Text = string.Format("{0}", App.AndroidBluetoothDeviceName);
                    }

                    btnContinue.IsVisible = true;
                }
                else
                {
                    //display status info
                    if (!Plugin.BLE.CrossBluetoothLE.Current.IsOn && Plugin.BLE.CrossBluetoothLE.Current.IsAvailable)
                    {
                        lblStatus.Text = string.Format("Try (1) restarting your phone, (2) switch Bluetooth Off and then On again, (3) rerun this app, to see if the issues resolve themselves?");

                    }
                    else if (!Plugin.BLE.CrossBluetoothLE.Current.IsAvailable)
                    {
                        lblStatus.Text = string.Format("Bluetooth does not seem to be supported on your device, so it is not compatible with this app - Sorry!");
                    }
                    else if (!App.IsGATTServiceRunning)
                    {
                        lblStatus.Text = string.Format("Bluetooth LE Features do not seem to be supported on your device so it is incompatible with this app - Sorry!");
                    }
                }
                imgBluetoothFeatures.IsVisible = true;

            }
            return false;
        }
        private void btnLocationSettings_Clicked(object sender, EventArgs e)
        {
            MessagingCenter.Send<CheckPage>(this, "GotoLocationSettings");
        }

        private void btnBluetoothSettings_Clicked(object sender, EventArgs e)
        {
            MessagingCenter.Send<CheckPage>(this, "GotoBluetoothSettings");
        }
        private void btnContinue_Clicked(object sender, EventArgs e)
        {
            this.Navigation.PushModalAsync(new ProfilePage());
        }
    }
}