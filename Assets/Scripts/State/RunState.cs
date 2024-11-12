using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using UnityEngine;

public class RunState : AState
{
    private static int animHash = Animator.StringToHash("bRun");
    public override State state => AState.State.Run;

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
        actor.UpdateMove(direction, actor.runSpeed);
        actor.UpdateRotateUsingCameraDirection(direction);
    }
}
