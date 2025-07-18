using System.Collections;
using UnityEngine;
using static AnnoyingRock;

public class AnnoyingRock : MonoBehaviour
{
    [SerializeField]
    private Rigidbody2D _rb;

    [SerializeField]
    private SpriteRenderer _renderer;

    [SerializeField]
    private float _paddingAmount = 0.5f;

    [SerializeField]
    private float _viewportHeight = 15.0f;

    [SerializeField]
    private float _spinSpeed = 25.0f;

    [SerializeField]
    private AudioSource _rockSource;

    [SerializeField]
    private AudioClip _rockHitSound;

    private Transform target;
    private bool hasHit;

    public delegate void RockHit();
    public RockHit OnRockHit;

    private void Start()
    {
        _renderer.enabled = false;
    }

    public void Fall(Transform target, float startOffset, float force)
    {
        _renderer.enabled = true;
        float cameraY = Camera.main.transform.position.y;

        transform.position = new Vector3(target.position.x + startOffset, cameraY + (_viewportHeight / 2.0f) + _paddingAmount);
        _rb.simulated = true;
        _rb.AddForceX(force, ForceMode2D.Impulse);

        this.target = target;
    }

    private void Update()
    {
        // spin
        _renderer.transform.localEulerAngles = new Vector3(0.0f, 0.0f, _renderer.transform.localEulerAngles.z + (_spinSpeed * Time.deltaTime));
    }

    private void FixedUpdate()
    {
        if (hasHit) return;

        if(transform.position.y <= target.position.y + 0.5f)
        {
            _rockSource.PlayOneShot(_rockHitSound, 1.0f);
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, 0.0f);
            _rb.AddForceY(5.0f, ForceMode2D.Impulse);
            OnRockHit?.Invoke();

            hasHit = true;
        }
    }
}
