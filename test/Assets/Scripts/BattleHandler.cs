using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using Random = UnityEngine.Random;
using DG.Tweening;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;

/* 
- Remember the following -

1. Three party members tops. Design for three ONLY
2. You can figure out enemy formation later 
3. You can figure out special battle events later
4. Just get the main flow working and have a couple fun attacks designed

- Paper Mario Notes

1. You can change the order you attack in with a button press (probably to complement the execution order)
2. Action gets chosen and is immediately executed rather than picking all of them at once
3. Party members look dreary when low health
4. Sleeping shows the amount of turns before wearing off
5. Depending on if the enemy or player strikes first, the music changes intro

 */


/* Address the following:
 * - Enemies can take one item into battle (this is not random)
 * - Units can have status effects that last for multiple turns
 * - All actions execute some kind of animation before completing
 */


namespace Hank.Battles
{
    public enum EBattleAction
    {
        None,
        Bash,
        Bag,
        Guard,
        Skill,
    }

    public class TurnData
    {
        public EBattleAction BattleAction;
        public bool IsTurnUsed;
        public BattleUnit Target;
        public int SelectionIndex;
    }

    public class BattleHandler : MonoBehaviour
    {
        #region Fields
        private const float MAX_PERFORMANCE_DAMAGE_MULTIPLIER = 1.5f;

        #region Accessors
        public static BattleHandler Instance { get { return instance; } }
        public Dictionary<PartyMemberUnit, TurnData> PartyTurnDataPairs => partyTurnDataPairs;
        #endregion

        public enum EBattleState
        {
            Inactive,           // No battle currently active
            Intro,              // Play dope ass intro animation
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
        [SerializeField] private float _rotationSpeed = 0.15f;

        [Header("References")]
        [SerializeField] private BattleTurnBuilder _battleTurnBuilder;
        [SerializeField] private PerformanceHitmarker _performanceHitmarker;
        [SerializeField] private HealthValueMarker _healthHitmarker;

        #region Private Fields
        private static BattleHandler instance;
        private Dictionary<EBattleState, BattleState> battleStateValuePairs = new Dictionary<EBattleState, BattleState>();
        private Dictionary<PartyMemberUnit, TurnData> partyTurnDataPairs = new Dictionary<PartyMemberUnit, TurnData>();
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
            battleStateValuePairs.Add(EBattleState.ExecutingPartyTurn, new B_ExecutingPartyTurn(this));
            battleStateValuePairs.Add(EBattleState.ExecutingEnemyTurn, new B_ExecutingEnemyTurn(this));
            battleStateValuePairs.Add(EBattleState.BattleComplete, new B_BattleWon(this));
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

            // Clear and register all units
            partyTurnDataPairs.Clear();
            foreach (PartyMemberUnit unit in party)
            {
                partyTurnDataPairs.Add(unit, new TurnData());
            }

            // We pass the current battle into the event so the UI can register the correct UI elements
            OnBattleStart?.Invoke(currentBattle);

            // Set state immediately
            partyMemberTurnIndex = -1;
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
            if (!hasPlayedIntro) StartCoroutine(IPlayIntro());

            // Otherwise, we're coming back to the intro state, so immediately show the flavor text (dont make the player sit through the typewriting)
            else _battleTurnBuilder.OpenFlavorText("Enemies approach Hank!", EDialogueAppearance.Immediate);
        }

        public void UpdateIntroState()
        {
            if (!hasPlayedIntro) return;

            if (Input.GetKeyDown(KeyCode.Z))
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
            _battleTurnBuilder.OpenFlavorText("Enemies approach Hank!", EDialogueAppearance.Typewriter);
        }
        #endregion

