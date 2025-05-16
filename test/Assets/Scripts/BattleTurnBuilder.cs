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
        TacticsSelection,
        BagSelection,
        ItemTargetSelection
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
    private ItemMenu _itemsMenu;

    [SerializeField]
    private GameObject _downArrow;

    [SerializeField]
    private GameObject _upArrow;

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
    private int targetIndex;
    private PartyMemberUnit currentPartyMember;
    private EUISubstate turnSubstate;
    #endregion
    #endregion

    #region MonoBehaviour
    void Start()
    {
        //CloseAllPlayerMenus();
    }
    #endregion

    // this is bad
    public TurnData currentTurnData;

    #region Helper Functions
    // Bash, Skills, Bag
    public void UpdateMenus()
    {
        switch(turnSubstate)
        {
            case EUISubstate.ActionSelection:
                _actionMenu.UpdateMenu();

                // If there's no items, deactivate the item option
                if(PartyManager.Instance.Bag.IsBagEmpty())
                {
                    _actionMenu.SetOptionActive(false, 1);
                }

                break;
            case EUISubstate.TargetSelection:
                SelectTarget(false);
                break;
            case EUISubstate.BagSelection:
                _itemsMenu.UpdateMenu();

                break;
            case EUISubstate.ItemTargetSelection:
                SelectTarget(true);
                break;
        }
    }

    private void SelectTarget(bool isPartyTarget)
    {
        // get index
        if (Input.GetKeyDown(KeyCode.DownArrow)) targetIndex++;
        else if (Input.GetKeyDown(KeyCode.UpArrow)) targetIndex--;
        else return;

        if (isPartyTarget)
        {
            targetIndex %= BattleHandler.Instance.CurrentBattle.PartyInBattle.Count;

            if (targetIndex < 0) targetIndex = BattleHandler.Instance.CurrentBattle.PartyInBattle.Count - 1;

            UpdatePartyTarget();
        }
        else
        {
            targetIndex %= BattleHandler.Instance.CurrentBattle.EnemiesInBattle.Count;
            
            if (targetIndex < 0) targetIndex = BattleHandler.Instance.CurrentBattle.EnemiesInBattle.Count - 1;

            UpdateEnemyTarget();
        }
    }

    private void UpdatePartyTarget()
    {
        int count = BattleHandler.Instance.CurrentBattle.PartyInBattle.Count;

        for (int i = 0; i < count; i++)
        {
            Debug.Log($"Index: {i}, Party member name {BattleHandler.Instance.CurrentBattle.PartyInBattle[i].MyStats.Name}");
            if (i == targetIndex)
            {
                BattleHandler.Instance.CurrentBattle.PartyInBattle[i].SetTarget(true);
            }
            else
            {
                BattleHandler.Instance.CurrentBattle.PartyInBattle[i].SetTarget(false);
            }
        }
    }

    private void UpdateEnemyTarget()
    {
        int count = BattleHandler.Instance.CurrentBattle.EnemiesInBattle.Count;

        for (int i = 0; i < count; i++)
        {
            if (i == targetIndex)
            {
                BattleHandler.Instance.CurrentBattle.EnemiesInBattle[i].SetTarget(true);
            }
            else
            {
                BattleHandler.Instance.CurrentBattle.EnemiesInBattle[i].SetTarget(false);
            }
        }
    }

    // Backup a step through submenus. Returns true if sucessfully backed up, returns false if can't backup any further
    public bool TryBackup()
    {
        if(turnSubstate == EUISubstate.TargetSelection)
        {
            _actionMenu.SetLightDisable(false);
            turnSubstate = EUISubstate.ActionSelection;
            CloseTargetMenu();

            return true;
        }

        if (turnSubstate == EUISubstate.ItemTargetSelection)
        {
            turnSubstate = EUISubstate.BagSelection;
            CloseTargetMenu();

            return true;
        }

        if (turnSubstate == EUISubstate.BagSelection)
        {
            _actionMenu.SetLightDisable(false);
            turnSubstate = EUISubstate.ActionSelection;
            _itemsMenu.SetVisibility(false);
        }

        return false;
    }

    public bool IsInSubmenu() => turnSubstate != EUISubstate.ActionSelection;

    // Select options until you cant no mo
    public bool TrySelectOption()
    {
        currentTurnData = new TurnData() { BattleAction = (EBattleAction)_actionMenu.CurrentIndex + 1 };

        // Handle action selection
        if (turnSubstate == EUISubstate.ActionSelection)
        {
            _actionMenu.Select();

            // Bash
            if (_actionMenu.CurrentIndex == 0)
            {
                _actionMenu.SetLightDisable(true);
                turnSubstate = EUISubstate.TargetSelection;
                targetIndex = 0;
                UpdateEnemyTarget();
            }

            // Bag
            else if (_actionMenu.CurrentIndex == 1)
            {
                _actionMenu.SetLightDisable(true);
                turnSubstate = EUISubstate.BagSelection;
                currentTurnData.SelectionIndex = _itemsMenu.CurrentIndex;
                OpenItemsMenu();
            }

            // Tactics
            else if (_actionMenu.CurrentIndex == 2)
            {
                turnSubstate = EUISubstate.TacticsSelection;
            }

            return true;
        }

        else if(turnSubstate == EUISubstate.TargetSelection)
        {
            currentTurnData.Target = BattleHandler.Instance.CurrentBattle.EnemiesInBattle[targetIndex];
            Debug.Log($"Selected target at index {targetIndex}, name: {currentTurnData.Target.MyStats.Name}");
            _actionMenu.SetLightDisable(false);
            CloseTargetMenu();
        }

        else if(turnSubstate == EUISubstate.TacticsSelection)
        {

        }

        else if(turnSubstate == EUISubstate.BagSelection)
        {
            turnSubstate = EUISubstate.ItemTargetSelection;
            targetIndex = BattleHandler.Instance.CurrentBattle.PartyInBattle.IndexOf(currentPartyMember);
            UpdatePartyTarget();

            return true;
        }

        else if (turnSubstate == EUISubstate.ItemTargetSelection)
        {
            currentTurnData.Target = BattleHandler.Instance.CurrentBattle.PartyInBattle[targetIndex];
            currentTurnData.SelectionIndex = _itemsMenu.CurrentIndex;
            _actionMenu.SetLightDisable(false);
            CloseTargetMenu();
            CloseItemsMenu();
        }

        turnSubstate = EUISubstate.ActionSelection;

        _actionMenu.SetIndex(0);
        return false;
    }

    public void OpenItemsMenu()
    {

        _itemsMenu.SetVisibility(true);
    }

    public void CloseItemsMenu()
    {
        _itemsMenu.SetIndex(0);
        _itemsMenu.SetVisibility(false);
    }

    public void CloseTargetMenu()
    {
        for (int i = 0; i < BattleHandler.Instance.CurrentBattle.EnemiesInBattle.Count; i++)
        {
            BattleHandler.Instance.CurrentBattle.EnemiesInBattle[i].SetTarget(false);
        }

        for (int i = 0; i < BattleHandler.Instance.CurrentBattle.PartyInBattle.Count; i++)
        {
            BattleHandler.Instance.CurrentBattle.PartyInBattle[i].SetTarget(false);
        }
    }

    public void SetupBattleUI(Battle currentBattle)
    {
        // Setup stat cards
        for (int i = 0; i < currentBattle.PartyInBattle.Count; i++)
        {
            // Create and initialize a new stat card
            StatCardUI statCard = Instantiate(_statCardUI, _statHolder);

            // Initialize stat card
            statCard.Initialize(currentBattle.PartyInBattle[i]);
            statCards.Add(statCard);

            statCard.gameObject.SetActive(false);
        }
    }

    public void SetPartyMember(PartyMemberUnit partyMember)
    {
        currentPartyMember = partyMember;

        // Ensures that the option is saved while the user is deciding their moves, making it easier to immediately jump to where they last were
        _partyHeaderText.text = currentPartyMember.MyStats.Name;

        Debug.Log($"Changing party member: {partyMember.MyStats.Name}");
    }
    #endregion

    #region Menu Opening / Closing
    public void OpenPartyMenus()
    {
        _actionMenu.SetVisibility(true);
        _actionMenu.SetInteractable(true);

        // If there's no items, deactivate the item option
        if (PartyManager.Instance.Bag.IsBagEmpty())
        {
            _actionMenu.SetOptionActive(false, 1);
            return;
        }
    }

    public void ClosePartyMenus()
    {
        _actionMenu.SetVisibility(false);
        _actionMenu.SetInteractable(false);
        CloseItemsMenu();

        currentPartyMember = null;
    }

    public void OpenFlavorText(string text, EDialogueAppearance appearance)
    {
        _battleDialogueHolder.gameObject.SetActive(true);
        //_battleDialogueWriter.QueueDialoguePayload(new DialogueData() { Text = text, Appearance = appearance });
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