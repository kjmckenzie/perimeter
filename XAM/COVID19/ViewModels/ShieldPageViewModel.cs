using Perimeter.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Xml.Linq;
using Xamarin.Forms.Maps;

namespace Perimeter.ViewModels
{
    public class ShieldPageViewModel : INotifyPropertyChanged
    {

        public string _status;
        public string Status
        {
            get
            {
                return _status;
            }
            set
            {
                _status = value;

                RaisePropertyChanged("Status");
            }
        }
        public bool _isScanning;
        public bool IsScanning { 
            get 
            { 
                    return _isScanning; 
            } 
            set 
            { 
                _isScanning = value;
                if(_isScanning)
                {
                    Status = "Perimeter scanning ...";
                } else
                {
                    if (DeviceList.Count == 0)
                        Status = "";
                    else
                        Status="Total:" + DeviceList.Count.ToString();
                }

               // RaisePropertyChanged("Status");
                RaisePropertyChanged("IsScanning"); 
            } 
        }


        public Position _mapPosition;
        public Position MapPosition { get { return _mapPosition; } set { _mapPosition = value; RaisePropertyChanged("MapPosition"); } }
       
        public ObservableCollection<Pin> _pins;
        public ObservableCollection<Pin> Pins { get { return _pins; } set { _pins = value; RaisePropertyChanged("Pins"); } }

        public ObservableCollection<Item> _deviceList;
        public ObservableCollection<Item> DeviceList { get { return _deviceList; } set { _deviceList = value; RaisePropertyChanged("DeviceList"); } }
        public Item LastDeviceItem { get; set; }

        #region singleton
        public static ShieldPageViewModel Instance => _instance ?? (_instance = new ShieldPageViewModel());
        static ShieldPageViewModel _instance;
        ShieldPageViewModel()
        {
     
            _deviceList = new ObservableCollection<Item>();
            _pins = new ObservableCollection<Pin>();

        }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }


    }

}

