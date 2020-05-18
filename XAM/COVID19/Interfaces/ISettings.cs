using System;
using System.Collections.Generic;
using System.Text;

namespace Perimeter.Interfaces
{
    public interface ISetttings
    {
        void StartBluetoothSettings();
        void StartLocationSettings();
        string GetMACAdress(object device);
        bool StartGattServer();

    }
}
