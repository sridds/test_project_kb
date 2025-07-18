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
    private UnitFlasher _flasher;

    [SerializeField]
    protected SpriteRenderer _unitRenderer;
    #endregion

    public delegate void ActionComplete();
    public ActionComplete OnActionComplete;

    protected bool isDead;

    private void Awake()
    {
        // Setup health
        health = new Health(stats.MaxHP);

        health.OnHealthUpdated += HandleHealthUpdate;
        health.OnHealthDepleted += HandleDeath;
    }

    public virtual void Cleanup()
    {
        Destroy(gameObject);
    }

    public virtual bool CheckForDeath()
    {
        return isDead;
    }

    public virtual void HandleDeath()
    {
        isDead = true;
    }

    public abstract void HandleHealthUpdate(int oldHealth, int newHealth);

    public abstract void StartBash(BattleUnit target);

    public virtual void UpdateBashState() { }

    protected virtual void HandleActionEnding() { }

    public void SetTarget(bool isTarget)
    {
        if (isTarget)
        {
            Debug.Log($"my name is {MyStats.Name}");
            _flasher.EnableFlash();
        }
        else
        {
            Debug.Log($"DISABLED my name is {MyStats.Name}");
            _flasher.DisableFlashing();
        }
    }

    #region Helpers
    public void SetRendererEnabled(bool enabled) => _unitRenderer.enabled = enabled;

    public Vector2 GetSpriteSize() => _unitRenderer.bounds.size;
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