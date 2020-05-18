using System.Collections.Specialized;
using System.Windows.Input;
using Perimeter.Interfaces;
using Perimeter.iOS;
using Foundation;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(StartSettingsDialogs))]
namespace Perimeter.iOS
{
    public class StartSettingsDialogs : ISetttings
    {
        public void StartLocationSettings()
        {
            var url = new NSUrl($"app-settings:");
            UIApplication.SharedApplication.OpenUrl(url);
        }
        public void StartBluetoothSettings()
        {
            var url = new NSUrl($"app-settings:");
            UIApplication.SharedApplication.OpenUrl(url);
        }
        public bool StartGattServer()
        {
            BleGattServerManager.BleGattServerCreate();

            return true;
        }
        public string GetMACAdress(object device)
        {
            string result = "";
            try
            {
                //Android.Bluetooth.BluetoothDevice ble = (Android.Bluetooth.BluetoothDevice)device;

                //if (ble != null)
                //{
                //    result = ble.Address;
                //}
            }
            catch (System.Exception ex)
            {

            }

            return result;
        }
    }
}