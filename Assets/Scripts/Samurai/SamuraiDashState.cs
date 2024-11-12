using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SamuraiDashState : AState
{
    private static int animHash = Animator.StringToHash("bDash");
    public override State state => State.Dash;
    private Vector3 vdashDirection = Vector3.zero;
    private float fdashTime = 0.25f;
    private float fcurrentDashTime = 0f;
    public override void EndState(Actor actor)
    {
        actor.anim.SetBool(animHash, false);
    }

    public override void StartState(Actor actor)
    {        
        actor.anim.SetBool(animHash, true);
        vdashDirection = GameUtility.GetInputDirection();
        actor.SetStateLock(true);
        SetBlur();
        
    }

    public override void UpdateState(Actor actor)
    {
        actor.UpdateMove(vdashDirection, actor.dashSpeed);
        actor.UpdateRotateUsingCameraDirection(vdashDirection);

        fcurrentDashTime += Time.deltaTime;
        if (fcurrentDashTime > fdashTime)
        {
            actor.SetStateLock(false);
        }
    }

    private void SetBlur()
    {

 
    }
}
