using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SamuraiDamegedState : AState
{
    public static int ANIM_DAMEGED = Animator.StringToHash("tDameged"); 
    public override State state => State.Dameged;

    private float fdamegeTime = 0.25f;
    private float fcurrentTime = 0f;
    public override void EndState(Actor actor)
    {        
        actor.SetStateLock(false);
    }

    public override void StartState(Actor actor)
    {
        actor.anim.SetTrigger(ANIM_DAMEGED);
        actor.SetStateLock(true);
    }

    public override void UpdateState(Actor actor)
    {
        fcurrentTime += Time.deltaTime;
        if (fcurrentTime >= fdamegeTime)
            actor.SetStateLock(false);
    }
}
