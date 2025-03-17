using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using Random = UnityEngine.Random;
using DG.Tweening;

/* 

- New Battle System - 

1. Intro / Flavor text
2. Choose party commands
    [Bash] Standard timed attack
    [Skills] Combo moves that cost SP
        - Skills will either target 1 enemy, or target all of them. Selecting two targets is annoying
        - Skills that involve another party member will have their turn skipped
        - Skills cost skill points. Could go the deltarune approach with cumilative SP 
    [Bag] Grab an item from the shared inventory
3. Pick party leader (rotate) -- this ALSO alters the arrangement in which orders are executed. Leader first, then top, then bottom
    - The point of ordering is to reserve two buttons for dodging for more variation. Solves Mario & Luigi - Paper Jam
4. Enemy dialogue
5. Enemies attack
6. Repeat

- Remaining Questions -

1. What happens when a party member LEADER is downed during an attack?
2. Is switching an enjoyable spin on combat? (takes prototyping)
3. Should party members have differing options in their skill menus? (like how only Kris can ACT in DR)

- Remember the following -

1. Three party members tops. Design for three ONLY
2. You can figure out enemy formation later 
3. You can figure out special battle events later
4. Just get the main flow working and have a couple fun attacks designed

 */

namespace Hank.Battles
{
    public enum EBattleAction
    {
        Bash,
        Skill,
        Bag
    }

    public class BattleHandlerV2 : MonoBehaviour
    {
        #region Fields
        private const float MAX_PERFORMANCE_DAMAGE_MULTIPLIER = 1.5f;

        public static BattleHandlerV2 Instance { get { return instance; } }
        private static BattleHandlerV2 instance;

        public enum EBattleState
        {
            Inactive,           // No battle currently active
            Intro,              // Play dope ass intro animation
            WaitingForPlayer,   // Player deciding turn
            ChoosingLeader,     // Player choosing the leader to tank damage
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
        [SerializeField] private float _rotationSpeed = 0.15f;

        [Header("References")]
        [SerializeField] private BattleTurnBuilder _battleTurnBuilder;

        #region Private Fields
        private Dictionary<EBattleState, BattleState> battleStateValuePairs = new Dictionary<EBattleState, BattleState>();
        private Dictionary<PartyMemberUnit, TurnMenuData> turnDataValuePairs = new Dictionary<PartyMemberUnit, TurnMenuData>();
        private List<PartyMemberUnit> rotationOrder = new List<PartyMemberUnit>();
        private EBattleState currentBattleState;
        private Battle currentBattle;
        private int partyMemberTurnIndex;
        private bool hasPlayedIntro;
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
        #endregion

        #region MonoBehaviour
        private void Awake()
        {
            instance = this;

            // Initialize state machine
            battleStateValuePairs.Add(EBattleState.Inactive, null);
            battleStateValuePairs.Add(EBattleState.Intro, new B_Intro(this));
            battleStateValuePairs.Add(EBattleState.WaitingForPlayer, new B_WaitingForPlayer(this));
            battleStateValuePairs.Add(EBattleState.ChoosingLeader, new B_ChoosingLeader(this));
            battleStateValuePairs.Add(EBattleState.ExecutingPartyTurn, new B_ExecutingPartyTurn(this));
            battleStateValuePairs.Add(EBattleState.ExecutingEnemyTurn, new B_ExecutingEnemyTurn(this));
        }

        private void Update()
        {
            // Start battle
            if (Input.GetKeyDown(KeyCode.E) && currentBattleState == EBattleState.Inactive) StartBattle(partyUnits, enemyUnits);
            if (Input.GetKeyDown(KeyCode.R)) UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);

            if (currentBattleState == EBattleState.Inactive) return;

            battleStateValuePairs[currentBattleState]?.Update();
        }
        #endregion

