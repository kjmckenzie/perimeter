
using CoreBluetooth;
using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Perimeter.iOS
{
    public static class BleGattServerManager
    {
        private static CBPeripheralManager _PeripheralManager;
       
        public static void BleGattServerCreate()
        {
            if (_PeripheralManager != null)
                _PeripheralManager.StopAdvertising();

            //set up peripheral
            _PeripheralManager = new CBPeripheralManager();
            _PeripheralManager.StateUpdated += _PeripheralManager_StateUpdated;
            _PeripheralManager.AdvertisingStarted += _PeripheralManager_AdvertisingStarted;
            _PeripheralManager.ServiceAdded += _PeripheralManager_ServiceAdded;
            _PeripheralManager.ReadRequestReceived += _PeripheralManager_ReadRequestReceived;
           // _PeripheralManager.WriteRequestsReceived += _PeripheralManager_WriteRequestsReceived;
            
        }
        
        private static void _PeripheralManager_WriteRequestsReceived(object sender, CBATTRequestsEventArgs e)
        {
            
        }

        private static void _PeripheralManager_ReadRequestReceived(object sender, CBATTRequestEventArgs e)
        {
            byte[] bytes = null;

            CBATTRequest req = e.Request;

            if (req != null)
            {
                CBCharacteristic _char = req.Characteristic;

                if (_char != null)
                {
                    CBATTError err = new CBATTError();

                    if (_char.UUID.Equals(CBUUID.FromString(App.DeviceContact)))
                    {
                      
                        req.Value = NSData.FromString(App.DeviceContactValue);
                        _PeripheralManager.RespondToRequest(req, err);
                    }
                    //else if (_char.UUID.Equals(CBUUID.FromString(App.DeviceInfo)))
                    //{
                       
                    //    req.Value = NSData.FromString(App.DeviceInfoValue);
                    //    _PeripheralManager.RespondToRequest(req, err);

                    //}
                    else if (_char.UUID.Equals(CBUUID.FromString(App.DeviceNickname)))
                    {

                        req.Value = NSData.FromString(App.DeviceNicknameValue);
                        _PeripheralManager.RespondToRequest(req, err);


                    }
                    //else if (_char.UUID.Equals(CBUUID.FromString(App.DeviceMAC)))
                    //{

                    //    req.Value = NSData.FromString(App.DeviceMACValue);
                    //    _PeripheralManager.RespondToRequest(req, err);


                    //}
                    else if (_char.UUID.Equals(CBUUID.FromString(App.IsInfected)))
                    {

                        req.Value = NSData.FromString(App.IsInfectedValue);
                        _PeripheralManager.RespondToRequest(req, err);

                    }
                    else
                    {
                        //bytes = e.Characteristic.GetValue();
                    }

                }

            }

        }

        private static void _PeripheralManager_ServiceAdded(object sender, CBPeripheralManagerServiceEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Service added");
        }

        private static void _PeripheralManager_AdvertisingStarted(object sender, NSErrorEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Advertising started");

            App.IsGATTServiceRunning = true;
        }

        private static void _PeripheralManager_StateUpdated(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("State=" + _PeripheralManager.State);

            if (_PeripheralManager.State == CBPeripheralManagerState.PoweredOn)
            {
              
                //set up the GATT service
                CBUUID uuidCharactMan = CBUUID.FromString(App.ManufacturerName);
                NSData nsMan = NSData.FromString(App.ManufacturerNameValue);

                CBUUID uuidCharactFirmware = CBUUID.FromString(App.FirmwareVersion);
                NSData nsFirmware = NSData.FromString(App.FirmwareVersionValue);

                CBUUID uuidCharactNickname = CBUUID.FromString(App.DeviceNickname);
                NSData nsNickname = null; // NSData.FromString(App.DeviceNicknameValue);

                CBUUID uuidCharactMAC = CBUUID.FromString(App.DeviceMAC);
                NSData nsMAC = NSData.FromString(App.DeviceMACValue);

                CBUUID uuidCharactContact = CBUUID.FromString(App.DeviceContact);
                NSData nsContact = null; // NSData.FromString(App.DeviceContactValue);

                CBUUID uuidCharactInfo = CBUUID.FromString(App.DeviceInfo);
                NSData nsInfo = NSData.FromString(App.DeviceInfoValue);

                CBUUID uuidCharactInfected = CBUUID.FromString(App.IsInfected);
                NSData nsInfected = null; // NSData.FromString(App.IsInfectedValue);

                
                CBMutableCharacteristic _characMan = new CBMutableCharacteristic(uuidCharactMan, CBCharacteristicProperties.Read, nsMan, CBAttributePermissions.Readable);
                CBMutableCharacteristic _characFirmware = new CBMutableCharacteristic(uuidCharactFirmware, CBCharacteristicProperties.Read, nsFirmware, CBAttributePermissions.Readable);
                CBMutableCharacteristic _characNickname = new CBMutableCharacteristic(uuidCharactNickname, CBCharacteristicProperties.Read, nsNickname, CBAttributePermissions.Readable);
                CBMutableCharacteristic _characMAC = new CBMutableCharacteristic(uuidCharactMAC, CBCharacteristicProperties.Read, nsMAC, CBAttributePermissions.Readable);
                CBMutableCharacteristic _characContact = new CBMutableCharacteristic(uuidCharactContact, CBCharacteristicProperties.Read, nsContact, CBAttributePermissions.Readable);
                CBMutableCharacteristic _characInfo = new CBMutableCharacteristic(uuidCharactInfo, CBCharacteristicProperties.Read, nsInfo, CBAttributePermissions.Readable);
                CBMutableCharacteristic _characInfected = new CBMutableCharacteristic(uuidCharactInfected, CBCharacteristicProperties.Read, nsInfected, CBAttributePermissions.Readable);

                CBMutableService service = new CBMutableService(CBUUID.FromString(App.ServiceId), true);
                service.Characteristics = new CBMutableCharacteristic[] { _characMan, _characFirmware, 
                    _characNickname,_characContact,_characInfected,_characInfo,_characMAC };
                
                _PeripheralManager.AddService(service);
                //string one = "one";
                //string two = "two";
                //string three = "three";

                //NSDictionary dic = new NSDictionary(one, two, three);

                StartAdvertisingOptions advData = new StartAdvertisingOptions  { LocalName = App.ManufacturerNameValue,ServicesUUID = new CBUUID[] { CBUUID.FromString(App.ServiceId) } };
                
                _PeripheralManager.StartAdvertising(advData);
                


            }
        }
    }
}