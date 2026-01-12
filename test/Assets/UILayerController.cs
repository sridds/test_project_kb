using System.Collections.Generic;
using UnityEngine;

namespace Hank.UI
{
    // inheritors get specific about what kind of layer it is and contains functionality for the specific panel
    public abstract class UILayerController<T> : MonoBehaviour where T : IUIPanelController
    {
        protected Stack<IUIPanelController> panelStackHistory = new Stack<IUIPanelController>();
        protected IUIPanelController currentFocusedPanel;
        private bool panelValidatedFlag = false;

        public virtual void OpenPanel(T panel)
        {
            // Push a panel onto the stack (this way we can backtrack)
            if (currentFocusedPanel != null)
            {
                currentFocusedPanel.OnLostFocus();
                panelStackHistory.Push(currentFocusedPanel);
            }

            currentFocusedPanel = panel;
            currentFocusedPanel.Open(x => panelValidatedFlag = true);
            currentFocusedPanel.OnGainFocus();
        }

        public virtual void HandleInput(UIInput input)
        {
            if (!panelValidatedFlag) return;

            Debug.Log("I am the panel. I am handling input as fuckk");
            currentFocusedPanel.HandleInput(input);
        }

        public virtual void CloseCurrentPanel()
        {
            // A panel must first exist before attempting to close it
            if (currentFocusedPanel == null)
            {
                Debug.LogWarning("Failed to close current panel! Current panel is null");
                return;
            }

            // close panel
            panelValidatedFlag = false;
            currentFocusedPanel.OnLostFocus();
            currentFocusedPanel.Close();

            // pop from the panel stack and refocus the last panel
            if (panelStackHistory.Count > 0)
            {
                currentFocusedPanel = panelStackHistory.Pop();
                currentFocusedPanel.OnGainFocus();
            }
            else
            {
                currentFocusedPanel = null;
                Debug.Log("All panels from stack closed!");
            }
        }
    }
}