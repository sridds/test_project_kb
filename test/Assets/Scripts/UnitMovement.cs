using UnityEngine;

public class UnitMovement : MonoBehaviour
{
    [SerializeField]
    private float _unitSpeed;

    [SerializeField]
    private float _runSpeed;

    private Rigidbody2D rb;
    private Vector2 frameInput;
    private bool isRunning;
    private bool isMoving;

    public bool IsRunning { get { return isRunning; } }
    public bool IsMoving { get { return isMoving; } }
    public Vector2 Velocity { get { return rb.linearVelocity; } }


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

        // Get speed
        isRunning = Input.GetKey(KeyCode.X);
        float speed = isRunning ? _runSpeed : _unitSpeed;

        rb.linearVelocity = new Vector2(frameInput.x, frameInput.y).normalized * speed;
    }

    private void LateUpdate()
    {
        isMoving = rb.linearVelocity.magnitude > 0.1f && frameInput != Vector2.zero;
    }
}
