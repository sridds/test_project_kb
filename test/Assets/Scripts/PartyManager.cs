using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Hank.Battles;

public class PartyManager : MonoBehaviour
{
    public static PartyManager Instance => instance;
    private static PartyManager instance;

    #region Inspector Fields
    [SerializeField]
    private PartyMemberUnit[] partyMembers;

    [SerializeField]
    private Item[] startingItems;

    //[SerializeField]
    //private int _maxBagSize = 15;
    #endregion

    #region Private Fields
    //private Inventory bag;
    #endregion

    #region Accessors
    /// <summary>
    /// public Inventory Bag => bag;
    /// </summary>
    #endregion


    void Awake()
    {
        /*
        bag = new Inventory(_maxBagSize);

        instance = this;

        foreach (Item item in startingItems)
        {
            bag.TryAddItem(item);
        }*/
    }
}