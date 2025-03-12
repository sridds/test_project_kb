using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class BattleUIHandler : MonoBehaviour
{
    [Header("Turn Order")]
    [SerializeField]
    private RectTransform _turnOrderUIHolder;

    [SerializeField]
    private Image _turnOrderPrefab;

    [SerializeField]
    private RectTransform _statHolder;

    [SerializeField]
    private StatCardUI _statCardUI;

    private List<Image> turnOrderImages = new List<Image>();
    private List<StatCardUI> statCards = new List<StatCardUI>();

    void Start()
    {
        _turnOrderUIHolder.gameObject.SetActive(false);

        BattleHandler.Instance.OnTurnOrderUpdated += TurnOrderUpdated;
        BattleHandler.Instance.OnBattleStart += SetupBattleUI;
        BattleHandler.Instance.OnBattleStateUpdated += BattleStateUpdated;
    }

    private void SetupBattleUI()
    {
        SetupStatCards();
    }

    private void BattleStateUpdated(BattleHandler.EBattleState battleState)
    {
        if (battleState == BattleHandler.EBattleState.PlayerTurn)
        {
            SetStatCardVisibility(true);
            SetTurnOrderVisibility(true);
        }

        else
        {
            SetStatCardVisibility(false);
            SetTurnOrderVisibility(false);
        }
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
