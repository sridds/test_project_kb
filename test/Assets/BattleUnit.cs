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
    public List<UnitAttack> MyAttacks => _myAttacks;
    #endregion

    #region Private Fields
    private Health health;
    #endregion

    [SerializeField]
    protected Stats stats;

    [SerializeField]
    protected List<UnitAttack> _myAttacks = new List<UnitAttack>(); // this would probably be a list of structs -- the unit attack and the level at which you need to be to use it

    [SerializeField]
    protected Sprite _turnOrderPortrait;

    [SerializeField]
    protected SpriteRenderer _unitRenderer;

    protected UnitAttack currentAttack;

    private void Start()
    {
        // Setup health
        health = new Health(stats.MaxHP);
    }

    public void HandleAttack(int index, EnemyUnit target)
    {
        // Create attack animation and hide the battle sprite
        SetRendererEnabled(false);

        UnitAttack attack = Instantiate(_myAttacks[index], transform.position, Quaternion.identity);
        attack.Execute(this, target);
        attack.OnAttackEnded += HandleAttackEnd;

        currentAttack = attack;
    }

    private void HandleAttackEnd()
    {
        SetRendererEnabled(true);

        currentAttack.OnAttackEnded -= HandleAttackEnd;
        Destroy(currentAttack.gameObject);
    }

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