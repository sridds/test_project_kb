using UnityEngine;

public class UnitDataObject : ScriptableObject
{
    public BattleEntity BattlePrefab;
    public Stats DefaultStats;
}

[System.Serializable]
public struct Stats
{
    public string Name;
    public int MaxHP;
    public int BaseAttack;
    public int BaseDefense;
}
