using UnityEngine;

[CreateAssetMenu(fileName = "PartyDataObject", menuName = "Party/PartyDataObject")]
public class PartyDataObject : ScriptableObject
{
    public FieldPartyMember DefaultPrefab;
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