using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SamuraiWalkState : AState
{
    private static int anim_x = Animator.StringToHash("fDirX");
    private static int anim_z = Animator.StringToHash("fDirZ");
    private static int anim_b = Animator.StringToHash("bWalk");
    private static int anim_rotationVelocity = Animator.StringToHash("fVelRot");
    private static int anim_moveVelocity = Animator.StringToHash("fVelMove");
    public override State state => State.Walk;

    private ActorCamera ac;
    float currentX = 0f;
    float currentZ = 0f;

    float velX = 0f;
    float velZ = 0f;
    float smoothTime = 0.1f;
    public override void EndState(Actor actor)
    {
        actor.anim.SetBool(anim_b, false);
    }

    public override void StartState(Actor actor)
    {
        ac = actor.GetActorCamera();
        actor.anim.SetBool(anim_b,true);
    }

    public override void UpdateState(Actor actor)
    {       
        Vector3 direction = GameUtility.GetInputDirection();        
        actor.UpdateMove(direction, actor.moveSpeed);        
        actor.UpdateRotateUsingCameraDirection(direction);

        if(ac.IsLockOn)
        {
            float currentX = actor.anim.GetFloat(anim_x);
            float currentZ = actor.anim.GetFloat(anim_z);

            actor.anim.SetFloat(anim_x, Mathf.SmoothDamp(currentX, direction.x, ref velX, smoothTime));
            actor.anim.SetFloat(anim_z, Mathf.SmoothDamp(currentZ, direction.z, ref velZ, smoothTime));
        }
        else
        {
            actor.anim.SetFloat(anim_x, direction.x);
            actor.anim.SetFloat(anim_z, direction.z);
        }
        actor.anim.SetFloat(anim_rotationVelocity, actor.RotVelocity);
        actor.anim.SetFloat(anim_moveVelocity, actor.charController.velocity.magnitude);
    }
}
