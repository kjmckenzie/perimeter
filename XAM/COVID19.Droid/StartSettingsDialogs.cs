
using Android.Content;
using Perimeter.Droid;
using Perimeter.Interfaces;
using Xamarin.Forms;
using Application = Android.App.Application;

[assembly: Dependency(typeof(StartSettingsDialogs))]
namespace Perimeter.Droid
{
    public class StartSettingsDialogs : MainActivity, ISetttings
    {
        public void StartLocationSettings()
        {
            var intent = new Intent(Android.Provider.Settings.ActionLocationSourceSettings);
            intent.SetFlags(ActivityFlags.NewTask);
            Application.Context.StartActivity(intent);
        }

        public void StartBluetoothSettings()
        {

            var intent = new Intent(Android.Provider.Settings.ActionBluetoothSettings);
            intent.SetFlags(ActivityFlags.NewTask);
            Application.Context.StartActivity(intent);
        }
        public bool StartGattServer()
        {


            return true;
        }

        public string GetMACAdress(object device)
        {
            string result = "";
            try
            {
                Android.Bluetooth.BluetoothDevice ble = (Android.Bluetooth.BluetoothDevice)device;

                if (ble != null)
                {
                    result = ble.Address;
                }
            }
            catch (System.Exception ex)
            {

            }

            return result;
        }
    }
}