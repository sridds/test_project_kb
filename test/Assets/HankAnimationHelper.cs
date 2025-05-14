using UnityEngine;

public class HankAnimationHelper : MonoBehaviour
{
    private const string IDLE_HASH = "Hank_Idle";
    private const string WALK_HASH = "Hank_Walk";

    [SerializeField]
    private UnitMovement _movement;

    [SerializeField]
    private SpriteRenderer _renderer;

    [SerializeField]
    private float _walkSpeedMultiplier;

    [SerializeField]
    private float _runSpeedMultiplier;

    [SerializeField]
    private Animator _hankAnimator;

    [SerializeField]
    private bool _footstepsEnabled;

    [SerializeField]
    private AudioClip _footstepClip;

    [SerializeField]
    private AudioSource _source;

    private void Update()
    {
        if(_movement.Velocity.x != 0)
        {
            _renderer.flipX = _movement.Velocity.x < 0;
        }

        if (_movement.IsMoving)
        {
            _hankAnimator.Play(WALK_HASH);
        }
        else
        {
            _hankAnimator.Play(IDLE_HASH);
        }

        if(_movement.IsRunning)
        {
            _hankAnimator.SetFloat("Speed", _runSpeedMultiplier);
        }
        else
        {
            _hankAnimator.SetFloat("Speed", _walkSpeedMultiplier);
        }
    }

    public void Footstep()
    {
        if (!_footstepsEnabled) return;

        _source.pitch = Random.Range(0.9f, 1.1f);
        _source.PlayOneShot(_footstepClip);
    }
}
