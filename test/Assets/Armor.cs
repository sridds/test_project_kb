using UnityEngine;
using NaughtyAttributes;
// An armor item grants the holder a defense bonus when placed in its own reserved equippable slot
[CreateAssetMenu(menuName = "Items/Armor")]
public class Armor : Item
{
    [Header("Armor Settings")]

    [EnumFlags]
    public EPartyExclusiveTag memberExclusiveTag;

    public int defenseBonus;
}
