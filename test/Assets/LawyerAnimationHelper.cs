using UnityEngine;

public class LawyerAnimationHelper : MonoBehaviour
{
    [SerializeField]
    private Follower _follower;

    [SerializeField]
    private Sprite[] _walkForward;

    [SerializeField]
    private Sprite[] _walkBackwards;

    [SerializeField]
    private Sprite[] _walkLeft;

    [SerializeField] 
    private Sprite[] _walkRight;

    [SerializeField]
    private SpriteRenderer _renderer;

    [SerializeField]
    private float _walkSpriteTime;

    [SerializeField]
    private float _runSpriteTime;

    int index = 0;
    float timer;
    Sprite[] walkSprites;
    EDirection lastDirection;

    private void Start()
    {
        walkSprites = _walkForward;
    }

    void Update()
    {
        UnitMovement movement = _follower.followManager.leader;

        if (movement.IsMoving)
        {
            if(lastDirection != _follower.direction)
            {
                index = 0;
                lastDirection = _follower.direction;
            }

            walkSprites = GetWalkSpritesFromDirection();

            if(timer > (movement.IsRunning ? _runSpriteTime : _walkSpriteTime))
            {
                index++;
                index %= walkSprites.Length;
                timer = 0.0f;
            }

            _renderer.sprite = walkSprites[index];
            timer += Time.deltaTime;
        }
        else
        {
            if(index == 1 || index == 3)
            {
                timer += Time.deltaTime;

                if (timer > (movement.IsRunning ? _runSpriteTime : _walkSpriteTime))
                {
                    index = 0;
                    timer = 0.0f;
                    _renderer.sprite = walkSprites[0];
                }
            }
            else
            {
                index = 0;
                timer = 0.0f;
                _renderer.sprite = walkSprites[0];
            }
        }
    }

    private Sprite[] GetWalkSpritesFromDirection()
    {
        switch (_follower.direction)
        {
            case EDirection.Left:
                return _walkLeft;
            case EDirection.Right:
                return _walkRight;
            case EDirection.Up:
                return _walkBackwards;
            case EDirection.Down:
                return _walkForward;
        }

        return null;
    }
}
