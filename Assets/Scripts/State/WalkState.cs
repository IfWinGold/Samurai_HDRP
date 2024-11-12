using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkState : AState
{
    private static int animHash = Animator.StringToHash("bWalk");
    private float speed = 1f;

    public override State state => State.Walk;

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
        Vector3 direction = GameUtility.GetInputDirection();
        actor.UpdateMove(direction, speed);
        actor.UpdateRotateUsingCameraDirection(direction);
    }
}
