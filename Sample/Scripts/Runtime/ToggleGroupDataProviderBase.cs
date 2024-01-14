#nullable enable

using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityHelper.UI
{
    public abstract class ToggleGroupDataProviderBase : ScriptableObject
    {
        public abstract List<string> ToggleLabelList { get; }
        public event Action? ListUpdated;
        protected void InvokeListUpdateEvent() => ListUpdated?.Invoke();
    }
}