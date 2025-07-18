using UnityEngine;
using NaughtyAttributes;
// A weapon grants the holder an attack bonus when placed in its own reserved equippable slot
[CreateAssetMenu(menuName = "Items/Weapon")]
public class Weapon : Item
{
    [Header("Weapon Settings")]

    [EnumFlags]
    public EPartyExclusiveTag memberExclusiveTag;

    public int attackBonus;
}
