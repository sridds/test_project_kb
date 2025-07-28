using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "PartyDataObject", menuName = "Party/PartyDataObject")]
public class PartyDataObject : UnitDataObject
{
    public FieldPartyMember DefaultPrefab;

    [SerializeField] public Color UIColor;
}

