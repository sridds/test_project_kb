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
        public Unit EnemyUnit;
    }

    public enum EFlavorTextAppearanceType
    {
        FirstThenShuffle,       // Only shows the first in the array, then shuffles the rest
        Shuffle,                // Shuffles all flavor text
        Loop                    // Goes through and loops
    }

    public EnemyFormation[] EnemyUnitFormations;
    public EFlavorTextAppearanceType FlavorTextAppearanceType;
    public string[] FlavorText;
}

public class BattleManager : MonoBehaviour
{
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

    [Header("References")]
    [SerializeField] private PartyUnit _hankUnit;

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

    private int partyMemberTurnIndex;
    private Battle currentBattle;

    #region Events
    public delegate void UnitFormation();
    public UnitFormation OnUnitsFormation;

    public delegate void IntroFinished();
    public IntroFinished OnIntroFinished;
    #endregion

    public void StartBattle(Vector2 enemyStartPoint, Battle battle)
    {
        partyMemberTurnIndex = 0;
        currentBattle = battle;
        StartCoroutine(IPlayBattleIntro(enemyStartPoint));

        // Retrieve party equivalent of party on screen
    }

    private IEnumerator IPlayBattleIntro(Vector2 startPoint)
    {
        AudioManager.Instance.PauseMusic();
        GameManager.Instance.ChangeGameState(GameManager.EGameState.Battle);

        _source.PlayOneShot(_battleIntroClip);
        yield return new WaitForSeconds(0.8f);
        OnUnitsFormation?.Invoke();

        // Spawn party and move into formation
        Unit hankUnit = Instantiate(_hankUnit, FindObjectOfType<HankAnimationHelper>().transform.position, Quaternion.identity);
        hankUnit.transform.DOMove(new Vector2(_defaultXValue, 0.0f), 0.4f).SetEase(Ease.OutQuad);

        // Spawn all enemies and move them into formation
        foreach (Battle.EnemyFormation formation in currentBattle.EnemyUnitFormations)
        {
            Unit enemyUnit = Instantiate(formation.EnemyUnit, startPoint, Quaternion.identity);
            enemyUnit.transform.DOMove(new Vector2(_enemyDefaultXValue + formation.SpawnOffset.x, formation.SpawnOffset.y), 0.4f).SetEase(Ease.OutQuad);
        }

        // Fade background in
        _background.DOFade(1.0f, 0.4f).SetEase(Ease.OutQuad);
        yield return new WaitForSeconds(0.4f);

        // Intro finished, play music
        AudioManager.Instance.PlayDefaultBattleMusic();
        OnIntroFinished?.Invoke();
    }

    #region Waiting For Player State
    private void EnterWaitingForPlayerState()
    {

    }

    private void UpdateWaitingForPlayerState()
    {

    }

    private void ExitWaitingForPlayerState()
    {

    }
    #endregion

    #region Executing Player State
    private void EnterExecutingPlayerState()
    {

    }

    private void UpdateExecutingPlayerState()
    {

    }

    private void ExitExecutingPlayerState()
    {

    }
    #endregion

    #region Dialogue State
    private void EnterDialogueState()
    {

    }

    private void UpdateDialogueState()
    {

    }

    private void ExitDialogueState()
    {

    }
    #endregion

    #region Internal Helper Functions
    #endregion
}