        #region Battle Functions
        public void StartBattle(PartyMemberUnit[] party, EnemyUnit[] enemies)
        {
            Debug.Log($"Battle started");

            // 1 - Initalize values
            // 2 - Enter state
            currentBattle = new Battle(party, enemies);
            rotationOrder = party.ToList();

            // We pass the current battle into the event so the UI can register the correct UI elements
            OnBattleStart?.Invoke(currentBattle);

            // Set state immediately
            partyMemberTurnIndex = 0;
            currentBattleState = EBattleState.Intro;
            battleStateValuePairs[currentBattleState]?.EnterState();

            // Setup UI
            _battleTurnBuilder.SetupBattleUI(currentBattle);
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

        private void CleanupBattle()
        {
            hasPlayedIntro = false;
        }
        #endregion

        #region States
        #region [STATE] Intro
        public void EnterIntroState()
        {
            // If we haven't seen the intro, show it
            if(!hasPlayedIntro) StartCoroutine(IPlayIntro());

            // Otherwise, we're coming back to the intro state, so immediately show the flavor text (dont make the player sit through the typewriting)
            else _battleTurnBuilder.OpenFlavorText(EDialogueAppearance.Immediate);
        }

        public void UpdateIntroState()
        {
            if (!hasPlayedIntro) return;

            if(Input.GetKeyDown(KeyCode.Z))
            {
                // Immediately enter next state, doesn't matter if player continues text or not
                SetState(EBattleState.WaitingForPlayer);
                _battleTurnBuilder.HideFlavorText();
            }
        }

        public void ExitIntroState()
        {

        }

        private IEnumerator IPlayIntro()
        {
            Debug.Log($"Pretend there's a cool fucking intro here...");

            yield return new WaitForSeconds(1.0f);

            // Now we can start the dialogue
            hasPlayedIntro = true;

            UpdatePartyArrangement();
            _battleTurnBuilder.OpenFlavorText(EDialogueAppearance.Typewriter);
        }
        #endregion

        #region [STATE] Waiting State
        public void EnterWaitingState()
        {
            Debug.Log($"Waiting for player...");

            // Reset values
            partyMemberTurnIndex = 0;
            _battleTurnBuilder.ResetTurnMenuData();

            // Open UI
            _battleTurnBuilder.SetPartyMember(rotationOrder[partyMemberTurnIndex]);
            _battleTurnBuilder.OpenPartyMenus();
        }

        public void UpdateWaitingState()
        {
            _battleTurnBuilder.UpdateActionMenu();

            // Waiting State
            // 1. Select options
            // 2. UI takes care of submenus and builds a turn
            // 3. Turns, along with the Action they fire, are stored in TurnData which is sent to the BattleHandler, arranged in the same order as the rotation order
            // 4. Continue until all party members have selected

            // If there are no more UI submenus to backup, resort to going back a turn
            if(Input.GetKeyDown(KeyCode.X) && !_battleTurnBuilder.Backup())
            {
                UpdateTurnIndex(partyMemberTurnIndex - 1);
                Debug.Log($"Going back [index: {partyMemberTurnIndex}]");
            }

            // If there are no more options to select, continue through the turn index
            if(Input.GetKeyDown(KeyCode.Z) && !_battleTurnBuilder.SelectOption())
            {
                UpdateTurnIndex(partyMemberTurnIndex + 1);
                Debug.Log($"Going forward [index: {partyMemberTurnIndex}]");
            }
        }

        private void UpdateTurnIndex(int index)
        {
            partyMemberTurnIndex = index;

            // If we've gone back far enough, show the intro flavor text again
            if (partyMemberTurnIndex < 0) SetState(EBattleState.Intro);

            // If we've gone over all party members, execute their actions
            else if (partyMemberTurnIndex > rotationOrder.Count - 1) SetState(EBattleState.ChoosingLeader);

            // Otherwise, set the party member
            else _battleTurnBuilder.SetPartyMember(rotationOrder[partyMemberTurnIndex]);
        }

        public void ExitWaitingState()
        {
            // 1 - Close menus
            _battleTurnBuilder.ClosePartyMenus();
        }
        #endregion

        #region [STATE] Choosing Leader State
        public void EnterChoosingLeaderState()
        {
            // Open menus
            _battleTurnBuilder.OpenChoosingLeaderUI();
            UpdatePartyArrangement();
        }

        public void UpdateChoosingLeaderState()
        {
            // Rotate clockwise
            if(Input.GetKeyDown(KeyCode.LeftArrow)) RotateArrangementLeft();

            // Rotate counter-clockwise
            if(Input.GetKeyDown(KeyCode.RightArrow)) RotateArrangementRight();

            // Select leader
            if (Input.GetKeyDown(KeyCode.Z)) SetState(EBattleState.ExecutingPartyTurn);
        }

        public void ExitChoosingLeaderState()
        {
            // Cache the current leader of the rotation order as the leader
            currentBattle.SelectLeader(rotationOrder[0]);

            // Close UI menus
            _battleTurnBuilder.CloseChoosingLeaderUI();
        }

        #endregion

        #region [STATE] Executing Party Turn State
        public void EnterPartyTurnState()
        {
            // 1. Start Coroutine
            StartCoroutine(IExecutePartyTurn());
        }

        private IEnumerator IExecutePartyTurn()
        {
            // Handle each party member in the rotation order
            for(int i = 0; i < rotationOrder.Count; i++)
            {
                TurnMenuData currentData = _battleTurnBuilder.PartyTurnDataPairs[rotationOrder[i]];

                switch(currentData.BattleAction)
                {
                    case EBattleAction.Bash:
                        Debug.Log($"{rotationOrder[i].MyStats.Name} used BASH!");
                        break;
                    case EBattleAction.Skill:
                        Debug.Log($"{rotationOrder[i].MyStats.Name} used SKILL!");
                        break;
                    case EBattleAction.Bag:
                        Debug.Log($"{rotationOrder[i].MyStats.Name} used BAG!");
                        break;
                }

                yield return new WaitForSeconds(1);
            }

            SetState(EBattleState.ExecutingEnemyTurn);
        }

        public void UpdatePartyTurnState()
        {
            
        }

        public void ExitPartyTurnState()
        {

        }
        #endregion

        #region [STATE] Executing Enemy Turn State
        public void EnterEnemyTurnState()
        {
            Debug.Log($"Enemy turn!");

            StartCoroutine(IExecuteEnemyTurn());
        }

        private IEnumerator IExecuteEnemyTurn()
        {
            BattleUnit currentUnit = null;

            for (int i = 0; i < currentBattle.EnemiesInBattle.Count; i++)
            {
                Debug.Log($"{currentBattle.EnemiesInBattle[i].MyStats.Name} is attacking {currentBattle.Leader}!");

                yield return new WaitForSeconds(1);
            }

            SetState(EBattleState.WaitingForPlayer);
            yield return null;
        }

        public void UpdateEnemyTurnState()
        {
            // 1 - The party has the chance of dying here, so check for death

            // Leader - Dodge
            if(Input.GetKeyDown(KeyCode.Z))
            {
                //currentPartyMember.AlternateDodge();
            }

            // Leader - Alternate dodge
            if (Input.GetKeyDown(KeyCode.X))
            {
                //currentPartyMember.AlternateDodge();
            }

            // When the enemy dies, play a death animation, then continue
        }

        public void ExitEnemyTurnState()
        {

        }

        #endregion
        #endregion

        #region Internal Helper Functions
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

        private void RotateArrangementLeft()
        {
            int count = rotationOrder.Count();
            PartyMemberUnit[] temp = new PartyMemberUnit[count];

            for (int i = count - 1; i >= 0; i--)
            {
                int newPosition = (i + (count - 1)) % count;

                temp[newPosition] = rotationOrder[i];
            }

            rotationOrder = temp.ToList();
            UpdatePartyArrangement();
        }

        private void RotateArrangementRight()
        {
            int count = rotationOrder.Count();
            PartyMemberUnit[] temp = new PartyMemberUnit[count];

            for (int i = 0; i < count; i++)
            {
                int newPosition = (i + 1) % count;

                temp[newPosition] = rotationOrder[i];
            }

            rotationOrder = temp.ToList();
            UpdatePartyArrangement();
        }

        public void UpdatePartyArrangement()
        {
            int count = rotationOrder.Count();

            for (int i = 0; i < count; i++)
            {
                rotationOrder[i].transform.DOKill(false);

                if (i == 0)
                {
                    rotationOrder[i].transform.DOMove(new Vector3(_forwardXValue, 0.0f), _rotationSpeed).SetEase(Ease.OutQuad);
                    continue;
                }

                rotationOrder[i].transform.DOMove(new Vector3(_defaultXValue, (i - (count / 2.0f)) * 4), _rotationSpeed).SetEase(Ease.OutQuad);
            }
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
            return Mathf.Lerp(1, MAX_PERFORMANCE_DAMAGE_MULTIPLIER, (float)performance / (float)Enum.GetNames(typeof(EAttackPerformance)).Length);
        }
        #endregion
    }

