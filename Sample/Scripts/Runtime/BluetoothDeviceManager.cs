#nullable enable

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityHelper.UI;

namespace Bluetooth
{
    public class BluetoothDeviceManager: MonoBehaviour
    {
        [SerializeField] private BluetoothUnityInterface bluetoothUnityInterface = default!;
        [SerializeField] private ToggleGroupDataProvider toggleGroupDataProvider = default!;
        [SerializeField] private float listUpdateInterval = 3.0f;
        [SerializeField] private UnityEvent<ConnectionStatus> onStatusChanged;
        private float latestUpdateTime = float.NegativeInfinity;
        private ConnectionStatus connectionStatus = ConnectionStatus.Disabled;

        private List<(string name, string address)> pairedDevices = new ();

        public void Connect(int index)
        {
            if (index >= 0 && index < pairedDevices.Count)
            {
                var device = pairedDevices[index];
                bluetoothUnityInterface.Connect(device.address);
            }
        }

        private void Update()
        {
            var currentTime = Time.time;

            if (currentTime - latestUpdateTime > listUpdateInterval)
            {
                UpdateConnectionStatus();
                latestUpdateTime = currentTime;
            }
        }

        private void UpdateConnectionStatus()
        {
            var newConnectionStatus = bluetoothUnityInterface.GetConnectionStatus();
            if (newConnectionStatus != connectionStatus)
            {
                connectionStatus = newConnectionStatus;
                onStatusChanged.Invoke(newConnectionStatus);
            }

            if (connectionStatus == ConnectionStatus.Standby)
            {
                var currentPairedDevices = bluetoothUnityInterface.GetPairedDevices();
                Debug.Log($"Paired device: {string.Join(", ", currentPairedDevices.Select(device => $"({device.name}, {device.address})"))}");
                if (!currentPairedDevices.SequenceEqual(pairedDevices))
                {
                    pairedDevices = currentPairedDevices;

                    var deviceNames = currentPairedDevices.Select(device => device.name).ToList();
                    toggleGroupDataProvider.SetNewList(deviceNames);
                }
            }
        }
    }
}