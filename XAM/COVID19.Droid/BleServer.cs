using Android.Bluetooth;
using Android.Bluetooth.LE;
using Android.Content;
using Android.OS;
using Perimeter.Interfaces;
using Java.Lang;
using Java.Util;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Random = System.Random;


namespace Perimeter.Droid
{
    public class BleServer
    {
        private bool _isRunning;

        private readonly BluetoothManager _bluetoothManager;
        private BluetoothAdapter _bluetoothAdapter;
        private BleGattServerCallback _bluettothServerCallback;
        private BluetoothGattServer _bluetoothServer;

        private BluetoothGattCharacteristic _characteristicDataExchange;
        private BluetoothGattCharacteristic _characteristicFirmwareVersion;
        private BluetoothGattCharacteristic _characteristicDeviceNickname;
        private BluetoothGattDescriptor _descriptorDeviceNickname;
        private BluetoothGattCharacteristic _characteristicDeviceContact;
        private BluetoothGattCharacteristic _characteristicDeviceInfo;
        private BluetoothGattCharacteristic _characteristicInitData;
        private BluetoothGattCharacteristic _characteristicInfected;
        private BluetoothGattCharacteristic _characteristicMAC;
        private BluetoothGattCharacteristic _characteristicManufacturerName;

        public bool IsRunning { get { return _isRunning;  } }

        public string GetDeviceName { get { return _bluetoothAdapter.Name; } }
       

        // use predefined Bluetooth SIG definitions for well know characteristics on UWP- Windows project.
        //public static Guid ManufacturerName = GattCharacteristicUuids.ManufacturerNameString;

