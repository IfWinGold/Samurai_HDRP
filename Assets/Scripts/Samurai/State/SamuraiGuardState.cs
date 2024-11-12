using NSamurai;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;

public class SamuraiGuardState : AState
{
    private static int anim_x = Animator.StringToHash("fDirX");
    private static int anim_z = Animator.StringToHash("fDirZ");
    private static int anim_guard = Animator.StringToHash("bGuard");
    private static int anim_rotationVelocity = Animator.StringToHash("fVelMove");
    private static int anim_moveVelocity = Animator.StringToHash("fVelMove");
    private const string GUARD_BLOCK = "tGuardBlock";    

    public override State state => State.Guard;
    private float speed = 1.5f;
    private float parryTime = 0.25f;
    private bool canParry = false;

    private ActorCamera actorCamera;
    private float testRef;
    private Coroutine mParryRoutine = null;
    private Coroutine mParryTimer = null;

    //처형 가능한 상태일 시 True
    private bool canExecuteReady = false;

    public override void EndState(Actor actor)
    {
        actor.SetStateLock(false);
        actor.anim.SetBool(anim_guard, false);
        //actor.anim.SetBool(anim_parry, false);        
        if (mParryTimer != null)
        {
            actor.StopCoroutine(mParryTimer);
        }                
    }

    public override void StartState(Actor actor)
    {
        actor.SetStateLock(true);
        actor.anim.SetBool(anim_guard, true);
        actorCamera = actor.GetActorCamera();
        mParryTimer = actor.StartCoroutine(Co_ParryTimer(parryTime));
    }

    public override void UpdateState(Actor actor)
    {
        //Guard End
        if (!Input.GetMouseButton(1)&&mParryRoutine == null)
            EndState(actor);

        //Update Move
        Vector3 moveDirection = GameUtility.GetInputDirection();
        actor.UpdateMove(moveDirection, actor.moveSpeed);
        actor.UpdateRotateUsingCameraDirection(new Vector3(0f, 0f, 1f));

        //actor.UpdateRotateUsingCameraDirection(direction);
        actor.anim.SetFloat(anim_x, moveDirection.x);
        actor.anim.SetFloat(anim_z, moveDirection.z);
    }
    public bool CanParry()
    {
        return canParry;
    }
    public void Guard(Actor actor)
    {
        Samurai samurai = actor as Samurai;
        samurai.VFXPlayBlock();
        samurai.SFXPlayBlock();
        StringBuilder sb = new StringBuilder();
        sb.Append(GUARD_BLOCK);
        sb.Append(Random.Range(1, 4).ToString());
        samurai.anim.SetTrigger(sb.ToString());
        sb.Clear();
        //TODO >> GUARD
    }
    IEnumerator Co_ParryTimer(float time)
    {
        canParry = true;
        yield return new WaitForSeconds(time);
        canParry = false;
        mParryTimer = null;
    }
}
