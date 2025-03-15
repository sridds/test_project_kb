using UnityEngine;
using DG.Tweening;

public class BattleCamera : MonoBehaviour
{
    [SerializeField]
    private float _moveAmount = 2.0f;

    [SerializeField]
    private float _moveEasingTime = 1.5f;

    private void Start()
    {
        BattleHandler.Instance.OnBattleStateUpdated += BattleStateUpdate;
    }

    private void BattleStateUpdate(BattleHandler.EBattleState state)
    {
        if(state == BattleHandler.EBattleState.PlayerTurn)
        {
            transform.DOKill(false);
            transform.DOMoveX(-_moveAmount, _moveEasingTime).SetEase(Ease.OutQuad);
        }

        else if(state == BattleHandler.EBattleState.Attacking)
        {
            transform.DOKill(false);
            transform.DOMoveX(0.0f, _moveEasingTime).SetEase(Ease.OutQuad);
        }
    }
}