    // This class will handle all data during the battle. This class will also be read from by enemies and the party
    public class Battle
    {
        // Accessors
        public List<PartyMemberUnit> PartyInBattle { get { return party; } }
        public List<EnemyUnit> EnemiesInBattle { get { return enemies; } }
        public PartyMemberUnit Leader { get { return leader; } }

        // Private fields
        private List<PartyMemberUnit> party; // These are stored as a list in case any are added
        private List<EnemyUnit> enemies; // These are stored as a list in case any are added during battle
        private PartyMemberUnit leader = null;

        // Events
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

        public void SelectLeader(PartyMemberUnit leader)
        {
            this.leader = leader;
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

    public class B_Intro : BattleState
    {
        public B_Intro(BattleHandlerV2 handler) : base(handler) { }

        public override void EnterState() => battleHandler.EnterIntroState();
        public override void Update() => battleHandler.UpdateIntroState();
        public override void ExitState() => battleHandler.ExitIntroState();
    }

    public class B_WaitingForPlayer : BattleState
    {
        public B_WaitingForPlayer(BattleHandlerV2 handler) : base(handler) { }

        public override void EnterState() => battleHandler.EnterWaitingState();
        public override void Update() => battleHandler.UpdateWaitingState();
        public override void ExitState() => battleHandler.ExitWaitingState();
    }

    public class B_ChoosingLeader : BattleState
    {
        public B_ChoosingLeader(BattleHandlerV2 handler) : base(handler) { }

        public override void EnterState() => battleHandler.EnterChoosingLeaderState();
        public override void Update() => battleHandler.UpdateChoosingLeaderState();
        public override void ExitState() => battleHandler.ExitChoosingLeaderState();
    }

    public class B_ExecutingPartyTurn : BattleState
    {
        public B_ExecutingPartyTurn(BattleHandlerV2 handler) : base(handler) { }

        public override void EnterState() => battleHandler.EnterPartyTurnState();
        public override void Update() => battleHandler.UpdatePartyTurnState();
        public override void ExitState() => battleHandler.ExitPartyTurnState();
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

    #region Helpers
    private void CreatePerformanceMarker(Vector2 pos, string txt)
    {
        PerformanceHitmarker hitmarker = Instantiate(_performanceHitmarkerPrefab, pos, Quaternion.identity);
        hitmarker.SetText(txt);
    }
    #endregion
}*/