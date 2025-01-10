using System.Collections.Generic;
using UnityEngine;

namespace FrameWork.Tooltip
{
    public class TooltipManager : Singleton<TooltipManager>
    {
        private Dictionary<GameObject, TooltipStyle> _tooltip = new Dictionary<GameObject, TooltipStyle>();
        private Transform _parent;

        protected override void Initialize()
        {
            _parent = transform.GetChild(0);
        }

        internal void Show(TooltipTrigger trigger)
        {
            var prefab = trigger.tooltipStyle.gameObject;
            TooltipStyle style;

            if (_tooltip.TryGetValue(prefab, out style))
            {
                style.Show();
            }
            else
            {
                var instance = CreateTooltip(trigger.tooltipStyle);
                style = instance.GetComponent<TooltipStyle>();

                _tooltip.Add(prefab, style);
            }

            style.ApplyData(trigger.tooltipData);
            style.transform.position = trigger.transform.position;
        }

        internal void Hide(TooltipTrigger trigger)
        {
            var tooltipPrefab = trigger.tooltipStyle.gameObject;
            TooltipStyle style;

            if (_tooltip.TryGetValue(tooltipPrefab, out style))
            {
                style.Hide(true);
            }
        }

        internal TooltipStyle GetTooltipStyle(TooltipTrigger trigger)
        {
            var tooltipPrefab = trigger.tooltipStyle.gameObject;
            TooltipStyle style;

            if (_tooltip.TryGetValue(tooltipPrefab, out style))
            {
                return style;
            }
            else
            {
                return null;
            }
        }

        private GameObject CreateTooltip(TooltipStyle style)
        {
            var tooltipPrefab = style.gameObject;
            var instance = Instantiate(tooltipPrefab, _parent);

            return instance;
        }
    }
}