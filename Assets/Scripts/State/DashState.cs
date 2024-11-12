using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DashState : AState
{
    private static int animHash = Animator.StringToHash("bDash");
    public override State state => State.Dash;
    private float speed = 5f;
    private Vector3 vdashDirection = Vector3.zero;
    private float fdashTime = 0.25f;
    private float fcurrentDashTime = 0f;

   
    public override void EndState(Actor actor)
    {
        actor.anim.SetBool(animHash, false);
        //VolumeController.instance.SetActiveFullScreenGray(false);
        //Time.timeScale = 1f;
        //VolumeController.instance.blurIntensity = 0f;
    }
    public override void StartState(Actor actor)
    {
        vdashDirection = GameUtility.GetInputDirection();
        actor.anim.SetBool(animHash,true);
        actor.SetStateLock(true);

        SetBlur();
        //VolumeController.instance.blurIntensity = 0.01f;
        //VolumeController.instance.SetActiveFullScreenGray(true);
        //Time.timeScale = 0.2f;
    }
    private void SetBlur()
    {
        Vector2 blurDirection = new Vector2
            (
            vdashDirection.x,
            0f
            );
        DOTween.To(
            () => VolumeController.instance.blurIntensity,
            x => VolumeController.instance.blurIntensity = x,
            0.01f, 0.1f).SetLoops(2, LoopType.Yoyo);
    }
    public override void UpdateState(Actor actor)
    {
        actor.UpdateMove(vdashDirection, speed);
        actor.UpdateRotateUsingCameraDirection(vdashDirection);

        fcurrentDashTime += Time.deltaTime;
        if(fcurrentDashTime > fdashTime)
        {      
            actor.SetStateLock(false);
        }
    }    
}
