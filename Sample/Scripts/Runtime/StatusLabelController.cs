#nullable enable

using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Bluetooth
{
    public class StatusLabelController : MonoBehaviour
    {
        [SerializeField] private Image background = default!;
        [SerializeField] private TMP_Text statusLabel = default!;
        [SerializeField] private Color disabledColor = new (1, 0, 0);
        [SerializeField] private Color standbyColor = new (0, 1, 0);
        [SerializeField] private Color connectingColor = new(1, 0.5f, 0);
        [SerializeField] private Color connectedColor = new(0, 0, 1);

        public void ChangeStatusLabel(ConnectionStatus status)
        {
            switch (status)
            {
                case ConnectionStatus.Disabled:
                    {
                        background.color = disabledColor;
                        statusLabel.text = "Disabled";
                        break;
                    }
                case ConnectionStatus.Standby:
                    {
                        background.color = standbyColor;
                        statusLabel.text = "Standby";
                        break;
                    }
                case ConnectionStatus.Connecting:
                    {
                        background.color = connectingColor;
                        statusLabel.text = "Connecting";
                        break;                        
                    }
                case ConnectionStatus.Connected:
                    {
                        background.color = connectedColor;
                        statusLabel.text = "Connected";
                        break;
                    }
            }
        }
    }
}