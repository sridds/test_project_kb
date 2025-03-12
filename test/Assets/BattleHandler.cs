using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using DG.Tweening;
using Hank.Systems.StateMachine;

public class BattleHandler : MonoBehaviour
{
    public static BattleHandler Instance { get { return instance; } }
    private static BattleHandler instance;

    // Player turn
    public enum EBattleState
    {
        None,
        PlayerTurn,
        EnemyTurn,
        Attacking
    }

    #region Private Fields
    private EBattleState battleState;
    private StateMachine currentBattleState;
    private BattleUnit currentAttackingUnit;
    private List<BattleUnit> turnOrder = new List<BattleUnit>();

    private bool attackFlagDebug;
    private bool flag2;
    #endregion

    #region Accessors
    public EBattleState BattleState { get { return battleState; } }
    public List<PartyMemberUnit> PartyInBattle { get { return partyUnits; } }
    public List<EnemyUnit> EnemiesInBattle { get { return enemyUnits; } }
    #endregion

    #region Events
    public delegate void BattleStart();
    public BattleStart OnBattleStart;

    public delegate void BattleStateUpdated(EBattleState battleState);
    public BattleStateUpdated OnBattleStateUpdated;

    public delegate void TurnOrderUpdated(List<BattleUnit> newTurnOrder);
    public TurnOrderUpdated OnTurnOrderUpdated;
    #endregion

    [Header("DEBUG")]
    [SerializeField] private List<PartyMemberUnit> partyUnits = new List<PartyMemberUnit>();
    [SerializeField] private List<EnemyUnit> enemyUnits = new List<EnemyUnit>();

    [Header("Modifiers")]
    [SerializeField] private float defaultXValue = -7.5f;
    [SerializeField] private float forwardXValue = -6.5f;

    [Header("References")]
    [SerializeField] private PerformanceHitmarker _performanceHitmarkerPrefab;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeStateMachine()
    {
        B_PlayerTurnState playerTurnState = new B_PlayerTurnState(currentBattleState, this);
        B_EnemyTurnState enemyTurnState = new B_EnemyTurnState(currentBattleState, this);
        B_Attacking attackState = new B_Attacking(currentBattleState, this);

        // If the battle state has not yet been initialized
        if (currentBattleState == null)
        {
            currentBattleState = new StateMachine();

            // Add transitions
            currentBattleState.AddTransition(playerTurnState, attackState, new FuncPredicate(() => attackFlagDebug));
            currentBattleState.AddTransition(attackState, enemyTurnState, new FuncPredicate(() => flag2));
        }

        // Set state immediately
        currentBattleState.SetState(playerTurnState);
    }

    public void StartBattle()
    {
        // Initalize turn order
        turnOrder = new List<BattleUnit>();

        foreach (PartyMemberUnit u in partyUnits) turnOrder.Add(u);
        foreach (EnemyUnit e in enemyUnits) turnOrder.Add(e);

        OnTurnOrderUpdated?.Invoke(turnOrder);
        OnBattleStart?.Invoke();
        InitializeStateMachine();

        // Update the order at which the party members are arranged
        UpdateOrder();
    }

    private void Update()
    {
        // Start battle
        if (Input.GetKeyDown(KeyCode.E) && battleState == EBattleState.None) StartBattle();

        // Update battle state
        if (battleState != EBattleState.None)
        {
            currentBattleState.Update();
        }
    }

    #region [STATE] Player Turn
    public void EnterPlayerTurn()
    {
        // Update the turn state
        UpdateBattleState(EBattleState.PlayerTurn);
    }

    public void UpdatePlayerTurn()
    {
        // Debug key for spawning an attack
        if (Input.GetKeyDown(KeyCode.D))
        {
            currentAttackingUnit = partyUnits[0];
            SpawnAttack(0);
            attackFlagDebug = true;
        }

        // switch order
        //if (Input.GetKeyDown(KeyCode.LeftArrow)) SwitchOrderLeft();
        //if (Input.GetKeyDown(KeyCode.RightArrow)) SwitchOrderRight();
    }
    #endregion

