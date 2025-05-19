using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Hank.Battles;
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

    [SerializeField]
    private AudioSource _source;

    [SerializeField]
    private AudioClip _battleIntroClip;

    [Header("Modifiers")]
    [SerializeField] private float _defaultXValue = -7.5f;

    [SerializeField] private float _forwardXValue = -6.5f;

    [SerializeField]
    private float _partySpacing;

    private int partyMemberTurnIndex;

    public void StartBattle()
    {
        StartCoroutine(IPlayBattleIntro());

        // Retrieve party equivalent of party on screen
    }

    private IEnumerator IPlayBattleIntro()
    {
        GameManager.Instance.ChangeGameState(GameManager.EGameState.Battle);

        _source.PlayOneShot(_battleIntroClip);
        yield return new WaitForSeconds(1.2f);
        AudioManager.Instance.PlayDefaultBattleMusic();
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
