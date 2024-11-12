using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SamuraiIdleState : AState
{
    private static int anim_x = Animator.StringToHash("fDirX");
    private static int anim_z = Animator.StringToHash("fDirZ");
    private static int anim_rotationVelocity = Animator.StringToHash("fVelRot");
    private static int anim_moveVelocity = Animator.StringToHash("fVelMove");
    public override State state => State.Idle;

    private Vector3 currentDirection = Vector3.zero;
    private Vector3 targetDirection = Vector3.zero;
    public override void EndState(Actor actor)
    {
        
    }

    public override void StartState(Actor actor)
    {
        //currentDirection = actor.charController.velocity.normalized;
    }

    public override void UpdateState(Actor actor)
    {
        Samurai samurai = actor as Samurai;
        actor.UpdateRotateWithAngle(actor.PrevRotAngle);
        actor.UpdateMove(Vector3.zero, 0f);
        actor.anim.SetFloat(anim_rotationVelocity, actor.RotVelocity);
        actor.anim.SetFloat(anim_moveVelocity, actor.charController.velocity.magnitude);
    }   
}
