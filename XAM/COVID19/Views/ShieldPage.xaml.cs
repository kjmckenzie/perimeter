using Perimeter.Controls;
using Perimeter.Interfaces;
using Perimeter.Models;
using Perimeter.ViewModels;
using Plugin.BLE;
using Plugin.BLE.Abstractions;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.Exceptions;
using Plugin.Geolocator.Abstractions;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xam.Plugin;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;

namespace Perimeter.Views
{

    public partial class ShieldPage : ContentPage, INotifyPropertyChanged
    {
        public PopupMenu Popup;


        
        ObservableCollection<Plugin.BLE.Abstractions.Contracts.IDevice> deviceList;

        ASCIIEncoding ascii = new ASCIIEncoding();

        double _drawerHeight;
      
        private string _status;
        public string Status { get { return _status; } set { _status = value; OnPropertyChanged(); } }

        bool isFirstTime { get; set; }

        bool isTimerRunning { get; set; }

        bool isEnabled { get; set; }
        private Xamarin.Essentials.Location LastLocation { get; set; }


        int MaximumHeight = 50;

        public ShieldPageViewModel ShieldPageVM => ShieldPageViewModel.Instance;

        public ShieldPage()
        {
            InitializeComponent();

            NavigationPage.SetHasNavigationBar(this, false);

            BindingContext = ShieldPageVM;

            LastLocation = App.DefaultLocation != null ? new Xamarin.Essentials.Location(App.DefaultLocation.Latitude, App.DefaultLocation.Longitude) : new Xamarin.Essentials.Location(0, 0);

            isFirstTime = true;

            if (App.DefaultLocation != null)
                ShieldPageVM.MapPosition = App.DefaultLocation;

            CrossBluetoothLE.Current.Adapter.DeviceDiscovered += async (s, a) =>
            {
                bool seen = App.DevicesSeen.Contains(a.Device.Id);
                int rssi = a.Device.Rssi;

                if (!seen)               
                {
                   
                    deviceList.Add(a.Device);
                    IDevice device = a.Device;

                    string guid = device.Id.ToString();

                    IService service = null;
                    string nickname = "";
                    string id = "";
                    string infected = "";

                    {
                        bool isAppleDevice = HasAppleManHeader(device);

                        try
                        {

                            if (isAppleDevice)
                            {
                                try
                                {
                                    CancellationTokenSource cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
                                    CancellationToken token = cts.Token;
                                    token.ThrowIfCancellationRequested();

                                    await CrossBluetoothLE.Current.Adapter.ConnectToDeviceAsync(device, new ConnectParameters(false), token);
                                }
                                catch (Exception exApple)
                                {

                                }

                            }
                            else if (HasServiceUUIDInManHeader(device))
                            {

                                for (int i = 0; i < 3; i++)
                                {

                                    CancellationTokenSource cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
                                    CancellationToken token = cts.Token;
                                    token.ThrowIfCancellationRequested();

                                    try
                                    {
                
                                        await CrossBluetoothLE.Current.Adapter.ConnectToDeviceAsync(device, new ConnectParameters(false), token);
                                        i = 3;
                                    }
                                    catch (Exception exOther)
                                    {

                                    }

                                }


                            }

                            if (device.State == DeviceState.Connected)
                            {
                                service = await device.GetServiceAsync(Guid.Parse(App.ServiceId));
                                if (service != null)
                                {
                                    try
                                    {
                                        id = await GetCharacteristicValue(App.DeviceMAC, service);
                                    }
                                    catch (Exception) { }
                                    try
                                    {
                                        nickname = await GetCharacteristicValue(App.DeviceNickname, service);
                                    }
                                    catch (Exception) { }
                                    try
                                    {
                                        infected = await GetCharacteristicValue(App.IsInfected, service);
                                    }
                                    catch (Exception) { }


                                    if (!string.IsNullOrEmpty(id))
                                        AddItem(device, nickname, infected, id, rssi);
                                }

                                try
                                {
                                    await CrossBluetoothLE.Current.Adapter.DisconnectDeviceAsync(device);
                                }
                                catch (Exception ex) { }

                            }


                        }
                        catch (Exception exCancel)
                        {

                        }

                    }

                }
                else
                {
                  
                    if(ShieldPageVM.DeviceList.Count(x=>x.GUID.Equals(a.Device.Id.ToString()))>0)
                    {
                        string infected = "";

                        try
                        {
                            CancellationTokenSource cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
                            CancellationToken token = cts.Token;
                            token.ThrowIfCancellationRequested();

                            await CrossBluetoothLE.Current.Adapter.ConnectToDeviceAsync(a.Device, new ConnectParameters(false), token);
                            IService service = await a.Device.GetServiceAsync(Guid.Parse(App.ServiceId));
                            if (service != null)
                                infected = await GetCharacteristicValue(App.IsInfected, service);

                        }
                        catch { }
                       
                        AddItem(a.Device,infected:infected,rssi:rssi);
                    }
                        
                }

            };
            CrossBluetoothLE.Current.StateChanged += Current_StateChanged;
            CrossBluetoothLE.Current.Adapter.ScanTimeout = 15000;
            CrossBluetoothLE.Current.Adapter.ScanMode = ScanMode.Balanced;
            CrossBluetoothLE.Current.Adapter.ScanTimeoutElapsed +=  (s, a) =>
            {
                ShieldPageVM.IsScanning = false;

            };

            MessagingCenter.Subscribe<ShieldPage>(this, "Refresh",  (sender) =>
            {
                ShieldPageVM.IsScanning = false;

              

            });

            MessagingCenter.Subscribe<ShieldPage>(this, "ReloadProfile",  (sender) =>
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    lblName.Text = App.DeviceNicknameValue;

                    if (App.IsInfectedValue.ToLower().Equals("true"))
                    {
                        lblHealth.Text = "I have symptoms";
                        imgInfection.Source = "assets_infectionon.png";
                    }
                    else
                    {
                        lblHealth.Text = "No symptoms";
                        imgInfection.Source = "assets_infectionoff.png";
                    }

                    var items = await App.GetSIHistory();
                    //var items = App.GetSIDummyData();

                    if (items != null && items.Count > 0)
                    {
                        ShieldPageVM.DeviceList.Clear();

                        foreach (Item item in items)
                            ShieldPageVM.DeviceList.Add(item);

                        drawer.IsVisible = true;
                    }
                });
            });

            deviceList = new ObservableCollection<IDevice>();
           
          
            SizeChanged += ShieldPage_SizeChanged;
        }

        private void ShieldPage_SizeChanged(object sender, EventArgs e)
        {
            double percentage = ((250 *100)/this.Height);
            string percent = string.Format("0.{0}", Convert.ToInt16((250 * 100) / this.Height));
            _drawerHeight = Convert.ToDouble(percent);


            drawer.LockStates = new double[] { 0, _drawerHeight};
        
            
        }

        private void Current_StateChanged(object sender, Plugin.BLE.Abstractions.EventArgs.BluetoothStateChangedArgs e)
        {
            if(!e.NewState.Equals(BluetoothState.On))
            {

            }else
            {

            }
        }

        bool HasServiceUUIDInManHeader(IDevice device)
        {
            bool result = false;

            try
            {
                Plugin.BLE.Abstractions.AdvertisementRecord ar = device.AdvertisementRecords.FirstOrDefault(x => x.Type.Equals(Plugin.BLE.Abstractions.AdvertisementRecordType.UuidsComplete128Bit));
                if (device.Name != null)
                {
               
                    byte[] bytes = ar.Data;
                    string uuid = ByteArrayToString(bytes);
                    string serviceUUID = App.ServiceId.Replace("-", "");
                    if (uuid.ToLower().Equals(serviceUUID.ToLower()))
                        result = true;
               }
            }
            catch (Exception ex)
            {

            }

            return result;

        }
        bool HasAppleManHeader(IDevice device)
        {
            bool result = false;

            try
            {

                if (device.AdvertisementRecords != null)
                {
                    Plugin.BLE.Abstractions.AdvertisementRecord ar = device.AdvertisementRecords.FirstOrDefault(x => x.Type.Equals(Plugin.BLE.Abstractions.AdvertisementRecordType.ManufacturerSpecificData));
                    if (ar != null && ar.Data.Length > 0 && ar.Data[0]==0x004C)
                    {
                        result = true;
                    }

                }

            }
            catch (Exception ex)
            {


            }

            return result;
        }

        int GetTxPower(IDevice device)
        {
            int result = -59;

            try
            {
                if (device.AdvertisementRecords != null)
                {
                    Plugin.BLE.Abstractions.AdvertisementRecord ar = device.AdvertisementRecords.FirstOrDefault(x => x.Type.Equals(Plugin.BLE.Abstractions.AdvertisementRecordType.TxPowerLevel));
                    if (ar != null)
                    {
                        result = Convert.ToInt32(ByteArrayToString(ar.Data));
                        switch (result)
                        {
                            case 0:
                                result = -115;
                                break;
                            case 1:
                                result = -84;
                                break;
                            case 2:
                                result = -81;
                                break;
                            case 3:
                                result = -77;
                                break;
                            case 4:
                                result = -72;
                                break;
                            case 5:
                                result = -69;
                                break;
                            case 6:
                                result = -65;
                                break;
                            case 7:
                                result = -59;
                                break;


                        }

                    }

                }

            }
            catch (Exception ex)
            {


            }

            return result;
        }


        private bool OnTimerRefresh()
        {
            if (isEnabled)
            {
                Task.Run(async () =>
                {
                    await SaveSIHistoryAsync();
                    bool result = await DiscoverServicesAsync(this.LastLocation.Latitude, this.LastLocation.Longitude);

                    if (result)
                        App.VibrateDevice();

                });

            }

            return true;
        }
 
        async Task SaveSIHistoryAsync()
        {
            try
            {

                var filter = ShieldPageVM.DeviceList.Where(x => x.DateCreated > DateTime.Now.AddDays(-2)).ToList();
                var filterRemove = ShieldPageVM.DeviceList.Where(x => x.DateCreated < DateTime.Now.AddDays(-2)).ToList();

                if (filterRemove!=null)
                {
                    foreach(Item item in filterRemove)
                    {
                        ShieldPageVM.DeviceList.Remove(item);
                    }
                    
                }

                await App.SetSIHistory(filter);
            }
            catch (Exception ex)
            {

            }
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            MaximumHeight = Convert.ToInt32(this.Height * .6);

            Device.BeginInvokeOnMainThread(() =>
            {
                lblName.Text = App.DeviceNicknameValue;

                if (App.IsInfectedValue.ToLower().Equals("true"))
                {
                    lblHealth.Text = "I have symptoms";
                    imgInfection.Source = "assets_infectionon.png";
                }
                else
                {
                    lblHealth.Text = "No symptoms";
                    imgInfection.Source = "assets_infectionoff.png";
                }

            });

            if (isFirstTime)
            {
                isFirstTime = false;

            }

            try
            {

                var loc = await Geolocation.GetLocationAsync();

                if (loc != null)
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        this.LastLocation = loc;
                        ShieldPageVM.MapPosition = new Xamarin.Forms.Maps.Position(this.LastLocation.Latitude, this.LastLocation.Longitude);
                        //ShieldPageVM.MapPosition = new Xamarin.Forms.Maps.Position(51.500152, -0.126236);

                    });
                }

                customMap.IsShowingUser = true;
            }
            catch (Exception ex)
            {

            }

        }

        public async Task<bool> DiscoverServicesAsync(double lat = 0, double lng = 0)
        {
            bool result = false;
            Random rnd = new Random(DateTime.Now.Second);

            try
            {

                //lblStatus.Text = "Scanning ...";
                ShieldPageVM.IsScanning = true;
                deviceList.Clear();
                var _cancellationTokenSource = new CancellationTokenSource();
                CrossBluetoothLE.Current.Adapter.ScanMode = ScanMode.LowLatency;
                await CrossBluetoothLE.Current.Adapter.StartScanningForDevicesAsync(null,null,false,_cancellationTokenSource.Token);

            }
            catch (Exception ex)
            {
                isEnabled = true;
                btnTrace_Clicked(null,null);
            }

           
            return result;

        }
        bool AddItem(IDevice device, string nickname="",string infected="",string id="",int rssi=0)
        {
            bool result = false;

            try
            {
 
                Item[] items = ShieldPageVM.DeviceList.ToArray();

                
                {

                    Item oldItem = null;
                    if(string.IsNullOrEmpty(id))
                        oldItem = ShieldPageVM.DeviceList.FirstOrDefault(x => x.GUID.Equals(device.Id.ToString()));
                    else
                        oldItem = ShieldPageVM.DeviceList.FirstOrDefault(x => x.MAC.Equals(id));

                    int index = -1;
                   
                    if (oldItem != null && (oldItem.DateCreated - DateTime.Now).TotalHours>1)
                    {
                        index = -1;
                        infected = string.IsNullOrEmpty(infected) ? (oldItem.Infected ? "true" : "false") : infected;
                        nickname = oldItem.Nickname;
                        id = oldItem.MAC;
                    }                        
                    else
                    {
                        index = ShieldPageVM.DeviceList.IndexOf(oldItem);
                    }

                    if (index>=0)
                    {
                        ShieldPageVM.DeviceList[index].DateUpdated = DateTime.Now;
                        if (!string.IsNullOrEmpty(nickname) && ShieldPageVM.DeviceList[index].Nickname== "?????")
                            ShieldPageVM.DeviceList[index].Nickname = nickname;

                        if (!string.IsNullOrEmpty(infected))
                        {
                            if (infected.ToLower().Equals("true"))
                            {
                                ShieldPageVM.DeviceList[index].Infected = true;
                            }
                            else
                            {
                                ShieldPageVM.DeviceList[index].Infected = false;
                            }

                        }

                        ShieldPageVM.DeviceList[index].Longitude = this.LastLocation.Longitude;
                        ShieldPageVM.DeviceList[index].Latitude = this.LastLocation.Latitude;
                        
                        if(rssi!=0 && rssi > ShieldPageVM.DeviceList[index].RSSI)
                        {
                            ShieldPageVM.DeviceList[index].RSSI = rssi;
                            ShieldPageVM.DeviceList[index].TxPower = GetTxPower(device);
                        }

                        ShieldPageVM.DeviceList[index].TotalHits += 1;
                        ShieldPageVM.DeviceList.Move(index, 0);
                    }
                    else
                    {
                        if(!App.DevicesSeen.Contains(device.Id))
                            App.DevicesSeen.Add(device.Id);

                        Item item = new Item();

                        item.GUID = device.Id.ToString();
                        //string mac = 
                        //item.MAC = PutColon(mac,2).ToUpper();
                        item.MAC = id;

                        item.RSSI = rssi;
                        item.TxPower = GetTxPower(device);

                        if (string.IsNullOrEmpty(device.Name))
                        {
                            item.DeviceName = HasAppleManHeader(device) ? "Apple" : "Android";
                            item.IsApple = HasAppleManHeader(device) ? true : false;
                        } else
                        {
                            if (device.Name.Equals(App.ManufacturerNameValue))
                                item.DeviceName = "Apple";
                            else
                                item.DeviceName = device.Name;
                        }

                        //item.DeviceName = HasAppleManHeader(device) ? "Apple" : "Android";
                        item.Longitude = this.LastLocation.Longitude;
                        item.Latitude = this.LastLocation.Latitude;

                        //item.Manufacturer = await GetCharacteristicValue(App.ManufacturerName, service);
                        //item.Firmware = await GetCharacteristicValue(App.FirmwareVersion, service);
                        //item.Contact = await GetCharacteristicValue(App.DeviceContact, service);
                        //item.DeviceName = await GetCharacteristicValue(App.DeviceInfo, service);

                        if (string.IsNullOrEmpty(nickname))
                            item.Nickname = "?????";
                        else
                            item.Nickname = nickname;

                       if (infected.ToLower().Equals("true"))
                        {
                            item.Infected = true;
                            result = true;

                        }
                        else
                        {
                            item.Infected = false;
                        }
                        ShieldPageVM.DeviceList.Insert(0,item);

                    }

                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("UI Exception {0}", ex.Message);
            }

            Console.WriteLine("Device {0}:{1}", device.Name, device.Id.ToString());

            MessagingCenter.Send<ShieldPage>(this,"Refresh");

            return result;
            
        }

     
        public string ByteArrayToString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }
        async Task<string> GetCharacteristicValue(string guid, IService service)
        {
            string result = "";

            try
            {
                var characteristic = await service.GetCharacteristicAsync(Guid.Parse(guid));
                if (characteristic != null)
                {
                    byte[] data = null;

                    try
                    {
                        data = await characteristic.ReadAsync();
                    }
                    catch (Exception exAsync)
                    {

                    }

                    if (data != null && data.Length > 0)
                    {
                        result = Encoding.UTF8.GetString(data);

                    }
                    else
                    {
                        result = characteristic.StringValue;
                    }

                }
            }
            catch (Exception ex)
            {

            }

            Console.WriteLine("GetCharacteristicValue RQ Value:{0} GUID:{1}", result, guid);

            return result;
        }

        string GetCharacteristicValue(string guid)
        {
            string result = "";

            try
            {
                switch (guid.ToUpper())
                {
                    case App.ManufacturerName:
                        result = App.ManufacturerNameValue;
                        break;
                    case App.DeviceContact:
                        result = App.DeviceContactValue;
                        break;
                    case App.DeviceNickname:
                        result = App.DeviceNicknameValue;
                        break;
                    case App.FirmwareVersion:
                        result = App.FirmwareVersionValue;
                        break;
                    case App.IsInfected:
                        result = App.IsInfectedValue;
                        break;

                    default:

                        break;
                }
            }
            catch (Exception ex)
            {

            }

            return result;

        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

        }
        private void imgSettings_Tapped(object sender, EventArgs e)
        {

            App.OpenBluetoothSettingsAsync();
        }
        private async void imgMore_Tapped(object sender, EventArgs e)
        {
            var page = new SidebarPage(ShieldPageVM);

            await PopupNavigation.Instance.PushAsync(page);

      
        }

        void btnIconTapped_Clicked(object sender, EventArgs args)
        {
            try
            {
                INavigate rb = (INavigate)Application.Current.MainPage;

                TappedEventArgs e = args as TappedEventArgs;

            }
            catch (Exception ex)
            {

            }
        }

        public async Task StartListening()
        {
            if (App.Locator.IsListening) return;

            await App.Locator.StartListeningAsync(TimeSpan.FromSeconds(5), 10, true);
            App.Locator.PositionChanged += Locator_PositionChanged;
            App.Locator.PositionError += Locator_PositionError;
        }
        public async Task StopListening()
        {
            await App.Locator.StopListeningAsync();

            App.Locator.PositionChanged -= Locator_PositionChanged;
            App.Locator.PositionError -= Locator_PositionError;

        }
        private void Locator_PositionError(object sender, PositionErrorEventArgs e)
        {
            //lblError.Text = e.Error.ToString();
        }


        private void btnToggle_Clicked(object sender, EventArgs e)
        {
            try
            {
                drawer.IsExpanded = !drawer.IsExpanded;
                 
             

            }
            catch (Exception ex)
            {

            }

        }
        private void Locator_PositionChanged(object sender, Plugin.Geolocator.Abstractions.PositionEventArgs e)
        {
            try
            {
                var distance = Xamarin.Essentials.Location.CalculateDistance(
                                      new Xamarin.Essentials.Location(customMap.MapPosition.Latitude, customMap.MapPosition.Longitude),
                                      new Xamarin.Essentials.Location(e.Position.Latitude, e.Position.Longitude), DistanceUnits.Kilometers);

                if (distance > 0.2)
                {
                    this.LastLocation = new Xamarin.Essentials.Location(customMap.MapPosition.Latitude, customMap.MapPosition.Longitude);

                    //customMap.Pins[0].Position = new Xamarin.Forms.Maps.Position(e.Position.Latitude, e.Position.Longitude);
                    ShieldPageVM.MapPosition = new Xamarin.Forms.Maps.Position(e.Position.Latitude, e.Position.Longitude);

                }

            }
            catch (Exception ex)
            {
            }

        }

        private void lstGrid_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            try
            {
                var item = e.SelectedItem as Item;

                if (item != null)
                {
                    ShieldPageVM.Pins.Clear();

                    Pin pin = new Pin() { Label = item.Nickname, Position = new Xamarin.Forms.Maps.Position(item.Latitude, item.Longitude) };

                    ShieldPageVM.Pins.Add(pin);
                }

            }
            catch (Exception ex)
            {

            }
        }


        #region INotifyPropertyChanged

        public void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected bool SetProperty<T>(
         ref T backingStore, T value,
         [CallerMemberName]string propertyName = "",
         Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            NotifyPropertyChanged(propertyName);
            return true;
        }

        #endregion

        private async void btnTrace_Clicked(object sender, EventArgs e)
        {
            if (!isEnabled)
            {
                //RowDefinition row = grid.RowDefinitions[1];
                //row.Height = 250;

                btnTracePlay.IsVisible = false;
                btnTraceStop.IsVisible = true;

                isEnabled = true;

                if (!isTimerRunning)
                {
                    isTimerRunning = true;
                    ShieldPageVM.Status= "Starting up ...";
                    //lblStatus.Text = "Starting up ...";

                    //await DiscoverServicesAsync();
                    drawer.IsVisible = true;

                    await DiscoverServicesAsync();

                    Device.StartTimer(TimeSpan.FromMilliseconds(15000), OnTimerRefresh);
                }

                //AnimatedTextControl.Text = "SPACE - Enabled";

                await StartListening();

            }
            else
            {
                btnTracePlay.IsVisible = true;
                btnTraceStop.IsVisible = false;

                //disabled
                isEnabled = false;

                //AnimatedTextControl.Text = "SPACE - Disabled";
              
                await StopListening();

                MessagingCenter.Send<ShieldPage>(this, "Refresh");

            }

        }

        private void btnReset_Clicked(object sender, EventArgs e)
        {

            Device.BeginInvokeOnMainThread( async () =>
            {
                //lblStatus.Text = "";
                try
                {

                    ShieldPageVM.DeviceList.Clear();
                    ShieldPageVM.Status = "";
                    deviceList.Clear();
                    await App.SetSIHistory(new List<Item>());

                }
                catch (Exception ex)
                {

                }

               // lstGrid.ItemsSource = ShieldPageVM.DeviceList;
            });


        }

        private void lstGrid_ItemTapped(object sender, ItemTappedEventArgs e)
        {

            var item = e.Item as Item;

            if (item != null)
            {
                ShieldPageVM.Pins.Clear();

                Pin pin = new Pin() { Label = item.Nickname, Position = new Xamarin.Forms.Maps.Position(item.Latitude, item.Longitude) };

                ShieldPageVM.Pins.Add(pin);
            }

        }


        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            TappedEventArgs param = e as TappedEventArgs;

            var item = param.Parameter as Item;

            if (item != null && item.Latitude!=0 && item.Longitude!=0)
            {
                ShieldPageVM.Pins.Clear();

                Pin pin = new Pin() { Label = item.Nickname, Position = new Xamarin.Forms.Maps.Position(item.Latitude, item.Longitude) };

                ShieldPageVM.MapPosition = pin.Position;
                ShieldPageVM.Pins.Add(pin);

                drawer.IsExpanded = false;
            }

        }
    }
}