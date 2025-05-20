using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

/* 
- Remember the following -

1. Two party members tops. Design for two ONLY
2. You can figure out enemy formation later 
3. You can figure out special battle events later
4. Just get the main flow working and have a couple fun attacks designed

- Paper Mario Notes

1. You can change the order you attack in with a button press (probably to complement the execution order)
2. Action gets chosen and is immediately executed rather than picking all of them at once
3. Party members look dreary when low health
4. Sleeping shows the amount of turns before wearing off
5. Depending on if the enemy or player strikes first, the music changes intro

For sake of this system, I'm assuming there will always be two party members

 */


/* Address the following:
 * - Enemies can take one item into battle (this is not random)
 * - Units can have status effects that last for multiple turns
 * - All actions execute some kind of animation before completing
 */

/* Target - Enum (All Party, All Enemies, One Enemy Unit, One Party Unit)
 * - Enemies can have multiple target spots without being multiple enemies, but they are listed in parenthesis
 * - Status effects show how many turns remain until it wears off
 * - 
 * 
 * 
 * 
 * 
 */

[System.Serializable]
public class Battle
{
    [System.Serializable]
    public struct EnemyFormation
    {
        [Tooltip("The offset from the chosen battle position on screen (controlled by BattleManager)")]
        public Vector2 SpawnOffset;

        [Tooltip("The enemy to spawn")]
        public EnemyUnit EnemyUnit;
    }

    public enum EFlavorTextAppearanceType
    {
        FirstThenShuffle,       // Only shows the first in the array, then shuffles the rest
        Shuffle,                // Shuffles all flavor text
        Loop                    // Goes through and loops
    }

    public EnemyFormation[] EnemyUnitFormations;
    public EFlavorTextAppearanceType FlavorTextAppearanceType;
    public DialogueData[] FlavorText;
}

public class BattleManager : MonoBehaviour
{
    #region Fields
    private const float MAX_PERFORMANCE_DAMAGE_MULTIPLIER = 1.5f;

    public enum EBattleState
    {
        None,
        Intro,
        WaitingForPlayer,
        ExecutingPlayerTurn,
        Dialogue,
        ExecutingEnemyTurn,
        Outro
    }

    #region Public Fields
    [Header("References")]
    [SerializeField] private BattleUIHandler _battleUI;

    [Header("Modifiers")]
    [SerializeField] private float _defaultXValue = -7.5f;
    [SerializeField] private float _forwardXValue = -6.5f;
    [SerializeField] private float _enemyDefaultXValue = 7.5f;
    [SerializeField] private float _partySpacing;

    [Header("Visuals")]
    [SerializeField] private SpriteRenderer _background;

    [Header("Audio")]
    [SerializeField] private AudioSource _source;
    [SerializeField] private AudioClip _battleIntroClip;
    #endregion

    #region Private Fields
    private List<PartyUnit> _activePartyUnits;
    private List<EnemyUnit> _activeEnemyUnits;

    private int partyMemberTurnIndex;
    private int turnNumber;
    private Battle currentBattle;
    private EBattleState currentBattleState;
    private Dictionary<EBattleState, BattleState> battleStateValuePairs = new Dictionary<EBattleState, BattleState>();
    #endregion

    #region Events
    public delegate void UnitFormation();
    public UnitFormation OnUnitsFormation;

    public delegate void IntroFinished();
    public IntroFinished OnIntroFinished;
    #endregion
    #endregion

    private void Awake()
    {
        // Initialize state machine
        battleStateValuePairs.Add(EBattleState.None, null);
        battleStateValuePairs.Add(EBattleState.Intro, new B_Intro(this));
        battleStateValuePairs.Add(EBattleState.WaitingForPlayer, new B_WaitingForPlayer(this));
        battleStateValuePairs.Add(EBattleState.ExecutingPlayerTurn, new B_ExecutingPartyTurn(this));
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);

        if (currentBattleState == EBattleState.None) return;
          
