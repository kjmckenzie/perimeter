using System;
using Xamarin.Forms;

namespace Perimeter.Models
{
    public class Item : BaseDTOClass
    {
        public Item()
        {
            DateCreated = DateTime.Now;
            DateUpdated = DateCreated;
        }

        public string GUID { get; set; }
        public string MAC { get; set; }
        public string Id { get; set; }
        public string Manufacturer { get; set; }
        public string Firmware { get; set; }
        public string Nickname { get; set; }
        public string Contact { get; set; }
        public string Text { get; set; }
        public string Description { get; set; }
        public string DeviceName { get; set; }
        public DateTime DateCreated { get; set; }
        
        private DateTime _dateUpdated;
        public DateTime DateUpdated
        {
            get { return this._dateUpdated; }
            set
            {
                this._dateUpdated = value;
                this.RaisePropertyChanged("DateUpdated");
            }

        }

        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int TotalHits { get; set; } = 1;
        public bool IsApple { get; set; }

        private bool infected;
        public bool Infected
        {
            get { return this.infected; }
            set
            {
                this.infected = value;
                this.RaisePropertyChanged("Infected");
            }

        }

        private double rssi;
        public double RSSI
        {
            get { return this.rssi; }
            set
            {
                this.rssi = value;
                this.RaisePropertyChanged("RSSI");
                this.RaisePropertyChanged("Distance");
            }

        }
        private double txPower;
        public double TxPower
        {
            get { return this.txPower; }
            set
            {
                this.txPower = value;
                this.RaisePropertyChanged("TxPower");
                this.RaisePropertyChanged("Distance");
            }

        }

        public double Distance
        {
            get
            {
                double distance = 0;
                var ratio = rssi * 1.0 / TxPower;
                if (ratio < 1.0)
                {
                    distance = Math.Pow(ratio, 10);
                    
                }
                else
                {
                    distance = Math.Pow(ratio, 7.7095) + 0.111;

                }
 
                return distance;
            }
        }

        //public string Distance
        //{
        //    get
        //    {
        //        string result = "";

        //        if (RSSI < -70)
        //            result= "Far";
        //        if (RSSI < -55)
        //            result = "Near";
        //        if (RSSI < 0 )
        //            result = "Too close";

        //        return result;
        //    }
        //}
        //public string Icon { get; set; }
        public bool IsSelected { get; set; }
        public bool IsVisible { get; set; }

    }
}