    #region [STATE] Enemy Turn
    public void EnterEnemyTurn()
    {
        // Update the turn state
        UpdateBattleState(EBattleState.EnemyTurn);
    }

    public void UpdateEnemyTurn()
    {
        partyUnits[0].UpdateDefendState();

        currentAttackingUnit.UpdateAttackState();
    }
    #endregion

    #region [STATE] Attacking
    public void EnterAttackingState()
    {
        // Update the turn state
        UpdateBattleState(EBattleState.Attacking);
    }

    public void UpdateAttackingState()
    {
        currentAttackingUnit.UpdateAttackState();
    }

    private void SpawnAttack(int index)
    {
        partyUnits[0].HandleAttack(index, enemyUnits[0]);
    }
    #endregion

    public void RegisterEnemyHit(PartyMemberUnit attacker, EnemyUnit target, EAttackPerformance performance)
    {
        target.MyHealth.TakeDamage(attacker.MyStats.BaseAttack);

        PerformanceHitmarker hitmarker = Instantiate(_performanceHitmarkerPrefab, target.transform.position, Quaternion.identity);
        hitmarker.SetText(performance.ToString());
    }

    #region Helpers
    private void SwitchOrderLeft()
    {
        PartyMemberUnit[] temp = new PartyMemberUnit[partyUnits.Count];

        for (int i = partyUnits.Count - 1; i >= 0; i--)
        {
            int newPosition = (i + (partyUnits.Count - 1)) % partyUnits.Count;

            temp[newPosition] = partyUnits[i];
        }

        partyUnits = temp.ToList();
        UpdateOrder();
    }

    private void SwitchOrderRight()
    {
        PartyMemberUnit[] temp = new PartyMemberUnit[partyUnits.Count];

        for (int i = 0; i < partyUnits.Count; i++)
        {
            int newPosition = (i + 1) % partyUnits.Count;

            temp[newPosition] = partyUnits[i];
        }

        partyUnits = temp.ToList();
        UpdateOrder();
    }

    public void UpdateOrder()
    {
        for(int i = 0; i < partyUnits.Count; i++)
        {
            partyUnits[i].transform.DOKill(false);

            if (i == 0)
            {
                partyUnits[i].transform.DOMove(new Vector3(forwardXValue, 0.0f), 0.3f).SetEase(Ease.OutQuad);
                continue;
            }

            partyUnits[i].transform.DOMove(new Vector3(defaultXValue, (i - (partyUnits.Count / 2.0f)) * 4), 0.3f).SetEase(Ease.OutQuad);
        }
    }

    public void UpdateBattleState(EBattleState state)
    {
        battleState = state;

        OnBattleStateUpdated?.Invoke(state);
    }
    #endregion
}

public abstract class B_BattleState : BaseState
{
    protected BattleHandler handler;

    public B_BattleState(StateMachine stateMachine, BattleHandler handler) : base(stateMachine) { this.handler = handler; }
}

public class B_PlayerTurnState : B_BattleState
{
    public B_PlayerTurnState(StateMachine stateMachine, BattleHandler handler) : base(stateMachine, handler) { }

    public override void OnEnter()
    {
        Debug.Log("Entered player turn state");

        handler.EnterPlayerTurn();
    }

    public override void Update()
    {
        handler.UpdatePlayerTurn();
    }
}

public class B_EnemyTurnState : B_BattleState
{
    public B_EnemyTurnState(StateMachine stateMachine, BattleHandler handler) : base(stateMachine, handler) { }

    public override void OnEnter()
    {
        Debug.Log("Entered enemy turn state");

        handler.EnterEnemyTurn();
    }

    public override void Update()
    {
        handler.UpdateEnemyTurn();
    }
}

public class B_Attacking : B_BattleState
{
    public B_Attacking(StateMachine stateMachine, BattleHandler handler) : base(stateMachine, handler) { }

    public override void OnEnter()
    {
        Debug.Log("Entered attack turn state");

        handler.EnterAttackingState();
    }

    public override void Update()
    {
        handler.UpdateAttackingState();
    }
}