using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using Random = UnityEngine.Random;

namespace Hank.Battles
{
    public class BattleHandlerV2 : MonoBehaviour
    {
        public static BattleHandlerV2 Instance { get { return instance; } }
        private static BattleHandlerV2 instance;

        public enum EBattleState
        {
            Inactive,           // No battle currently active
            WaitingForPlayer,   // Player deciding turn
            ExecutingPartyTurn, // Executing party members turn
            ExecutingEnemyTurn, // Executing enemies turn
            BattleComplete      // Player successfully completed the battle
        }

        [Header("DEBUG")]
        [SerializeField] private PartyMemberUnit[] partyUnits;
        [SerializeField] private EnemyUnit[] enemyUnits;

        [Header("Modifiers")]
        [SerializeField] private float _defaultXValue = -7.5f;
        [SerializeField] private float _forwardXValue = -6.5f;

        [Header("References")]
        [SerializeField] private BattleUIHandler battleUI;

        #region Private Fields
        private Dictionary<EBattleState, BattleState> battleStateValuePairs = new Dictionary<EBattleState, BattleState>();
        private EBattleState currentBattleState;
        private Battle currentBattle;
        private PartyMemberUnit partyMemberUp;
        #endregion

        #region Accessors
        public EBattleState CurrentBattleState { get { return currentBattleState; } }
        public Battle CurrentBattle { get { return currentBattle; } }
        #endregion

        #region Delegates
        public delegate void BattleStateUpdated(EBattleState previousState, EBattleState newState);
        public BattleStateUpdated OnBattleStateUpdated;

        public delegate void BattleStart(Battle battle);
        public BattleStart OnBattleStart;
        #endregion

        #region MonoBehaviour
        private void Awake()
        {
            instance = this;

            // Initialize state machine
            battleStateValuePairs.Add(EBattleState.Inactive, null);
            battleStateValuePairs.Add(EBattleState.WaitingForPlayer, new B_WaitingForPlayer(this));
            battleStateValuePairs.Add(EBattleState.ExecutingPartyTurn, new B_ExecutingPartyTurn(this));
            battleStateValuePairs.Add(EBattleState.ExecutingEnemyTurn, new B_ExecutingEnemyTurn(this));
        }

        private void Update()
        {
            // Start battle
            if (Input.GetKeyDown(KeyCode.E) && currentBattleState == EBattleState.Inactive) StartBattle(partyUnits, enemyUnits);
            // Restart
            if (Input.GetKeyDown(KeyCode.R)) UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);

            if (currentBattleState == EBattleState.Inactive) return;

            battleStateValuePairs[currentBattleState]?.Update();
        }
        #endregion

        #region Helper Functions
        public void StartBattle(PartyMemberUnit[] party, EnemyUnit[] enemies)
        {
            Debug.Log($"Battle started");

            // 1 - Initalize values
            // 2 - Enter state
            currentBattle = new Battle(party, enemies);

            // We pass the current battle into the event so the UI can register the correct UI elements
            OnBattleStart?.Invoke(currentBattle);

            // Set state immediately
            currentBattleState = EBattleState.WaitingForPlayer;
            battleStateValuePairs[currentBattleState]?.EnterState();

            // Setup UI
            battleUI.SetupBattleUI(currentBattle);
        }

        private void SetState(EBattleState state)
        {
            Debug.Log($"Setting current EBattleState [{currentBattleState}] to {state}");

            // Don't bother if its the same state
            if (currentBattleState == state) return;

            EBattleState previousState = currentBattleState;

            // Exit current state and enter new one
            battleStateValuePairs[previousState]?.ExitState();
            battleStateValuePairs[state]?.EnterState();

            // Set an invoke events
            currentBattleState = state;
            OnBattleStateUpdated?.Invoke(previousState, state);
        }

        public bool IsPartyWon()
        {
            // Return true if all enemies are dead and the party is alive
            return !IsPartyDead() && currentBattle.EnemiesInBattle.All(unit => unit.MyHealth.IsDead);
        }

        public bool IsPartyDead()
        {
            // Return true if all party members are alive
            return currentBattle.PartyInBattle.All(unit => unit.MyHealth.IsDead);
        }
        #endregion

        #region Waiting State
        public void EnterWaitingState()
        {
            Debug.Log($"Waiting for player...");

            // 1 - Call events (UI menus open)
            battleUI.OpenPartyMenus(partyMemberUp);
        }

        public void UpdateWaitingState()
        {
            // 1 - Update UI
            // 2 - Await response from UI
            // 4 - When ready, go to next state
        }

        public void ExitWaitingState()
        {
            // 1 - Close menus
            battleUI.ClosePartyMenus();
        }
        #endregion

        #region Executing Player Turn State
        public void EnterPlayerTurnState()
        {
            // 1. Start Coroutine
        }

        private IEnumerator IExecutePlayerTurn()
        {
            // 1. Wait for the current unit to handle the action (or replace this with a turn class)
            //    yield return new currentUnit.HandleAction();

            // 2. Display damage total (if player), wait for a second

            // 3.

            yield return null;
        }

        public void UpdatePlayerTurnState()
        {
            // 1 - The party has the chance of dying here, so check for death
            if(IsPartyDead())
            {

            }
        }

        public void ExitPlayerTurnState()
        {

        }
        #endregion

        #region Executing Enemy Turn State
        public void EnterEnemyTurnState()
        {

        }

        private IEnumerator IExecuteEnemyTurn()
        {
            BattleUnit currentUnit = null;

            // 1. Wait for the current unit to handle the action (or replace this with a turn class)
            //    yield return new currentUnit.HandleAction();

            // 2. Display damage total (if player), wait for a second

            // 3. 

            yield return null;
        }

