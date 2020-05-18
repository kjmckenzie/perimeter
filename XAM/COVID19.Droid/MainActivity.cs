
using Android;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Perimeter.Enums;
using Perimeter.Interfaces;

using Plugin.Permissions;
using System;


namespace Perimeter.Droid
{
    [Activity(Label = "Perimeter", Icon = "@drawable/icon", Theme = "@style/MyTheme", MainLauncher = false, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Sensor)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        private BleServer _bleServer;
        private const int RequestLocationId = 0;
        private App parentApp;
        private readonly string[] Permissions =
       {
            Manifest.Permission.Bluetooth,
            Manifest.Permission.BluetoothAdmin,
            Manifest.Permission.BluetoothPrivileged,
            Manifest.Permission.AccessCoarseLocation,
            Manifest.Permission.AccessFineLocation
        };

        private readonly string[] LocationPermissions =
        {
            Manifest.Permission.AccessCoarseLocation,
            Manifest.Permission.AccessFineLocation
        };

        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);
            Rg.Plugins.Popup.Popup.Init(this, savedInstanceState);

            //CrossCurrentActivity. .Init(this, savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

            Xamarin.FormsMaps.Init(this, savedInstanceState);
            LeoJHarris.FormsPlugin.Droid.EnhancedEntryRenderer.Init(this);

            var width = Resources.DisplayMetrics.WidthPixels;
            var height = Resources.DisplayMetrics.HeightPixels;
            var density = Resources.DisplayMetrics.Density;

            App.ScreenWidth = (width - 0.5f) / density;
            App.ScreenHeight = (height - 0.5f) / density;

            //FFImageLoading.Forms.Platform.CachedImageRenderer.Init(enableFastRenderer: true);

            CheckPermissions();

            this.parentApp = new App();

            _bleServer = new BleServer(this.ApplicationContext);
            

            App.IsGATTServiceRunning = _bleServer.IsRunning;
            App.AndroidBluetoothDeviceName = _bleServer.GetDeviceName;
            
            LoadApplication(this.parentApp);


        }
        public override void OnBackPressed()
        {
            if (Rg.Plugins.Popup.Popup.SendBackPressed(base.OnBackPressed))
            {
                // Do something if there are some pages in the `PopupStack`
            }
            else
            {
                // Do something if there are not any pages in the `PopupStack`
            }
        }
        public void OpenSettings()
        {
            Xamarin.Forms.Forms.Context.StartActivity(new Android.Content.Intent(Android.Provider.Settings.ActionLocat‌​ionSourceSettings));
        }
        private void CheckPermissions()
        {
            bool minimumPermissionsGranted = true;
            if ((int)Build.VERSION.SdkInt >= 23)
            {

                foreach (string permission in Permissions)
                {
                    if (CheckSelfPermission(permission) != Permission.Granted) minimumPermissionsGranted = false;
                }

                // If one of the minimum permissions aren't granted, we request them from the user
                if (!minimumPermissionsGranted) RequestPermissions(Permissions, 0);

            }
            else
            {

            }

        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            if (requestCode == RequestLocationId)
            {
                if ((grantResults.Length == 2) && (grantResults[0] == (int)Android.Content.PM.Permission.Granted))
                {
                    var parent = parentApp as IPermissionsChanged;

                    parent.permissionsChanged(new GeneralPermission[] { GeneralPermission.Location });
                }

            }
            else
            {
                base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            }
        }

        protected override void OnStart()
        {
            base.OnStart();

            //if ((int)Build.VERSION.SdkInt >= 23)
            //{


            //    if (CheckSelfPermission(Manifest.Permission.AccessFineLocation) != Android.Content.PM.Permission.Granted)
            //    {
            //        RequestPermissions(LocationPermissions, RequestLocationId);
            //    }
            //    else
            //    {
            //        //this.showText("Permission already granted");
            //    }
            //}

        }

        protected override void OnResume()
        {
            base.OnResume();

        }

        protected override void OnRestart()
        {
            base.OnRestart();

        }

    }
    [Application(Debuggable = IS_DEBUG, AllowBackup = true, AllowClearUserData = true)]
    public class MyApplication : Application
    {
        public const Boolean IS_DEBUG =
#if DEBUG
         true;
#else
         false;
#endif

        protected MyApplication(IntPtr javaReference, JniHandleOwnership transfer)
           : base(javaReference, transfer)
        {
        }

    }

}
