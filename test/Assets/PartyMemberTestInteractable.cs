using UnityEngine;

public class PartyMemberTestInteractable : Interactable
{
    [SerializeField]
    private PartyDataObject _partyMember;

    protected override void HandleInteraction()
    {
        FieldPartyMember member = Instantiate(_partyMember.DefaultPrefab);
        
    }
}
