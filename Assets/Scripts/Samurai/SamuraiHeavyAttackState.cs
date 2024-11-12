using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SamuraiHeavyAttackState : AState
{
    private static int animHash_ready = Animator.StringToHash("bHeavyAttack_Ready");
    private static int animHash_start = Animator.StringToHash("tHeavyAttack_Start");
    public override State state => State.HeavyAttack;
    
    //private SSamuraiHeavyAttackReady ssamuraiHeavyAttackReady = new SSamuraiHeavyAttackReady();

    public override void EndState(Actor actor)
    {
        actor.SetStateLock(false);
        actor.anim.SetBool(animHash_ready, false);
    }

    public override void StartState(Actor actor)
    {
        actor.anim.SetBool(animHash_ready, true);
        actor.SetStateLock(true);
    }

    public override void UpdateState(Actor actor)
    {
        if(Input.GetMouseButtonDown(0))
        {
            actor.anim.SetTrigger(animHash_start);
        }
    }
}
