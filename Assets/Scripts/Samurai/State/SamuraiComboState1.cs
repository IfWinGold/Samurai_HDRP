using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SamuraiComboState1 : AState
{
    private static int anim_combo = Animator.StringToHash("nCombo");
    private static int anim_attack = Animator.StringToHash("bAttack");
    public override State state => State.Attack;

    public override void EndState(Actor actor)
    {
        throw new System.NotImplementedException();
    }

    public override void StartState(Actor actor)
    {
        throw new System.NotImplementedException();
    }

    public override void UpdateState(Actor actor)
    {
        throw new System.NotImplementedException();
    }
}
