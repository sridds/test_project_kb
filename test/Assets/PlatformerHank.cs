using Hank.Systems.StateMachine;
using UnityEngine;

// must be simple to understand and easy
public class PlatformerHank : MonoBehaviour
{
    public enum EHankState
    {
        Idle,
        Walking,
        Airborne
    }

    [Header("Modifiers")]
    [SerializeField] private float _accel;
    [SerializeField] private float _maxWalkSpeed;
    [SerializeField] private float _jumpForce;
    [SerializeField] private float _jumpGravity;
    [SerializeField] private float _fallGravity;

    private float xFrameInput;
    private bool isJumpQueued;
    private float jumpQueueTimestamp;
    private Rigidbody2D rigidbody;
    private StateMachine states;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        GetInput();
    }

    private void FixedUpdate()
    {
        if(isJumpQueued)
        {
            Jump();
        }
    }

    private void GetInput()
    {
        xFrameInput = Input.GetAxisRaw("Horizontal");

        // Queue jump
        if(Input.GetKeyDown(KeyCode.Space))
        {
            isJumpQueued = Input.GetKeyDown(KeyCode.Space);
            jumpQueueTimestamp = Time.time;
        }
    }

    private void Jump()
    {
        rigidbody.linearVelocity = new Vector2(rigidbody.linearVelocity.x, 0.0f);
        rigidbody.AddForce(new Vector2(0.0f, _jumpForce), ForceMode2D.Impulse);
    }

    public class PlatformerState
    {

    }

    public class Idle : PlatformerState
    {

    }

    public class Walk : PlatformerState
    {

    }

    public class Airborne : PlatformerState
    {

    }
}
