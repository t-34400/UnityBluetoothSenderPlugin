#nullable enable

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UnityHelper.UI
{
    public class DynamicToggleGenerator : MonoBehaviour
    {
        [SerializeField] private ToggleGroupDataProviderBase toggleGroupDataProvider = default!;
        [SerializeField] private ToggleGroup toggleGroup = default!;
        [SerializeField] private GameObject toggleRootObject = default!;
        [SerializeField] private ToggleInstanceManager toggleInstancePrefab = default!;
        [SerializeField] private bool generateOnStart = true;
        [SerializeField] private UnityEvent<int> onToggleConfirmed = default!;

        private Dictionary<int, Toggle> toggles = new();

        public void Confirm()
        {
            foreach (var (index, toggle) in toggles)
            {
                if (toggle.isOn)
                {
                    onToggleConfirmed.Invoke(index);
                }
            }
        }

        public void Regenerate()
        {
            foreach (var (_, toggle) in toggles)
            {
                Destroy(toggle.gameObject);
            }
            toggles.Clear();

            GenerateToggles();
        }

        private void Start()
        {
            if (generateOnStart)
            {
                GenerateToggles();
            }
        }

        private void OnEnable()
        {
            toggleGroupDataProvider.ListUpdated += Regenerate;
        }

        private void OnDisable()
        {
            toggleGroupDataProvider.ListUpdated -= Regenerate;
        }

        private void GenerateToggles()
        {
            foreach (var (label, index) in toggleGroupDataProvider.ToggleLabelList.Select((label, index) => (label, index)))
            {
                var instance = Instantiate(toggleInstancePrefab);
                instance.UpdateLabel(label);
                
                var instanceToggle = instance.InstanceToggle;
                instanceToggle.group = toggleGroup;
                instanceToggle.transform.SetParent(toggleRootObject.transform);
                instanceToggle.transform.localPosition = Vector3.zero;
                instanceToggle.transform.localRotation = Quaternion.identity;
                instanceToggle.transform.localScale = Vector3.one;

                toggles.Add(index, instanceToggle);
            }
        }
    }
}