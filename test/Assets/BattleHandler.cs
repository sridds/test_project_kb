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
    #endregion

    #region Accessors
    public EBattleState BattleState { get { return battleState; } }
    public List<PartyMemberUnit> PartyInBattle { get { return partyUnits; } }
    public List<EnemyUnit> EnemiesInBattle { get { return enemyUnits; } }
    #endregion

    [Header("DEBUG")]
    [SerializeField] private List<PartyMemberUnit> partyUnits = new List<PartyMemberUnit>();
    [SerializeField] private List<EnemyUnit> enemyUnits = new List<EnemyUnit>();

    [Header("Modifiers")]
    [SerializeField] private float defaultXValue = -7.5f;
    [SerializeField] private float forwardXValue = -6.5f;

    private List<BattleUnit> turnOrder = new List<BattleUnit>();

    #region Events
    public delegate void BattleStart();
    public BattleStart OnBattleStart;

    public delegate void BattleStateUpdated(EBattleState battleState);
    public BattleStateUpdated OnBattleStateUpdated;

    public delegate void TurnOrderUpdated(List<BattleUnit> newTurnOrder);
    public TurnOrderUpdated OnTurnOrderUpdated;
    #endregion

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

    private bool attackFlagDebug;
    private bool flag2;

    private void Start()
    {
        currentBattleState = new StateMachine();

        B_PlayerTurnState playerTurnState = new B_PlayerTurnState(currentBattleState, this);
        B_EnemyTurnState enemyTurnState = new B_EnemyTurnState(currentBattleState, this);
        B_Attacking attackState = new B_Attacking(currentBattleState, this);

        // Add transitions
        currentBattleState.AddTransition(playerTurnState, attackState, new FuncPredicate(() => attackFlagDebug));
        currentBattleState.AddTransition(attackState, enemyTurnState, new FuncPredicate(() => flag2));

        // Set state immediately
        currentBattleState.SetState(playerTurnState);
    }

    public void StartBattle()
    {
        // Update the turn state
        UpdateBattleState(EBattleState.PlayerTurn);

        // Initalize turn order
        turnOrder = new List<BattleUnit>();
        foreach (PartyMemberUnit u in partyUnits) turnOrder.Add(u);
        foreach (EnemyUnit e in enemyUnits) turnOrder.Add(e);
        OnTurnOrderUpdated?.Invoke(turnOrder);
        OnBattleStart?.Invoke();

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

    private void SpawnAttack(int index)
    {
        partyUnits[0].HandleAttack(index, enemyUnits[0]);
    }

    public void UpdatePlayerTurn()
    {
        // Debug key for spawning an attack
        if (Input.GetKeyDown(KeyCode.D))
        {
            SpawnAttack(0);
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow)) SwitchOrderLeft();
        if (Input.GetKeyDown(KeyCode.RightArrow)) SwitchOrderRight();
    }

    public void UpdateEnemyTurn()
    {

    }

    public void UpdateAttackingState()
    {

    }

    #region Helpers
    public void UpdateBattleState(EBattleState state)
    {
        battleState = state;

        OnBattleStateUpdated?.Invoke(state);
    }

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
    }

    public override void Update()
    {
        handler.UpdateAttackingState();
    }
}