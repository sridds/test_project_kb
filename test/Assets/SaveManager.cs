using System;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public class SaveManager
{
    public static string SaveFilePath() => Application.persistentDataPath + "/save" + ".save";
    public static bool DoesSaveFileExist() => File.Exists(SaveFilePath());

    public static void DeleteData()
    {
        Debug.Log("Deleting save data");
        File.Delete(SaveFilePath());
    }

    public static bool Save(SaveData data)
    {
        try
        {
            // Delete file if it exists already
            if (File.Exists(SaveFilePath()))
            {
                Debug.Log("Data exists. Replacing old file");
                File.Delete(SaveFilePath());
            }

            Debug.Log("Writing to file");
            using FileStream stream = File.Create(SaveFilePath());
            stream.Close();
            File.WriteAllText(SaveFilePath(), JsonConvert.SerializeObject(data));

            return true;
        }
        catch(Exception e)
        {
            Debug.LogError($"Unable to save data: {e.Message} {e.StackTrace}");
            return false;
        }
    }

    public static SaveData Load()
    {
        if (!File.Exists(SaveFilePath()))
        {
            Debug.Log("No save file exists!");
            throw new FileNotFoundException($"{SaveFilePath()} does not exist!");
        }

        try
        {
            SaveData data = JsonConvert.DeserializeObject<SaveData>(File.ReadAllText(SaveFilePath()));
            return data;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load save data: {e.Message} {e.StackTrace}");
            throw e;
        }
    }
}

[System.Serializable]
public class SaveData
{
    public float hankPositionX;
    public float hankPositionY;
    public int currentSceneIndex;
    public int roomIndex;

    public PartySaveData partySaveData;
    public PartyMemberSaveData[] partyMemberSaveData;
}

[System.Serializable]
public class PartySaveData
{
    public int money;
    public Consumable[] inventory;
    public Key[] keys;
}

[System.Serializable]
public class PartyMemberSaveData
{
    public string nameKey;
    public int hp;
    public Armor equippedArmor;
    public Armor[] armors;
    public Weapon equippedWeapon;
    public Weapon[] weapons;
    public Vector2 worldPosition;
}