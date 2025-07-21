using UnityEngine;

public class WizardAnimationHelper : MonoBehaviour
{
    [SerializeField]
    private Follower _myFollower;

    [SerializeField]
    private SpriteRenderer _robe;

    [SerializeField]
    private SpriteRenderer _hat;

    [SerializeField]
    private SpriteRenderer _head;

    [SerializeField]
    private Sprite[] _robeSprites;

    [SerializeField]
    private Sprite[] _hatSprites;

    [SerializeField]
    private float _clothingAnimationFrameCycleTime = 0.4f;

    private float hatTimer;
    private float robeTimer;

    private int hatIndex;
    private int robeIndex;

    private Vector2 hatOffset;
    private Vector2 robeOffset;

    private void Start()
    {
        hatTimer += _clothingAnimationFrameCycleTime / 2.0f;

        hatOffset = _hat.transform.localPosition;
        robeOffset = _robe.transform.localPosition;
    }

    private void Update()
    {
        hatTimer += Time.deltaTime;
        robeTimer += Time.deltaTime;

        if(robeTimer >= _clothingAnimationFrameCycleTime)
        {
            robeIndex++;
            robeIndex %= _robeSprites.Length;
            _robe.sprite = _robeSprites[robeIndex];

            robeTimer = 0.0f;
        }

        if (hatTimer >= _clothingAnimationFrameCycleTime)
        {
            hatIndex++;
            hatIndex %= _hatSprites.Length;
            _hat.sprite = _hatSprites[hatIndex];

            hatTimer = 0.0f;
        }

        _hat.transform.localPosition = new Vector2(Mathf.Cos(Time.time * 1.6f) * 0.07f, Mathf.Sin(Time.time * 1.8f) * 0.04f) + hatOffset;
        _robe.transform.localPosition = new Vector2(Mathf.Sin((Time.time * 1.6f) - 0.5f) * 0.08f, Mathf.Cos((Time.time * 3.5f) - 0.5f) * 0.04f) + robeOffset;

        if(_myFollower.direction == EDirection.Left)
        {
            _head.flipX = true;
            _robe.flipX = true;
        }
        else if(_myFollower.direction == EDirection.Right)
        {
            _head.flipX = false;
            _robe.flipX = false;
        }
    }
}
