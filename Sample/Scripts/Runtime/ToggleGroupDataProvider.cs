#nullable enable

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityHelper.UI
{
    [CreateAssetMenu(fileName = "ToggleGroupDataProvider", menuName = "UnityHelper/UI/Toggle Group Data Provider")]
    public class ToggleGroupDataProvider : ToggleGroupDataProviderBase
    {
        private List<string> toggleLabelList = new();
        public override List<string> ToggleLabelList => toggleLabelList.ToList();

        public void SetNewList(List<string> newList)
        {
            toggleLabelList = newList;
            InvokeListUpdateEvent();
        }
    }
}