using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Hank.Battles;
using TMPro;
using System;

public class BattleTurnBuilder : MonoBehaviour
{
    #region Fields
    public enum EUISubstate
    {
        ActionSelection,
        TargetSelection,
        SkillsSelection,
        BagSelection
    }

    #region Inspector Fields
    [Header("Dialogue")]
    [SerializeField]
    private DialogueTextWriter _battleDialogueWriter;

    [SerializeField]
    private RectTransform _battleDialogueHolder;

    [Header("Leader Choosing UI")]
    [SerializeField]
    private GameObject _leaderUI;

    [Header("Turn Order")]
    [SerializeField]
    private RectTransform _turnOrderUIHolder;

    [SerializeField]
    private Image _turnOrderPrefab;

    [Header("Action Menu")]
    [SerializeField]
    private TextMeshProUGUI _partyHeaderText;

    [SerializeField]
    private Menu _actionMenu;

    [SerializeField]
    private Menu _targetMenu;

    [SerializeField]
    private MenuOption _targetOptionPrefab;

    [Header("Stat Card")]
    [SerializeField]
    private StatCardUI _statCardUI;

    [SerializeField]
    private RectTransform _statHolder;
    #endregion

    #region Private Fields
    private List<Image> turnOrderImages = new List<Image>();
    private List<StatCardUI> statCards = new List<StatCardUI>();
    private PartyMemberUnit currentPartyMember;
    private Dictionary<PartyMemberUnit, TurnMenuData> partyTurnDataPairs = new Dictionary<PartyMemberUnit, TurnMenuData>();
    private EUISubstate turnSubstate;
    #endregion

    #region Accessors
    public Dictionary<PartyMemberUnit, TurnMenuData> PartyTurnDataPairs => partyTurnDataPairs;
    #endregion
    #endregion

    #region MonoBehaviour
    void Start()
    {
        _actionMenu.OnMenuItemSelected += ActionItemSelected;

        CloseAllPlayerMenus();
    }
    #endregion

    #region Helper Functions
    private void ActionItemSelected(int index)
    {
        // Save values
        partyTurnDataPairs[currentPartyMember].BattleAction = (EBattleAction)index;
        partyTurnDataPairs[currentPartyMember].SavedActionIndex = index;
        Debug.Log($"Cached index {partyTurnDataPairs[currentPartyMember].SavedActionIndex} to {currentPartyMember.MyStats.Name}");
    }

    // Bash, Skills, Bag
    public void UpdateActionMenu()
    {
        _actionMenu.UpdateMenu();
    }

    // Backup a step through submenus. Returns true if sucessfully backed up, returns false if can't backup any further
    public bool Backup()
    {
        return false;
    }

    // Select options until you cant no mo
    public bool SelectOption()
    {
        _actionMenu.Select();

        return false;
    }

    public void SetupBattleUI(Battle currentBattle)
    {
        // Setup stat cards
        for (int i = 0; i < currentBattle.PartyInBattle.Count; i++)
        {
            // Create and initialize a new stat card
            StatCardUI statCard = Instantiate(_statCardUI, _statHolder);

            partyTurnDataPairs.Clear();
            partyTurnDataPairs.Add(currentBattle.PartyInBattle[i], new TurnMenuData());

            // Initialize stat card
            statCard.Initialize(currentBattle.PartyInBattle[i]);
            statCards.Add(statCard);

            statCard.gameObject.SetActive(false);
        }
    }

    public void ResetTurnMenuData()
    {
        // Resets all the saved turn menu data
        for(int i = 0; i < BattleHandlerV2.Instance.CurrentBattle.PartyInBattle.Count; i++)
        {
            partyTurnDataPairs[BattleHandlerV2.Instance.CurrentBattle.PartyInBattle[i]] = new TurnMenuData();
        }
    }

    public void SetPartyMember(PartyMemberUnit partyMember)
    {
        currentPartyMember = partyMember;

        // Ensures that the option is saved while the user is deciding their moves, making it easier to immediately jump to where they last were
        _actionMenu.SetIndex(partyTurnDataPairs[currentPartyMember].SavedActionIndex);
        _partyHeaderText.text = currentPartyMember.MyStats.Name;

        Debug.Log($"Changing party member: {partyMember.MyStats.Name}, Action index: {partyTurnDataPairs[currentPartyMember].SavedActionIndex}");
    }
    #endregion

    #region Menu Opening / Closing
    public void OpenPartyMenus()
    {
        _actionMenu.SetVisibility(true);
        _actionMenu.SetInteractable(true);
    }

    public void ClosePartyMenus()
    {
        _actionMenu.SetVisibility(false);
        _actionMenu.SetInteractable(false);

        currentPartyMember = null;
    }

    public void OpenFlavorText(EDialogueAppearance appearance)
    {
        _battleDialogueHolder.gameObject.SetActive(true);
        _battleDialogueWriter.QueueDialoguePayload(new DialogueData() { Text = "Enemies approach Hank!", Appearance = appearance });
    }

    public void HideFlavorText()
    {
        _battleDialogueHolder.gameObject.SetActive(false);
        _battleDialogueWriter.Cleanup();
    }

    public void OpenChoosingLeaderUI()
    {
        _leaderUI.SetActive(true);
    }

    public void CloseChoosingLeaderUI()
    {
        _leaderUI.SetActive(false);
    }

    private void CloseAllPlayerMenus()
    {
        // Turn order
        _turnOrderUIHolder.gameObject.SetActive(false);

        // Action menu
        _actionMenu.SetVisibility(false);
        _actionMenu.SetInteractable(false);
    }
    #endregion
}

public class TurnMenuData
{
    public int SavedActionIndex;
    public EBattleAction BattleAction;
}