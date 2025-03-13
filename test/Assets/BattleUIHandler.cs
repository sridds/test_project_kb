using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

// Battle UI handles all UI, then sends a payload back to the battle handler

public class BattleUIHandler : MonoBehaviour
{
    public enum EUISubstate
    {
        ActionSelection,
        TargetSelection,
    }

    [Header("Turn Order")]
    [SerializeField]
    private RectTransform _turnOrderUIHolder;

    [SerializeField]
    private Image _turnOrderPrefab;

    [Header("Action Menu")]
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

    private List<Image> turnOrderImages = new List<Image>();
    private List<StatCardUI> statCards = new List<StatCardUI>();

    private EUISubstate turnSubstate;

    void Start()
    {
        _turnOrderUIHolder.gameObject.SetActive(false);

        BattleHandler.Instance.OnTurnOrderUpdated += TurnOrderUpdated;
        BattleHandler.Instance.OnBattleStart += SetupBattleUI;
        BattleHandler.Instance.OnBattleStateUpdated += BattleStateUpdated;

        _actionMenu.OnMenuItemSelected += ActionItemSelected;
        _targetMenu.OnMenuItemHovered += UpdateEnemyTargetHovered;
        _targetMenu.OnMenuItemSelected += SelectTargetEnemy;

        HideAllPlayerMenus();
    }

    private void HideAllPlayerMenus()
    {
        SetSkillMenuVisibility(false);
        CloseTargetMenu();
        BattleHandler.Instance.DisableAllTargetOverlays();
    }

    private void LateUpdate()
    {
        if(BattleHandler.Instance.BattleState == BattleHandler.EBattleState.PlayerTurn)
        {
            switch (turnSubstate)
            {
                case EUISubstate.ActionSelection:
                    _actionMenu.UpdateMenu();
                    break;
                case EUISubstate.TargetSelection:

                    if (Input.GetKeyDown(KeyCode.X))
                    {
                        SwitchToSubstate(EUISubstate.ActionSelection);
                        BattleHandler.Instance.DisableAllTargetOverlays();
                    }

                    _targetMenu.UpdateMenu();
                    break;
            }
        }
    }

    private void SelectTargetEnemy(int selectionIndex)
    {
        BattleHandler.Instance.HandleBash(BattleHandler.Instance.EnemiesInBattle[selectionIndex]);
    }

    private void UpdateEnemyTargetHovered(int selectionIndex)
    {
        BattleHandler.Instance.SetTargetOverlayEnabled(selectionIndex);
    }

    private void SetupBattleUI()
    {
        SetupStatCards();
        SetupTargets();
    }

    private void SetupTargets()
    {
        foreach(EnemyUnit enemy in BattleHandler.Instance.EnemiesInBattle)
        {
            _targetMenu.AddMenuOption(_targetOptionPrefab, enemy.MyStats.Name);
        }
    }

    private void ActionItemSelected(int index)
    {
        switch (index)
        {
            // Bash
            case 0:
                Debug.Log(index + " -- Bash");
                SwitchToSubstate(EUISubstate.TargetSelection);
                break;
            // Skill
            case 1:
                Debug.Log(index + " -- Skill");

                break;
            // Bag
            case 2:
                Debug.Log(index + " -- Bag");

                break;

            default:
                Debug.Log("Fuck man... i dont know about this action item man case " + index + " doesnt exist man");
                break;
        }
    }

    private void BattleStateUpdated(BattleHandler.EBattleState battleState)
    {
        if (battleState == BattleHandler.EBattleState.PlayerTurn)
        {
            SwitchToSubstate(EUISubstate.ActionSelection);
        }

        else
        {
            HideAllPlayerMenus();
        }
    }

    private void SwitchToSubstate(EUISubstate substate)
    {
        turnSubstate = substate;

        switch (substate)
        {
            case EUISubstate.ActionSelection:
                SetTurnOrderVisibility(true);
                _actionMenu.SetLightDisable(false);
                SetSkillMenuVisibility(true);
                CloseTargetMenu();
                break;
            case EUISubstate.TargetSelection:
                OpenTargetMenu();
                _actionMenu.SetInteractable(false);
                _actionMenu.SetLightDisable(true);
                break;
        }
    }

    private void OpenTargetMenu()
    {
        _targetMenu.SetVisibility(true);
        _targetMenu.SetInteractable(true);

        turnSubstate = EUISubstate.TargetSelection;
    }

    private void CloseTargetMenu()
    {
        _targetMenu.SetVisibility(false);
        _targetMenu.SetInteractable(false);
    }

    private void SetupStatCards()
    {
        for(int i = 0; i < BattleHandler.Instance.PartyInBattle.Count; i++)
        {
            // Create and initialize a new stat card
            StatCardUI statCard = Instantiate(_statCardUI, _statHolder);
            statCard.Initialize(BattleHandler.Instance.PartyInBattle[i]);
            statCards.Add(statCard);

            statCard.gameObject.SetActive(false);
        }
    }

    private void SetStatCardVisibility(bool visible)
    {
        for(int i = 0; i < statCards.Count; i++)
        {
            statCards[i].gameObject.SetActive(visible);
        }
    }

    private void SetTurnOrderVisibility(bool visible)
    {
        _turnOrderUIHolder.gameObject.SetActive(visible);

        for (int i = 0; i < turnOrderImages.Count; i++)
        {
            turnOrderImages[i].gameObject.SetActive(visible);
        }
    }

    private void SetSkillMenuVisibility(bool visible)
    {
        _actionMenu.SetVisibility(visible);
        _actionMenu.SetInteractable(visible);
    }

    private void TurnOrderUpdated(List<BattleUnit> order)
    {
        _turnOrderUIHolder.gameObject.SetActive(true);

        for (int i = 0; i < order.Count; i++)
        {
            if (turnOrderImages.Count < order.Count)
            {
                turnOrderImages.Add(Instantiate(_turnOrderPrefab, _turnOrderUIHolder));
            }

            turnOrderImages[i].sprite = order[i].MyOrderPortrait;
        }

        if(turnOrderImages.Count > order.Count)
        {
            for(int i = 0; i < turnOrderImages.Count; i++)
            {
                if(i > order.Count)
                {
                    Destroy(turnOrderImages[i]);
                    turnOrderImages.RemoveAt(i);
                    i--;
                }
            }
        }
    }
}
