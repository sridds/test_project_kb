using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PartyMemberUnit : BattleUnit
{
    public override void UpdateAttackState()
    {
        // take input

        if(Input.GetKeyDown(KeyCode.Z))
        {
            Debug.Log("ht key");
            currentAttack.RegisterInput();
        }
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