        public BleServer(Context ctx)
        {
            _isRunning = false;

            try
            {
                _bluetoothManager = (BluetoothManager)ctx.GetSystemService(Context.BluetoothService);
                
                _bluetoothAdapter = _bluetoothManager.Adapter;

                _bluettothServerCallback = new BleGattServerCallback();
                _bluetoothServer = _bluetoothManager.OpenGattServer(ctx, _bluettothServerCallback);

                var service = new BluetoothGattService(UUID.FromString(App.ServiceId), GattServiceType.Primary);
                
                //_characteristicDataExchange = new BluetoothGattCharacteristic(UUID.FromString(App.DataExchange), GattProperty.Read | GattProperty.Notify | GattProperty.Write, GattPermission.Read | GattPermission.Write);
                _characteristicInitData = new BluetoothGattCharacteristic(UUID.FromString(App.InitData), GattProperty.Read | GattProperty.Notify | GattProperty.Write, GattPermission.Read | GattPermission.Write);

                _characteristicFirmwareVersion = new BluetoothGattCharacteristic(UUID.FromString(App.FirmwareVersion), GattProperty.Read, GattPermission.Read);
                _characteristicDeviceNickname = new BluetoothGattCharacteristic(UUID.FromString(App.DeviceNickname), GattProperty.Read, GattPermission.Read);
                _descriptorDeviceNickname = new BluetoothGattDescriptor(UUID.FromString(App.DeviceNicknameDescriptor), GattDescriptorPermission.Read);
                _characteristicDeviceContact = new BluetoothGattCharacteristic(UUID.FromString(App.DeviceContact), GattProperty.Read, GattPermission.Read);
                _characteristicDeviceInfo = new BluetoothGattCharacteristic(UUID.FromString(App.DeviceInfo), GattProperty.Read, GattPermission.Read);
                _characteristicInfected = new BluetoothGattCharacteristic(UUID.FromString(App.IsInfected), GattProperty.Read, GattPermission.Read);
                _characteristicManufacturerName = new BluetoothGattCharacteristic(UUID.FromString(App.ManufacturerName), GattProperty.Read, GattPermission.Read);
                _characteristicMAC = new BluetoothGattCharacteristic(UUID.FromString(App.DeviceMAC), GattProperty.Read, GattPermission.Read);

                //_characteristicDataExchange.SetValue("Data exchange");
                //service.AddCharacteristic(_characteristicDataExchange);

                _characteristicFirmwareVersion.SetValue(App.FirmwareVersionValue);
                service.AddCharacteristic(_characteristicFirmwareVersion);

                _characteristicDeviceNickname.SetValue(App.DeviceNicknameValue);
                service.AddCharacteristic(_characteristicDeviceNickname);

                _characteristicDeviceContact.SetValue(App.DeviceContactValue);
                service.AddCharacteristic(_characteristicDeviceContact);

                _characteristicDeviceInfo.SetValue(App.DeviceInfoValue);
                service.AddCharacteristic(_characteristicDeviceInfo);

                _characteristicMAC.SetValue(App.DeviceMACValue);
                service.AddCharacteristic(_characteristicMAC);

                //_characteristicInitData.SetValue("SPACE - Copyright 2020");
                //service.AddCharacteristic(_characteristicInitData);

                _characteristicInfected.SetValue(App.IsInfectedValue);
                service.AddCharacteristic(_characteristicInfected);

                _characteristicManufacturerName.SetValue(App.ManufacturerNameValue);

                service.AddCharacteristic(_characteristicManufacturerName);

                _bluetoothServer.AddService(service);

                _bluettothServerCallback.CharacteristicReadRequest += _bluettothServerCallback_CharacteristicReadRequest;
                _bluettothServerCallback.CharacteristicWriteRequest += _bluettothServerCallback_CharacteristicWriteRequest;
                //_bluettothServerCallback.NotificationSent += _bluettothServerCallback_NotificationSent;

                Console.WriteLine("Server created!");

                BluetoothLeAdvertiser myBluetoothLeAdvertiser = _bluetoothAdapter.BluetoothLeAdvertiser;

                var builder = new AdvertiseSettings.Builder();
                builder.SetAdvertiseMode(AdvertiseMode.LowLatency);
                builder.SetConnectable(true);
                builder.SetTimeout(0);
                builder.SetTxPowerLevel(AdvertiseTx.PowerHigh);
              

                AdvertiseData.Builder dataBuilder = new AdvertiseData.Builder();
                //dataBuilder.SetIncludeDeviceName(false);
                dataBuilder.AddServiceUuid(ParcelUuid.FromString(App.ServiceId));
                dataBuilder.SetIncludeTxPowerLevel(true);
                //byte[] data = Encoding.UTF8.GetBytes(App.ManufacturerNameValue);
                //dataBuilder.AddManufacturerData(0x00E0, data);


                AdvertiseData.Builder dataResponseBuilder = new AdvertiseData.Builder();
                dataResponseBuilder.SetIncludeDeviceName(true);
                
                //AdvertiseData.Builder dataManufacturerBuilder = new AdvertiseData.Builder();               
                //byte[] data = Encoding.UTF8.GetBytes(App.ManufacturerNameValue);
                //dataManufacturerBuilder.AddManufacturerData(49177, data);

                myBluetoothLeAdvertiser.StartAdvertising(builder.Build(), dataBuilder.Build(), dataResponseBuilder.Build(), new BleAdvertiseCallback()); 

                _isRunning = true; 
            }
            catch (System.Exception ex)
            {

            }

        }

        private void _bluettothServerCallback_CharacteristicWriteRequest(object sender, BleEventArgs e)
        {
            
        }

        private int _count = 0;
        private Stopwatch _sw = new Stopwatch();

        void _bluettothServerCallback_NotificationSent(object sender, BleEventArgs e)
        {
            if (_count == 0)
            {
                _sw = new Stopwatch();
                _sw.Start();
            }

            if (_count < 1000)
            {
                var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                var random = new Random();
                var result = new string(
                    Enumerable.Repeat(chars, 20)
                        .Select(s => s[random.Next(s.Length)])
                        .ToArray());
                
                //_characteristic.SetValue(result);

                //_bluetoothServer.NotifyCharacteristicChanged(e.Device, _characteristic, false);

                _count++;

            }
            else
            {
                _sw.Stop();
                Console.WriteLine("Sent # {0} notifcations. Total kb:{2}. Time {3}(s). Throughput {1} bytes/s", _count,
                    _count * 20.0f / _sw.Elapsed.TotalSeconds, _count * 20 / 1000, _sw.Elapsed.TotalSeconds);
            }
        }

