using UnityEngine;

namespace Hank.UI
{
    // notes
    // - you could probably handle list formatting in here aswell. its weird to have a single widget hahndle list formatting
    public class UIPanelSelectionController : UIPanelController
    {
        public enum EUINavigationSettings
        {
            Horizontal,
            Vertical,
        }

        [Header("Selection Settings")]
        [SerializeField] private UIButtonWidget[] _widgetElements;
        [SerializeField] private EUINavigationSettings _navigationSettings;
        [SerializeField] private bool _wrappingEnabled;

        private int lastSelectionIndex = -1; // set to -1 by default to make sure it processes the first input
        public int selectionIndex { get; protected set; }

        public override void HandleInput(UIInput input)
        {
            if (_widgetElements.Length == 0) return;

            // use right / left arrow
            if (_navigationSettings == EUINavigationSettings.Horizontal)
            {
                if (input.navigateRightPressed) UpdateIndex(selectionIndex + 1);
                else if (input.navigateLeftPressed) UpdateIndex(selectionIndex - 1);
                else return;
            }

            // use up and down arrow
            else if (_navigationSettings == EUINavigationSettings.Vertical)
            {
                if (input.navigateDownPressed) UpdateIndex(selectionIndex + 1);
                else if (input.navigateUpPressed) UpdateIndex(selectionIndex - 1);
                else return;
            }

            UpdateWidgetSelection();
        }

        private void UpdateWidgetSelection()
        {
            // ensure selection changed before updating
            if (lastSelectionIndex == selectionIndex) return;

            // If we have more than one element, exit last element
            if (_widgetElements.Length > 1) _widgetElements[lastSelectionIndex].OnSelectionExit();
            _widgetElements[selectionIndex].OnSelectionEnter();

            lastSelectionIndex = selectionIndex;
        }

        // updates widget with respect to list length and wrapping
        private void UpdateIndex(int index)
        {
            selectionIndex = index;

            if (selectionIndex < 0) selectionIndex = _wrappingEnabled ? 0 : _widgetElements.Length - 1;
            if (selectionIndex >= _widgetElements.Length) selectionIndex = _wrappingEnabled ? _widgetElements.Length - 1 : selectionIndex % _widgetElements.Length;
        }
    }
}