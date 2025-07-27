using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public enum BattleStates
    {
        START,
        PLAN,
        PARTYACTION,
        ENEMYACTION,
    }

    private BattleStates battleState = BattleStates.START;

    [SerializeField] private List<BattleEntity> partyMembers = new();
    [SerializeField] private List<BattleEntity> enemies = new();

    [SerializeField] private BattleEntity selectedUnit;
    private Vector2Int selectedUnitCordinates = Vector2Int.zero;

    BattleUIManager battleUIManager;

    #region -Acsessors-
    public BattleStates BattleState { get { return battleState; } }
    #endregion

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
        GetReferences();
        SetUpBattle();
    }

    void GetReferences()
    {
        battleUIManager = BattleUIManager.instance;
    }

    private void Update()
    {
        ControlBattleFlow();
    }

    void ControlBattleFlow()
    {
        switch (battleState)
        {
            case BattleStates.START:
                ChangeBattleState(BattleStates.PLAN);
                break;
            case BattleStates.PLAN:
                ControlUnitSelector();
                break;
            case BattleStates.PARTYACTION:
                break;
            case BattleStates.ENEMYACTION:
                break;
        }
    }

    public void ChangeBattleState(BattleStates newState)
    {
        battleState = newState;

        //If something happens on switch, put it here.
        switch (battleState)
        {
            case BattleStates.START:
                break;
            case BattleStates.PLAN:
                break;
            case BattleStates.PARTYACTION:
                break;
            case BattleStates.ENEMYACTION:
                break;
        }
    }

    void SetUpBattle()
    {
        selectedUnit = partyMembers[0];
    }

    public void ControlUnitSelector()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            selectedUnitCordinates.y--;

            if (selectedUnitCordinates.x == 0 && selectedUnitCordinates.y < 0)
            {
                selectedUnitCordinates.y = partyMembers.Count - 1;
            }
            if (selectedUnitCordinates.x == 1 && selectedUnitCordinates.y < 0)
            {
                selectedUnitCordinates.y = enemies.Count - 1;
            }

            SetSelectedUnit();
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            selectedUnitCordinates.y++;

            if (selectedUnitCordinates.x == 0 && selectedUnitCordinates.y > partyMembers.Count - 1)
            {
                selectedUnitCordinates.y = 0;
            }
            if (selectedUnitCordinates.x == 1 && selectedUnitCordinates.y > enemies.Count - 1)
            {
                selectedUnitCordinates.y = 0;
            }

            SetSelectedUnit();
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            selectedUnitCordinates.x = 1;
            if (selectedUnitCordinates.y > enemies.Count - 1)
            {
                selectedUnitCordinates.y = enemies.Count - 1;
            }
            SetSelectedUnit();
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            selectedUnitCordinates.x = 0;
            if (selectedUnitCordinates.y > partyMembers.Count - 1)
            {
                selectedUnitCordinates.y = partyMembers.Count - 1;
            }
            SetSelectedUnit();
        }
    }

    void SetSelectedUnit()
    {
        if (selectedUnitCordinates.x == 0)
        {
            selectedUnit = partyMembers[selectedUnitCordinates.y];
        }
        if (selectedUnitCordinates.x == 1)
        {
            selectedUnit = enemies[selectedUnitCordinates.y];
        }

        Vector2 targetPosition = selectedUnit.selectorCenter + (Vector2)selectedUnit.transform.position;
        Vector2 targetSize = selectedUnit.selectorSize;
        battleUIManager.MoveUnitSelector(targetPosition);
        battleUIManager.ResizeUnitSelector(targetSize);
    }









}
