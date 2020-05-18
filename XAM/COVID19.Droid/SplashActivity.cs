
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.App;

namespace Perimeter.Droid
{
    //[Activity(Theme = "@style/SplashTheme", MainLauncher = true,NoHistory = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    [Activity(Theme = "@style/Splash", MainLauncher = true, NoHistory = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class SplashActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            //http://www.c-sharpcorner.com/uploadfile/oussamaalrifai/xamarin-forms-android-workaround-for-splash-screen-with-lo/

            var intent = new Intent(this, typeof(MainActivity));

            StartActivity(intent);

            Finish();
            // Create your application here
        }

        protected override void OnResume()
        {
            base.OnResume();
        }

        // Prevent the back button from canceling the startup process
        public override void OnBackPressed() { }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
        }

        private void PreLoad()
        {

            StartActivity(new Intent(Application.Context, typeof(MainActivity)));

        }

    }
}