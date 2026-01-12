using UnityEngine;

namespace Hank.UI
{
    public class UIButtonWidget : UIWidgetBehaviour
    {
        public void OnSelectionEnter()
        {
            Debug.Log("erm what the button");
        }

        public void OnSelectionExit()
        {
            Debug.Log("Erm what the unbutton");
        }
    }
}