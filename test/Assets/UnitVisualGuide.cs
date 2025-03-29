using UnityEngine;

[CreateAssetMenu(menuName = "Battles/UnitVisualGuide")]
public class UnitVisualGuide : ScriptableObject
{
    public Color BaseColor = Color.white;
    public Color AlternateColor = Color.white;
    public Sprite BattleIcon;
}
