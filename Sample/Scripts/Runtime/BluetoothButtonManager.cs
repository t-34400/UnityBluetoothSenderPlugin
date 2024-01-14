#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Bluetooth
{
    public class BluetoothButtonManager : MonoBehaviour
    {
        [SerializeField] private Button connectButton = default!;
        [SerializeField] private Button cancelButton = default!;
        [SerializeField] private Button disconnectButton = default!;

        public void ChangeButton(ConnectionStatus status)
        {
            switch (status)
            {
                case ConnectionStatus.Disabled:
                    {
                        connectButton.gameObject.SetActive(false);
                        cancelButton.gameObject.SetActive(false);
                        disconnectButton.gameObject.SetActive(false);
                        break;
                    }
                case ConnectionStatus.Standby:
                    {
                        connectButton.gameObject.SetActive(true);
                        cancelButton.gameObject.SetActive(false);
                        disconnectButton.gameObject.SetActive(false);
                        break;
                    }
                case ConnectionStatus.Connecting:
                    {
                        connectButton.gameObject.SetActive(false);
                        cancelButton.gameObject.SetActive(true);
                        disconnectButton.gameObject.SetActive(false);
                        break;                        
                    }
                case ConnectionStatus.Connected:
                    {
                        connectButton.gameObject.SetActive(false);
                        cancelButton.gameObject.SetActive(false);
                        disconnectButton.gameObject.SetActive(true);
                        break;
                    }
            }
        }    }
}