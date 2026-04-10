using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hank.UI
{
    public interface IUIPanelController
    {
        void Open(Action<UIPanelController> callback = null);
        void Close(Action<UIPanelController> callback = null);
        void OnGainFocus();
        void OnLostFocus();
        void HandleInput(UIInput input);
        bool IsOpen { get; }
        bool IsFocused { get; }
    }

    // Controls showing / hiding of a widget container

    // example: dialogue choice panel (show dialogue, then show choice widgets)
    // example: selection panel (control how you scroll through)

    [DisallowMultipleComponent]
    [RequireComponent(typeof(CanvasGroup))]
    public abstract partial class UIPanelController : MonoBehaviour, IUIPanelController
    {
        protected CanvasGroup canvasGroup;
        protected RectTransform rectTransform;
        protected UIWidgetBehaviour[] widgetBehaviours;

        private Coroutine openCoroutine;
        private Coroutine closeCoroutine;

        public bool IsOpen { get; private set; }
        public bool IsFocused { get; private set; }

        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            rectTransform = GetComponent<RectTransform>();
            widgetBehaviours = GetComponents<UIWidgetBehaviour>();
        }

        public void Open(Action<UIPanelController> callback = null)
        {
            if (IsOpen) return;

            openCoroutine = StartCoroutine(IOpen(callback));
        }

        public void Close(Action<UIPanelController> callback = null)
        {
            if (!IsOpen) return;

            closeCoroutine = StartCoroutine(IClose(callback));
        }

        protected virtual void PreOpen()
        {
            DispatchOnBeforePanelOpen();
        }

        protected virtual void PostOpen()
        {
            IsOpen = true;

            DispatchOnPanelOpen();
        }

        protected virtual void BeforeClose()
        {
            DispatchOnBeforePanelClose();
        }

        protected virtual void AfterClose()
        {
            IsOpen = false;

            DispatchOnPanelClosed();
        }

        private IEnumerator IOpen(Action<UIPanelController> callback)
        {
            if(openCoroutine != null) StopCoroutine(openCoroutine);

            PreOpen();
            yield return StartCoroutine(ITransitionIn());
            PostOpen();

            callback?.Invoke(this);
        }

        private IEnumerator IClose(Action<UIPanelController> callback)
        {
            if(closeCoroutine != null) StopCoroutine(closeCoroutine);

            BeforeClose();
            yield return StartCoroutine(ITransitionOut());
            AfterClose();
            IsOpen = false;
            
            callback?.Invoke(this);
        }

        public virtual void HandleInput(UIInput input) { }

        public virtual void OnGainFocus()
        {
            IsFocused = true;
            DispatchOnGainFocus();
        }

        public virtual void OnLostFocus()
        {
            IsFocused = false;
            DispatchOnLostFocus();
        }

        protected virtual IEnumerator ITransitionIn()
        {
            canvasGroup.alpha = 1;

            yield return null;
        }

        protected virtual IEnumerator ITransitionOut()
        {
            canvasGroup.alpha = 0;

            yield return null;
        }
    }
}