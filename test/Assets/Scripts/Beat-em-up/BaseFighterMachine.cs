using UnityEngine;
using Hank.Systems.StateMachine;

// Issues
// - Transitioning inside a state involved stupid flags
// - Redundant code, especially when taking damage
// - There should be multiple ways to move, such as input OR moving to a specified point
// - State transitions get fucky sometimes

namespace Hank.BeatEmUp
{
    public class BaseFighterMachine : MonoBehaviour
    {
        #region Constants & Type Definitions
        public const float VERTICAL_MOVE_MULTIPLIER = 0.6f;
        public enum EDirectionFacing { LEFT = -1, RIGHT = 1 };
        #endregion

        #region Inspector Fields
        [SerializeField] private float _moveSpeed;
        #endregion

        #region Members
        private StateMachine stateMachine;
        private EDirectionFacing directionFacing;
        private Vector2 moveInput;
        private float currentYPlane;
        #endregion

        #region Accessors
        public EDirectionFacing DirectionFacing { get { return directionFacing; } }
        public float CurrentYPlane { get { return currentYPlane; } }
        #endregion

        private void Start()
        {
            stateMachine = new StateMachine();
        }

        private void Update()
        {
            stateMachine.Update();
        }

        private void FixedUpdate()
        {
            stateMachine.FixedUpdate();
        }

        public void SetYPlane()
        {
            currentYPlane = transform.position.y;
        }

        public void ProcessFrameInput(FighterFrameInput input)
        {
            moveInput = input.AxisInput;

            if (input.JumpPressed) QueueJump();
            if (input.AttackPressed) QueueAttack();
        }

        public void QueueJump()
        {
            
        }

        public void QueueAttack()
        {

        }
    }

    public class S_Idle
    {

    }

    public class S_Walk
    {

    }

    public class S_Jump
    {

    }

    public class S_Land
    {

    }
}