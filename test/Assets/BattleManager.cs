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

    public enum PlanningStates
    {
        CHOOSEUNIT,
        CHOOSEACTION,
        CHOOSESUBACTION,
        CHOOSETARGET,
    }

    public enum SelectionTypes
    {
        PARTYMEMBERS,
        ENEMIES,
        ALL,
    }

    private PlanningStates planningState = PlanningStates.CHOOSEUNIT;

    private BattleStates battleState = BattleStates.START;

    [SerializeField] private List<BattleEntity> partyMembers = new();
    [SerializeField] private List<BattleEntity> enemies = new();

    [SerializeField] private BattleEntity hoveredUnit;
    [SerializeField] private BattleEntity selectedUnit;

    private Vector2Int hoveredUnitCoordinates = Vector2Int.zero;

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
                ControlPlanningFlow();
                break;
            case BattleStates.PARTYACTION:
                break;
            case BattleStates.ENEMYACTION:
                break;
        }
    }

    void ControlPlanningFlow()
    {
        switch (planningState)
        {
            case PlanningStates.CHOOSEUNIT:
                ControlUnitSelector(SelectionTypes.PARTYMEMBERS);
                break;
            case PlanningStates.CHOOSEACTION:
                break;
            case PlanningStates.CHOOSESUBACTION:
                break;
            case PlanningStates.CHOOSETARGET: 
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
        hoveredUnit = partyMembers[0];
    }

    public void ControlUnitSelector(SelectionTypes targets)
    {
        if (hoveredUnit == null)
        {
            SetHoveredUnit();
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            hoveredUnitCoordinates.y--;

            if (hoveredUnitCoordinates.x == 0 && hoveredUnitCoordinates.y < 0)
            {
                hoveredUnitCoordinates.y = partyMembers.Count - 1;
            }
            if (hoveredUnitCoordinates.x == 1 && hoveredUnitCoordinates.y < 0)
            {
                hoveredUnitCoordinates.y = enemies.Count - 1;
            }

            SetHoveredUnit();
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            hoveredUnitCoordinates.y++;

            if (hoveredUnitCoordinates.x == 0 && hoveredUnitCoordinates.y > partyMembers.Count - 1)
            {
                hoveredUnitCoordinates.y = 0;
            }
            if (hoveredUnitCoordinates.x == 1 && hoveredUnitCoordinates.y > enemies.Count - 1)
            {
                hoveredUnitCoordinates.y = 0;
            }

            SetHoveredUnit();
        }


        if (targets == SelectionTypes.PARTYMEMBERS)
        {
            hoveredUnitCoordinates.y = 0;
        }

        if (targets == SelectionTypes.PARTYMEMBERS)
        {
            hoveredUnitCoordinates.y = 1;
        }

        if (targets == SelectionTypes.ALL)
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                hoveredUnitCoordinates.x = 1;
                if (hoveredUnitCoordinates.y > enemies.Count - 1)
                {
                    hoveredUnitCoordinates.y = enemies.Count - 1;
                }
                SetHoveredUnit();
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                hoveredUnitCoordinates.x = 0;
                if (hoveredUnitCoordinates.y > partyMembers.Count - 1)
                {
                    hoveredUnitCoordinates.y = partyMembers.Count - 1;
                }
                SetHoveredUnit();
            }
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            SetSelectedUnit();
        }
    }

    void SetHoveredUnit()
    {
        if (hoveredUnitCoordinates.x == 0)
        {
            hoveredUnit = partyMembers[hoveredUnitCoordinates.y];
        }
        if (hoveredUnitCoordinates.x == 1)
        {
            hoveredUnit = enemies[hoveredUnitCoordinates.y];
        }

        Vector2 targetPosition = hoveredUnit.selectorCenter + (Vector2)hoveredUnit.transform.position;
        Vector2 targetSize = hoveredUnit.selectorSize;
        battleUIManager.MoveUnitSelector(targetPosition);
        battleUIManager.ResizeUnitSelector(targetSize);
    }

    void SetSelectedUnit()
    {

        switch (planningState)
        {
            case PlanningStates.CHOOSEUNIT:
                Unit selectedPartyUnit = hoveredUnit.Unit;
                battleUIManager.SetCurrentBattleUIToUnit((PartyDataObject)selectedPartyUnit.Data);
                planningState = PlanningStates.CHOOSEACTION;
                break;
            case PlanningStates.CHOOSEACTION:
                break;
            case PlanningStates.CHOOSESUBACTION:
                break;
            case PlanningStates.CHOOSETARGET:
                break;
        }
    }







}
