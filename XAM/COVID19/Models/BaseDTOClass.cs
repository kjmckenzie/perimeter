using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Perimeter.Models
{
    public class BaseDTOClass : INotifyPropertyChanged
    {
        public BaseDTOClass()
        {
            
        }

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
