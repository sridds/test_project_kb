using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Hank.UI
{
    public struct UIInput
    {
        public bool navigateUpPressed;
        public bool navigateLeftPressed;
        public bool navigateRightPressed;
        public bool navigateDownPressed;
        public bool confirmPressed;
        public bool cancelPressed;
    }

    /// <summary>
    /// Manages the flow of UI and ensures no two panels are focused at the same time.
    /// 
    /// major problems from before:
    /// - logic intertwined with ui
    /// - ui quickly becoming messy and unmanagable as a result of poor planning and inheritance
    /// - visuals intertwined with logic
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float _inputDelay = 0.1f;
        [SerializeField] private UILayerController<IUIPanelController> currentLayer;

        public static UIManager Instance { get; private set; }

 
        private float inputTimer;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);

            DontDestroyOnLoad(this);
        }

        private void Update()
        {
            if(inputTimer < _inputDelay)
            {
                inputTimer += Time.deltaTime;
                return;
            }

            // send to current UI layer
            currentLayer.HandleInput(GetInput());

            // reset timer if anything was pressed
            if(Input.anyKey) inputTimer = 0.0f;
        }

        private UIInput GetInput()
        {
            UIInput input = new UIInput();

            input.navigateLeftPressed = Input.GetKeyDown(KeyCode.LeftArrow);
            input.navigateRightPressed = Input.GetKeyDown(KeyCode.RightArrow);
            input.navigateUpPressed = Input.GetKeyDown(KeyCode.UpArrow);
            input.navigateDownPressed = Input.GetKeyDown(KeyCode.DownArrow);
            input.confirmPressed = Input.GetKeyDown(KeyCode.Z);
            input.cancelPressed = Input.GetKeyDown(KeyCode.X);

            return input;
        }
    }

    public class UIEventDispatcher : MonoBehaviour
    {
        // calls events that other ui subscribes to
    }
}