        #region [STATE] Waiting State
        public void EnterWaitingState()
        {
            Debug.Log($"Waiting for player...");

            for (int i = 0; i < currentBattle.PartyInBattle.Count; i++)
            {
                Debug.Log($"NAME: {currentBattle.PartyInBattle[i]}, {partyTurnDataPairs[currentBattle.PartyInBattle[i]].IsTurnUsed}");
                currentBattle.PartyInBattle[i].SetDecommissionedVisual(partyTurnDataPairs[currentBattle.PartyInBattle[i]].IsTurnUsed);
            }

            // Get current index
            partyMemberTurnIndex = -1;
            partyMemberTurnIndex = GetNextValidIndex();

            _battleTurnBuilder.SetPartyMember(currentBattle.PartyInBattle[partyMemberTurnIndex]);
            _battleTurnBuilder.OpenPartyMenus();

            UpdatePartyArrangement();
        }

        public void UpdateWaitingState()
        {
            _battleTurnBuilder.UpdateMenus();

            // Rotate order
            if (Input.GetKeyDown(KeyCode.C) && !_battleTurnBuilder.IsInSubmenu())
            {
                // Get the next available index 
                partyMemberTurnIndex = GetNextValidIndex();
                _battleTurnBuilder.SetPartyMember(GetCurrentPartyMember());
                UpdatePartyArrangement();
            }

            if (Input.GetKeyDown(KeyCode.X))
            {
                _battleTurnBuilder.TryBackup();
            }

            // If there are no more options to select, continue through the turn index
            if (Input.GetKeyDown(KeyCode.Z) && !_battleTurnBuilder.TrySelectOption())
            {
                partyTurnDataPairs[GetCurrentPartyMember()] = _battleTurnBuilder.currentTurnData;
                partyTurnDataPairs[GetCurrentPartyMember()].IsTurnUsed = true;

                SetState(EBattleState.ExecutingPartyTurn);
            }
        }

        public void ExitWaitingState()
        {
            // 1 - Close menus
            _battleTurnBuilder.ClosePartyMenus();
        }
        #endregion

        #region [STATE] Executing Party Turn State
        // battle action handler takes care of the turn

        public void EnterPartyTurnState()
        {
            // Set visual
            for (int i = 0; i < currentBattle.PartyInBattle.Count; i++)
            {
                currentBattle.PartyInBattle[i].SetDecommissionedVisual(false);
            }

            Debug.Log($"fuck {partyTurnDataPairs[GetCurrentPartyMember()].BattleAction}");

            // Perform battle action
            switch (partyTurnDataPairs[GetCurrentPartyMember()].BattleAction)
            {
                // each one of these will instantiate a gameobject animation, which this coroutine waits for
                case EBattleAction.Bash:
                    HandleBash();
                    break;
                case EBattleAction.Guard:
                    HandleGuard();
                    break;
                case EBattleAction.Bag:
                    HandleItem();
                    break;
            }
        }

        public void UpdatePartyTurnState()
        {
            // Perform battle action
            switch (partyTurnDataPairs[GetCurrentPartyMember()].BattleAction)
            {
                // each one of these will instantiate a gameobject animation, which this coroutine waits for
                case EBattleAction.Bash:
                    GetCurrentPartyMember().UpdateBashState();
                    break;
                case EBattleAction.Guard:
                    break;
                case EBattleAction.Bag:
                    break;
            }
        }

        public void ExitPartyTurnState()
        {

        }
        #endregion

        #region Action Methods

        public void HandleBash()
        {
            GetCurrentPartyMember().StartBash(partyTurnDataPairs[GetCurrentPartyMember()].Target);
            GetCurrentPartyMember().OnActionComplete += OnActionComplete;
        }

        public void HandleGuard()
        {

        }

        public void HandleItem()
        {
            GetCurrentPartyMember().StartUsingItem(partyTurnDataPairs[GetCurrentPartyMember()].SelectionIndex, partyTurnDataPairs[GetCurrentPartyMember()].Target);
            GetCurrentPartyMember().OnActionComplete += OnActionComplete;
        }

