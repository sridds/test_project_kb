using System.Collections.Generic;
using UnityEngine;
public class Unit
{
    protected UnitDataObject data;
    protected Health health;

    public BattleAction attack;
    public BattleAction guard;
    public List<BattleAction> abilities;

    #region -Accessors-
    public UnitDataObject Data { get { return data; } }
    public Health Health { get { return health; } }
    public int Attack { get { return data.DefaultStats.BaseAttack; } }
    public int Defense { get { return data.DefaultStats.BaseDefense; } }

    #endregion

    public Unit(UnitDataObject data)
    {
        this.data = data;
        attack = data.attack; 
        guard = data.guard;
        foreach (BattleAction action in data.startingAbilities)
        {
            abilities.Add(action);
        }
    }
}