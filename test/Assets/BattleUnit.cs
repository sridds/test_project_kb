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

    #region Inspector Fields
    [SerializeField]
    protected Stats stats;

    [SerializeField]
    protected Sprite _turnOrderPortrait;

    [SerializeField]
    protected SpriteRenderer _unitRenderer;

    [SerializeField]
    protected float _feetPos;
    #endregion

    public delegate void AttackFinished();
    public AttackFinished OnAttackFinished;

    private void Start()
    {
        // Setup health
        health = new Health(stats.MaxHP);

        health.OnHealthUpdated += HandleHealthUpdate;
    }

    public abstract void HandleHealthUpdate(int oldHealth, int newHealth);

    public abstract void StartAttack(int attackIndex, BattleUnit target);

    public virtual void UpdateAttackState() { }

    public virtual void EndAttack()
    {
        OnAttackFinished?.Invoke();
    }

    #region Helpers
    public void SetRendererEnabled(bool enabled) => _unitRenderer.enabled = enabled;

    public Vector2 GetSpriteSize() => _unitRenderer.bounds.size;

    public float GetFeetPos() => _feetPos;
    #endregion
}

[System.Serializable]
public struct Stats
{
    public string Name;
    public int MaxHP;
    public int BaseAttack;
    public int BaseDefense;
}