        public void OnActionComplete()
        {
            // Deregister from events
            GetCurrentPartyMember().OnActionComplete -= OnActionComplete;
            GetCurrentPartyMember().transform.DOMoveX(_defaultXValue, _rotationSpeed).SetEase(Ease.OutQuad);

            // Check if any enemies died
            for(int i = 0; i < currentBattle.EnemiesInBattle.Count; i++)
            {
                if (currentBattle.EnemiesInBattle[i].CheckForDeath())
                {
                    currentBattle.EnemiesInBattle[i].Cleanup();
                    currentBattle.DeregisterEnemyUnit(i);

                    i--;
                }
            }

            // Check for win state
            if (IsPartyWon())
            {
                SetState(EBattleState.BattleComplete);
                return;
            }

            // Change state accordingly
            if (CurrentBattle.PartyInBattle.All(unit => partyTurnDataPairs[unit].IsTurnUsed)) SetState(EBattleState.ExecutingEnemyTurn);
            else SetState(EBattleState.WaitingForPlayer);
        }

        #endregion

        #region [STATE] Executing Enemy Turn State
        public void EnterEnemyTurnState()
        {
            Debug.Log($"Enemy turn!");

            ResetTurnData();
            StartCoroutine(IExecuteEnemyTurn());
        }

        private IEnumerator IExecuteEnemyTurn()
        {
            BattleUnit currentUnit = null;

            // check for status effects first
            for (int i = 0; i < currentBattle.EnemiesInBattle.Count; i++)
            {
                yield return new WaitForSeconds(1);
            }

            SetState(EBattleState.WaitingForPlayer);
            yield return null;
        }

        public void UpdateEnemyTurnState()
        {
            // 1 - The party has the chance of dying here, so check for death

            // Leader - Dodge
            if (Input.GetKeyDown(KeyCode.Z))
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

        #region [STATE] Battle Won State
        public void EnterBattleCompleteState()
        {
            Debug.Log("Battle complete!");

            _battleTurnBuilder.OpenFlavorText("You won!", EDialogueAppearance.Typewriter);
        }

        public void UpdateBattleCompleteState()
        {

        }

        public void ExitBattleCompleteState()
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

        public PartyMemberUnit GetCurrentPartyMember() => currentBattle.PartyInBattle[partyMemberTurnIndex];

        public void UpdatePartyArrangement()
        {
            for (int i = 0; i < currentBattle.PartyInBattle.Count; i++)
            {
                currentBattle.PartyInBattle[i].transform.DOKill(false);

                if (i == partyMemberTurnIndex)
                {
                    currentBattle.PartyInBattle[i].transform.DOMoveX(_forwardXValue, _rotationSpeed).SetEase(Ease.OutQuad);
                    continue;
                }

                currentBattle.PartyInBattle[i].transform.DOMoveX(_defaultXValue, _rotationSpeed).SetEase(Ease.OutQuad);
            }
        }

        public void RegisterTurnData(PartyMemberUnit key, TurnData turnData)
        {
            partyTurnDataPairs[key] = turnData;
        }

        private void ResetTurnData()
        {
            for (int i = 0; i < currentBattle.PartyInBattle.Count; i++)
            {
                partyTurnDataPairs[currentBattle.PartyInBattle[i]] = new TurnData();
            }
        }

        private int GetNextValidIndex()
        {
            int startIndex = partyMemberTurnIndex;
            int currentIndex = partyMemberTurnIndex;

            do
            {
                currentIndex = (currentIndex + 1) % currentBattle.PartyInBattle.Count;

                if (!partyTurnDataPairs[currentBattle.PartyInBattle[currentIndex]].IsTurnUsed)
                    return currentIndex;
            }
            while (currentIndex != startIndex); // Stop if we've looped back to the original index

            return -1; // No valid index found
        }
        #endregion

        #region Damage / Hits
        public void RegisterUnitHeal(BattleUnit healingUnit, int health)
        {
            HealthValueMarker healthHitmarker = Instantiate(_healthHitmarker, healingUnit.transform.position, Quaternion.identity);
            healthHitmarker.Setup(health, false);
        }

        public void RegisterPartyMemberHit(EnemyUnit attackingUnit, PartyMemberUnit attackedUnit, int rawDamage)
        {
            RegisterUnitDamage(attackingUnit, attackedUnit, rawDamage, 1.0f);

            // create hitmarker
        }

        public void RegisterEnemyHit(PartyMemberUnit attackingUnit, EnemyUnit attackedUnit, int rawDamage, EAttackPerformance performance)
        {
            RegisterUnitDamage(attackingUnit, attackedUnit, rawDamage, GetPerformanceMultiplier(performance));

            // create damage hitmarker
            // show performance
            PerformanceHitmarker hitmarker = Instantiate(_performanceHitmarker, attackedUnit.transform.position, Quaternion.identity);
            hitmarker.SetText(performance.ToString());
        }

        private void RegisterUnitDamage(BattleUnit attackingUnit, BattleUnit attackedUnit, int rawDamage, float performanceMultiplier)
        {
            int damage = CalculateDamage(rawDamage, attackingUnit.MyStats.BaseAttack, attackedUnit.MyStats.BaseDefense, performanceMultiplier);
            attackedUnit.MyHealth.TakeDamage(damage);

            // Create hitmarker
            HealthValueMarker healthHitmarker = Instantiate(_healthHitmarker, attackedUnit.transform.position, Quaternion.identity);
            healthHitmarker.Setup(damage, true);
        }

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
        public List<PartyMemberUnit> PartyInBattle => party;
        public List<EnemyUnit> EnemiesInBattle => enemies;

        // Private fields
        private List<PartyMemberUnit> party; // These are stored as a list in case any are added
        private List<EnemyUnit> enemies; // These are stored as a list in case any are added during battle

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

        public void DeregisterEnemyUnit(int index)
        {
            enemies.RemoveAt(index);
        }
    }

