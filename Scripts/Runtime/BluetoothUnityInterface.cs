#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Bluetooth
{
    public enum ConnectionStatus
    {
        Standby = 0,
        Connecting = 1,
        Connected = 2,
        Disabled = -1,
    }

    public class BluetoothUnityInterface: MonoBehaviour
    {
        private AndroidJavaObject? connectionManager;

        private void Start()
        {
            UpdateConnectionManagerIfNeeded();
        }

        public List<(string name, string address)> GetPairedDevices()
        {
            UpdateConnectionManagerIfNeeded();
            if (connectionManager == null)
            {
                return new();
            }

            var dataString = connectionManager.Call<string>("getPairedDeviceJson");
            var data = JsonUtility.FromJson<DataFormat>(dataString);

            return data.devices.Select(device => (device.name, device.address)).ToList();
        }

        public void Connect(string address)
        {
            UpdateConnectionManagerIfNeeded();
            if (connectionManager == null)
            {
                return;
            }

            connectionManager.Call("connectToDeviceByAddress", address);
        }

        public ConnectionStatus GetConnectionStatus()
        {
            if (connectionManager == null)
            {
                return ConnectionStatus.Disabled;
            }

            var status = (ConnectionStatus) connectionManager.Call<int>("getConnectionStatus");

            if (status == ConnectionStatus.Standby)
            {
                var bluetoothStatus = connectionManager.Call<int>("getBluetoothStatus");
                if (bluetoothStatus != 0)
                {
                    status = ConnectionStatus.Disabled;
                }
            }

            return status;
        }

        public void Send(byte[] data)
        {
            if (connectionManager == null)
            {
                return;
            }

            connectionManager.Call("send", data);
        }

        public void Close()
        {
            if (connectionManager != null)
            {
                connectionManager.Call("close");
            }
        }

        private void OnDestroy()
        {
            if (connectionManager != null)
            {
                connectionManager.Call("close");
                connectionManager = null;
            }
        }

        private void UpdateConnectionManagerIfNeeded()
        {
            if (connectionManager != null && !connectionManager.Call<bool>("isActivityAlive"))
            {
                connectionManager.Call("close");
                connectionManager = null;
            }

            if (connectionManager == null)
            {
                AndroidJavaClass unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                AndroidJavaObject activity = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");

                AndroidJavaClass connectionManagerClass = new AndroidJavaClass("com.example.bluetoothapp.BluetoothConnectionManagerUnityWrapper");
                connectionManager = connectionManagerClass.CallStatic<AndroidJavaObject>("getInstance", activity);
            }
        }

        [Serializable]
        struct DataFormat
        {
            public List<DeviceFormat> devices;
        }

        [Serializable]
        struct DeviceFormat
        {
            public string name;
            public string address;
        }
    }
}