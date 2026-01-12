using System.Collections.Generic;

namespace Hank.UI
{
    public partial class UIPanelController
    {
        private List<IOnBeforePanelOpen> onBeforeOpenPanelListeners;
        private List<IOnPanelOpened> onAfterPanelOpenListeners;
        private List<IOnBeforePanelClose> onBeforePanelCloseListeners;
        private List<IOnPanelClosed> onAfterPanelCloseListeners;
        private List<IOnPanelGainedFocus> onGainFocusListeners;
        private List<IOnPanelLostFocus> onLostFocusListeners;

        private void UpdateListener<T>(ref List<T> targetType) where T : class
        {
            if (targetType == null)
                targetType = new List<T>();
            else
                targetType.Clear();

            if (this != null && gameObject != null)
                gameObject.GetComponentsInChildren(true, targetType);
        }

        private void DispatchOnBeforePanelOpen()
        {
            UpdateListener(ref onBeforeOpenPanelListeners);

            for (int i = 0; i < onBeforeOpenPanelListeners.Count; i++)
                onBeforeOpenPanelListeners[i].OnBeforePanelOpen();
        }

        private void DispatchOnPanelOpen()
        {
            UpdateListener(ref onAfterPanelOpenListeners);

            for (int i = 0; i < onAfterPanelOpenListeners.Count; i++)
                onAfterPanelOpenListeners[i].OnPanelOpen();
        }

        private void DispatchOnBeforePanelClose()
        {
            UpdateListener(ref onBeforePanelCloseListeners);

            for (int i = 0; i < onBeforePanelCloseListeners.Count; i++)
                onBeforePanelCloseListeners[i].OnBeforePanelClosed();
        }

        private void DispatchOnPanelClosed()
        {
            UpdateListener(ref onAfterPanelCloseListeners);

            for (int i = 0; i < onAfterPanelCloseListeners.Count; i++)
                onAfterPanelCloseListeners[i].OnPanelClosed();
        }

        private void DispatchOnGainFocus()
        {
            UpdateListener(ref onGainFocusListeners);

            for (int i = 0; i < onGainFocusListeners.Count; i++)
                onGainFocusListeners[i].OnGainedFocus();
        }

        private void DispatchOnLostFocus()
        {
            UpdateListener(ref onLostFocusListeners);

            for (int i = 0; i < onLostFocusListeners.Count; i++)
                onLostFocusListeners[i].OnLostFocus();
        }
    }

    public interface IOnBeforePanelOpen
    {
        void OnBeforePanelOpen();
    }

    public interface IOnPanelOpened
    {
        void OnPanelOpen();
    }

    public interface IOnBeforePanelClose
    {
        void OnBeforePanelClosed();
    }

    public interface IOnPanelClosed
    {
        void OnPanelClosed();
    }

    public interface IOnPanelGainedFocus
    {
        void OnGainedFocus();
    }

    public interface IOnPanelLostFocus
    {
        void OnLostFocus();
    }
}