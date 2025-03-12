using UnityEngine;
using TMPro;

public class MenuOption : MonoBehaviour
{
    [SerializeField]
    private Color _defaultColor;

    [SerializeField]
    private Color _selectedColor;

    [SerializeField]
    private TextMeshProUGUI _text;


    public void Hover()
    {
        _text.color = _selectedColor;
    }

    public void Unhover()
    {
        _text.color = _defaultColor;
    }
}
