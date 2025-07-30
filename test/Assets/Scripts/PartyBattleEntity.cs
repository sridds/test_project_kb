using UnityEngine;

public class PartyBattleEntity : BattleEntity
{
    //Should Never directly access this except for initting in the test battle scene.
    [SerializeField] public PartyDataObject partyData;

    private PartyMember partyMember;

    #region -Accessors-
    public PartyMember PartyMember { get { return partyMember; } }

    #endregion


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public override void Init(Unit unit)
    {
        base.Init(unit);

        if (unit.GetType() != typeof(PartyMember))
        {
            Debug.LogError("Initting party battle entity with wrong Unit class type");
            return;
        }

        partyMember = (PartyMember)unit;
    }
}
