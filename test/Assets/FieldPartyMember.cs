using UnityEngine;

public class FieldPartyMember : MonoBehaviour
{
    [SerializeField]
    private PartyUnit _myBattleUnit;

    public PartyUnit MyBattleUnit { get { return _myBattleUnit; }}
}
