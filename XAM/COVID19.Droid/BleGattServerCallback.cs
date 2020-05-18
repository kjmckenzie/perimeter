using Android.Bluetooth;
using System;
using System.Text;

namespace Perimeter.Droid
{
    public class BleEventArgs : EventArgs
    {
        public BluetoothDevice Device { get; set; }
        public GattStatus GattStatus { get; set; }
        public BluetoothGattCharacteristic Characteristic { get; set; }
        public byte[] Value { get; set; }
        public int RequestId { get; set; }
        public int Offset { get; set; }
    }

    public class BleGattServerCallback : BluetoothGattServerCallback
    {

        public event EventHandler<BleEventArgs> NotificationSent;
        public event EventHandler<BleEventArgs> CharacteristicReadRequest;
        public event EventHandler<BleEventArgs> CharacteristicWriteRequest;

        public BleGattServerCallback()
        {

        }

        public override void OnCharacteristicReadRequest(BluetoothDevice device, int requestId, int offset,
                BluetoothGattCharacteristic characteristic)
        {
            base.OnCharacteristicReadRequest(device, requestId, offset, characteristic);

            //Console.WriteLine("Read request from {0}", device.Name);
            try
            {
                //App.ItemsList.Add(new Models.Item() { Text = string.Format("Read request from {0}", device.Name) });

            }
            catch (Exception ex)
            {


            }

            if (CharacteristicReadRequest != null)
            {
                CharacteristicReadRequest(this, new BleEventArgs() { Device = device, Characteristic = characteristic, RequestId = requestId, Offset = offset });
            }
        }

        public override void OnCharacteristicWriteRequest(BluetoothDevice device, int requestId, BluetoothGattCharacteristic characteristic,
            bool preparedWrite, bool responseNeeded, int offset, byte[] value)
        {
            base.OnCharacteristicWriteRequest(device, requestId, characteristic, preparedWrite, responseNeeded, offset, value);

            ASCIIEncoding ascii = new ASCIIEncoding();

            string decoded = ascii.GetString(value);

            //Console.WriteLine("Input: [{0}]", decoded);
            try
            {

                //App.ItemsList.Add(new Models.Item() { Text = string.Format("Write request [{0}]", decoded) });

            }
            catch (Exception ex)
            {


            }

            if (CharacteristicWriteRequest != null)
            {
                CharacteristicWriteRequest(this, new BleEventArgs() { Device = device, Characteristic = characteristic, Value = value, RequestId = requestId, Offset = offset });
            }
        }

        public override void OnConnectionStateChange(BluetoothDevice device, ProfileState status, ProfileState newState)
        {
            base.OnConnectionStateChange(device, status, newState);
            //Console.WriteLine("State changed to {0}", newState);

            try
            {
                //App.ItemsList.Add(new Models.Item() { Text = string.Format("State changed to {0}", newState) });

            }
            catch (Exception ex)
            {


            }

        }

        public override void OnNotificationSent(BluetoothDevice device, GattStatus status)
        {
            base.OnNotificationSent(device, status);

            if (NotificationSent != null)
            {
                NotificationSent(this, new BleEventArgs() { Device = device });
            }
        }

    }
}