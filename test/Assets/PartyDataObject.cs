using UnityEngine;

[CreateAssetMenu(fileName = "PartyDataObject", menuName = "Party/PartyDataObject")]
public class PartyDataObject : ScriptableObject
{
    public FieldPartyMember DefaultPrefab;
    public Stats DefaultStats;
}