    #region States
    public class BattleState
    {
        protected BattleHandler battleHandler;

        public BattleState(BattleHandler handler) => battleHandler = handler;

        public virtual void EnterState() { }
        public virtual void ExitState() { }
        public virtual void Update() { }
    }

    public class B_Intro : BattleState
    {
        public B_Intro(BattleHandler handler) : base(handler) { }

        public override void EnterState() => battleHandler.EnterIntroState();
        public override void Update() => battleHandler.UpdateIntroState();
        public override void ExitState() => battleHandler.ExitIntroState();
    }

    public class B_WaitingForPlayer : BattleState
    {
        public B_WaitingForPlayer(BattleHandler handler) : base(handler) { }

        public override void EnterState() => battleHandler.EnterWaitingState();
        public override void Update() => battleHandler.UpdateWaitingState();
        public override void ExitState() => battleHandler.ExitWaitingState();
    }

    public class B_ExecutingPartyTurn : BattleState
    {
        public B_ExecutingPartyTurn(BattleHandler handler) : base(handler) { }

        public override void EnterState() => battleHandler.EnterPartyTurnState();
        public override void Update() => battleHandler.UpdatePartyTurnState();
        public override void ExitState() => battleHandler.ExitPartyTurnState();
    }

    public class B_ExecutingEnemyTurn : BattleState
    {
        public B_ExecutingEnemyTurn(BattleHandler handler) : base(handler) { }

        public override void EnterState() => battleHandler.EnterEnemyTurnState();
        public override void Update() => battleHandler.UpdateEnemyTurnState();
        public override void ExitState() => battleHandler.ExitEnemyTurnState();
    }

    public class B_BattleWon : BattleState
    {
        public B_BattleWon(BattleHandler handler) : base(handler) { }

        public override void EnterState() => battleHandler.EnterBattleCompleteState();
        public override void Update() => battleHandler.UpdateBattleCompleteState();
        public override void ExitState() => battleHandler.ExitBattleCompleteState();
    }
    #endregion
}