        private bool _notificationsStarted = false;

        private int _readRequestCount = 0;
        void _bluettothServerCallback_CharacteristicReadRequest(object sender, BleEventArgs e)
        {
            ASCIIEncoding ascii = new ASCIIEncoding();

            try
            {

                if (_readRequestCount == 5)
                {
                    _notificationsStarted = !_notificationsStarted;
                    _readRequestCount = 0;

                }
                else
                {
                    _readRequestCount++;
                    Console.WriteLine("Read req {0}", _readRequestCount);
                    Console.WriteLine("Uuid req {0}", e.Characteristic.Uuid.ToString());

                    byte[] bytes = null;

                    if (e.Characteristic.Uuid.Equals(UUID.FromString(App.DeviceNickname)))
                    {
                        bytes = Encoding.UTF8.GetBytes(App.DeviceNicknameValue);
                    }
                    else if (e.Characteristic.Uuid.Equals(UUID.FromString(App.DeviceContact)))
                    {
                        bytes = Encoding.UTF8.GetBytes(App.DeviceContactValue);
                    }
                    else if (e.Characteristic.Uuid.Equals(UUID.FromString(App.DeviceInfo)))
                    {
                        bytes = Encoding.UTF8.GetBytes(App.DeviceInfoValue);
                    }
                    else if (e.Characteristic.Uuid.Equals(UUID.FromString(App.DeviceMAC)))
                    {
                        bytes = Encoding.UTF8.GetBytes(App.DeviceMACValue);
                    }
                    else if (e.Characteristic.Uuid.Equals(UUID.FromString(App.IsInfected)))
                    {
                        bytes = Encoding.UTF8.GetBytes(App.IsInfectedValue);
                    }
                    else
                    {
                        bytes = e.Characteristic.GetValue();
                    }

                    string decoded = ascii.GetString(bytes);

                    _bluetoothServer.SendResponse(e.Device, e.RequestId, GattStatus.Success, e.Offset,
                        bytes);

                    return;
                }

            }
            catch (System.Exception ex)
            {

            }

            //if (_notificationsStarted)
            //{
            //    _count = 0;

            //    Console.WriteLine("Started notifcations!");

            //    e.Characteristic.SetValue(String.Format("Start {0}!", _readRequestCount));
            //    _bluetoothServer.SendResponse(e.Device, e.RequestId, GattStatus.Success, e.Offset,
            //        e.Characteristic.GetValue());
            //    _bluetoothServer.NotifyCharacteristicChanged(e.Device, e.Characteristic, false);
            //}
            //else
            //{
            //    Console.WriteLine("Stopped notifcations!");
            //    e.Characteristic.SetValue(String.Format("Stop {0}!", _readRequestCount));
            //    _bluetoothServer.SendResponse(e.Device, e.RequestId, GattStatus.Success, e.Offset,
            //        e.Characteristic.GetValue());
            //    _bluetoothServer.NotifyCharacteristicChanged(e.Device, e.Characteristic, false);
            //}
        }

    }

    public class BleAdvertiseCallback : AdvertiseCallback
    {
        public override void OnStartFailure(AdvertiseFailure errorCode)
        {
            Console.WriteLine("Adevertise start failure {0}", errorCode);
            //App.ItemsList.Add(new Models.Item() { Text = string.Format("Advertise start failure {0}", errorCode) });

            base.OnStartFailure(errorCode);
        }

        public override void OnStartSuccess(AdvertiseSettings settingsInEffect)
        {
            //Console.WriteLine("Adevertise start success {0}", settingsInEffect.Mode);

            //App.ItemsList.Add(new Models.Item() { Text= string.Format("Advertise start success {0}", settingsInEffect.Mode)});

            base.OnStartSuccess(settingsInEffect);
        }
    }
}