using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using DG.Tweening;
using Hank.Systems.StateMachine;
using Random = UnityEngine.Random;

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

    private bool playerAttackFlag;
    private bool playerAttackEndedFlag;
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

    [Header("Balancing")]
    [SerializeField] private float maxPerformanceMultiplier = 2.0f;

    [Header("References")]
    [SerializeField] private PerformanceHitmarker _performanceHitmarkerPrefab;
    [SerializeField] private PerformanceHitmarker _damageHitmarkerPrefab;

    private void Awake()
    {
        // Initialize static instance

        if(instance == null) instance = this;
        else Destroy(gameObject);
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
            currentBattleState.AddTransition(playerTurnState, attackState, new FuncPredicate(() => playerAttackFlag));
            //currentBattleState.AddTransition(attackState, enemyTurnState, new FuncPredicate(() => playerAttackEndedFlag));
        }

        // Set state immediately
        currentBattleState.SetStateImmediate(playerTurnState);
    }

    public void StartBattle()
    {
        // Initalize turn order
        turnOrder = new List<BattleUnit>();

        foreach (PartyMemberUnit u in partyUnits) turnOrder.Add(u);
        foreach (EnemyUnit e in enemyUnits) turnOrder.Add(e);

        // This order is really important
        OnTurnOrderUpdated?.Invoke(turnOrder);
        OnBattleStart?.Invoke();
        InitializeStateMachine();

        // Update the order at which the party members are arranged
        UpdatePartyArrangement();
    }

    private void Update()
    {
        // Start battle
        if (Input.GetKeyDown(KeyCode.E) && battleState == EBattleState.None) StartBattle();

        // Restart
        if (Input.GetKeyDown(KeyCode.R))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
        }

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
            TurnAttack();
        }

        // switch order
        //if (Input.GetKeyDown(KeyCode.LeftArrow)) SwitchOrderLeft();
        //if (Input.GetKeyDown(KeyCode.RightArrow)) SwitchOrderRight();
    }

    private void TurnAttack()
    {
        currentAttackingUnit = turnOrder[0];

        if (turnOrder[0] is PartyMemberUnit)
        {
            SpawnAttack(turnOrder[0], enemyUnits[0], 0);
            playerAttackFlag = true;
        }
        else if (turnOrder[0] is EnemyUnit)
        {
            SpawnAttack(turnOrder[0], partyUnits[Random.Range(0, partyUnits.Count)], 0);
        }
    }

    public void AdvanceNextTurn()
    {
        // take out turn and add to end
        BattleUnit unit = turnOrder[0];
        turnOrder.RemoveAt(0);
        turnOrder.Add(unit);
        OnTurnOrderUpdated?.Invoke(turnOrder);

        TurnAttack();
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

    private void SpawnAttack(BattleUnit unit, BattleUnit target, int index)
    {
        unit.StartAttack(index, target);

        unit.OnAttackFinished += AttackCompleted;
    }

    private void AttackCompleted()
    {
        AdvanceNextTurn();
    }
    #endregion

    #region Damage / Hits
    private int CalculateDamage(int rawAttackDamage, int attackStat, int defenseStat, float performanceMultiplier)
    {
        int rawDamage = rawAttackDamage + attackStat;
        int adjDamage = rawDamage - defenseStat;
        int damage = adjDamage + Mathf.RoundToInt(rawDamage * performanceMultiplier);

        if (damage < 0) damage = 0;

        return damage;
    }

    private float GetPerformanceMultiplier(EAttackPerformance performance)
    {
        return Mathf.Lerp(1, maxPerformanceMultiplier, (float)performance / (float)Enum.GetNames(typeof(EAttackPerformance)).Length);
    }

    public void RegisterPartyMemberHit(EnemyUnit attacker, PartyMemberUnit target)
    {
        int damage = attacker.MyStats.BaseAttack;
        target.MyHealth.TakeDamage(damage);

        CreatePerformanceMarker(target.transform.position, damage.ToString());
    }

    public void RegisterEnemyHit(PartyMemberUnit attacker, EnemyUnit target, EAttackPerformance performance)
    {
        // Calculate damage and take damage
        int damage = CalculateDamage(0, attacker.MyStats.BaseAttack, target.MyStats.BaseDefense, GetPerformanceMultiplier(performance));
        target.MyHealth.TakeDamage(damage);

        CreatePerformanceMarker(target.transform.position, performance.ToString());
    }

    public void RegisterMiss(EnemyUnit target)
    {
        CreatePerformanceMarker(target.transform.position, "MISS");
    }
    #endregion

    #region Helpers
    private void CreatePerformanceMarker(Vector2 pos, string txt)
    {
        PerformanceHitmarker hitmarker = Instantiate(_performanceHitmarkerPrefab, pos, Quaternion.identity);
        hitmarker.SetText(txt);
    }

    private void RotateArrangementLeft()
    {
        PartyMemberUnit[] temp = new PartyMemberUnit[partyUnits.Count];

        for (int i = partyUnits.Count - 1; i >= 0; i--)
        {
            int newPosition = (i + (partyUnits.Count - 1)) % partyUnits.Count;

            temp[newPosition] = partyUnits[i];
        }

        partyUnits = temp.ToList();
        UpdatePartyArrangement();
    }

    private void RotateArrangementRight()
    {
        PartyMemberUnit[] temp = new PartyMemberUnit[partyUnits.Count];

        for (int i = 0; i < partyUnits.Count; i++)
        {
            int newPosition = (i + 1) % partyUnits.Count;

            temp[newPosition] = partyUnits[i];
        }

        partyUnits = temp.ToList();
        UpdatePartyArrangement();
    }

    public void UpdatePartyArrangement()
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