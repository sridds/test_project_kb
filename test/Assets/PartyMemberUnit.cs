using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class PartyMemberUnit : BattleUnit
{
    public List<PartyMemberAttack> MyAttacks => _myAttacks;
    public PartyMemberAttack CurrentAttack => currentAttack;

    [SerializeField]
    private List<PartyMemberAttack> _myAttacks = new List<PartyMemberAttack>(); // this would probably be a list of structs -- the unit attack and the level at which you need to be to use it

    private PartyMemberAttack currentAttack;

    // During an update, the player can press Z during an attack
    public override void UpdateAttackState()
    {
        if(Input.GetKeyDown(KeyCode.Z))
        {
            currentAttack.RegisterInput();
        }
    }

    public override void HandleAttack(int attackIndex, BattleUnit target)
    {
        // Create attack animation and hide the battle sprite
        SetRendererEnabled(false);

        // Initalize attack
        PartyMemberAttack attack = Instantiate(_myAttacks[attackIndex], transform.position, Quaternion.identity);
        attack.Init(this, target);
        attack.OnAttackEnded += HandleAttackEnd;

        currentAttack = attack;
    }

    private void HandleAttackEnd()
    {
        SetRendererEnabled(true);

        currentAttack.OnAttackEnded -= HandleAttackEnd;
        Destroy(currentAttack.gameObject);

        // Advance to the next turn
        BattleHandler.Instance.AdvanceNextTurn();
    }

    public void UpdateDefendState()
    {
        // take input

        if(Input.GetKeyDown(KeyCode.Z))
        {

        }

        if(Input.GetKeyDown(KeyCode.X))
        {

        }
    }

    public override void HandleHealthUpdate(int oldHealth, int newHealth)
    {
        // Damage Taken
        if(newHealth < oldHealth)
        {

        }
    }
}