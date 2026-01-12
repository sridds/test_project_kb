using UnityEngine;

namespace Hank.UI
{
    // Defines special behaviours of widgets (dialogue, selection, etc)

    // example: button
    // example: data preview
    // example: tooltip
    // example: open on select [require button]
    // example: close on select, etc

    public abstract class UIWidgetBehaviour : MonoBehaviour, IOnPanelGainedFocus, IOnPanelLostFocus
    {
        // some kind of behaviour in here for resetting history
        public virtual void UpdateWidget() { }

        public virtual void OnGainedFocus() { }

        public virtual void OnLostFocus() { }
    }
}