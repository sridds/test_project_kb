using UnityEngine;

public class FieldPartyMember : MonoBehaviour
{
    [SerializeField]
    private PartyDataObject _myDataObject;

    #region - Accessors -
    public PartyDataObject myDataObject { get { return _myDataObject; }}
    #endregion
}
