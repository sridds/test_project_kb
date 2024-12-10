using UnityEngine;
using TMPro;

public class SafeZoneOption : MonoBehaviour
{
    public TextMeshPro text;
    public SafeZoneHandler handler;

    public bool isEasy;
    public bool isMedium;
    public bool isHard;

    public void Hover()
    {
        text.color = Color.yellow;
    }

    public void Unhover()
    {
        text.color = Color.white;
    }

    public void Select()
    {
        if (isEasy)
        {
            handler.EasySelected();
        }
        else if (isMedium)
        {
            handler.MediumSelected();
        }
        else if (isHard)
        {
            handler.HardSelected();
        }
    }
}
