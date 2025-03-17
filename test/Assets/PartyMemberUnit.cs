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

    public override void StartAttack(int attackIndex, BattleUnit target)
    {
        // Create attack animation and hide the battle sprite
        SetRendererEnabled(false);

        // Initalize attack
        PartyMemberAttack attack = Instantiate(_myAttacks[attackIndex], transform.position, Quaternion.identity);
        attack.Init(this, target);

        currentAttack = attack;
    }

    public void UseItem()
    {
        Debug.Log("I just used an item lowkey gangnam style");
    }

    protected override void HandleAttackEnding()
    {
        Destroy(currentAttack.gameObject);

        // Advance to the next turn
        base.HandleAttackEnding();
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