        public void UpdateEnemyTurnState()
        {
            // 1 - The party has the chance of dying here, so check for death
            if (IsPartyDead())
            {

            }

            // When the enemy dies, play a death animation, then continue
        }

        public void ExitEnemyTurnState()
        {

        }

        #endregion
    }

    public class Battle
    {
        public List<PartyMemberUnit> PartyInBattle { get { return party; } }
        public List<EnemyUnit> EnemiesInBattle { get { return enemies; } }

        private List<PartyMemberUnit> party; // These are stored as a list in case any are added
        private List<EnemyUnit> enemies; // These are stored as a list in case any are added during battle

        public delegate void EnemyUnitAdded(EnemyUnit unit);
        public EnemyUnitAdded OnEnemyUnitAdded;

        public Battle(PartyMemberUnit[] party, EnemyUnit[] enemies)
        {
            this.party = party.ToList();
            this.enemies = enemies.ToList();
        }

        // Helper function that assists with adding enemies to the battle
        public void AddEnemyUnit(EnemyUnit unit)
        {
            enemies.Add(unit);

            OnEnemyUnitAdded?.Invoke(unit);
        }
    }

    #region States
    public class BattleState
    {
        protected BattleHandlerV2 battleHandler;

        public BattleState(BattleHandlerV2 handler) => battleHandler = handler;

        public virtual void EnterState() { }
        public virtual void ExitState() { }
        public virtual void Update() { }
    }

    public class B_WaitingForPlayer : BattleState
    {
        public B_WaitingForPlayer(BattleHandlerV2 handler) : base(handler) { }

        public override void EnterState() => battleHandler.EnterWaitingState();
        public override void Update() => battleHandler.UpdateWaitingState();
        public override void ExitState() => battleHandler.ExitWaitingState();
    }

    public class B_ExecutingPartyTurn : BattleState
    {
        public B_ExecutingPartyTurn(BattleHandlerV2 handler) : base(handler) { }

        public override void EnterState() => battleHandler.EnterPlayerTurnState();
        public override void Update() => battleHandler.UpdatePlayerTurnState();
        public override void ExitState() => battleHandler.ExitPlayerTurnState();
    }

    public class B_ExecutingEnemyTurn : BattleState
    {
        public B_ExecutingEnemyTurn(BattleHandlerV2 handler) : base(handler) { }

        public override void EnterState() => battleHandler.EnterEnemyTurnState();
        public override void Update() => battleHandler.UpdateEnemyTurnState();
        public override void ExitState() => battleHandler.ExitEnemyTurnState();
    }
    #endregion
}

/*
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

    // Order #1
    // - Whoever's turn it is, select option
    // - Repeat until all party members have selected
    // - Select frontmost party member
    // - When it's the enemies turn, execute all party member actions

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
            currentBattleState.AddTransition(attackState, playerTurnState, new FuncPredicate(() => !playerAttackFlag && IsPartyMembersTurn()));
            currentBattleState.AddTransition(attackState, enemyTurnState, new FuncPredicate(() => !IsPartyMembersTurn()));
            currentBattleState.AddTransition(enemyTurnState, playerTurnState, new FuncPredicate(() => IsPartyMembersTurn()));
        }

        // Set state immediately
        currentBattleState.SetStateImmediate(playerTurnState);
    }

    private bool IsPartyMembersTurn() => turnOrder[0] is PartyMemberUnit;

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

    public void SetTargetOverlayEnabled(int index)
    {
        for(int i = 0; i < enemyUnits.Count; i++)
        {
            if(i == index)
            {
                enemyUnits[i].SetTarget(true);
                continue;
            }

            enemyUnits[i].SetTarget(false);
        }
    }

    public void DisableAllTargetOverlays()
    {
        foreach(EnemyUnit enemy in enemyUnits)
        {
            enemy.SetTarget(false);
        }
    }

    public void UpdatePlayerTurn()
    {
        // switch order
        //if (Input.GetKeyDown(KeyCode.LeftArrow)) SwitchOrderLeft();
        //if (Input.GetKeyDown(KeyCode.RightArrow)) SwitchOrderRight();
    }

    public void HandleBash(EnemyUnit target)
    {
        currentAttackingUnit = turnOrder[0];
        SpawnAttack(turnOrder[0], target, 0);
        playerAttackFlag = true;
    }

    public void AdvanceNextTurn()
    {
        // take out turn and add to end
        BattleUnit unit = turnOrder[0];
        turnOrder.RemoveAt(0);
        turnOrder.Add(unit);
        playerAttackFlag = false;
        OnTurnOrderUpdated?.Invoke(turnOrder);

        if(turnOrder[0] is EnemyUnit)
        {
            SpawnEnemyAttack();
        }
    }

    private void SpawnEnemyAttack()
    {
        currentAttackingUnit = turnOrder[0];
        SpawnAttack(turnOrder[0], partyUnits[Random.Range(0, partyUnits.Count)], 0);

    }
    #endregion

    #region [STATE] Enemy Turn
    public void EnterEnemyTurn()
    {
        //SpawnEnemyAttack();

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
        currentAttackingUnit?.UpdateAttackState();
    }

    private void SpawnAttack(BattleUnit unit, BattleUnit target, int index)
    {
        unit.StartAttack(index, target);

        unit.OnAttackFinished += AttackCompleted;
    }

    private void AttackCompleted()
    {
        currentAttackingUnit.OnAttackFinished -= AttackCompleted;
        currentAttackingUnit = null;

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
}*/