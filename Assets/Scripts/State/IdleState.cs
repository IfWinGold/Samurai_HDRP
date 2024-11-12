using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : AState
{
    private static int animHash = Animator.StringToHash("bIdle");

    public override State state => State.Idle;

    public override void EndState(Actor actor)
    {
        actor.anim.SetBool(animHash, false);
    }

    public override void StartState(Actor actor)
    {
        actor.anim.SetBool(animHash, true);        
    }

    public override void UpdateState(Actor actor)
    {
       
    }
}
