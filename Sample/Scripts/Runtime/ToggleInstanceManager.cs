#nullable enable

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace UnityHelper.UI
{
    public class ToggleInstanceManager : MonoBehaviour
    {
        [SerializeField] private Toggle toggle = default!;
        [SerializeField] private UnityEvent<string> updateLabel = default!;

        public Toggle InstanceToggle => toggle;
        public void UpdateLabel(string label) => updateLabel.Invoke(label);
    }
}