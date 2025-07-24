using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    private List<Unit> partyMembers = new();
    private List<Unit> enemies = new();


    #region - Important Once Hooked Up to Overworld -

    [SerializeField] private BattleEntity BattleEntityPrefab;

    #region - Spawn Position Information -

    [Foldout("Spawn Position Information")]
    [SerializeField] private float partySpawnX;
    [Foldout("Spawn Position Information")]
    [SerializeField] private float partySpawnMaxY;
    [Foldout("Spawn Position Information")]
    [SerializeField] private float partySpawnMinY;
    [Foldout("Spawn Position Information")]
    [SerializeField] private float enemySpawnX;
    [Foldout("Spawn Position Information")]
    [SerializeField] private float enemySpawnMaxY;
    [Foldout("Spawn Position Information")]
    [SerializeField] private float enemySpawnMinY;

    #endregion
    #endregion

    public void Init()
    {

    }

    private void Start()
    {
        SetUpBattle();
    }

    void SetUpBattle()
    {
        foreach (PartyBattleEntity party in FindObjectsByType<PartyBattleEntity>(FindObjectsSortMode.None))
        {
            RegisterPartyMember(party.Unit);
        }

        foreach (EnemyBattleEntity enemy in FindObjectsByType<EnemyBattleEntity>(FindObjectsSortMode.None))
        {
            RegisterPartyMember(enemy.Unit);
        }
    }

    public void RegisterPartyMember(Unit unit)
    {
        partyMembers.Add(unit);
    }

    public void RegisterEnemy(Unit unit)
    {
        enemies.Add(unit);
    }















    //UNUSED. Once hooked up with overworld, this should get called to spawn in units.
    void SpawnBattleEntities()
    {
        //Set up spawn y height
        float[] tValues = new float[partyMembers.Count + 2];
        for (int i = 0; i < tValues.Length; i++)
        {
            tValues[i] = i / tValues.Length - 1;
        }

        int index = 1;
        foreach (Unit unit in partyMembers)
        {
            //Set Spawn Position
            float spawnY = Mathf.Lerp(partySpawnMinY, partySpawnMaxY, tValues[index]);
            Vector2 spawnPos = new Vector2(partySpawnX, spawnY);
            index++;

            //Create battle entity
            BattleEntity newBattleEntity = Instantiate(BattleEntityPrefab, spawnPos, Quaternion.identity);
            newBattleEntity.Init(unit);
        }
    }
}
