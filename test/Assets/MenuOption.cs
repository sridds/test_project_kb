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

    private void Start()
    {
        originX = transform.localPosition.x;

        Unhover();
    }

    public void SetText(string text)
    {
        _text.text = text;
    }

    public void Hover(bool force = false)
    {
        if (hovered && !force) return;

        transform.DOKill(false);
        transform.DOLocalMoveX(originX + _jutAmount, _easeTime).SetEase(Ease.OutQuad);
        _text.color = _selectedColor;

        hovered = true;
    }

    public void Unhover(bool force = false)
    {
        if (!hovered && !force) return;

        transform.DOKill(false);
        transform.DOLocalMoveX(originX, _easeTime).SetEase(Ease.OutQuad);
        _text.color = _defaultColor;

        hovered = false;
    }

    public void Disable(bool disableHover)
    {
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
        if (hovered) Hover(true);
        else Unhover(true);
    }
}
