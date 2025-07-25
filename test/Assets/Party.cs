using UnityEngine;
using System.Collections.Generic;

public class Party : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int _maxInventorySize = 12;
    [SerializeField] private int _maxKeyItemSlots = 12;
    [SerializeField] private int _maxEquippables = 48;

    private int money;
    private int mana;
    private Dictionary<PartyDataObject, PartyMember> party = new Dictionary<PartyDataObject, PartyMember>();
    private Consumable[] myInventory;
    private Key[] keys;

    #region - Accessors -
    public int Money => money;
    public int Mana => mana;
    public Consumable[] Bag => myInventory;
    #endregion

    public delegate void MoneyValueChanged(int oldValue, int newValue);
    public MoneyValueChanged OnMoneyValueChanged;

    private void Awake()
    {
        foreach(FieldPartyMember member in FindObjectsByType<FieldPartyMember>(FindObjectsSortMode.None))
        {
            RegisterPartyMember(member);
        }

        // Initialize inventory
        myInventory = new Consumable[_maxInventorySize];
        keys = new Key[_maxKeyItemSlots];
    }

    public void AddMoney(int amount)
    {
        money += amount;
        OnMoneyValueChanged?.Invoke(money - amount, money);
    }

    public void TakeMoney(int amount)
    {
        money -= amount;
        OnMoneyValueChanged?.Invoke(money + amount, money);
    }

    // When registering a party member from data, this function needs to know where to spawn the party member
    public void RegisterPartyMember(PartyDataObject data, Vector2 spawnPoint)
    {
        CreatePartyMemberFromData(data);

        // Create party member on field
        FieldPartyMember fieldMember = Instantiate(data.DefaultPrefab, transform);
        fieldMember.transform.position = spawnPoint;
    }

    // Adds member to the party
    public void RegisterPartyMember(FieldPartyMember fieldMember)
    {
        CreatePartyMemberFromData(fieldMember.myDataObject);
    }

    // Create / load party member data and add to the list of party members. This function will handle loading data from file aswell
    private void CreatePartyMemberFromData(PartyDataObject data)
    {
        PartyMember partyMember = new PartyMember(data, _maxEquippables);
        party.Add(data, partyMember);

        Debug.Log($"Registered new party member: [{data.DefaultStats.Name}]");
    }

    public PartyMember GetPartyMember(PartyDataObject obj) => party[obj];
}

public class PartyMember : Unit
{
    // Data / stats
    private PartyDataObject data;

    // Party members carry their own exclusive weapons and armors (like Deltarune) max of 48
    private Weapon[] weapons;
    private Armor[] armors;

    // Equipped items
    private Weapon equippedWeapon;
    private Armor equippedArmor;

    #region Accessors
    public Weapon EquippedWeapon { get { return equippedWeapon; } }
    public Armor EquippedArmor { get { return equippedArmor; } }
    #endregion

    public PartyMember(PartyDataObject data, int maxInventorySize)
    {
        this.data = data;
        health = new Health();

        weapons = new Weapon[maxInventorySize];
        armors = new Armor[maxInventorySize];
    }

    public void EquipWeapon(int slotIndex)
    {
        if (slotIndex > weapons.Length || slotIndex < 0) return;

        // Swap current equipped weapon with weapon at slotIndex
        Weapon temp = weapons[slotIndex];
        weapons[slotIndex] = equippedWeapon;
        equippedWeapon = temp;
    }

    public void EquipArmor(int slotIndex)
    {
        if (slotIndex > weapons.Length || slotIndex < 0) return;

        // Swap current equipped armor with armor at slotIndex
        Armor temp = armors[slotIndex];
        armors[slotIndex] = equippedArmor;
        equippedArmor = temp;
    }
}
