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

    [SerializeField]
    private int _maxBagSize = 15;
    #endregion

    #region Private Fields
    private Inventory bag;
    #endregion

    #region Accessors
    public Inventory Bag => bag;
    #endregion

    void Awake()
    {
        bag = new Inventory(_maxBagSize);

        instance = this;

        foreach (Item item in startingItems)
        {
            bag.TryAddItem(item);
        }
    }
}

public class Inventory
{
    public List<Item> Items => items;

    private List<Item> items;
    private int maxSize;

    public Inventory(int size)
    {
        items = new List<Item>();
        maxSize = size;
    }

    public Item IndexOf(int index)
    {
        if (!IsIndexValid(index)) return null;

        return items[index];
    }

    // Returns true if sucessfully added item, returns false if failed
    public bool TryAddItem(Item item)
    {
        if (IsBagFull())
        {
            Debug.Log($"Failed to add \"{item.ItemName}\" to bag! Bag is full");
            return false;
        }

        items.Add(item);
        Debug.Log($"Added item to index {items.IndexOf(item)}");

        return true;
    }

    public bool TryRemoveItem(int index, out Item item)
    {
        item = null;
        if (!IsIndexValid(index)) return false;

        item = items[index];
        items.RemoveAt(index);

        return true;
    }

    private bool IsIndexValid(int index)
    {
        if (index >= 0 && index < items.Count) return true;

        Debug.Log($"Index: {index} is invalid! (inventory size: {items.Count}");
        return false;
    }

    public int MaxSize() => maxSize;

    public int Count() => items.Count();
    
    public bool IsBagFull() => items.Count >= maxSize;

    public bool IsBagEmpty() => items.Count == 0;
}