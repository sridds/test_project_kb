using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class PartyMemberUnit : BattleUnit
{
    public List<PartyMemberAttack> MyAttacks => _myAttacks;

    [SerializeField]
    private List<PartyMemberAttack> _myAttacks = new List<PartyMemberAttack>(); // this would probably be a list of structs -- the unit attack and the level at which you need to be to use it

    private PartyMemberAttack currentAttack;

    public override void UpdateAttackState()
    {
        // take input

        if(Input.GetKeyDown(KeyCode.Z))
        {
            Debug.Log("ht key");
            currentAttack.RegisterInput();
        }
    }

    public override void HandleAttack(int attackIndex, BattleUnit target)
    {
        // Create attack animation and hide the battle sprite
        SetRendererEnabled(false);

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
}