using UnityEngine;
using DG.Tweening;

public class NumberSprite : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer _renderer;

    [SerializeField]
    private Sprite[] _numberSprites;

    public SpriteRenderer Renderer { get { return _renderer; } }
    public Sprite[] NumberSprites { get { return _numberSprites; } }
    public int MyDigit { get { return digit; } }
    public int MyIndex { get { return index; } }

    int digit;
    int index = 0;

    public void SetDigit(int digit, int index)
    {
        _renderer.sprite = _numberSprites[digit];

        this.index = index;
        this.digit = digit;
    }
}
