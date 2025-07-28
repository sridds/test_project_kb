using System.Collections.Generic;
using UnityEngine;
public class Unit
{
    protected UnitDataObject data;
    protected Health health;

    public ActionData attack;
    public ActionData guard;
    public List<ActionData> abilities;

    #region -Accessors-
    public UnitDataObject Data { get { return data; } }
    public Health Health { get { return health; } }
    public int Attack { get { return data.DefaultStats.BaseAttack; } }
    public int Defense { get { return data.DefaultStats.BaseDefense; } }

    #endregion
}