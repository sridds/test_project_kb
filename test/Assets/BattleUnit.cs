using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUnit
{
    Stats MyStats { get; }
    Health MyHealth { get; }
}

public abstract class BattleUnit : MonoBehaviour, IUnit
{
    #region Accessors
    public Stats MyStats => stats;
    public Health MyHealth => health;
    public Sprite MyOrderPortrait => _turnOrderPortrait;
    #endregion

    #region Private Fields
    private Health health;
    #endregion

    [SerializeField]
    protected Stats stats;

    [SerializeField]
    protected Sprite _turnOrderPortrait;

    [SerializeField]
    protected SpriteRenderer _unitRenderer;

    private void Start()
    {
        // Setup health
        health = new Health(stats.MaxHP);
    }

    public abstract void HandleAttack(int attackIndex, BattleUnit target);

    public virtual void UpdateAttackState() { }

    public void SetRendererEnabled(bool enabled) => _unitRenderer.enabled = enabled;

    public Vector2 GetSpriteSize() => _unitRenderer.bounds.size;
}

[System.Serializable]
public struct Stats
{
    public string Name;
    public int MaxHP;
    public int BaseAttack;
}