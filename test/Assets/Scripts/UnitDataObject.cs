using UnityEngine;

public class UnitDataObject : ScriptableObject
{
    public BattleEntity BattlePrefab;
    public Stats DefaultStats;
    public BattleAction attack;
    public BattleAction guard;
    public BattleAction[] startingAbilities;
}

[System.Serializable]
public struct Stats
{
    public string Name;
    public int MaxHP;
    public int BaseAttack;
    public int BaseDefense;
}
