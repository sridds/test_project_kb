using UnityEngine;

public enum EDirectionFacing
{
    Left,
    Right,
    Up,
    Down
}

public class UnitMovement : MonoBehaviour
{
    [SerializeField]
    private float _unitSpeed;

    [SerializeField]
    private float _runSpeed;

    [SerializeField]
    private SpriteRenderer _testSpr;

    [SerializeField]
    private Sprite[] _testDir;

    private Rigidbody2D rb;
    private Vector2 frameInput;
    private bool isRunning;
    private bool isMoving;
    private EDirectionFacing direction;

    public bool IsRunning { get { return isRunning; } }
    public bool IsMoving { get { return isMoving; } }
    public Vector2 Velocity { get { return rb.linearVelocity; } }
    public EDirectionFacing Direction { get { return direction; } }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (GameManager.Instance.GameState != GameManager.EGameState.Playing)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        // Get axis
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        frameInput = new Vector2(x, y);

        // Set current direction
        if(Mathf.Abs(frameInput.normalized.x) == 1)
        {
            direction = frameInput.x > 0 ? EDirectionFacing.Right : EDirectionFacing.Left;
        }
        else if(Mathf.Abs(frameInput.normalized.y) == 1)
        {
            direction = frameInput.y > 0 ? EDirectionFacing.Up : EDirectionFacing.Down;
        }

        // Get speed
        isRunning = Input.GetKey(KeyCode.X);
    }

    private void FixedUpdate()
    {
        float speed = isRunning ? _runSpeed : _unitSpeed;
        rb.linearVelocity = new Vector2(frameInput.x, frameInput.y).normalized * speed;
    }

    private void LateUpdate()
    {
        isMoving = rb.linearVelocity.magnitude > 0.1f && frameInput != Vector2.zero;
    }
}
