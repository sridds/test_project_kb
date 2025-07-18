using System.IO;
using UnityEngine;

public class SaveManager
{
    private static SaveData saveData = new SaveData();

    [System.Serializable]
    public struct SaveData
    {
        public PartySaveData partySaveData;
        public PartyMemberSaveData[] partyMemberSaveData;
    }

    public static string SaveFilePath() => Application.persistentDataPath + "/save" + ".save";

    public static void Save()
    {
        HandleSave();

        File.WriteAllText(SaveFilePath(), JsonUtility.ToJson(saveData, true));
    }

    public static void Load()
    {
        string saveContent = File.ReadAllText(SaveFilePath());

        saveData = JsonUtility.FromJson<SaveData>(saveContent);
        HandleLoad();
    }

    private static void HandleLoad()
    {
        // distribute data to other sources

        // for like NPCs and stuff , every time the scene is loaded or a room is loaded they will load from the save manager if they havent already
    }

    private static void HandleSave()
    {
        // get all data (party relevant, game relevant, save it)

        // party can be accessed through the game manager, which has a reference to the active party
        // game manager oversees the distribution of that data
        //
    }
}

[System.Serializable]
public struct PartySaveData
{
    public int money;
    public Consumable[] inventory;
    public Key[] keys;
}

[System.Serializable]
public struct PartyMemberSaveData
{
    public string nameKey;
    public int hp;
    public Armor equippedArmor;
    public Armor[] armors;
    public Weapon equippedWeapon;
    public Weapon[] weapons;
    public Vector2 worldPosition;
}