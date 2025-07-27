using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

public class BattleUIManager : MonoBehaviour
{
    public static BattleUIManager instance;

    #region -Unit Selector-

    [Foldout("Unit Selector Movement")]
    [SerializeField] private GameObject unitSelectorParent;
    [SerializeField] private GameObject unitSelectorTL;
    [SerializeField] private GameObject unitSelectorTR;
    [SerializeField] private GameObject unitSelectorBL;
    [SerializeField] private GameObject unitSelectorBR;


    [Foldout("Unit Selector Movement")]
    [SerializeField] private float unitSelectorMoveDuration;

    Tween unitSelectorMovementTween;

    #endregion

    private void Awake()
    {
        Init();
    }
    void Init()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public void MoveUnitSelector(Vector2 targetPosition)
    {
        if (unitSelectorMovementTween != null)
        {
            unitSelectorMovementTween.Complete();
        }
        unitSelectorMovementTween = unitSelectorParent.transform.DOMove(targetPosition, unitSelectorMoveDuration);
    }

    public void ResizeUnitSelector(Vector2 targetSize)
    {
        unitSelectorTL.transform.DOComplete();
        unitSelectorTL.transform.DOLocalMove(new Vector2(-targetSize.x, targetSize.y), unitSelectorMoveDuration);
        unitSelectorTL.transform.DOComplete();
        unitSelectorTR.transform.DOLocalMove(new Vector2(targetSize.x, targetSize.y), unitSelectorMoveDuration);
        unitSelectorTL.transform.DOComplete();
        unitSelectorBL.transform.DOLocalMove(new Vector2(-targetSize.x, -targetSize.y), unitSelectorMoveDuration);
        unitSelectorTL.transform.DOComplete();
        unitSelectorBR.transform.DOLocalMove(new Vector2(targetSize.x, -targetSize.y), unitSelectorMoveDuration);
    }

}
