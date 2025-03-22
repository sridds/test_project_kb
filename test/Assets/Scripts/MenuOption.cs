using UnityEngine;
using TMPro;
using DG.Tweening;

public class MenuOption : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private RectTransform _myRectTransform;

    [Header("Selection")]
    [SerializeField]
    private Color _defaultColor;

    [SerializeField]
    private Color _selectedColor;

    [SerializeField]
    private Color _disabledColor;

    [SerializeField]
    private TextMeshProUGUI _text;

    [Header("Animation")]
    [SerializeField]
    private float _jutAmount = 3.0f;

    [SerializeField]
    private float _easeTime = 0.3f;

    private float originX = 0.0f;
    private bool hovered;
    private bool lateFlag = true;
    private bool isEnabled = true;

    public bool IsEnabled => isEnabled;

    private void Start()
    {
        originX = transform.localPosition.x;

        Unhover();
    }

    private void LateUpdate()
    {
        if(hovered && !lateFlag)
        {
            transform.DOKill(false);
            transform.DOLocalMoveX(originX + _jutAmount, _easeTime).SetEase(Ease.OutQuad);
            _text.color = _selectedColor;

            lateFlag = true;
        }

        else if(!hovered && !lateFlag)
        {
            transform.DOKill(false);
            transform.DOLocalMoveX(originX, _easeTime).SetEase(Ease.OutQuad);
            _text.color = _defaultColor;

            lateFlag = true;
        }
    }

    public void SetText(string text)
    {
        _text.text = text;
    }

    public void Hover(bool force = false)
    {
        if (hovered && !force) return;

        lateFlag = false;
        hovered = true;
    }

    public void Unhover(bool force = false)
    {
        if (!hovered && !force) return;

        lateFlag = false;
        hovered = false;
    }

    public void Disable(bool disableHover)
    {
        isEnabled = false;

        if (disableHover && hovered)
        {
            Unhover();
            hovered = false;
        }

        if(hovered) _text.color = _defaultColor;
        else _text.color = _disabledColor;
    }

    public void Enable()
    {
        isEnabled = true;

        if (hovered) Hover(true);
        else Unhover(true);
    }
}