        battleStateValuePairs[currentBattleState]?.Update();
    }

    public void StartBattle(Vector2 enemyStartPoint, Battle battle)
    {
        // Reset values
        Cleanup();

        currentBattle = battle;
        SetState(EBattleState.Intro);
        StartCoroutine(IPlayBattleIntro(enemyStartPoint));
        GameManager.Instance.ChangeGameState(GameManager.EGameState.Battle);
    }

    private void Cleanup()
    {
        turnNumber = 0;
        partyMemberTurnIndex = 0;

        _activePartyUnits = new List<PartyUnit>();
        _activeEnemyUnits = new List<EnemyUnit>();
    }

    private IEnumerator IPlayBattleIntro(Vector2 startPoint)
    {
        AudioManager.Instance.PauseMusic();

        _source.PlayOneShot(_battleIntroClip);
        yield return new WaitForSeconds(0.8f);
        OnUnitsFormation?.Invoke();

        // Find all party members on the field and create their battle versions
        foreach(FieldPartyMember partyMember in FindObjectsByType<FieldPartyMember>(FindObjectsSortMode.None))
        {
            PartyUnit partyUnit = Instantiate(partyMember.MyBattleUnit, partyMember.transform.position, Quaternion.identity);
            partyUnit.transform.DOMove(new Vector2(_defaultXValue, 0.0f), 0.4f).SetEase(Ease.OutQuad);

            _activePartyUnits.Add(partyUnit);
        }

        // Spawn all enemies and move them into formation
        foreach (Battle.EnemyFormation formation in currentBattle.EnemyUnitFormations)
        {
            EnemyUnit enemyUnit = Instantiate(formation.EnemyUnit, startPoint, Quaternion.identity);
            enemyUnit.transform.DOMove(new Vector2(_enemyDefaultXValue + formation.SpawnOffset.x, formation.SpawnOffset.y), 0.4f).SetEase(Ease.OutQuad);

            _activeEnemyUnits.Add(enemyUnit);
        }

        // Fade background in
        _background.DOFade(1.0f, 0.4f).SetEase(Ease.OutQuad);
        yield return new WaitForSeconds(0.4f);

        // Intro finished, play music
        AudioManager.Instance.PlayDefaultBattleMusic();

        // Finish intro
        OnIntroFinished?.Invoke();
        SetState(EBattleState.WaitingForPlayer);
    }


    #region Intro State
    public void EnterIntroState()
    {

    }

    #endregion

    #region Waiting For Player State
    public void EnterWaitingForPlayerState()
    {
        // Show the current flavor text
        _battleUI.ShowFlavorText(currentBattle.FlavorText, currentBattle.FlavorTextAppearanceType, turnNumber);
    }

    public void UpdateWaitingForPlayerState()
    {

    }

    public void ExitWaitingForPlayerState()
    {

    }
    #endregion

    #region Executing Player State
    public void EnterExecutingPlayerState()
    {

    }

    public void UpdateExecutingPlayerState()
    {

    }

    public void ExitExecutingPlayerState()
    {

    }
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
        //OnBattleStateUpdated?.Invoke(previousState, state);
    }

    private void TargetUnit(Unit unit)
    {
        unit.SetTarget(true);
    }

    private void TargetAllEnemies()
    {
        foreach (EnemyUnit enemy in _activeEnemyUnits)
        {
            enemy.SetTarget(true);
        }
    }

    private void TargetAllParty()
    {
        foreach (PartyUnit partyMember in _activePartyUnits)
        {
            partyMember.SetTarget(true);
        }
    }

    private void ClearTargets()
    {
        foreach (EnemyUnit enemy in _activeEnemyUnits)
        {
            enemy.SetTarget(false);
        }

        foreach (PartyUnit partyMember in _activePartyUnits)
        {
            partyMember.SetTarget(false);
        }
    }
    #endregion
}

#region States
public class BattleState
{
    protected BattleManager battleHandler;

    public BattleState(BattleManager handler) => battleHandler = handler;

    public virtual void EnterState() { }
    public virtual void ExitState() { }
    public virtual void Update() { }
}

public class B_Intro : BattleState
{
    public B_Intro(BattleManager handler) : base(handler) { }

    public override void EnterState() => battleHandler.EnterIntroState();
}

public class B_WaitingForPlayer : BattleState
{
    public B_WaitingForPlayer(BattleManager handler) : base(handler) { }

    public override void EnterState() => battleHandler.EnterWaitingForPlayerState();
    public override void Update() => battleHandler.UpdateWaitingForPlayerState();
    public override void ExitState() => battleHandler.ExitWaitingForPlayerState();
}

public class B_ExecutingPartyTurn : BattleState
{
    public B_ExecutingPartyTurn(BattleManager handler) : base(handler) { }

    public override void EnterState() => battleHandler.EnterExecutingPlayerState();
    public override void Update() => battleHandler.UpdateExecutingPlayerState();
    public override void ExitState() => battleHandler.ExitExecutingPlayerState();
}

#endregion