using UnityEngine;

namespace Hank.BeatEmUp
{
    public struct FighterFrameInput
    {
        public Vector2 AxisInput;
        public bool AttackPressed;
        public bool JumpPressed;
    }

    [RequireComponent(typeof(BaseFighterMachine))]
    public class FighterInputDriver : MonoBehaviour
    {
        private BaseFighterMachine fighterMachine;

        [SerializeField]
        private KeyCode _jumpKey;

        [SerializeField]
        private KeyCode _attackKey;

        private void Start()
        {
            fighterMachine = GetComponent<BaseFighterMachine>();
        }

        void Update()
        {
            // Get inputs
            Vector2 axisInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            FighterFrameInput input = new FighterFrameInput() { AxisInput = axisInput, JumpPressed = Input.GetKeyDown(_jumpKey), AttackPressed = Input.GetKeyDown(_attackKey) };

            // Send input to machine
            fighterMachine.ProcessFrameInput(input);
        }
    }
}