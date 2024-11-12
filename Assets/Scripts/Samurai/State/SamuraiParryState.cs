using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SamuraiParryState : AState
{
    private static int anim_parry = Animator.StringToHash("tParry");
    private static int anim_guard = Animator.StringToHash("bGuard");
    public override State state => State.Parry;

    private float currentTime = 0f;
    private float parryTime = 0.2f;
    public override void EndState(Actor actor)
    {
        actor.anim.ResetTrigger(anim_parry);
        actor.anim.SetBool(anim_guard, false);
    }

    public override void StartState(Actor actor)
    {        
        Samurai samurai = actor as Samurai;
        samurai.SetStateLock(true);
        samurai.SFXPlayParry();
        samurai.anim.SetBool(anim_guard, true);
        actor.anim.SetTrigger(anim_parry);
        samurai.VFXPlayParry();
    }

    public override void UpdateState(Actor actor)
    {
        currentTime += Time.deltaTime;
        if (currentTime > parryTime)
            actor.SetStateLock(false);
    }
}
