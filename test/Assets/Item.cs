using UnityEngine;

public enum EPartyExclusiveTag
{
    Hank = 1 << 0,
    Lawyer = 1 << 1,
    Wizard = 1 << 2,
}
public abstract class Item : ScriptableObject
{
    [Header("General Settings")]
    public string Name;
    public string Description;
}
